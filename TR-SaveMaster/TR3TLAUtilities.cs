﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR3TLAUtilities
    {
        // Offsets
        private const int saveNumberOffset = 0x4B;
        private const int levelIndexOffset = 0x5C6;
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
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;   // Standing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;   // On water
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x12) return true;   // Underwater

            return false;
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

        public void DetermineOffsets()
        {
            byte levelIndex = GetLevelIndex();

            if (levelIndex == 1)        // Highland Fling
            {
                smallMedipackOffset = 0xE6;
                largeMedipackOffset = 0xE7;
                flaresOffset = 0xE9;

                weaponsConfigNumOffset = 0xED;
                harpoonGunOffset = 0xEE;

                deagleAmmoOffset = 0xD8;
                uziAmmoOffset = 0xDA;
                shotgunAmmoOffset = 0xDC;
                mp5AmmoOffset = 0xDE;
                rocketLauncherAmmoOffset = 0xE0;
                harpoonGunAmmoOffset = 0xE2;
                grenadeLauncherAmmoOffset = 0xE4;

                deagleAmmoOffset2 = 0x17DF;
                uziAmmoOffset2 = 0x17E3;
                shotgunAmmoOffset2 = 0x17E7;
                harpoonGunAmmoOffset2 = 0x17EB;
                rocketLauncherAmmoOffset2 = 0x17EF;
                grenadeLauncherAmmoOffset2 = 0x17F3;
                mp5AmmoOffset2 = 0x17F7;

                SetHealthOffsets(0x1435, 0x1447, 0x1459);
            }
            else if (levelIndex == 2)   // Willard's Lair
            {
                smallMedipackOffset = 0x119;
                largeMedipackOffset = 0x11A;
                flaresOffset = 0x11C;

                weaponsConfigNumOffset = 0x120;
                harpoonGunOffset = 0x121;

                deagleAmmoOffset = 0x10B;
                uziAmmoOffset = 0x10D;
                shotgunAmmoOffset = 0x10F;
                mp5AmmoOffset = 0x111;
                rocketLauncherAmmoOffset = 0x113;
                harpoonGunAmmoOffset = 0x115;
                grenadeLauncherAmmoOffset = 0x117;

                deagleAmmoOffset2 = 0x1ACB;
                uziAmmoOffset2 = 0x1ACF;
                shotgunAmmoOffset2 = 0x1AD3;
                harpoonGunAmmoOffset2 = 0x1AD7;
                rocketLauncherAmmoOffset2 = 0x1ADB;
                grenadeLauncherAmmoOffset2 = 0x1ADF;
                mp5AmmoOffset2 = 0x1AE3;

                SetHealthOffsets(0xF5B, 0xF6D);
            }
            else if (levelIndex == 3)   // Shakespeare Cliff
            {
                smallMedipackOffset = 0x14C;
                largeMedipackOffset = 0x14D;
                flaresOffset = 0x14F;

                weaponsConfigNumOffset = 0x153;
                harpoonGunOffset = 0x154;

                deagleAmmoOffset = 0x13E;
                uziAmmoOffset = 0x140;
                shotgunAmmoOffset = 0x142;
                mp5AmmoOffset = 0x144;
                rocketLauncherAmmoOffset = 0x146;
                harpoonGunAmmoOffset = 0x148;
                grenadeLauncherAmmoOffset = 0x14A;

                deagleAmmoOffset2 = 0x1AC4;
                uziAmmoOffset2 = 0x1AC8;
                shotgunAmmoOffset2 = 0x1ACC;
                harpoonGunAmmoOffset2 = 0x1AD0;
                rocketLauncherAmmoOffset2 = 0x1AD4;
                grenadeLauncherAmmoOffset2 = 0x1AD8;
                mp5AmmoOffset2 = 0x1ADC;

                SetHealthOffsets(0xCDB, 0xCED);
            }
            else if (levelIndex == 4)   // Sleeping with the Fishes
            {
                smallMedipackOffset = 0x17F;
                largeMedipackOffset = 0x180;
                flaresOffset = 0x182;

                weaponsConfigNumOffset = 0x186;
                harpoonGunOffset = 0x187;

                deagleAmmoOffset = 0x171;
                uziAmmoOffset = 0x173;
                shotgunAmmoOffset = 0x175;
                mp5AmmoOffset = 0x177;
                rocketLauncherAmmoOffset = 0x179;
                harpoonGunAmmoOffset = 0x17B;
                grenadeLauncherAmmoOffset = 0x17D;

                deagleAmmoOffset2 = 0x19A1;
                uziAmmoOffset2 = 0x19A5;
                shotgunAmmoOffset2 = 0x19A9;
                harpoonGunAmmoOffset2 = 0x19AD;
                rocketLauncherAmmoOffset2 = 0x19B1;
                grenadeLauncherAmmoOffset2 = 0x19B5;
                mp5AmmoOffset2 = 0x19B9;

                SetHealthOffsets(0x705);
            }
            else if (levelIndex == 5)   // It's a Madhouse!
            {
                smallMedipackOffset = 0x1B2;
                largeMedipackOffset = 0x1B3;
                flaresOffset = 0x1B5;

                weaponsConfigNumOffset = 0x1B9;
                harpoonGunOffset = 0x1BA;

                deagleAmmoOffset = 0x1A4;
                uziAmmoOffset = 0x1A6;
                shotgunAmmoOffset = 0x1A8;
                mp5AmmoOffset = 0x1AA;
                rocketLauncherAmmoOffset = 0x1AC;
                harpoonGunAmmoOffset = 0x1AE;
                grenadeLauncherAmmoOffset = 0x1B0;

                deagleAmmoOffset2 = 0x16EB;
                uziAmmoOffset2 = 0x16EF;
                shotgunAmmoOffset2 = 0x16F3;
                harpoonGunAmmoOffset2 = 0x16F7;
                rocketLauncherAmmoOffset2 = 0x16FB;
                grenadeLauncherAmmoOffset2 = 0x16FF;
                mp5AmmoOffset2 = 0x1703;

                SetHealthOffsets(0xB37, 0xB5B, 0xB6D, 0xB7F, 0xB91);
            }
            else if (levelIndex == 6)   // Reunion
            {
                smallMedipackOffset = 0x1E5;
                largeMedipackOffset = 0x1E6;
                flaresOffset = 0x1E8;

                weaponsConfigNumOffset = 0x1EC;
                harpoonGunOffset = 0x1ED;

                deagleAmmoOffset = 0x1D7;
                uziAmmoOffset = 0x1D9;
                shotgunAmmoOffset = 0x1DB;
                mp5AmmoOffset = 0x1DD;
                rocketLauncherAmmoOffset = 0x1DF;
                harpoonGunAmmoOffset = 0x1E1;
                grenadeLauncherAmmoOffset = 0x1E3;

                deagleAmmoOffset2 = 0x11F5;
                uziAmmoOffset2 = 0x11F9;
                shotgunAmmoOffset2 = 0x11FD;
                harpoonGunAmmoOffset2 = 0x1201;
                rocketLauncherAmmoOffset2 = 0x1205;
                grenadeLauncherAmmoOffset2 = 0x1209;
                mp5AmmoOffset2 = 0x120D;

                SetHealthOffsets(0x10FB, 0x110D, 0x111F);
            }
        }

        private readonly Dictionary<byte, int[]> ammoIndexData = new Dictionary<byte, int[]>
        {
            { 1, new int[] { 0x17FF, 0x1800, 0x1801, 0x1802 } },    // Highland Fling
            { 2, new int[] { 0x1AEB, 0x1AEC, 0x1AED, 0x1AEE } },    // Willard's Lair
            { 3, new int[] { 0x1AE4, 0x1AE5, 0x1AE6, 0x1AE7 } },    // Shakespeare Cliff
            { 4, new int[] { 0x19C1, 0x19C2, 0x19C3, 0x19C4 } },    // Sleeping with the Fishes
            { 5, new int[] { 0x170B, 0x170C, 0x170D, 0x170E } },    // It's a Madhouse!
            { 6, new int[] { 0x1215, 0x1216, 0x1217, 0x1218 } },    // Reunion
        };

        private int GetSecondaryAmmoIndex()
        {
            byte levelIndex = GetLevelIndex();

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                int[] indexData = ammoIndexData[levelIndex];

                int[] offsets1 = new int[indexData.Length];
                int[] offsets2 = new int[indexData.Length];

                for (int index = 0; index < 15; index++)
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

        private int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
        {
            List<int> secondaryOffsets = new List<int>();
            List<int> validOffsets = new List<int>();

            for (int i = 0; i < 15; i++)
            {
                secondaryOffsets.Add(baseSecondaryOffset + i * 0x12);
            }

            validOffsets.Add(primaryOffset);

            if (secondaryAmmoIndex != -1)
            {
                validOffsets.Add(secondaryOffsets[secondaryAmmoIndex]);
            }

            return validOffsets.ToArray();
        }

        private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validShotgunAmmoOffsets[0], ammo);
                WriteUInt16(validShotgunAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validShotgunAmmoOffsets[0], ammo);
                WriteUInt16(validShotgunAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validShotgunAmmoOffsets[0], ammo);
            }
        }

        private void WriteDeagleAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validDeagleAmmoOffsets = GetValidAmmoOffsets(deagleAmmoOffset, deagleAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validDeagleAmmoOffsets[0], ammo);
                WriteUInt16(validDeagleAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validDeagleAmmoOffsets[0], ammo);
                WriteUInt16(validDeagleAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validDeagleAmmoOffsets[0], ammo);
            }
        }

        private void WriteGrenadeLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validGrenadeLauncherAmmoOffsets = GetValidAmmoOffsets(grenadeLauncherAmmoOffset, grenadeLauncherAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validGrenadeLauncherAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validGrenadeLauncherAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[0], ammo);
            }
        }

        private void WriteRocketLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validRocketLauncherAmmoOffsets = GetValidAmmoOffsets(rocketLauncherAmmoOffset, rocketLauncherAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validRocketLauncherAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validRocketLauncherAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[0], ammo);
            }
        }

        private void WriteHarpoonAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validHarpoonGunAmmoOffsets = GetValidAmmoOffsets(harpoonGunAmmoOffset, harpoonGunAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validHarpoonGunAmmoOffsets[0], ammo);
                WriteUInt16(validHarpoonGunAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validHarpoonGunAmmoOffsets[0], ammo);
                WriteUInt16(validHarpoonGunAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validHarpoonGunAmmoOffsets[0], ammo);
            }
        }

        private void WriteMP5Ammo(bool isPresent, UInt16 ammo)
        {
            int[] validMp5AmmoOffsets = GetValidAmmoOffsets(mp5AmmoOffset, mp5AmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validMp5AmmoOffsets[0], ammo);
                WriteUInt16(validMp5AmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validMp5AmmoOffsets[0], ammo);
                WriteUInt16(validMp5AmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validMp5AmmoOffsets[0], ammo);
            }
        }

        private void WriteUziAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validUziAmmoOffsets = GetValidAmmoOffsets(uziAmmoOffset, uziAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validUziAmmoOffsets[0], ammo);
                WriteUInt16(validUziAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validUziAmmoOffsets[0], ammo);
                WriteUInt16(validUziAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validUziAmmoOffsets[0], ammo);
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

        private void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
            }
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

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumberOffset, value);
        }

        private void WriteNumFlares(byte value)
        {
            WriteByte(flaresOffset, value);
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

        public void WriteChanges(CheckBox chkPistols, CheckBox chkDeagle, CheckBox chkUzi, CheckBox chkShotgun, CheckBox chkMP5,
            CheckBox chkRocketLauncher, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudSaveNumber, NumericUpDown nudShotgunAmmo,
            NumericUpDown nudDeagleAmmo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudRocketLauncherAmmo,
            NumericUpDown nudHarpoonGunAmmo, NumericUpDown nudMP5Ammo, NumericUpDown nudUziAmmo, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
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

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteDeagleAmmo(chkDeagle.Checked, (UInt16)nudDeagleAmmo.Value);
            WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
            WriteRocketLauncherAmmo(chkRocketLauncher.Checked, (UInt16)nudRocketLauncherAmmo.Value);
            WriteHarpoonAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            WriteMP5Ammo(chkMP5.Checked, (UInt16)nudMP5Ammo.Value);
            WriteUziAmmo(chkUzi.Checked, (UInt16)nudUziAmmo.Value);

            if (trbHealth.Enabled)
            {
                WriteHealthValue((double)trbHealth.Value);
            }
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
            return (levelIndex >= 1 && levelIndex <= 6) && IsSavegameFile(path);
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
