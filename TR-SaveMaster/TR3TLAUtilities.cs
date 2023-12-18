using System;
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
        private const int saveNumOffset = 0x4B;
        private int smallMedipackOffset;
        private int largeMedipackOffset;
        private int numFlaresOffset;
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
        private int harpoonAmmoOffset;
        private int harpoonAmmoOffset2;
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

        private double GetHealthPercentage()
        {
            int healthOffset = GetHealthOffset();

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

        private byte GetWeaponsConfigNum()
        {
            return ReadByte(weaponsConfigNumOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumOffset);
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

        private string GetCleanLvlName()
        {
            string lvlName = GetLvlName();
            lvlName = lvlName.Trim();

            if (lvlName.StartsWith("Highland Fling")) return "Highland Fling";
            else if (lvlName.StartsWith("Willard's Lair")) return "Willard's Lair";
            else if (lvlName.StartsWith("Shakespeare Cliff")) return "Shakespeare Cliff";
            else if (lvlName.StartsWith("Sleeping with the Fishes")) return "Sleeping with the Fishes";
            else if (lvlName.StartsWith("It's a Madhouse!")) return "It's a Madhouse!";
            else if (lvlName.StartsWith("Reunion")) return "Reunion";

            return null;
        }

        public void DetermineOffsets()
        {
            string lvlName = GetCleanLvlName();

            if (lvlName == "Highland Fling")
            {
                smallMedipackOffset = 0xE6;
                largeMedipackOffset = 0xE7;
                numFlaresOffset = 0xE9;

                weaponsConfigNumOffset = 0xED;
                harpoonGunOffset = 0xEE;

                deagleAmmoOffset = 0xD8;
                uziAmmoOffset = 0xDA;
                shotgunAmmoOffset = 0xDC;
                mp5AmmoOffset = 0xDE;
                rocketLauncherAmmoOffset = 0xE0;
                harpoonAmmoOffset = 0xE2;
                grenadeLauncherAmmoOffset = 0xE4;

                deagleAmmoOffset2 = 0x17DF;
                uziAmmoOffset2 = 0x17E3;
                shotgunAmmoOffset2 = 0x17E7;
                harpoonAmmoOffset2 = 0x17EB;
                rocketLauncherAmmoOffset2 = 0x17EF;
                grenadeLauncherAmmoOffset2 = 0x17F3;
                mp5AmmoOffset2 = 0x17F7;

                SetHealthOffsets(0x1435, 0x1447, 0x1459);
            }
            else if (lvlName == "Willard's Lair")
            {
                smallMedipackOffset = 0x119;
                largeMedipackOffset = 0x11A;
                numFlaresOffset = 0x11C;

                weaponsConfigNumOffset = 0x120;
                harpoonGunOffset = 0x121;

                deagleAmmoOffset = 0x10B;
                uziAmmoOffset = 0x10D;
                shotgunAmmoOffset = 0x10F;
                mp5AmmoOffset = 0x111;
                rocketLauncherAmmoOffset = 0x113;
                harpoonAmmoOffset = 0x115;
                grenadeLauncherAmmoOffset = 0x117;

                deagleAmmoOffset2 = 0x1ACB;
                uziAmmoOffset2 = 0x1ACF;
                shotgunAmmoOffset2 = 0x1AD3;
                harpoonAmmoOffset2 = 0x1AD7;
                rocketLauncherAmmoOffset2 = 0x1ADB;
                grenadeLauncherAmmoOffset2 = 0x1ADF;
                mp5AmmoOffset2 = 0x1AE3;

                SetHealthOffsets(0xF5B, 0xF6D);
            }
            else if (lvlName == "Shakespeare Cliff")
            {
                smallMedipackOffset = 0x14C;
                largeMedipackOffset = 0x14D;
                numFlaresOffset = 0x14F;

                weaponsConfigNumOffset = 0x153;
                harpoonGunOffset = 0x154;

                deagleAmmoOffset = 0x13E;
                uziAmmoOffset = 0x140;
                shotgunAmmoOffset = 0x142;
                mp5AmmoOffset = 0x144;
                rocketLauncherAmmoOffset = 0x146;
                harpoonAmmoOffset = 0x148;
                grenadeLauncherAmmoOffset = 0x14A;

                deagleAmmoOffset2 = 0x1AC4;
                uziAmmoOffset2 = 0x1AC8;
                shotgunAmmoOffset2 = 0x1ACC;
                harpoonAmmoOffset2 = 0x1AD0;
                rocketLauncherAmmoOffset2 = 0x1AD4;
                grenadeLauncherAmmoOffset2 = 0x1AD8;
                mp5AmmoOffset2 = 0x1ADC;

                SetHealthOffsets(0xCDB, 0xCED);
            }
            else if (lvlName == "Sleeping with the Fishes")
            {
                smallMedipackOffset = 0x17F;
                largeMedipackOffset = 0x180;
                numFlaresOffset = 0x182;

                weaponsConfigNumOffset = 0x186;
                harpoonGunOffset = 0x187;

                deagleAmmoOffset = 0x171;
                uziAmmoOffset = 0x173;
                shotgunAmmoOffset = 0x175;
                mp5AmmoOffset = 0x177;
                rocketLauncherAmmoOffset = 0x179;
                harpoonAmmoOffset = 0x17B;
                grenadeLauncherAmmoOffset = 0x17D;

                deagleAmmoOffset2 = 0x19A1;
                uziAmmoOffset2 = 0x19A5;
                shotgunAmmoOffset2 = 0x19A9;
                harpoonAmmoOffset2 = 0x19AD;
                rocketLauncherAmmoOffset2 = 0x19B1;
                grenadeLauncherAmmoOffset2 = 0x19B5;
                mp5AmmoOffset2 = 0x19B9;

                SetHealthOffsets(0x705);
            }
            else if (lvlName == "It's a Madhouse!")
            {
                smallMedipackOffset = 0x1B2;
                largeMedipackOffset = 0x1B3;
                numFlaresOffset = 0x1B5;

                weaponsConfigNumOffset = 0x1B9;
                harpoonGunOffset = 0x1BA;

                deagleAmmoOffset = 0x1A4;
                uziAmmoOffset = 0x1A6;
                shotgunAmmoOffset = 0x1A8;
                mp5AmmoOffset = 0x1AA;
                rocketLauncherAmmoOffset = 0x1AC;
                harpoonAmmoOffset = 0x1AE;
                grenadeLauncherAmmoOffset = 0x1B0;

                deagleAmmoOffset2 = 0x16EB;
                uziAmmoOffset2 = 0x16EF;
                shotgunAmmoOffset2 = 0x16F3;
                harpoonAmmoOffset2 = 0x16F7;
                rocketLauncherAmmoOffset2 = 0x16FB;
                grenadeLauncherAmmoOffset2 = 0x16FF;
                mp5AmmoOffset2 = 0x1703;

                SetHealthOffsets(0xB37, 0xB5B, 0xB6D, 0xB7F, 0xB91);
            }
            else if (lvlName == "Reunion")
            {
                smallMedipackOffset = 0x1E5;
                largeMedipackOffset = 0x1E6;
                numFlaresOffset = 0x1E8;

                weaponsConfigNumOffset = 0x1EC;
                harpoonGunOffset = 0x1ED;

                deagleAmmoOffset = 0x1D7;
                uziAmmoOffset = 0x1D9;
                shotgunAmmoOffset = 0x1DB;
                mp5AmmoOffset = 0x1DD;
                rocketLauncherAmmoOffset = 0x1DF;
                harpoonAmmoOffset = 0x1E1;
                grenadeLauncherAmmoOffset = 0x1E3;

                deagleAmmoOffset2 = 0x11F5;
                uziAmmoOffset2 = 0x11F9;
                shotgunAmmoOffset2 = 0x11FD;
                harpoonAmmoOffset2 = 0x1201;
                rocketLauncherAmmoOffset2 = 0x1205;
                grenadeLauncherAmmoOffset2 = 0x1209;
                mp5AmmoOffset2 = 0x120D;

                SetHealthOffsets(0x10FB, 0x110D, 0x111F);
            }
        }

        private readonly Dictionary<string, Dictionary<int, List<int[]>>> ammoIndexData =
            new Dictionary<string, Dictionary<int, List<int[]>>>
            {
                ["Highland Fling"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x17FF, 0x1800, 0x1801, 0x1802 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1811, 0x1812, 0x1813, 0x1814 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1823, 0x1824, 0x1825, 0x1826 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1835, 0x1836, 0x1837, 0x1838 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1847, 0x1848, 0x1849, 0x184A },
                    },
                },
                ["Willard's Lair"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1AEB, 0x1AEC, 0x1AED, 0x1AEF },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1AFD, 0x1AFE, 0x1AFF, 0x1B00 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1B0F, 0x1B10, 0x1B11, 0x1B12 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1B21, 0x1B22, 0x1B23, 0x1B24 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1B33, 0x1B34, 0x1B35, 0x1B36 },
                    },
                },
                ["Shakespeare Cliff"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1AE4, 0x1AE5, 0x1AE6, 0x1AE7 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1AF6, 0x1AF7, 0x1AF8, 0x1AF9 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1B08, 0x1B09, 0x1B0A, 0x1B0B },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1B1A, 0x1B1B, 0x1B1C, 0x1B1D },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1B2C, 0x1B2D, 0x1B2E, 0x1B2F },
                    },
                },
                ["Sleeping with the Fishes"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x19C1, 0x19C2, 0x19C3, 0x19C4 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x19D3, 0x19D4, 0x19D5, 0x19D6 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x19E5, 0x19E6, 0x19E7, 0x19E8 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x19F7, 0x19F8, 0x19F9, 0x19FA },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1A09, 0x1A0A, 0x1A0B, 0x1A0C },
                        new int[] { 0x1A13, 0x1A14, 0x1A15, 0x1A16 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1A1B, 0x1A1C, 0x1A1D, 0x1A1E },
                        new int[] { 0x1A25, 0x1A26, 0x1A27, 0x1A28 },
                    },
                },
                ["It's a Madhouse!"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x170B, 0x170C, 0x170D, 0x170E },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x171D, 0x171E, 0x171F, 0x1720 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x172F, 0x1730, 0x1731, 0x1732 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1741, 0x1742, 0x1743, 0x1744 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1753, 0x1754, 0x1755, 0x1756 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1765, 0x1766, 0x1767, 0x1768 },
                    },
                },
                ["Reunion"] = new Dictionary<int, List<int[]>>
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1215, 0x1216, 0x1217, 0x1218 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1227, 0x1228, 0x1229, 0x122A },
                        new int[] { 0x1231, 0x1232, 0x1233, 0x1234 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1239, 0x123A, 0x123B, 0x123C },
                        new int[] { 0x1243, 0x1244, 0x1245, 0x1246 },
                    },
                },
            };

        private int GetAmmoIndex()
        {
            string lvlName = GetCleanLvlName();
            int ammoIndex = 0;

            if (ammoIndexData.ContainsKey(lvlName))
            {
                Dictionary<int, List<int[]>> indexData = ammoIndexData[lvlName];

                for (int i = 0; i < indexData.Count; i++)
                {
                    int key = indexData.ElementAt(i).Key;
                    List<int[]> offsetsList = indexData.ElementAt(i).Value;

                    for (int j = 0; j < offsetsList.Count; j++)
                    {
                        int[] offsets = offsetsList[j];

                        if (offsets.All(offset => ReadByte(offset) == 0xFF))
                        {
                            ammoIndex = key;
                            break;
                        }
                    }
                }
            }

            return ammoIndex;
        }

        private int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
        {
            List<int> secondaryOffsets = new List<int>();
            List<int> validOffsets = new List<int>();

            int currentAmmoIndex = GetAmmoIndex();

            for (int i = 0; i < 10; i++)
            {
                secondaryOffsets.Add(baseSecondaryOffset + i * 0x12);
            }

            validOffsets.Add(primaryOffset);
            validOffsets.Add(secondaryOffsets[currentAmmoIndex]);

            return validOffsets.ToArray();
        }

        private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validShotgunAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validShotgunAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validShotgunAmmoOffsets[1], 0);
                WriteUInt16(shotgunAmmoOffset, ammo);
            }
        }

        private void WriteDeagleAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validDeagleAmmoOffsets = GetValidAmmoOffsets(deagleAmmoOffset, deagleAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validDeagleAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validDeagleAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validDeagleAmmoOffsets[1], 0);
                WriteUInt16(deagleAmmoOffset, ammo);
            }
        }

        private void WriteGrenadeLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validGrenadeLauncherAmmoOffsets = GetValidAmmoOffsets(grenadeLauncherAmmoOffset, grenadeLauncherAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validGrenadeLauncherAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validGrenadeLauncherAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[1], 0);
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
            }
        }

        private void WriteRocketLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validRocketLauncherAmmoOffsets = GetValidAmmoOffsets(rocketLauncherAmmoOffset, rocketLauncherAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validRocketLauncherAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validRocketLauncherAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[1], 0);
                WriteUInt16(rocketLauncherAmmoOffset, ammo);
            }
        }

        private void WriteHarpoonAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validHarpoonAmmoOffsets = GetValidAmmoOffsets(harpoonAmmoOffset, harpoonAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validHarpoonAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validHarpoonAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validHarpoonAmmoOffsets[1], 0);
                WriteUInt16(harpoonAmmoOffset, ammo);
            }
        }

        private void WriteMP5Ammo(bool isPresent, UInt16 ammo)
        {
            int[] validMp5AmmoOffsets = GetValidAmmoOffsets(mp5AmmoOffset, mp5AmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validMp5AmmoOffsets.Length; i++)
                {
                    WriteUInt16(validMp5AmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validMp5AmmoOffsets[1], 0);
                WriteUInt16(mp5AmmoOffset, ammo);
            }
        }

        private void WriteUziAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validUziAmmoOffsets = GetValidAmmoOffsets(uziAmmoOffset, uziAmmoOffset2);

            if (isPresent)
            {
                for (int i = 0; i < validUziAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validUziAmmoOffsets[i], ammo);
                }
            }
            else
            {
                WriteUInt16(validUziAmmoOffsets[1], 0);
                WriteUInt16(uziAmmoOffset, ammo);
            }
        }

        private void WriteHealthValue(double newHealthPercentage)
        {
            int healthOffset = GetHealthOffset();

            UInt16 newHealth = (UInt16)(newHealthPercentage / 100.0 * MAX_HEALTH_VALUE);
            WriteUInt16(healthOffset, newHealth);
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
            return ReadUInt16(harpoonAmmoOffset);
        }

        private UInt16 GetMP5Ammo()
        {
            return ReadUInt16(mp5AmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private void WriteSmallMedipacks(byte value)
        {
            WriteByte(smallMedipackOffset, value);
        }

        private void WriteLargeMedipacks(byte value)
        {
            WriteByte(largeMedipackOffset, value);
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumOffset, value);
        }

        private void WriteFlares(byte value)
        {
            WriteByte(numFlaresOffset, value);
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
            nudSmallMedipacks.Value = ReadByte(smallMedipackOffset);
            nudLargeMedipacks.Value = ReadByte(largeMedipackOffset);
            nudFlares.Value = ReadByte(numFlaresOffset);
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

            if (GetHealthOffset() != -1)
            {
                double healthPercentage = GetHealthPercentage();
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
            WriteFlares((byte)nudFlares.Value);
            WriteSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteLargeMedipacks((byte)nudLargeMedipacks.Value);

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

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteDeagleAmmo(chkDeagle.Checked, (UInt16)nudDeagleAmmo.Value);
            WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
            WriteRocketLauncherAmmo(chkRocketLauncher.Checked, (UInt16)nudRocketLauncherAmmo.Value);
            WriteHarpoonAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            WriteMP5Ammo(chkMP5.Checked, (UInt16)nudMP5Ammo.Value);
            WriteUziAmmo(chkUzi.Checked, (UInt16)nudUziAmmo.Value);

            if (GetHealthOffset() != -1)
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

            return GetCleanLvlName() != null;
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
