using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR1Utilities
    {
        // Offsets
        private const int saveNumberOffset = 0x4B;
        private const int smallMedipackOffset = 0x192;
        private const int largeMedipackOffset = 0x193;
        private const int weaponsConfigNumOffset = 0x197;
        private const int magnumAmmoOffset = 0x18C;
        private const int uziAmmoOffset = 0x18E;
        private const int shotgunAmmoOffset = 0x190;
        private const int levelIndexOffset = 0x1A3;
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

            if (levelIndex == 1)        // Caves
            {
                magnumAmmoOffset2 = 0x641;
                uziAmmoOffset2 = 0x64D;
                shotgunAmmoOffset2 = 0x659;

                SetHealthOffsets(0x1E7);
            }
            else if (levelIndex == 2)   // City of Vilcabamba
            {
                magnumAmmoOffset2 = 0xC24;
                uziAmmoOffset2 = 0xC30;
                shotgunAmmoOffset2 = 0xC3C;

                SetHealthOffsets(0xB6C);
            }
            else if (levelIndex == 3)   // Lost Valley
            {
                magnumAmmoOffset2 = 0x598;
                uziAmmoOffset2 = 0x5A4;
                shotgunAmmoOffset2 = 0x5B0;

                SetHealthOffsets(0x1EF);
            }
            else if (levelIndex == 4)   // Tomb of Qualopec
            {
                magnumAmmoOffset2 = 0x84A;
                uziAmmoOffset2 = 0x856;
                shotgunAmmoOffset2 = 0x862;

                SetHealthOffsets(0x44E);
            }
            else if (levelIndex == 5)   // St. Francis' Folly
            {
                magnumAmmoOffset2 = 0xD70;
                uziAmmoOffset2 = 0xD7C;
                shotgunAmmoOffset2 = 0xD88;

                SetHealthOffsets(0xCB0);
            }
            else if (levelIndex == 6)   // Colosseum
            {
                magnumAmmoOffset2 = 0xA56;
                uziAmmoOffset2 = 0xA62;
                shotgunAmmoOffset2 = 0xA6E;

                SetHealthOffsets(0x5A9);
            }
            else if (levelIndex == 7)   // Palace Midas
            {
                magnumAmmoOffset2 = 0xD48;
                uziAmmoOffset2 = 0xD54;
                shotgunAmmoOffset2 = 0xD60;

                SetHealthOffsets(0x1F1);
            }
            else if (levelIndex == 8)   // The Cistern
            {
                magnumAmmoOffset2 = 0xC8A;
                uziAmmoOffset2 = 0xC96;
                shotgunAmmoOffset2 = 0xCA2;

                SetHealthOffsets(0xBE0);
            }
            else if (levelIndex == 9)   // Tomb of Tihocan
            {
                magnumAmmoOffset2 = 0x93D;
                uziAmmoOffset2 = 0x949;
                shotgunAmmoOffset2 = 0x955;

                SetHealthOffsets(0x2E9);
            }
            else if (levelIndex == 10)  // City of Khamoon
            {
                magnumAmmoOffset2 = 0x85D;
                uziAmmoOffset2 = 0x869;
                shotgunAmmoOffset2 = 0x875;

                SetHealthOffsets(0x1E9);
            }
            else if (levelIndex == 11)  // Obelisk of Khamoon
            {
                magnumAmmoOffset2 = 0x8A3;
                uziAmmoOffset2 = 0x8AF;
                shotgunAmmoOffset2 = 0x8BB;

                SetHealthOffsets(0x2DF);
            }
            else if (levelIndex == 12)  // Sanctuary of the Scion
            {
                magnumAmmoOffset2 = 0x718;
                uziAmmoOffset2 = 0x724;
                shotgunAmmoOffset2 = 0x730;

                SetHealthOffsets(0x63D);
            }
            else if (levelIndex == 13)  // Natla's Mines
            {
                magnumAmmoOffset2 = 0x8A8;
                uziAmmoOffset2 = 0x8B4;
                shotgunAmmoOffset2 = 0x8C0;

                SetHealthOffsets(0x750);
            }
            else if (levelIndex == 14)  // Atlantis
            {
                magnumAmmoOffset2 = 0xFFA;
                uziAmmoOffset2 = 0x1006;
                shotgunAmmoOffset2 = 0x1012;

                SetHealthOffsets(0x447);
            }
            else if (levelIndex == 15)  // The Great Pyramid
            {
                magnumAmmoOffset2 = 0x8D2;
                uziAmmoOffset2 = 0x8DE;
                shotgunAmmoOffset2 = 0x8EA;

                SetHealthOffsets(0x5AD);
            }
        }

        public void DisplayGameInfo(TextBox txtLvlName, CheckBox chkPistols, CheckBox chkShotgun, CheckBox chkUzis, CheckBox chkMagnums,
            NumericUpDown nudSaveNumber, NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudUziAmmo,
            NumericUpDown nudMagnumAmmo, NumericUpDown nudShotgunAmmo, TrackBar trbHealth, Label lblHealth, Label lblHealthError)
        {
            txtLvlName.Text = GetLvlName();

            nudSaveNumber.Value = GetSaveNumber();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();
            nudUziAmmo.Value = GetUziAmmo();
            nudMagnumAmmo.Value = GetMagnumAmmo();
            nudShotgunAmmo.Value = GetShotgunAmmo() / 6;

            byte weaponsConfigNum = GetWeaponsConfigNum();

            const byte Pistols = 2;
            const byte Magnums = 4;
            const byte Uzis = 8;
            const byte Shotgun = 16;

            if (weaponsConfigNum == 1)
            {
                chkPistols.Checked = false;
                chkShotgun.Checked = false;
                chkUzis.Checked = false;
                chkMagnums.Checked = false;
            }
            else
            {
                chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
                chkMagnums.Checked = (weaponsConfigNum & Magnums) != 0;
                chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
                chkUzis.Checked = (weaponsConfigNum & Uzis) != 0;
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

        public void WriteChanges(CheckBox chkPistols, CheckBox chkMagnums, CheckBox chkUzis, CheckBox chkShotgun,
            NumericUpDown nudSaveNumber, NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks,
            NumericUpDown nudUziAmmo, NumericUpDown nudMagnumAmmo, NumericUpDown nudShotgunAmmo,
            TrackBar trbHealth)
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

            WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);
            WriteMagnumAmmo(chkMagnums.Checked, (UInt16)nudMagnumAmmo.Value);
            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));

            if (trbHealth.Enabled)
            {
                double newHealthPercentage = (double)trbHealth.Value;
                WriteHealthValue(newHealthPercentage);
            }
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumberOffset);
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

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private UInt16 GetMagnumAmmo()
        {
            return ReadUInt16(magnumAmmoOffset);
        }

        private UInt16 GetShotgunAmmo()
        {
            return ReadUInt16(shotgunAmmoOffset);
        }

        private byte GetWeaponsConfigNum()
        {
            return ReadByte(weaponsConfigNumOffset);
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
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

        private void WriteUziAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(uziAmmoOffset, ammo);
                WriteUInt16(uziAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(uziAmmoOffset, ammo);
                WriteUInt16(uziAmmoOffset2, 0);
            }
        }

        private void WriteMagnumAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(magnumAmmoOffset, ammo);
                WriteUInt16(magnumAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(magnumAmmoOffset, ammo);
                WriteUInt16(magnumAmmoOffset2, 0);
            }
        }

        private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(shotgunAmmoOffset, ammo);
                WriteUInt16(shotgunAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(shotgunAmmoOffset, ammo);
                WriteUInt16(shotgunAmmoOffset2, 0);
            }
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

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;        // Standing
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;        // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;        // On water
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x0D) return true;        // Underwater

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

            byte levelIndex = GetLevelIndex();
            return (levelIndex >= 1 && levelIndex <= 15) && IsSavegameFile(path);
        }

        private static bool IsSavegameFile(string path)
        {
            string extension = Path.GetExtension(path);

            if (extension.StartsWith("."))
            {
                string numericPart = extension.Substring(".".Length);
                bool isNumeric = int.TryParse(numericPart, out _);
                bool isTxtExtension = extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);

                return isNumeric || isTxtExtension;
            }

            return false;
        }

        public List<string> GetSavegamePaths(string gameDirectory)
        {
            List<string> savegamePaths = new List<string>();

            if (Directory.Exists(gameDirectory))
            {
                var matchingFiles = Directory.GetFiles(gameDirectory).Where(file => IsSavegameFile(file)).ToList();
                matchingFiles = matchingFiles.OrderBy(file => file, new NaturalComparer()).ToList();
                savegamePaths.AddRange(matchingFiles);
            }

            return savegamePaths;
        }
    }
}