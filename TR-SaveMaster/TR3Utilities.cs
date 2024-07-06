using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR3Utilities
    {
        // Static offsets
        private const int saveNumberOffset = 0x04B;
        private const int levelIndexOffset = 0x5C6;

        // Dynamic offsets
        private int smallMedipackOffset;
        private int largeMedipackOffset;
        private int flaresOffset;
        private int weaponsConfigNumOffset;
        private int harpoonGunOffset;
        private int shotgunAmmoOffset;
        private int shotgunAmmoOffset2;
        private int deagleAmmoOffset;
        private int deagleAmmoOffset2;
        private int grenadeLauncherAmmoOffset;
        private int grenadeLauncherAmmoOffset2;
        private int rocketLauncherAmmoOffset;
        private int rocketLauncherAmmoOffset2;
        private int harpoonGunAmmoOffset;
        private int harpoonGunAmmoOffset2;
        private int mp5AmmoOffset;
        private int mp5AmmoOffset2;
        private int uziAmmoOffset;
        private int uziAmmoOffset2;

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

            smallMedipackOffset = 0xB3 + (levelIndex * 0x33);
            largeMedipackOffset = 0xB4 + (levelIndex * 0x33);
            flaresOffset = 0xB6 + (levelIndex * 0x33);
            weaponsConfigNumOffset = 0xBA + (levelIndex * 0x33);
            harpoonGunOffset = 0xBB + (levelIndex * 0x33);
            deagleAmmoOffset = 0xA5 + (levelIndex * 0x33);
            uziAmmoOffset = 0xA7 + (levelIndex * 0x33);
            shotgunAmmoOffset = 0xA9 + (levelIndex * 0x33);
            mp5AmmoOffset = 0xAB + (levelIndex * 0x33);
            rocketLauncherAmmoOffset = 0xAD + (levelIndex * 0x33);
            harpoonGunAmmoOffset = 0xAF + (levelIndex * 0x33);
            grenadeLauncherAmmoOffset = 0xB1 + (levelIndex * 0x33);

            if (levelIndex == 1)        // Jungle
            {
                MIN_HEALTH_OFFSET = 0x6D3;
                MAX_HEALTH_OFFSET = 0x6D3;
            }
            else if (levelIndex == 2)   // Temple Ruins
            {
                MIN_HEALTH_OFFSET = 0x8F7;
                MAX_HEALTH_OFFSET = 0x909;
            }
            else if (levelIndex == 3)   // The River Ganges
            {
                MIN_HEALTH_OFFSET = 0x6B9;
                MAX_HEALTH_OFFSET = 0x6B9;
            }
            else if (levelIndex == 4)   // Caves Of Kaliya
            {
                MIN_HEALTH_OFFSET = 0xB05;
                MAX_HEALTH_OFFSET = 0xB05;
            }
            else if (levelIndex == 13)  // Nevada Desert
            {
                MIN_HEALTH_OFFSET = 0x6B5;
                MAX_HEALTH_OFFSET = 0x6B5;
            }
            else if (levelIndex == 14)  // High Security Compound
            {
                MIN_HEALTH_OFFSET = 0x6F7;
                MAX_HEALTH_OFFSET = 0x709;
            }
            else if (levelIndex == 15)  // Area 51
            {
                MIN_HEALTH_OFFSET = 0xC45;
                MAX_HEALTH_OFFSET = 0xC57;
            }
            else if (levelIndex == 5)   // Coastal Village
            {
                MIN_HEALTH_OFFSET = 0x7BB;
                MAX_HEALTH_OFFSET = 0x7BB;
            }
            else if (levelIndex == 6)   // Crash Site
            {
                MIN_HEALTH_OFFSET = 0x1785;
                MAX_HEALTH_OFFSET = 0x17BB;
            }
            else if (levelIndex == 7)   // Madubu Gorge
            {
                MIN_HEALTH_OFFSET = 0xBE3;
                MAX_HEALTH_OFFSET = 0xBF5;
            }
            else if (levelIndex == 8)   // Temple Of Puna
            {
                MIN_HEALTH_OFFSET = 0x68F;
                MAX_HEALTH_OFFSET = 0x68F;
            }
            else if (levelIndex == 9)   // Thames Wharf
            {
                MIN_HEALTH_OFFSET = 0xB15;
                MAX_HEALTH_OFFSET = 0xB39;
            }
            else if (levelIndex == 10)  // Aldwych
            {
                MIN_HEALTH_OFFSET = 0x2135;
                MAX_HEALTH_OFFSET = 0x2147;
            }
            else if (levelIndex == 11)  // Lud's Gate
            {
                MIN_HEALTH_OFFSET = 0xAB1;
                MAX_HEALTH_OFFSET = 0xAD5;
            }
            else if (levelIndex == 12)  // City
            {
                MIN_HEALTH_OFFSET = 0x737;
                MAX_HEALTH_OFFSET = 0x737;
            }
            else if (levelIndex == 16)  // Antarctica
            {
                MIN_HEALTH_OFFSET = 0x6C5;
                MAX_HEALTH_OFFSET = 0x6C5;
            }
            else if (levelIndex == 17)  // RX-Tech Mines
            {
                MIN_HEALTH_OFFSET = 0xA65;
                MAX_HEALTH_OFFSET = 0xA77;
            }
            else if (levelIndex == 18)  // Lost City Of Tinnos
            {
                MIN_HEALTH_OFFSET = 0x711;
                MAX_HEALTH_OFFSET = 0x711;
            }
            else if (levelIndex == 19)  // Meteorite Cavern
            {
                MIN_HEALTH_OFFSET = 0x68D;
                MAX_HEALTH_OFFSET = 0x68D;
            }
            else if (levelIndex == 20)  // All Hallows
            {
                MIN_HEALTH_OFFSET = 0xAF5;
                MAX_HEALTH_OFFSET = 0xB19;
            }
        }

        public void DisplayGameInfo(CheckBox chkPistols, CheckBox chkShotgun, CheckBox chkDeagle, CheckBox chkUzi, CheckBox chkMP5,
            CheckBox chkRocketLauncher, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, TextBox txtLvlName,
            NumericUpDown nudSaveNumber, NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudFlares,
            NumericUpDown nudShotgunAmmo, NumericUpDown nudDeagleAmmo, NumericUpDown nudGrenadeLauncherAmmo,
            NumericUpDown nudRocketLauncherAmmo, NumericUpDown nudHarpoonGunAmmo, NumericUpDown nudMP5Ammo, NumericUpDown nudUziAmmo,
            TrackBar trbHealth, Label lblHealth, Label lblHealthError)
        {
            txtLvlName.Text = GetLvlName();

            nudSaveNumber.Value = GetSaveNumber();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();
            nudFlares.Value = GetNumFlares();
            nudShotgunAmmo.Value = GetShotgunAmmo() / 6;
            nudDeagleAmmo.Value = GetDeagleAmmo();
            nudGrenadeLauncherAmmo.Value = GetGrenadeLauncherAmmo();
            nudRocketLauncherAmmo.Value = GetRocketLauncherAmmo();
            nudHarpoonGunAmmo.Value = GetHarpoonGunAmmo();
            nudMP5Ammo.Value = GetMP5Ammo();
            nudUziAmmo.Value = GetUziAmmo();

            byte weaponsConfigNum = GetWeaponsConfigNum();

            const byte Pistols = 2;
            const byte Deagle = 4;
            const byte Uzis = 8;
            const byte Shotgun = 16;
            const byte MP5 = 32;
            const byte RocketLauncher = 64;
            const byte GrenadeLauncher = 128;

            if (weaponsConfigNum == 1)
            {
                chkPistols.Checked = false;
                chkShotgun.Checked = false;
                chkDeagle.Checked = false;
                chkUzi.Checked = false;
                chkMP5.Checked = false;
                chkRocketLauncher.Checked = false;
                chkGrenadeLauncher.Checked = false;
            }
            else
            {
                chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
                chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
                chkDeagle.Checked = (weaponsConfigNum & Deagle) != 0;
                chkUzi.Checked = (weaponsConfigNum & Uzis) != 0;
                chkMP5.Checked = (weaponsConfigNum & MP5) != 0;
                chkRocketLauncher.Checked = (weaponsConfigNum & RocketLauncher) != 0;
                chkGrenadeLauncher.Checked = (weaponsConfigNum & GrenadeLauncher) != 0;
            }

            chkHarpoonGun.Checked = IsHarpoonGunPresent();

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

        public void WriteChanges(CheckBox chkPistols, CheckBox chkDeagle, CheckBox chkUzi, CheckBox chkShotgun, CheckBox chkMP5,
            CheckBox chkRocketLauncher, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudSaveNumber, NumericUpDown nudShotgunAmmo,
            NumericUpDown nudDeagleAmmo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudRocketLauncherAmmo,
            NumericUpDown nudHarpoonGunAmmo, NumericUpDown nudMP5Ammo, NumericUpDown nudUziAmmo, TrackBar trbHealth)
        {
            WriteSaveNumber((byte)nudSaveNumber.Value);
            WriteNumFlares((byte)nudFlares.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);

            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkDeagle.Checked) newWeaponsConfigNum += 4;
            if (chkUzi.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;
            if (chkMP5.Checked) newWeaponsConfigNum += 32;
            if (chkRocketLauncher.Checked) newWeaponsConfigNum += 64;
            if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 128;

            WriteWeaponsConfigNum(newWeaponsConfigNum);
            WriteHarpoonGunPresent(chkHarpoonGun.Checked);

            secondaryAmmoIndex = GetSecondaryAmmoIndex();

            if (secondaryAmmoIndex != -1)
            {
                byte levelIndex = GetLevelIndex();
                int baseSecondaryAmmoIndexOffset = ammoIndexData[levelIndex][0];

                deagleAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 32);
                uziAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 28);
                shotgunAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 24);
                harpoonGunAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 20);
                rocketLauncherAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 16);
                grenadeLauncherAmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 12);
                mp5AmmoOffset2 = GetSecondaryAmmoOffset(baseSecondaryAmmoIndexOffset - 8);
            }

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteDeagleAmmo(chkDeagle.Checked, (UInt16)nudDeagleAmmo.Value);
            WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
            WriteRocketLauncherAmmo(chkRocketLauncher.Checked, (UInt16)nudRocketLauncherAmmo.Value);
            WriteHarpoonGunAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            WriteMP5Ammo(chkMP5.Checked, (UInt16)nudMP5Ammo.Value);
            WriteUziAmmo(chkUzi.Checked, (UInt16)nudUziAmmo.Value);

            if (trbHealth.Enabled)
            {
                WriteHealthValue((UInt16)trbHealth.Value);
            }
        }

        private readonly Dictionary<byte, int[]> ammoIndexData = new Dictionary<byte, int[]>
        {
            { 1,  new int[] { 0x1663, 0x1664, 0x1665, 0x1666 } },   // Jungle
            { 2,  new int[] { 0x23D3, 0x23D4, 0x23D5, 0x23D6 } },   // Temple Ruins
            { 3,  new int[] { 0x181C, 0x181D, 0x181E, 0x181F } },   // The River Ganges
            { 4,  new int[] { 0x0D37, 0x0D38, 0x0D39, 0x0D3A } },   // Caves of Kaliya
            { 13, new int[] { 0x17BC, 0x17BD, 0x17BE, 0x17BF } },   // Nevada Desert
            { 14, new int[] { 0x1E63, 0x1E64, 0x1E65, 0x1E66 } },   // High Security Compound
            { 15, new int[] { 0x2125, 0x2126, 0x2127, 0x2128 } },   // Area 51
            { 5,  new int[] { 0x17C9, 0x17CA, 0x17CB, 0x17CC } },   // Coastal Village
            { 6,  new int[] { 0x18EB, 0x18EC, 0x18ED, 0x18EE } },   // Crash Site
            { 7,  new int[] { 0x1435, 0x1436, 0x1437, 0x1438 } },   // Madubu Gorge
            { 8,  new int[] { 0x110D, 0x110E, 0x110F, 0x1110 } },   // Temple of Puna
            { 9,  new int[] { 0x188B, 0x188C, 0x188D, 0x188E } },   // Thames Wharf
            { 10, new int[] { 0x2317, 0x2318, 0x2319, 0x231A } },   // Aldwych
            { 11, new int[] { 0x1D8F, 0x1D90, 0x1D91, 0x1D92 } },   // Lud's Gate
            { 12, new int[] { 0x0B0B, 0x0B0C, 0x0B0D, 0x0B0E } },   // City
            { 16, new int[] { 0x19AD, 0x19AE, 0x19AF, 0x19B0 } },   // Antarctica
            { 17, new int[] { 0x196F, 0x1970, 0x1971, 0x1972 } },   // RX-Tech Mines
            { 18, new int[] { 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2 } },   // Lost City of Tinnos
            { 19, new int[] { 0x0B01, 0x0B02, 0x0B03, 0x0B04 } },   // Meteorite Cavern
            { 20, new int[] { 0x1045, 0x1046, 0x1047, 0x1048 } },   // All Hallows
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

                        offsets1[i] += (index * 0x12);
                        offsets2[i] += (index * 0x12);
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
            return baseOffset + (secondaryAmmoIndex * 0x12);
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

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;       // Standing
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;       // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;       // On water
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x12) return true;       // Underwater
            if (byteFlag1 == 0x57 && byteFlag2 == 0x00 && byteFlag3 == 0x57) return true;       // Climbing 2

            return false;
        }

        private int GetHealthOffset()
        {
            for (int offset = MIN_HEALTH_OFFSET; offset <= MAX_HEALTH_OFFSET; offset += 0x12)
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

        private byte GetLevelIndex()
        {
            return ReadByte(levelIndexOffset);
        }

        private byte GetWeaponsConfigNum()
        {
            return ReadByte(weaponsConfigNumOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumberOffset);
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

        private bool IsHarpoonGunPresent()
        {
            return ReadByte(harpoonGunOffset) != 0;
        }

        private UInt16 GetShotgunAmmo()
        {
            return ReadUInt16(shotgunAmmoOffset);
        }

        private UInt16 GetDeagleAmmo()
        {
            return ReadUInt16(deagleAmmoOffset);
        }

        private UInt16 GetGrenadeLauncherAmmo()
        {
            return ReadUInt16(grenadeLauncherAmmoOffset);
        }

        private UInt16 GetRocketLauncherAmmo()
        {
            return ReadUInt16(rocketLauncherAmmoOffset);
        }

        private UInt16 GetHarpoonGunAmmo()
        {
            return ReadUInt16(harpoonGunAmmoOffset);
        }

        private UInt16 GetMP5Ammo()
        {
            return ReadUInt16(mp5AmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
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

        private void WriteSaveNumber(byte value)
        {
            WriteByte(saveNumberOffset, value);
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        private void WriteHarpoonGunPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(harpoonGunOffset, 1);
            }
            else
            {
                WriteByte(harpoonGunOffset, 0);
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

        private void WriteDeagleAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(deagleAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(deagleAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(deagleAmmoOffset2, 0);
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

        private void WriteRocketLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(rocketLauncherAmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(rocketLauncherAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(rocketLauncherAmmoOffset2, 0);
            }
        }

        private void WriteHarpoonGunAmmo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(harpoonGunAmmoOffset, ammo);

            if (secondaryAmmoIndex != -1)
            {
                WriteUInt16(harpoonGunAmmoOffset2, ammo);
            }
        }

        private void WriteMP5Ammo(bool isPresent, UInt16 ammo)
        {
            WriteUInt16(mp5AmmoOffset, ammo);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(mp5AmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(mp5AmmoOffset2, 0);
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

        private void WriteHealthValue(UInt16 newHealth)
        {
            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                WriteUInt16(healthOffset, newHealth);
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

            byte levelIndex = GetLevelIndex();
            return (levelIndex >= 1 && levelIndex <= 20);
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
