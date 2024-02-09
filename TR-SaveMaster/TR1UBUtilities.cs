using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR1UBUtilities
    {
        // Static offsets
        private const int saveNumberOffset = 0x4B;
        private const int magnumAmmoOffset = 0x9C;
        private const int uziAmmoOffset = 0x9E;
        private const int shotgunAmmoOffset = 0xA0;
        private const int smallMedipackOffset = 0xA2;
        private const int largeMedipackOffset = 0xA3;
        private const int weaponsConfigNumOffset = 0xA7;
        private const int levelIndexOffset = 0xB3;

        // Dynamic offsets
        private int magnumAmmoOffset2;
        private int uziAmmoOffset2;
        private int shotgunAmmoOffset2;

        // Health
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private const UInt16 MIN_HEALTH_VALUE = 0;
        private List<int> healthOffsets = new List<int>();

        // Strings
        private string savegamePath;

        // Ammo index
        private int secondaryAmmoIndex = -1;

        public void SetSavegamePath(string path)
        {
            savegamePath = path;
        }

        private string GetLvlName()
        {
            using (FileStream saveFileStream = new FileStream(savegamePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (saveFileStream.CanRead)
                {
                    using (StreamReader saveFileReader = new StreamReader(saveFileStream))
                    {
                        return saveFileReader.ReadLine().Trim();
                    }
                }
            }

            return null;
        }

        private byte ReadByte(int offset)
        {
            using (FileStream saveFile = new FileStream(savegamePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                return (byte)saveFile.ReadByte();
            }
        }

        private void WriteByte(int offset, byte value)
        {
            using (FileStream saveFile = new FileStream(savegamePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                byte[] byteData = { value };
                saveFile.Write(byteData, 0, byteData.Length);
            }
        }

        private UInt16 ReadUInt16(int offset)
        {
            byte lowerByte = ReadByte(offset);
            byte upperByte = ReadByte(offset + 1);

            return (UInt16)(lowerByte + (upperByte << 8));
        }

        private void WriteUInt16(int offset, UInt16 value)
        {
            if (value > 255)
            {
                byte upperByte = (byte)(value / 256);
                byte lowerByte = (byte)(value % 256);

                WriteByte(offset + 1, upperByte);
                WriteByte(offset, lowerByte);
            }
            else
            {
                WriteByte(offset + 1, 0);
                WriteByte(offset, (byte)value);
            }
        }

        public void DetermineOffsets()
        {
            byte levelIndex = GetLevelIndex();

            if (levelIndex == 0)        // Return to Egypt
            {
                magnumAmmoOffset2 = 0xDCF;
                uziAmmoOffset2 = 0xDDB;
                shotgunAmmoOffset2 = 0xDE7;

                SetHealthOffsets(0x165);
            }
            else if (levelIndex == 1)   // Temple of the Cat
            {
                magnumAmmoOffset2 = 0x1123;
                uziAmmoOffset2 = 0x112F;
                shotgunAmmoOffset2 = 0x113B;

                SetHealthOffsets(0x3DD, 0x3E9);
            }
            else if (levelIndex == 2)   // Atlantean Stronghold
            {
                magnumAmmoOffset2 = 0xC32;
                uziAmmoOffset2 = 0xC3E;
                shotgunAmmoOffset2 = 0xC4A;

                SetHealthOffsets(0x3C7, 0x3D3, 0x3DF);
            }
            else if (levelIndex == 3)   // The Hive
            {
                magnumAmmoOffset2 = 0x11E2;
                uziAmmoOffset2 = 0x11EE;
                shotgunAmmoOffset2 = 0x11FA;

                SetHealthOffsets(0x501);
            }
        }

        private readonly Dictionary<byte, int[]> ammoIndexDataATI = new Dictionary<byte, int[]>
        {
            { 0, new int[] { 0x0C7D, 0x0C7E, 0x0C7F, 0x0C80 } },    // Return to Egypt
            { 1, new int[] { 0x0FBF, 0x0FC0, 0x0FC1, 0x0FC2 } },    // Temple of the Cat
            { 2, new int[] { 0x0B31, 0x0B32, 0x0B33, 0x0B34 } },    // Atlantean Stronghold
            { 3, new int[] { 0x1099, 0x109A, 0x109B, 0x109C } },    // The Hive
        };

        private int GetSecondaryAmmoIndex()
        {
            byte levelIndex = GetLevelIndex();

            if (ammoIndexDataATI.ContainsKey(levelIndex))
            {
                int[] indexData = ammoIndexDataATI[levelIndex];

                int[] offsets = new int[indexData.Length];

                for (int index = 0; index < 40; index++)
                {
                    Array.Copy(indexData, offsets, indexData.Length);

                    for (int i = 0; i < indexData.Length; i++)
                    {
                        offsets[i] += (index * 0x3);
                    }

                    if (offsets.All(offset => ReadByte(offset) == 0xFF))
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        private int GetSecondaryAmmoOffset(int baseOffset)
        {
            return baseOffset + (secondaryAmmoIndex * 0x3);
        }

        private void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
            }
        }

        public void DisplayGameInfo(TextBox txtLevelName, NumericUpDown nudSaveNumber, NumericUpDown nudSmallMedipacks,
            NumericUpDown nudLargeMedipacks, NumericUpDown nudShotgunAmmo, NumericUpDown nudMagnumAmmo, NumericUpDown nudUziAmmo,
            CheckBox chkPistols, CheckBox chkMagnums, CheckBox chkUzis, CheckBox chkShotgun, TrackBar trbHealth, Label lblHealth,
            Label lblHealthError)
        {
            txtLevelName.Text = GetLvlName();

            nudSaveNumber.Value = GetSaveNumber();
            nudShotgunAmmo.Value = GetShotgunAmmo() / 6;
            nudMagnumAmmo.Value = GetMagnumAmmo();
            nudUziAmmo.Value = GetUziAmmo();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();

            byte weaponsConfigNum = ReadByte(weaponsConfigNumOffset);

            const byte Pistols = 2;
            const byte Magnums = 4;
            const byte Uzis = 8;
            const byte Shotgun = 16;

            if (weaponsConfigNum == 1)
            {
                chkPistols.Checked = false;
                chkMagnums.Checked = false;
                chkUzis.Checked = false;
                chkShotgun.Checked = false;
            }
            else
            {
                chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
                chkMagnums.Checked = (weaponsConfigNum & Magnums) != 0;
                chkUzis.Checked = (weaponsConfigNum & Uzis) != 0;
                chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
            }

            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                double healthPercentage = GetHealthPercentage(healthOffset);
                trbHealth.Value = (UInt16)healthPercentage;
                trbHealth.Enabled = true;

                lblHealth.Text = healthPercentage.ToString("0.0") + "%";
                lblHealthError.Visible = false;
                lblHealth.Visible = true;
            }
            else
            {
                trbHealth.Enabled = false;
                trbHealth.Value = 0;
                lblHealthError.Visible = true;
                lblHealth.Visible = false;
            }
        }

        public void WriteChanges(NumericUpDown nudSaveNumber, NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks,
            NumericUpDown nudShotgunAmmo, NumericUpDown nudMagnumAmmo, NumericUpDown nudUziAmmo, CheckBox chkPistols,
            CheckBox chkMagnums, CheckBox chkUzis, CheckBox chkShotgun, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);

            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkMagnums.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;

            WriteWeaponsConfigNum(newWeaponsConfigNum);

            if (IsATISavegame())
            {
                secondaryAmmoIndex = GetSecondaryAmmoIndex();

                if (secondaryAmmoIndex != -1)
                {
                    byte levelIndex = GetLevelIndex();
                    int baseSecondaryAmmoIndexOffset = ammoIndexDataATI[levelIndex][0];

                    magnumAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 40);
                    uziAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 28);
                    shotgunAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 16);
                }
            }
            else
            {
                secondaryAmmoIndex = 0;
            }

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteMagnumAmmo(chkMagnums.Checked, (UInt16)nudMagnumAmmo.Value);
            WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);

            if (trbHealth.Enabled)
            {
                WriteHealthValue((double)trbHealth.Value);
            }
        }

        private int GetHealthOffset()
        {
            for (int i = 0; i < healthOffsets.Count; i++)
            {
                UInt16 value = ReadUInt16(healthOffsets[i]);

                if (value > MIN_HEALTH_VALUE && value <= MAX_HEALTH_VALUE)
                {
                    byte byteFlag1 = ReadByte(healthOffsets[i] - 10);
                    byte byteFlag2 = ReadByte(healthOffsets[i] - 9);
                    byte byteFlag3 = ReadByte(healthOffsets[i] - 8);

                    if (IsKnownByteFlagPattern(byteFlag1, byteFlag2, byteFlag3))
                    {
                        return healthOffsets[i];
                    }
                }
            }

            return -1;
        }

        private double GetHealthPercentage(int healthOffset)
        {
            UInt16 health = ReadUInt16(healthOffset);
            double healthPercentage = ((double)health / MAX_HEALTH_VALUE) * 100.0;

            return healthPercentage;
        }

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;       // Standing
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;       // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;       // On water
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x0D) return true;       // Underwater

            return false;
        }

        private byte GetLevelIndex()
        {
            return ReadByte(levelIndexOffset);
        }

        private byte GetNumSmallMedipacks()
        {
            return ReadByte(smallMedipackOffset);
        }

        private byte GetNumLargeMedipacks()
        {
            return ReadByte(largeMedipackOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumberOffset);
        }

        private UInt16 GetShotgunAmmo()
        {
            return ReadUInt16(shotgunAmmoOffset);
        }

        private UInt16 GetMagnumAmmo()
        {
            return ReadUInt16(magnumAmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumberOffset, value);
        }

        private void WriteNumSmallMedipacks(byte value)
        {
            WriteByte(smallMedipackOffset, value);
        }

        private void WriteNumLargeMedipacks(byte value)
        {
            WriteByte(largeMedipackOffset, value);
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        private void WriteHealthValue(double newHealthPercentage)
        {
            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                UInt16 newHealth = (UInt16)(newHealthPercentage / 100.0 * MAX_HEALTH_VALUE);
                WriteUInt16(healthOffset, newHealth);
            }
        }

        private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(shotgunAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(shotgunAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(shotgunAmmoOffset2, 0);
            }
        }

        private void WriteMagnumAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(magnumAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(magnumAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(magnumAmmoOffset2, 0);
            }
        }

        private void WriteUziAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(uziAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(uziAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(uziAmmoOffset2, 0);
            }
        }

        private bool IsATISavegame()
        {
            int[] atiSignatureOffsets = { 0x34, 0x35, 0x36, 0x37 };

            return atiSignatureOffsets.All(offset => ReadByte(offset) == 0xFF);
        }

        private static bool IsNumericExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (extension.StartsWith("."))
            {
                string numericPart = extension.Substring(".".Length);
                return int.TryParse(numericPart, out _);
            }

            return false;
        }

        private class NaturalComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                string[] xParts = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                string[] yParts = Regex.Split(y.Replace(" ", ""), "([0-9]+)");

                for (int i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
                {
                    if (i % 2 == 0)
                    {
                        int nonNumericComparison = String.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);

                        if (nonNumericComparison != 0)
                        {
                            return nonNumericComparison;
                        }
                    }
                    else
                    {
                        int xNumeric = int.TryParse(xParts[i], out int xNumericValue) ? xNumericValue : int.MaxValue;
                        int yNumeric = int.TryParse(yParts[i], out int yNumericValue) ? yNumericValue : int.MaxValue;

                        int numericComparison = xNumeric.CompareTo(yNumeric);

                        if (numericComparison != 0)
                        {
                            return numericComparison;
                        }
                    }
                }

                return x.Length.CompareTo(y.Length);
            }
        }

        public bool IsValidSavegame(string path)
        {
            savegamePath = path;

            FileInfo fileInfo = new FileInfo(path);
            byte levelIndex = GetLevelIndex();

            return (levelIndex >= 0 && levelIndex <= 3) && (fileInfo.Length == 0x28C3);
        }

        public List<string> GetSavegamePaths(string gameDirectory)
        {
            List<string> savegamePaths = new List<string>();

            if (Directory.Exists(gameDirectory))
            {
                var matchingFiles = Directory.GetFiles(gameDirectory).Where(file => IsNumericExtension(file)).ToList();
                matchingFiles = matchingFiles.OrderBy(file => file, new NaturalComparer()).ToList();
                savegamePaths.AddRange(matchingFiles);
            }

            return savegamePaths;
        }
    }
}
