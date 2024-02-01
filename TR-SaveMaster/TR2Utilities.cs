using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR2Utilities
    {
        // Offsets
        private const int saveNumberOffset = 0x4B;
        private const int levelIndexOffset = 0x483;
        private int weaponsConfigNumOffset;
        private int smallMedipackOffset;
        private int largeMedipackOffset;
        private int flaresOffset;
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

            if (levelIndex == 1)        // The Great Wall
            {
                weaponsConfigNumOffset = 0x08F;

                smallMedipackOffset = 0x089;
                largeMedipackOffset = 0x08A;
                flaresOffset = 0x08C;

                automaticPistolsAmmoOffset = 0x7D;
                uziAmmoOffset = 0x7F;
                shotgunAmmoOffset = 0x81;
                m16AmmoOffset = 0x83;
                grenadeLauncherAmmoOffset = 0x85;
                harpoonGunAmmoOffset = 0x87;

                SetHealthOffsets(0x778);
            }
            else if (levelIndex == 2)   // Venice
            {
                weaponsConfigNumOffset = 0x0BB;

                smallMedipackOffset = 0x0B5;
                largeMedipackOffset = 0x0B6;
                flaresOffset = 0x0B8;

                automaticPistolsAmmoOffset = 0xA9;
                uziAmmoOffset = 0xAB;
                shotgunAmmoOffset = 0xAD;
                m16AmmoOffset = 0xAF;
                grenadeLauncherAmmoOffset = 0xB1;
                harpoonGunAmmoOffset = 0xB3;

                SetHealthOffsets(0x556);
            }
            else if (levelIndex == 3)   // Bartoli's Hideout
            {
                weaponsConfigNumOffset = 0x0E7;

                smallMedipackOffset = 0x0E1;
                largeMedipackOffset = 0x0E2;
                flaresOffset = 0x0E4;

                automaticPistolsAmmoOffset = 0xD5;
                uziAmmoOffset = 0xD7;
                shotgunAmmoOffset = 0xD9;
                m16AmmoOffset = 0xDB;
                grenadeLauncherAmmoOffset = 0xDD;
                harpoonGunAmmoOffset = 0xDF;

                SetHealthOffsets(0xE48);
            }
            else if (levelIndex == 4)   // Opera House
            {
                weaponsConfigNumOffset = 0x113;

                smallMedipackOffset = 0x10D;
                largeMedipackOffset = 0x10E;
                flaresOffset = 0x110;

                automaticPistolsAmmoOffset = 0x101;
                uziAmmoOffset = 0x103;
                shotgunAmmoOffset = 0x105;
                m16AmmoOffset = 0x107;
                grenadeLauncherAmmoOffset = 0x109;
                harpoonGunAmmoOffset = 0x10B;

                SetHealthOffsets(0x12A0, 0x12AC);
            }
            else if (levelIndex == 5)   // Offshore Rig
            {
                weaponsConfigNumOffset = 0x013F;

                smallMedipackOffset = 0x139;
                largeMedipackOffset = 0x13A;
                flaresOffset = 0x13C;

                automaticPistolsAmmoOffset = 0x12D;
                uziAmmoOffset = 0x12F;
                shotgunAmmoOffset = 0x131;
                m16AmmoOffset = 0x133;
                grenadeLauncherAmmoOffset = 0x135;
                harpoonGunAmmoOffset = 0x137;

                SetHealthOffsets(0x6F0, 0x708);
            }
            else if (levelIndex == 6)   // Diving Area
            {
                weaponsConfigNumOffset = 0x16B;

                smallMedipackOffset = 0x165;
                largeMedipackOffset = 0x166;
                flaresOffset = 0x168;

                automaticPistolsAmmoOffset = 0x159;
                uziAmmoOffset = 0x15B;
                shotgunAmmoOffset = 0x15D;
                m16AmmoOffset = 0x15F;
                grenadeLauncherAmmoOffset = 0x161;
                harpoonGunAmmoOffset = 0x163;

                SetHealthOffsets(0xBD4, 0xBE0, 0xBEC);
            }
            else if (levelIndex == 7)   // 40 Fathoms
            {
                weaponsConfigNumOffset = 0x197;

                smallMedipackOffset = 0x191;
                largeMedipackOffset = 0x192;
                flaresOffset = 0x194;

                automaticPistolsAmmoOffset = 0x185;
                uziAmmoOffset = 0x187;
                shotgunAmmoOffset = 0x189;
                m16AmmoOffset = 0x18B;
                grenadeLauncherAmmoOffset = 0x18D;
                harpoonGunAmmoOffset = 0x18F;

                SetHealthOffsets(0x558);
            }
            else if (levelIndex == 8)   // Wreck of the Maria Doria
            {
                weaponsConfigNumOffset = 0x1C3;

                smallMedipackOffset = 0x1BD;
                largeMedipackOffset = 0x1BE;
                flaresOffset = 0x1C0;

                automaticPistolsAmmoOffset = 0x1B1;
                uziAmmoOffset = 0x1B3;
                shotgunAmmoOffset = 0x1B5;
                m16AmmoOffset = 0x1B7;
                grenadeLauncherAmmoOffset = 0x1B9;
                harpoonGunAmmoOffset = 0x1BB;

                SetHealthOffsets(0x1612, 0x161E, 0x1642);
            }
            else if (levelIndex == 9)   // Living Quarters
            {
                weaponsConfigNumOffset = 0x1EF;

                smallMedipackOffset = 0x1E9;
                largeMedipackOffset = 0x1EA;
                flaresOffset = 0x1EC;

                automaticPistolsAmmoOffset = 0x1DD;
                uziAmmoOffset = 0x1DF;
                shotgunAmmoOffset = 0x1E1;
                m16AmmoOffset = 0x1E3;
                grenadeLauncherAmmoOffset = 0x1E5;
                harpoonGunAmmoOffset = 0x1E7;

                SetHealthOffsets(0x5F0);
            }
            else if (levelIndex == 10)  // The Deck
            {
                weaponsConfigNumOffset = 0x21B;

                smallMedipackOffset = 0x215;
                largeMedipackOffset = 0x216;
                flaresOffset = 0x218;

                automaticPistolsAmmoOffset = 0x209;
                uziAmmoOffset = 0x20B;
                shotgunAmmoOffset = 0x20D;
                m16AmmoOffset = 0x20F;
                grenadeLauncherAmmoOffset = 0x211;
                harpoonGunAmmoOffset = 0x213;

                SetHealthOffsets(0x7C4, 0x7E8, 0x7D0);
            }
            else if (levelIndex == 11)  // Tibetan Foothills
            {
                weaponsConfigNumOffset = 0x247;

                smallMedipackOffset = 0x241;
                largeMedipackOffset = 0x242;
                flaresOffset = 0x244;

                automaticPistolsAmmoOffset = 0x235;
                uziAmmoOffset = 0x237;
                shotgunAmmoOffset = 0x239;
                m16AmmoOffset = 0x23B;
                grenadeLauncherAmmoOffset = 0x23D;
                harpoonGunAmmoOffset = 0x23F;

                SetHealthOffsets(0xC8E, 0xCBE, 0xCA6);
            }
            else if (levelIndex == 12)  // Barkhang Monastery
            {
                weaponsConfigNumOffset = 0x273;

                smallMedipackOffset = 0x26D;
                largeMedipackOffset = 0x26E;
                flaresOffset = 0x270;

                automaticPistolsAmmoOffset = 0x261;
                uziAmmoOffset = 0x263;
                shotgunAmmoOffset = 0x265;
                m16AmmoOffset = 0x267;
                grenadeLauncherAmmoOffset = 0x269;
                harpoonGunAmmoOffset = 0x26B;

                SetHealthOffsets(0x167A, 0x1686, 0x1692, 0x169E);
            }
            else if (levelIndex == 13)  // Catacombs of the Talion
            {
                weaponsConfigNumOffset = 0x29F;

                smallMedipackOffset = 0x299;
                largeMedipackOffset = 0x29A;
                flaresOffset = 0x29C;

                automaticPistolsAmmoOffset = 0x28D;
                uziAmmoOffset = 0x28F;
                shotgunAmmoOffset = 0x291;
                m16AmmoOffset = 0x293;
                grenadeLauncherAmmoOffset = 0x295;
                harpoonGunAmmoOffset = 0x297;

                SetHealthOffsets(0x554);
            }
            else if (levelIndex == 14)  // Ice Palace
            {
                weaponsConfigNumOffset = 0x2CB;

                smallMedipackOffset = 0x2C5;
                largeMedipackOffset = 0x2C6;
                flaresOffset = 0x2C8;

                automaticPistolsAmmoOffset = 0x2B9;
                uziAmmoOffset = 0x2BB;
                shotgunAmmoOffset = 0x2BD;
                m16AmmoOffset = 0x2BF;
                grenadeLauncherAmmoOffset = 0x2C1;
                harpoonGunAmmoOffset = 0x2C3;

                SetHealthOffsets(0x91A, 0x932);
            }
            else if (levelIndex == 15)  // Temple of Xian
            {
                weaponsConfigNumOffset = 0x2F7;

                smallMedipackOffset = 0x2F1;
                largeMedipackOffset = 0x2F2;
                flaresOffset = 0x2F4;

                automaticPistolsAmmoOffset = 0x2E5;
                uziAmmoOffset = 0x2E7;
                shotgunAmmoOffset = 0x2E9;
                m16AmmoOffset = 0x2EB;
                grenadeLauncherAmmoOffset = 0x2ED;
                harpoonGunAmmoOffset = 0x2EF;

                SetHealthOffsets(0x196C, 0x1984, 0x19A8, 0x199C);
            }
            else if (levelIndex == 16)  // Floating Islands
            {
                weaponsConfigNumOffset = 0x323;

                smallMedipackOffset = 0x31D;
                largeMedipackOffset = 0x31E;
                flaresOffset = 0x320;

                automaticPistolsAmmoOffset = 0x311;
                uziAmmoOffset = 0x313;
                shotgunAmmoOffset = 0x315;
                m16AmmoOffset = 0x317;
                grenadeLauncherAmmoOffset = 0x319;
                harpoonGunAmmoOffset = 0x31B;

                SetHealthOffsets(0x676);
            }
            else if (levelIndex == 17)  // The Dragon's Lair
            {
                weaponsConfigNumOffset = 0x34F;

                smallMedipackOffset = 0x349;
                largeMedipackOffset = 0x34A;
                flaresOffset = 0x34C;

                automaticPistolsAmmoOffset = 0x33D;
                uziAmmoOffset = 0x33F;
                shotgunAmmoOffset = 0x341;
                m16AmmoOffset = 0x343;
                grenadeLauncherAmmoOffset = 0x345;
                harpoonGunAmmoOffset = 0x347;

                SetHealthOffsets(0x9F0);
            }
            else if (levelIndex == 18)  // Home Sweet Home
            {
                weaponsConfigNumOffset = 0x37B;

                smallMedipackOffset = 0x375;
                largeMedipackOffset = 0x376;
                flaresOffset = 0x378;

                shotgunAmmoOffset = 0x36D;

                SetHealthOffsets(0x974, 0x980);
            }

            SetSecondaryAmmoOffsets();
        }

        private readonly Dictionary<byte, Dictionary<int, List<int[]>>> ammoIndexData =
            new Dictionary<byte, Dictionary<int, List<int[]>>>
            {
                [1] = new Dictionary<int, List<int[]>>              // The Great Wall
                {
                    [0x0FE0] = new List<int[]>
                    {
                        new int[] { 0x0FE0, 0x0FE1, 0x0FE2, 0x0FE3 },
                        new int[] { 0x0FEA, 0x0FEB, 0x0FEC, 0x0FED },
                    },
                    [0x0FEC] = new List<int[]>
                    {
                        new int[] { 0x0FEC, 0x0FED, 0x0FEE, 0x0FEF },
                        new int[] { 0x0FF6, 0x0FF7, 0x0FF8, 0x0FF9 },
                    },
                    [0x0FF8] = new List<int[]>
                    {
                        new int[] { 0x0FF8, 0x0FF9, 0x0FFA, 0x0FFB },
                        new int[] { 0x1002, 0x1003, 0x1004, 0x1005 },
                    },
                    [0x1004] = new List<int[]>
                    {
                        new int[] { 0x1004, 0x1005, 0x1006, 0x1007 },
                        new int[] { 0x100E, 0x100F, 0x1010, 0x1011 },
                    },
                    [0x1010] = new List<int[]>
                    {
                        new int[] { 0x1010, 0x1011, 0x1012, 0x1013 },
                        new int[] { 0x101F, 0x1020, 0x1021, 0x1022 },
                    },
                },
                [2] = new Dictionary<int, List<int[]>>              // Venice
                {
                    [0x10EC] = new List<int[]>
                    {
                        new int[] { 0x10EC, 0x10ED, 0x10EE, 0x10EF },
                        new int[] { 0x10F6, 0x10F7, 0x10F8, 0x10F9 },
                    },
                    [0x10F8] = new List<int[]>
                    {
                        new int[] { 0x10F8, 0x10F9, 0x10FA, 0x10FB },
                        new int[] { 0x1102, 0x1103, 0x1104, 0x1105 },
                    },
                    [0x1104] = new List<int[]>
                    {
                        new int[] { 0x1104, 0x1105, 0x1106, 0x1107 },
                        new int[] { 0x110E, 0x110F, 0x1110, 0x1111 },
                    },
                    [0x1110] = new List<int[]>
                    {
                        new int[] { 0x1110, 0x1111, 0x1112, 0x1113 },
                        new int[] { 0x111A, 0x111B, 0x111C, 0x111D },
                    },
                    [0x111C] = new List<int[]>
                    {
                        new int[] { 0x111C, 0x111D, 0x111E, 0x111F },
                        new int[] { 0x1126, 0x1127, 0x1128, 0x1129 },
                    },
                },
                [3] = new Dictionary<int, List<int[]>>              // Bartoli's Hideout
                {
                    [0x12B4] = new List<int[]>
                    {
                        new int[] { 0x12B4, 0x12B5, 0x12B6, 0x12B7 },
                        new int[] { 0x12BE, 0x12BF, 0x12C0, 0x12C1 },
                    },
                    [0x12C0] = new List<int[]>
                    {
                        new int[] { 0x12C0, 0x12C1, 0x12C2, 0x12C3 },
                        new int[] { 0x12CA, 0x12CB, 0x12CC, 0x12CD },
                    },
                    [0x12CC] = new List<int[]>
                    {
                        new int[] { 0x12CC, 0x12CD, 0x12CE, 0x12CF },
                        new int[] { 0x12D6, 0x12D7, 0x12D8, 0x12D9 },
                    },
                    [0x12D8] = new List<int[]>
                    {
                        new int[] { 0x12D8, 0x12D9, 0x12DA, 0x12DB },
                        new int[] { 0x12E2, 0x12E3, 0x12E4, 0x12E5 },
                    },
                    [0x12E4] = new List<int[]>
                    {
                        new int[] { 0x12E4, 0x12E5, 0x12E6, 0x12E7 },
                        new int[] { 0x12EE, 0x12EF, 0x12F0, 0x12F1 },
                    },
                },
                [4] = new Dictionary<int, List<int[]>>              // Opera House
                {
                    [0x19EE] = new List<int[]>
                    {
                        new int[] { 0x19EE, 0x19EF, 0x19F0, 0x19F1 },
                        new int[] { 0x19F8, 0x19F9, 0x19FA, 0x19FB },
                    },
                    [0x19FA] = new List<int[]>
                    {
                        new int[] { 0x19FA, 0x19FB, 0x19FC, 0x19FD },
                        new int[] { 0x1A04, 0x1A05, 0x1A06, 0x1A07 },
                    },
                    [0x1A06] = new List<int[]>
                    {
                        new int[] { 0x1A06, 0x1A07, 0x1A08, 0x1A09 },
                        new int[] { 0x1A10, 0x1A11, 0x1A12, 0x1A13 },
                    },
                    [0x1A12] = new List<int[]>
                    {
                        new int[] { 0x1A12, 0x1A13, 0x1A14, 0x1A15 },
                        new int[] { 0x1A1C, 0x1A1D, 0x1A1E, 0x1A1F },
                    },
                    [0x1A1E] = new List<int[]>
                    {
                        new int[] { 0x1A1E, 0x1A1F, 0x1A20, 0x1A21 },
                        new int[] { 0x1A28, 0x1A29, 0x1A2A, 0x1A2B },
                    },
                    [0x1A2A] = new List<int[]>
                    {
                        new int[] { 0x1A2A, 0x1A2B, 0x1A2C, 0x1A2D },
                        new int[] { 0x1A34, 0x1A35, 0x1A36, 0x1A37 },
                    },
                },
                [5] = new Dictionary<int, List<int[]>>              // Offshore Rig
                {
                    [0x1020] = new List<int[]>
                    {
                        new int[] { 0x1020, 0x1021, 0x1022, 0x1023 },
                        new int[] { 0x102A, 0x102B, 0x102C, 0x102D },
                    },
                    [0x102C] = new List<int[]>
                    {
                        new int[] { 0x102C, 0x102D, 0x102E, 0x102F },
                        new int[] { 0x1036, 0x1037, 0x1038, 0x1039 },
                    },
                    [0x1038] = new List<int[]>
                    {
                        new int[] { 0x1038, 0x1039, 0x103A, 0x103B },
                        new int[] { 0x1042, 0x1043, 0x1044, 0x1045 },
                    },
                    [0x1044] = new List<int[]>
                    {
                        new int[] { 0x1044, 0x1045, 0x1046, 0x1047 },
                        new int[] { 0x104E, 0x104F, 0x1050, 0x1051 },
                    },
                },
                [6] = new Dictionary<int, List<int[]>>              // Diving Area
                {
                    [0x1274] = new List<int[]>
                    {
                        new int[] { 0x1274, 0x1275, 0x1276, 0x1277 },
                        new int[] { 0x127E, 0x127F, 0x1280, 0x1281 },
                    },
                    [0x1280] = new List<int[]>
                    {
                        new int[] { 0x1280, 0x1281, 0x1282, 0x1283 },
                        new int[] { 0x128A, 0x128B, 0x128C, 0x128D },
                    },
                    [0x128C] = new List<int[]>
                    {
                        new int[] { 0x128C, 0x128D, 0x128E, 0x128F },
                        new int[] { 0x1296, 0x1297, 0x1298, 0x1299 },
                    },
                    [0x1298] = new List<int[]>
                    {
                        new int[] { 0x1298, 0x1299, 0x129A, 0x129B },
                        new int[] { 0x12A2, 0x12A3, 0x12A4, 0x12A5 },
                    },
                    [0x12A4] = new List<int[]>
                    {
                        new int[] { 0x12A4, 0x12A5, 0x12A6, 0x12A7 },
                        new int[] { 0x12AE, 0x12AF, 0x12B0, 0x12B1 },
                    },
                    [0x12B0] = new List<int[]>
                    {
                        new int[] { 0x12B0, 0x12B1, 0x12B2, 0x12B3 },
                        new int[] { 0x12BA, 0x12BB, 0x12BC, 0x12BD },
                    }
                },
                [7] = new Dictionary<int, List<int[]>>              // 40 Fathoms
                {
                    [0x0C0E] = new List<int[]>
                    {
                        new int[] { 0x0C0E, 0x0C0F, 0x0C10, 0x0C11 },
                        new int[] { 0x0C18, 0x0C19, 0x0C1A, 0x0C1B },
                    },
                    [0x0C1A] = new List<int[]>
                    {
                        new int[] { 0x0C1A, 0x0C1B, 0x0C1C, 0x0C1D },
                        new int[] { 0x0C24, 0x0C25, 0x0C26, 0x0C27 },
                    },
                    [0x0C24] = new List<int[]>
                    {
                        new int[] { 0x0C24, 0x0C25, 0x0C26, 0x0C27 },
                        new int[] { 0x0C2E, 0x0C2F, 0x0C30, 0x0C31 },
                    },
                    [0x0C26] = new List<int[]>
                    {
                        new int[] { 0x0C26, 0x0C27, 0x0C28, 0x0C29 },
                        new int[] { 0x0C30, 0x0C31, 0x0C32, 0x0C33 },
                    },
                    [0x0C32] = new List<int[]>
                    {
                        new int[] { 0x0C32, 0x0C33, 0x0C34, 0x0C35 },
                        new int[] { 0x0C3C, 0x0C3D, 0x0C3E, 0x0C3F },
                    },
                },
                [8] = new Dictionary<int, List<int[]>>              // Wreck of the Maria Doria
                {
                    [0x16F4] = new List<int[]>
                    {
                        new int[] { 0x16F4, 0x16F5, 0x16F6, 0x16F7 },
                        new int[] { 0x16FE, 0x16FF, 0x1700, 0x1701 },
                    },
                    [0x1700] = new List<int[]>
                    {
                        new int[] { 0x1700, 0x1701, 0x1702, 0x1703 },
                        new int[] { 0x170A, 0x170B, 0x170C, 0x170D },
                    },
                    [0x170C] = new List<int[]>
                    {
                        new int[] { 0x170C, 0x170D, 0x170E, 0x170F },
                        new int[] { 0x1716, 0x1717, 0x1718, 0x1719 },
                    },
                    [0x1718] = new List<int[]>
                    {
                        new int[] { 0x1718, 0x1719, 0x171A, 0x171B },
                        new int[] { 0x1722, 0x1723, 0x1724, 0x1725 },
                    },
                    [0x1724] = new List<int[]>
                    {
                        new int[] { 0x1724, 0x1725, 0x1726, 0x1727 },
                        new int[] { 0x172E, 0x172F, 0x1730, 0x1731 },
                    },
                },
                [9] = new Dictionary<int, List<int[]>>              // Living Quarters
                {
                    [0x0EA4] = new List<int[]>
                    {
                        new int[] { 0x0EA4, 0x0EA5, 0x0EA6, 0x0EA7 },
                        new int[] { 0x0EAE, 0x0EAF, 0x0EB0, 0x0EB1 },
                    },
                    [0x0EB0] = new List<int[]>
                    {
                        new int[] { 0x0EB0, 0x0EB1, 0x0EB2, 0x0EB3 },
                        new int[] { 0x0EBA, 0x0EBB, 0x0EBC, 0x0EBD },
                    },
                    [0x0EBC] = new List<int[]>
                    {
                        new int[] { 0x0EBC, 0x0EBD, 0x0EBE, 0x0EBF },
                        new int[] { 0x0EC6, 0x0EC7, 0x0EC8, 0x0EC9 },
                    },
                    [0x0EC8] = new List<int[]>
                    {
                        new int[] { 0x0EC8, 0x0EC9, 0x0ECA, 0x0ECB },
                        new int[] { 0x0ED2, 0x0ED3, 0x0ED4, 0x0ED5 },
                    },
                },
                [10] = new Dictionary<int, List<int[]>>             // The Deck
                {
                    [0x11C8] = new List<int[]>
                    {
                        new int[] { 0x11C8, 0x11C9, 0x11CA, 0x11CB },
                        new int[] { 0x11D2, 0x11D3, 0x11D4, 0x11D5 },
                    },
                    [0x11D4] = new List<int[]>
                    {
                        new int[] { 0x11D4, 0x11D5, 0x11D6, 0x11D7 },
                        new int[] { 0x11DE, 0x11DF, 0x11E0, 0x11E1 },
                    },
                    [0x11E0] = new List<int[]>
                    {
                        new int[] { 0x11E0, 0x11E1, 0x11E2, 0x11E3 },
                        new int[] { 0x11EA, 0x11EB, 0x11EC, 0x11ED },
                    },
                    [0x11EC] = new List<int[]>
                    {
                        new int[] { 0x11EC, 0x11ED, 0x11EE, 0x11EF },
                        new int[] { 0x11F6, 0x11F7, 0x11F8, 0x11F9 },
                    },
                },
                [11] = new Dictionary<int, List<int[]>>             // Tibetan Foothills
                {
                    [0x1402] = new List<int[]>
                    {
                        new int[] { 0x1402, 0x1403, 0x1404, 0x1405 },
                        new int[] { 0x140C, 0x140D, 0x140E, 0x140F },
                    },
                    [0x140E] = new List<int[]>
                    {
                        new int[] { 0x140E, 0x140F, 0x1410, 0x1411 },
                        new int[] { 0x1418, 0x1419, 0x141A, 0x141B },
                    },
                    [0x1414] = new List<int[]>
                    {
                        new int[] { 0x1414, 0x1415, 0x1416, 0x1417 },
                        new int[] { 0x141E, 0x141F, 0x1420, 0x1421 },
                    },
                    [0x141A] = new List<int[]>
                    {
                        new int[] { 0x141A, 0x141B, 0x141C, 0x141D },
                        new int[] { 0x1424, 0x1425, 0x1426, 0x1427 },
                    },
                    [0x1426] = new List<int[]>
                    {
                        new int[] { 0x1426, 0x1427, 0x1428, 0x1429 },
                        new int[] { 0x1430, 0x1431, 0x1432, 0x1433 },
                    },
                    [0x1432] = new List<int[]>
                    {
                        new int[] { 0x1432, 0x1433, 0x1434, 0x1435 },
                        new int[] { 0x143C, 0x143D, 0x143E, 0x143F },
                    },
                    [0x143E] = new List<int[]>
                    {
                        new int[] { 0x143E, 0x143F, 0x1440, 0x1441 },
                        new int[] { 0x1448, 0x1449, 0x144A, 0x144B },
                    },
                    [0x142C] = new List<int[]>
                    {
                        new int[] { 0x142C, 0x142D, 0x142E, 0x142F },
                        new int[] { 0x1436, 0x1437, 0x1438, 0x1439 },
                    },
                    [0x144A] = new List<int[]>
                    {
                        new int[] { 0x144A, 0x144B, 0x144C, 0x144D },
                        new int[] { 0x1454, 0x1455, 0x1456, 0x1457 },
                    },
                },
                [12] = new Dictionary<int, List<int[]>>             // Barkhang Monastery
                {
                    [0x1972] = new List<int[]>
                    {
                        new int[] { 0x1972, 0x1973, 0x1974, 0x1975 },
                        new int[] { 0x197C, 0x197D, 0x197E, 0x197F },
                    },
                    [0x197E] = new List<int[]>
                    {
                        new int[] { 0x197E, 0x197F, 0x1980, 0x1981 },
                        new int[] { 0x1988, 0x1989, 0x198A, 0x198B },
                    },
                    [0x198A] = new List<int[]>
                    {
                        new int[] { 0x198A, 0x198B, 0x198C, 0x198D },
                        new int[] { 0x1994, 0x1995, 0x1996, 0x1997 },
                    },
                    [0x1996] = new List<int[]>
                    {
                        new int[] { 0x1996, 0x1997, 0x1998, 0x1999 },
                        new int[] { 0x19A0, 0x19A1, 0x19A2, 0x19A3 },
                    },
                    [0x19A2] = new List<int[]>
                    {
                        new int[] { 0x19A2, 0x19A3, 0x19A4, 0x19A5 },
                        new int[] { 0x19AC, 0x19AD, 0x19AE, 0x19AF },
                    },
                },
                [13] = new Dictionary<int, List<int[]>>             // Catacombs of the Talion
                {
                    [0x1522] = new List<int[]>
                    {
                        new int[] { 0x1522, 0x1523, 0x1524, 0x1525 },
                        new int[] { 0x152C, 0x152D, 0x152E, 0x152F },
                    },
                    [0x152E] = new List<int[]>
                    {
                        new int[] { 0x152E, 0x152F, 0x1530, 0x1531 },
                        new int[] { 0x1538, 0x1539, 0x153A, 0x153B },
                    },
                    [0x153A] = new List<int[]>
                    {
                        new int[] { 0x153A, 0x153B, 0x153C, 0x153D },
                        new int[] { 0x1544, 0x1545, 0x1546, 0x1547 },
                    },
                    [0x1546] = new List<int[]>
                    {
                        new int[] { 0x1546, 0x1547, 0x1548, 0x1549 },
                        new int[] { 0x1550, 0x1551, 0x1552, 0x1553 },
                    },
                },
                [14] = new Dictionary<int, List<int[]>>             // Ice Palace
                {
                    [0x122A] = new List<int[]>
                    {
                        new int[] { 0x122A, 0x122B, 0x122C, 0x122D },
                        new int[] { 0x1234, 0x1235, 0x1236, 0x1237 },
                    },
                    [0x1236] = new List<int[]>
                    {
                        new int[] { 0x1236, 0x1237, 0x1238, 0x1239 },
                        new int[] { 0x1240, 0x1241, 0x1242, 0x1243 },
                    },
                    [0x1242] = new List<int[]>
                    {
                        new int[] { 0x1242, 0x1243, 0x1244, 0x1245 },
                        new int[] { 0x124C, 0x124D, 0x124E, 0x124F },
                    },
                    [0x124E] = new List<int[]>
                    {
                        new int[] { 0x124E, 0x124F, 0x1250, 0x1251 },
                        new int[] { 0x1258, 0x1259, 0x125A, 0x125B },
                    },
                    [0x125A] = new List<int[]>
                    {
                        new int[] { 0x125A, 0x125B, 0x125C, 0x125D },
                        new int[] { 0x1264, 0x1265, 0x1266, 0x1267 },
                    },
                    [0x1266] = new List<int[]>
                    {
                        new int[] { 0x1266, 0x1267, 0x1268, 0x1269 },
                        new int[] { 0x1270, 0x1271, 0x1272, 0x1273 },
                    },
                },
                [15] = new Dictionary<int, List<int[]>>             // Temple of Xian
                {
                    [0x1A6A] = new List<int[]>
                    {
                        new int[] { 0x1A6A, 0x1A6B, 0x1A6C, 0x1A6D },
                        new int[] { 0x1A74, 0x1A75, 0x1A76, 0x1A77 },
                    },
                    [0x1A76] = new List<int[]>
                    {
                        new int[] { 0x1A76, 0x1A77, 0x1A78, 0x1A79 },
                        new int[] { 0x1A80, 0x1A81, 0x1A82, 0x1A83 },
                    },
                    [0x1A82] = new List<int[]>
                    {
                        new int[] { 0x1A82, 0x1A83, 0x1A84, 0x1A85 },
                        new int[] { 0x1A8C, 0x1A8D, 0x1A8E, 0x1A8F },
                    },
                    [0x1A8E] = new List<int[]>
                    {
                        new int[] { 0x1A8E, 0x1A8F, 0x1A90, 0x1A91 },
                        new int[] { 0x1A98, 0x1A99, 0x1A9A, 0x1A9B },
                    },
                    [0x1A9A] = new List<int[]>
                    {
                        new int[] { 0x1A9A, 0x1A9B, 0x1A9C, 0x1A9D },
                        new int[] { 0x1AA4, 0x1AA5, 0x1AA6, 0x1AA7 },
                    },
                    [0x1AA6] = new List<int[]>
                    {
                        new int[] { 0x1AA6, 0x1AA7, 0x1AA8, 0x1AA9 },
                        new int[] { 0x1AB0, 0x1AB1, 0x1AB2, 0x1AB3 },
                    },
                },
                [16] = new Dictionary<int, List<int[]>>             // Floating Islands
                {
                    [0x1204] = new List<int[]>
                    {
                        new int[] { 0x1204, 0x1205, 0x1206, 0x1207 },
                        new int[] { 0x120E, 0x120F, 0x1210, 0x1211 },
                    },
                    [0x1210] = new List<int[]>
                    {
                        new int[] { 0x1210, 0x1211, 0x1212, 0x1213 },
                        new int[] { 0x121A, 0x121B, 0x121C, 0x121D },
                    },
                },
                [17] = new Dictionary<int, List<int[]>>             // The Dragon's Lair
                {
                    [0x0D30] = new List<int[]>
                    {
                        new int[] { 0x0D30, 0x0D31, 0x0D32, 0x0D33 },
                        new int[] { 0x0D3A, 0x0D3B, 0x0D3C, 0x0D3D },
                    },
                    [0x0D3C] = new List<int[]>
                    {
                        new int[] { 0x0D3C, 0x0D3D, 0x0D3E, 0x0D3F },
                        new int[] { 0x0D46, 0x0D47, 0x0D48, 0x0D49 },
                    },
                    [0x0D48] = new List<int[]>
                    {
                        new int[] { 0x0D48, 0x0D49, 0x0D4A, 0x0D4B },
                        new int[] { 0x0D52, 0x0D53, 0x0D54, 0x0D55 },
                    },
                    [0x0D54] = new List<int[]>
                    {
                        new int[] { 0x0D54, 0x0D55, 0x0D56, 0x0D57 },
                        new int[] { 0x0D5E, 0x0D5F, 0x0D60, 0x0D61 },
                    }
                },
                [18] = new Dictionary<int, List<int[]>>             // Home Sweet Home
                {
                    [0x1020] = new List<int[]>
                    {
                        new int[] { 0x1020, 0x1021, 0x1022, 0x1023 },
                        new int[] { 0x102A, 0x102B, 0x102C, 0x102D },
                    },
                    [0x102C] = new List<int[]>
                    {
                        new int[] { 0x102C, 0x102D, 0x102E, 0x102F },
                        new int[] { 0x1036, 0x1037, 0x1038, 0x1039 },
                    },
                }
            };

        private void SetSecondaryAmmoOffsets()
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            automaticPistolsAmmoOffset2 = secondaryAmmoIndexMarker - 28;
            uziAmmoOffset2 = secondaryAmmoIndexMarker - 24;
            shotgunAmmoOffset2 = secondaryAmmoIndexMarker - 20;
            harpoonGunAmmoOffset2 = secondaryAmmoIndexMarker - 16;
            grenadeLauncherAmmoOffset2 = secondaryAmmoIndexMarker - 12;
            m16AmmoOffset2 = secondaryAmmoIndexMarker - 8;
        }

        private int GetSecondaryAmmoIndexMarker()
        {
            byte levelIndex = GetLevelIndex();
            int ammoIndexMarker = -1;

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                Dictionary<int, List<int[]>> indexData = ammoIndexData[levelIndex];
                var enumerator = indexData.GetEnumerator();

                for (int index = 0; index < indexData.Count && enumerator.MoveNext(); index++)
                {
                    var kvp = enumerator.Current;
                    int key = kvp.Key;

                    List<int[]> offsetsList = kvp.Value;

                    bool isMatch = offsetsList.Any(offsets => offsets.All(offset => ReadByte(offset) == 0xFF));

                    if (isMatch)
                    {
                        ammoIndexMarker = key;
                        break;
                    }
                }
            }

            return ammoIndexMarker;
        }

        private void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
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
            nudFlares.Value = GetNumFlares();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();

            if (GetLevelIndex() == 18)
            {
                chkPistols.Enabled = false;
                chkAutomaticPistols.Enabled = false;
                chkUzis.Enabled = false;
                chkM16.Enabled = false;
                chkGrenadeLauncher.Enabled = false;
                chkHarpoonGun.Enabled = false;

                nudAutomaticPistolsAmmo.Enabled = false;
                nudUziAmmo.Enabled = false;
                nudM16Ammo.Enabled = false;
                nudGrenadeLauncherAmmo.Enabled = false;
                nudHarpoonGunAmmo.Enabled = false;

                nudUziAmmo.Value = 0;
                nudAutomaticPistolsAmmo.Value = 0;
                nudM16Ammo.Value = 0;
                nudGrenadeLauncherAmmo.Value = 0;
                nudHarpoonGunAmmo.Value = 0;
            }
            else
            {
                chkPistols.Enabled = true;
                chkAutomaticPistols.Enabled = true;
                chkUzis.Enabled = true;
                chkM16.Enabled = true;
                chkGrenadeLauncher.Enabled = true;
                chkHarpoonGun.Enabled = true;

                nudAutomaticPistolsAmmo.Enabled = true;
                nudUziAmmo.Enabled = true;
                nudM16Ammo.Enabled = true;
                nudGrenadeLauncherAmmo.Enabled = true;
                nudHarpoonGunAmmo.Enabled = true;

                nudUziAmmo.Value = GetUziAmmo();
                nudAutomaticPistolsAmmo.Value = GetAutomaticPistolsAmmo();
                nudM16Ammo.Value = GetM16Ammo();
                nudGrenadeLauncherAmmo.Value = GetGrenadeLauncherAmmo();
                nudHarpoonGunAmmo.Value = GetHarpoonGunAmmo();
            }

            nudShotgunAmmo.Value = GetShotgunAmmo() / 6;

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
                chkAutomaticPistols.Checked = false;
                chkUzis.Checked = false;
                chkShotgun.Checked = false;
                chkM16.Checked = false;
                chkGrenadeLauncher.Checked = false;
                chkHarpoonGun.Checked = false;
            }
            else
            {
                chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
                chkAutomaticPistols.Checked = (weaponsConfigNum & AutomaticPistols) != 0;
                chkUzis.Checked = (weaponsConfigNum & Uzis) != 0;
                chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
                chkM16.Checked = (weaponsConfigNum & M16) != 0;
                chkGrenadeLauncher.Checked = (weaponsConfigNum & GrenadeLauncher) != 0;
                chkHarpoonGun.Checked = (weaponsConfigNum & HarpoonGun) != 0;
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

        public void WriteChanges(CheckBox chkPistols, CheckBox chkAutomaticPistols, CheckBox chkUzis, CheckBox chkShotgun,
            CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudSaveNumber, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudAutomaticPistolsAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudHarpoonGunAmmo,
            NumericUpDown nudShotgunAmmo, TrackBar trbHealth)
        {
            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkAutomaticPistols.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;
            if (chkM16.Checked) newWeaponsConfigNum += 32;
            if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 64;
            if (chkHarpoonGun.Checked) newWeaponsConfigNum += 128;

            WriteWeaponsConfigNum(newWeaponsConfigNum);

            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumFlares((byte)nudFlares.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);

            if (GetLevelIndex() != 18)
            {
                WriteAutomaticPistolsAmmo(chkAutomaticPistols.Checked, (UInt16)nudAutomaticPistolsAmmo.Value);
                WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);
                WriteM16Ammo(chkM16.Checked, (UInt16)nudM16Ammo.Value);
                WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
                WriteHarpoonGunAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            }

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));

            if (trbHealth.Enabled)
            {
                double newHealthPercentage = (double)trbHealth.Value;
                WriteHealthValue(newHealthPercentage);
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
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x0D) return true;       // Underwater
            if (byteFlag1 == 0x12 && byteFlag2 == 0x00 && byteFlag3 == 0x12) return true;       // Swimming
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;       // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;       // On water

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

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private UInt16 GetAutomaticPistolsAmmo()
        {
            return ReadUInt16(automaticPistolsAmmoOffset);
        }

        private UInt16 GetM16Ammo()
        {
            return ReadUInt16(m16AmmoOffset);
        }

        private UInt16 GetGrenadeLauncherAmmo()
        {
            return ReadUInt16(grenadeLauncherAmmoOffset);
        }

        private UInt16 GetShotgunAmmo()
        {
            return ReadUInt16(shotgunAmmoOffset);
        }

        private UInt16 GetHarpoonGunAmmo()
        {
            return ReadUInt16(harpoonGunAmmoOffset);
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

        private void WriteNumSmallMedipacks(byte value)
        {
            WriteByte(smallMedipackOffset, value);
        }

        private void WriteNumLargeMedipacks(byte value)
        {
            WriteByte(largeMedipackOffset, value);
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

        private void WriteAutomaticPistolsAmmo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(automaticPistolsAmmoOffset, ammo);
                WriteUInt16(automaticPistolsAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(automaticPistolsAmmoOffset, ammo);
                WriteUInt16(automaticPistolsAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(automaticPistolsAmmoOffset, ammo);
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

        private void WriteM16Ammo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(m16AmmoOffset, ammo);
                WriteUInt16(m16AmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(m16AmmoOffset, ammo);
                WriteUInt16(m16AmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(m16AmmoOffset, ammo);
            }
        }

        private void WriteGrenadeLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
                WriteUInt16(grenadeLauncherAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
                WriteUInt16(grenadeLauncherAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
            }
        }

        private void WriteHarpoonGunAmmo(bool isPresent, UInt16 ammo)
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            if (isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(harpoonGunAmmoOffset, ammo);
                WriteUInt16(harpoonGunAmmoOffset2, ammo);
            }
            else if (!isPresent && secondaryAmmoIndexMarker != -1)
            {
                WriteUInt16(harpoonGunAmmoOffset, ammo);
                WriteUInt16(harpoonGunAmmoOffset2, 0);
            }
            else
            {
                WriteUInt16(harpoonGunAmmoOffset, ammo);
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
            return (levelIndex >= 1 && levelIndex <= 18);
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
