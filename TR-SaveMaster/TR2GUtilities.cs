﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR2GUtilities
    {
        // Static offsets
        private const int saveNumberOffset = 0x04B;
        private const int levelIndexOffset = 0x483;

        // Dynamic offsets
        private int smallMedipackOffset;
        private int largeMedipackOffset;
        private int flaresOffset;
        private int weaponsConfigNumOffset;
        private int uziAmmoOffset;
        private int uziAmmoOffset2;
        private int automaticPistolsAmmoOffset;
        private int automaticPistolsAmmoOffset2;
        private int m16AmmoOffset;
        private int m16AmmoOffset2;
        private int grenadeLauncherAmmoOffset;
        private int grenadeLauncherAmmoOffset2;
        private int shotgunAmmoOffset;
        private int shotgunAmmoOffset2;
        private int harpoonGunAmmoOffset;
        private int harpoonGunAmmoOffset2;

        // Health
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private const UInt16 MIN_HEALTH_VALUE = 1;
        private int MAX_HEALTH_OFFSET;
        private int MIN_HEALTH_OFFSET;

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

            automaticPistolsAmmoOffset = 0x51 + (levelIndex * 0x2C);
            uziAmmoOffset = 0x53 + (levelIndex * 0x2C);
            shotgunAmmoOffset = 0x55 + (levelIndex * 0x2C);
            m16AmmoOffset = 0x57 + (levelIndex * 0x2C);
            grenadeLauncherAmmoOffset = 0x59 + (levelIndex * 0x2C);
            harpoonGunAmmoOffset = 0x5B + (levelIndex * 0x2C);
            smallMedipackOffset = 0x5D + (levelIndex * 0x2C);
            largeMedipackOffset = 0x5E + (levelIndex * 0x2C);
            flaresOffset = 0x60 + (levelIndex * 0x2C);
            weaponsConfigNumOffset = 0x63 + (levelIndex * 0x2C);

            if (levelIndex == 1)        // The Cold War
            {
                MIN_HEALTH_OFFSET = 0xE54;
                MAX_HEALTH_OFFSET = 0xE84;
            }
            else if (levelIndex == 2)   // Fool's Gold
            {
                MIN_HEALTH_OFFSET = 0x12D6;
                MAX_HEALTH_OFFSET = 0x12FA;
            }
            else if (levelIndex == 3)   // Furnace of the Gods
            {
                MIN_HEALTH_OFFSET = 0x1490;
                MAX_HEALTH_OFFSET = 0x14CC;
            }
            else if (levelIndex == 4)   // Kingdom
            {
                MIN_HEALTH_OFFSET = 0x600;
                MAX_HEALTH_OFFSET = 0x600;
            }
            else if (levelIndex == 5)   // Nightmare In Vegas
            {
                MIN_HEALTH_OFFSET = 0x8AE;
                MAX_HEALTH_OFFSET = 0x8C6;
            }
        }

        private readonly Dictionary<byte, int[]> ammoIndexData = new Dictionary<byte, int[]>
        {
            { 1, new int[] { 0x1C3A, 0x1C3B, 0x1C3C, 0x1C3D } },    // The Cold War
            { 2, new int[] { 0x1C44, 0x1C45, 0x1C46, 0x1C47 } },    // Fool's Gold
            { 3, new int[] { 0x1B34, 0x1B35, 0x1B36, 0x1B37 } },    // Furnace of the Gods
            { 4, new int[] { 0x1392, 0x1393, 0x1394, 0x1395 } },    // Kingdom
            { 5, new int[] { 0x1584, 0x1585, 0x1586, 0x1587 } },    // Nightmare in Vegas
        };

        private int GetSecondaryAmmoIndex()
        {
            byte levelIndex = GetLevelIndex();

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                int[] indexData = ammoIndexData[levelIndex];

                int[] offsets1 = new int[indexData.Length];
                int[] offsets2 = new int[indexData.Length];

                for (int index = 0; index < 20; index++)
                {
                    Array.Copy(indexData, offsets1, indexData.Length);

                    for (int i = 0; i < indexData.Length; i++)
                    {
                        offsets2[i] = offsets1[i] + 0xA;

                        offsets1[i] += (index * 0x6);
                        offsets2[i] += (index * 0x6);
                    }

                    if (offsets1.All(offset => ReadByte(offset) == 0xFF))
                    {
                        return index;
                    }

                    if (offsets2.All(offset => ReadByte(offset) == 0xFF))
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        private int GetSecondaryAmmoOffset(int baseOffset)
        {
            return baseOffset + (secondaryAmmoIndex * 0x6);
        }

        private void WriteNumSmallMedipacks(byte value)
        {
            WriteByte(smallMedipackOffset, value);
        }

        private void WriteNumLargeMedipacks(byte value)
        {
            WriteByte(largeMedipackOffset, value);
        }

        private void WriteNumFlares(byte value)
        {
            WriteByte(flaresOffset, value);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumberOffset, value);
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

        private void WriteAutomaticPistolsAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(automaticPistolsAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(automaticPistolsAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(automaticPistolsAmmoOffset2, 0);
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

        private void WriteM16Ammo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(m16AmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(m16AmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(m16AmmoOffset2, 0);
            }
        }

        private void WriteGrenadeLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(grenadeLauncherAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(grenadeLauncherAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(grenadeLauncherAmmoOffset2, 0);
            }
        }

        private void WriteHarpoonGunAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(harpoonGunAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(harpoonGunAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(harpoonGunAmmoOffset2, 0);
            }
        }

        private void WriteHealthValue(UInt16 newHealth)
        {
            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                WriteUInt16(healthOffset, newHealth);
            }
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        private byte GetLevelIndex()
        {
            return ReadByte(levelIndexOffset);
        }

        private byte GetWeaponsConfigNum()
        {
            return ReadByte(weaponsConfigNumOffset);
        }

        private byte GetNumSmallMedipacks()
        {
            return ReadByte(smallMedipackOffset);
        }

        private byte GetNumLargeMedipacks()
        {
            return ReadByte(largeMedipackOffset);
        }

        private byte GetNumFlares()
        {
            return ReadByte(flaresOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumberOffset);
        }

        private UInt16 GetAutomaticPistolsAmmo()
        {
            return ReadUInt16(automaticPistolsAmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private UInt16 GetShotgunAmmo()
        {
            return ReadUInt16(shotgunAmmoOffset);
        }

        private UInt16 GetM16Ammo()
        {
            return ReadUInt16(m16AmmoOffset);
        }

        private UInt16 GetGrenadeLauncherAmmo()
        {
            return ReadUInt16(grenadeLauncherAmmoOffset);
        }

        private UInt16 GetHarpoonGunAmmo()
        {
            return ReadUInt16(harpoonGunAmmoOffset);
        }

        public void SetLevelParams(CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun,
            NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudHarpoonGunAmmo)
        {
            if (GetLevelIndex() == 5)
            {
                chkM16.Enabled = false;
                chkGrenadeLauncher.Enabled = false;
                chkHarpoonGun.Enabled = false;
                nudM16Ammo.Enabled = false;
                nudGrenadeLauncherAmmo.Enabled = false;
                nudHarpoonGunAmmo.Enabled = false;
            }
            else
            {
                chkM16.Enabled = true;
                chkGrenadeLauncher.Enabled = true;
                chkHarpoonGun.Enabled = true;
                nudM16Ammo.Enabled = true;
                nudGrenadeLauncherAmmo.Enabled = true;
                nudHarpoonGunAmmo.Enabled = true;
            }
        }

        public void DisplayGameInfo(TextBox txtLvlName, CheckBox chkPistols, CheckBox chkAutomaticPistols, CheckBox chkUzis,
            CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudAutomaticPistolsAmmo,
            CheckBox chkShotgun, NumericUpDown nudUziAmmo, NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo,
            NumericUpDown nudHarpoonGunAmmo, NumericUpDown nudShotgunAmmo, NumericUpDown nudSaveNumber, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, TrackBar trbHealth, Label lblHealth,
            Label lblHealthError)
        {
            txtLvlName.Text = GetLvlName();

            nudSaveNumber.Value = GetSaveNumber();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();
            nudFlares.Value = GetNumFlares();
            nudAutomaticPistolsAmmo.Value = GetAutomaticPistolsAmmo();
            nudUziAmmo.Value = GetUziAmmo();
            nudShotgunAmmo.Value = GetShotgunAmmo() / 6;
            nudM16Ammo.Value = GetM16Ammo();
            nudGrenadeLauncherAmmo.Value = GetGrenadeLauncherAmmo();
            nudHarpoonGunAmmo.Value = GetHarpoonGunAmmo();

            byte weaponsConfigNum = GetWeaponsConfigNum();

            const byte Pistols = 2;
            const byte AutomaticPistols = 4;
            const byte Uzis = 8;
            const byte Shotgun = 16;
            const byte M16 = 32;
            const byte GrenadeLauncher = 64;
            const byte HarpoonGun = 128;

            if (weaponsConfigNum == 1)
            {
                chkPistols.Checked = false;
                chkShotgun.Checked = false;
                chkAutomaticPistols.Checked = false;
                chkUzis.Checked = false;
                chkM16.Checked = false;
                chkGrenadeLauncher.Checked = false;
                chkHarpoonGun.Checked = false;
            }
            else
            {
                chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
                chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
                chkAutomaticPistols.Checked = (weaponsConfigNum & AutomaticPistols) != 0;
                chkUzis.Checked = (weaponsConfigNum & Uzis) != 0;
                chkM16.Checked = (weaponsConfigNum & M16) != 0;
                chkGrenadeLauncher.Checked = (weaponsConfigNum & GrenadeLauncher) != 0;
                chkHarpoonGun.Checked = (weaponsConfigNum & HarpoonGun) != 0;
            }

            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                UInt16 health = GetHealthValue(healthOffset);
                double healthPercentage = ((double)health / MAX_HEALTH_VALUE) * 100;
                trbHealth.Value = health;
                trbHealth.Enabled = true;

                lblHealth.Text = healthPercentage.ToString("0.0") + "%";
                lblHealthError.Visible = false;
                lblHealth.Visible = true;
            }
            else
            {
                trbHealth.Enabled = false;
                trbHealth.Value = 1;
                lblHealthError.Visible = true;
                lblHealth.Visible = false;
            }
        }

        public void WriteChanges(CheckBox chkPistols, CheckBox chkAutomaticPistols, CheckBox chkUzis, CheckBox chkShotgun,
            CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudSaveNumber, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudAutomaticPistolsAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudHarpoonGunAmmo,
            NumericUpDown nudShotgunAmmo, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);
            WriteNumFlares((byte)nudFlares.Value);

            byte levelIndex = GetLevelIndex();
            secondaryAmmoIndex = GetSecondaryAmmoIndex();

            if (secondaryAmmoIndex != -1)
            {
                int baseSecondaryAmmoIndexOffset = ammoIndexData[levelIndex][0];

                automaticPistolsAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 28);
                uziAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 24);
                shotgunAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 20);
                harpoonGunAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 16);
                grenadeLauncherAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 12);
                m16AmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 8);
            }

            if (levelIndex != 5)
            {
                WriteM16Ammo(chkM16.Checked, (UInt16)nudM16Ammo.Value);
                WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
                WriteHarpoonGunAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            }

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);
            WriteAutomaticPistolsAmmo(chkAutomaticPistols.Checked, (UInt16)nudAutomaticPistolsAmmo.Value);

            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkAutomaticPistols.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;
            if (chkM16.Checked) newWeaponsConfigNum += 32;
            if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 64;
            if (chkHarpoonGun.Checked) newWeaponsConfigNum += 128;

            WriteWeaponsConfigNum(newWeaponsConfigNum);

            if (trbHealth.Enabled)
            {
                WriteHealthValue((UInt16)trbHealth.Value);
            }
        }

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;       // Standing
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x0D) return true;       // Underwater
            if (byteFlag1 == 0x12 && byteFlag2 == 0x00 && byteFlag3 == 0x12) return true;       // Swimming
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;       // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;       // On water

            return false;
        }

        private int GetHealthOffset()
        {
            for (int offset = MIN_HEALTH_OFFSET; offset <= MAX_HEALTH_OFFSET; offset += 0xC)
            {
                UInt16 value = ReadUInt16(offset);

                if (value >= MIN_HEALTH_VALUE && value <= MAX_HEALTH_VALUE)
                {
                    byte byteFlag1 = ReadByte(offset - 10);
                    byte byteFlag2 = ReadByte(offset - 9);
                    byte byteFlag3 = ReadByte(offset - 8);

                    if (IsKnownByteFlagPattern(byteFlag1, byteFlag2, byteFlag3))
                    {
                        return offset;
                    }
                }
            }

            return -1;
        }

        private UInt16 GetHealthValue(int healthOffset)
        {
            return ReadUInt16(healthOffset);
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

            byte levelIndex = GetLevelIndex();
            return (levelIndex >= 1 && levelIndex <= 5);
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
