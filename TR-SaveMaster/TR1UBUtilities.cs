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
        // Offsets
        private const int saveNumberOffset = 0x04B;
        private const int magnumAmmoOffset = 0x09C;
        private const int uziAmmoOffset = 0x09E;
        private const int shotgunAmmoOffset = 0x0A0;
        private const int smallMedipackOffset = 0x0A2;
        private const int largeMedipackOffset = 0x0A3;
        private const int weaponsConfigNumOffset = 0x0A7;
        private const int levelIndexOffset = 0x0B3;
        private int magnumAmmoOffset2;
        private int uziAmmoOffset2;
        private int shotgunAmmoOffset2;

        // Health
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private const UInt16 MIN_HEALTH_VALUE = 0;
        private List<int> healthOffsets = new List<int>();

        // Strings
        private string savegamePath;

        public void SetSavegamePath(string path)
        {
            savegamePath = path;
        }

        public string GetLvlName()
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
                SetHealthOffsets(0x165);
            }
            else if (levelIndex == 1)   // Temple of the Cat
            {
                SetHealthOffsets(0x3DD, 0x3E9);
            }
            else if (levelIndex == 2)   // Atlantean Stronghold
            {
                SetHealthOffsets(0x3C7, 0x3DF, 0x3D3);
            }
            else if (levelIndex == 3)   // The Hive
            {
                SetHealthOffsets(0x501);
            }

            SetSecondaryAmmoOffsets();
        }

        private readonly Dictionary<byte, Dictionary<int, int[]>> ammoIndexData = new Dictionary<byte, Dictionary<int, int[]>>
        {
            [0] = new Dictionary<int, int[]>                // Return to Egypt
            {
                [0xC7D] = new int[] { 0xC7D, 0xC7E, 0xC7F, 0xC80 },
                [0xC95] = new int[] { 0xC95, 0xC96, 0xC97, 0xC98 },
                [0xCA1] = new int[] { 0xCA1, 0xCA2, 0xCA3, 0xCA4 },
                [0xCC5] = new int[] { 0xCC5, 0xCC6, 0xCC7, 0xCC8 },
                [0xCD1] = new int[] { 0xCD1, 0xCD2, 0xCD3, 0xCD4 },
                [0xCDD] = new int[] { 0xCDD, 0xCDE, 0xCDF, 0xCE0 },
                [0xDF7] = new int[] { 0xDF7, 0xDF8, 0xDF9, 0xDFA },
            },
            [1] = new Dictionary<int, int[]>                // Temple of the Cat
            {
                [0xFBF] = new int[] { 0xFBF, 0xFC0, 0xFC1, 0xFC2 },
                [0xFCB] = new int[] { 0xFCB, 0xFCC, 0xFCD, 0xFCE },
                [0xFD7] = new int[] { 0xFD7, 0xFD8, 0xFD9, 0xFDA },
                [0x1007] = new int[] { 0x1027, 0x1028, 0x1029, 0x102A },
                [0x114B] = new int[] { 0x114B, 0x114C, 0x114D, 0x114E },
            },
            [2] = new Dictionary<int, int[]>                // Atlantean Stronghold
            {
                [0xB31] = new int[] { 0xB31, 0xB32, 0xB33, 0xB34 },
                [0xB3D] = new int[] { 0xB3D, 0xB3E, 0xB3F, 0xB40 },
                [0xB49] = new int[] { 0xB49, 0xB4A, 0xB4B, 0xB4C },
                [0xB55] = new int[] { 0xB55, 0xB56, 0xB57, 0xB58 },
                [0xB79] = new int[] { 0xB79, 0xB7A, 0xB7B, 0xB7C },
                [0xC5A] = new int[] { 0xC5A, 0xC5B, 0xC5C, 0xC5D },
            },
            [3] = new Dictionary<int, int[]>                // The Hive
            {
                [0x1099] = new int[] { 0x1099, 0x109A, 0x109B, 0x109C },
                [0x10A5] = new int[] { 0x10A5, 0x10A6, 0x10A7, 0x10A8 },
                [0x10B1] = new int[] { 0x10B1, 0x10B2, 0x10B3, 0x10B4 },
                [0x10BD] = new int[] { 0x10BD, 0x10BE, 0x10BF, 0x10C0 },
                [0x10C9] = new int[] { 0x10C9, 0x10CA, 0x10CB, 0x10CC },
                [0x10D5] = new int[] { 0x10D5, 0x10D6, 0x10D7, 0x10D8 },
                [0x120A] = new int[] { 0x120A, 0x120B, 0x120C, 0x120D },
            },
        };

        private void SetSecondaryAmmoOffsets()
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            shotgunAmmoOffset2 = secondaryAmmoIndexMarker - 16;
            uziAmmoOffset2 = secondaryAmmoIndexMarker - 28;
            magnumAmmoOffset2 = secondaryAmmoIndexMarker - 40;
        }

        private int GetSecondaryAmmoIndexMarker()
        {
            byte levelIndex = GetLevelIndex();
            int ammoIndexMarker = -1;

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                Dictionary<int, int[]> indexData = ammoIndexData[levelIndex];

                for (int i = 0; i < indexData.Count; i++)
                {
                    int key = indexData.ElementAt(i).Key;
                    int[] offsets = indexData.ElementAt(i).Value;

                    if (offsets.All(offset => ReadByte(offset) == 0xFF))
                    {
                        ammoIndexMarker = key;
                        break;
                    }
                }
            }

            return ammoIndexMarker;
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

        private double GetHealthPercentage(int healthOffset)
        {
            UInt16 health = ReadUInt16(healthOffset);
            double healthPercentage = ((double)health / MAX_HEALTH_VALUE) * 100.0;

            return healthPercentage;
        }

        private void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
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
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(shotgunAmmoOffset, ammo);
                WriteUInt16(shotgunAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(shotgunAmmoOffset, ammo);
                WriteUInt16(shotgunAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(shotgunAmmoOffset, ammo);
            }
        }

        private void WriteMagnumAmmo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(magnumAmmoOffset, ammo);
                WriteUInt16(magnumAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(magnumAmmoOffset, ammo);
                WriteUInt16(magnumAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(magnumAmmoOffset, ammo);
            }
        }

        private void WriteUziAmmo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(uziAmmoOffset, ammo);
                WriteUInt16(uziAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(uziAmmoOffset, ammo);
                WriteUInt16(uziAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(uziAmmoOffset, ammo);
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

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteMagnumAmmo(chkMagnums.Checked, (UInt16)nudMagnumAmmo.Value);
            WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);

            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkMagnums.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;

            WriteWeaponsConfigNum(newWeaponsConfigNum);

            if (trbHealth.Enabled)
            {
                double newHealthPercentage = (double)trbHealth.Value;
                WriteHealthValue(newHealthPercentage);
            }
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
