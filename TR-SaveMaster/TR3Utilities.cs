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

            if (levelIndex == 1)    // Jungle
            {
                // Health offsets
                smallMedipackOffset = 0xE6;
                largeMedipackOffset = 0xE7;
                SetHealthOffsets(0x6D3);

                // Misc offsets
                flaresOffset = 0xE9;

                // Weapon offsets
                weaponsConfigNumOffset = 0xED;
                harpoonGunOffset = 0xEE;

                // Ammo offsets
                shotgunAmmoOffset = 0xDC;
                shotgunAmmoOffset2 = 0x164B;
                deagleAmmoOffset = 0xD8;
                deagleAmmoOffset2 = 0x1643;
                grenadeLauncherAmmoOffset = 0xE4;
                grenadeLauncherAmmoOffset2 = 0x1657;
                rocketLauncherAmmoOffset = 0xE0;
                rocketLauncherAmmoOffset2 = 0x1653;
                harpoonGunAmmoOffset = 0xE2;
                harpoonGunAmmoOffset2 = 0x164F;
                mp5AmmoOffset = 0xDE;
                mp5AmmoOffset2 = 0x165B;
                uziAmmoOffset = 0xDA;
                uziAmmoOffset2 = 0x1647;
            }
            else if (levelIndex == 2)   // Temple Ruins
            {
                // Health offsets
                smallMedipackOffset = 0x119;
                largeMedipackOffset = 0x11A;
                SetHealthOffsets(0x8F7, 0x909);

                // Misc offsets
                flaresOffset = 0x11C;

                // Weapon offsets
                weaponsConfigNumOffset = 0x120;
                harpoonGunOffset = 0x121;

                // Ammo offsets
                shotgunAmmoOffset = 0x10F;
                shotgunAmmoOffset2 = 0x23BB;
                deagleAmmoOffset = 0x10B;
                deagleAmmoOffset2 = 0x23B3;
                grenadeLauncherAmmoOffset = 0x117;
                grenadeLauncherAmmoOffset2 = 0x23C7;
                rocketLauncherAmmoOffset = 0x113;
                rocketLauncherAmmoOffset2 = 0x23C3;
                harpoonGunAmmoOffset = 0x115;
                harpoonGunAmmoOffset2 = 0x23BF;
                mp5AmmoOffset = 0x111;
                mp5AmmoOffset2 = 0x23CB;
                uziAmmoOffset = 0x10D;
                uziAmmoOffset2 = 0x23B7;
            }
            else if (levelIndex == 3)   // The River Ganges
            {
                // Health offsets
                smallMedipackOffset = 0x14C;
                largeMedipackOffset = 0x14D;
                SetHealthOffsets(0x6B9, 0xB05);

                // Misc offsets
                flaresOffset = 0x14F;

                // Weapon offsets
                weaponsConfigNumOffset = 0x153;
                harpoonGunOffset = 0x154;

                // Ammo offsets
                shotgunAmmoOffset = 0x142;
                shotgunAmmoOffset2 = 0x1804;
                deagleAmmoOffset = 0x13E;
                deagleAmmoOffset2 = 0x17FC;
                grenadeLauncherAmmoOffset = 0x14A;
                grenadeLauncherAmmoOffset2 = 0x1810;
                rocketLauncherAmmoOffset = 0x146;
                rocketLauncherAmmoOffset2 = 0x180C;
                harpoonGunAmmoOffset = 0x148;
                harpoonGunAmmoOffset2 = 0x1808;
                mp5AmmoOffset = 0x144;
                mp5AmmoOffset2 = 0x1814;
                uziAmmoOffset = 0x140;
                uziAmmoOffset2 = 0x1800;
            }
            else if (levelIndex == 4)   // Caves Of Kaliya
            {
                // Health offsets
                smallMedipackOffset = 0x17F;
                largeMedipackOffset = 0x180;
                SetHealthOffsets(0xB05);

                // Misc offsets
                flaresOffset = 0x182;

                // Weapon offsets
                weaponsConfigNumOffset = 0x186;
                harpoonGunOffset = 0x187;

                // Ammo offsets
                shotgunAmmoOffset = 0x175;
                shotgunAmmoOffset2 = 0xD1F;
                deagleAmmoOffset = 0x171;
                deagleAmmoOffset2 = 0xD17;
                grenadeLauncherAmmoOffset = 0x17D;
                grenadeLauncherAmmoOffset2 = 0xD2B;
                rocketLauncherAmmoOffset = 0x179;
                rocketLauncherAmmoOffset2 = 0xD27;
                harpoonGunAmmoOffset = 0x17B;
                harpoonGunAmmoOffset2 = 0xD23;
                mp5AmmoOffset = 0x177;
                mp5AmmoOffset2 = 0xD2F;
                uziAmmoOffset = 0x173;
                uziAmmoOffset2 = 0xD1B;
            }
            else if (levelIndex == 13)  // Nevada Desert
            {
                // Health offsets
                smallMedipackOffset = 0x34A;
                largeMedipackOffset = 0x34B;
                SetHealthOffsets(0x6B5);

                // Misc offsets
                flaresOffset = 0x34D;

                // Weapon offsets
                weaponsConfigNumOffset = 0x351;
                harpoonGunOffset = 0x352;

                // Ammo offsets
                shotgunAmmoOffset = 0x340;
                shotgunAmmoOffset2 = 0x17A4;
                deagleAmmoOffset = 0x33C;
                deagleAmmoOffset2 = 0x179C;
                grenadeLauncherAmmoOffset = 0x348;
                grenadeLauncherAmmoOffset2 = 0x17B0;
                rocketLauncherAmmoOffset = 0x344;
                rocketLauncherAmmoOffset2 = 0x17AC;
                harpoonGunAmmoOffset = 0x346;
                harpoonGunAmmoOffset2 = 0x17A8;
                mp5AmmoOffset = 0x342;
                mp5AmmoOffset2 = 0x17B4;
                uziAmmoOffset = 0x33E;
                uziAmmoOffset2 = 0x17A0;
            }
            else if (levelIndex == 14)  // High Security Compound
            {
                // Health offsets
                smallMedipackOffset = 0x37D;
                largeMedipackOffset = 0x37E;
                SetHealthOffsets(0x6F7, 0x709);

                // Misc offsets
                flaresOffset = 0x380;

                // Weapon offsets
                weaponsConfigNumOffset = 0x384;
                harpoonGunOffset = 0x385;

                // Ammo offsets
                shotgunAmmoOffset = 0x373;
                shotgunAmmoOffset2 = 0x1E4B;
                deagleAmmoOffset = 0x36F;
                deagleAmmoOffset2 = 0x1E43;
                grenadeLauncherAmmoOffset = 0x37B;
                grenadeLauncherAmmoOffset2 = 0x1E57;
                rocketLauncherAmmoOffset = 0x377;
                rocketLauncherAmmoOffset2 = 0x1E53;
                harpoonGunAmmoOffset = 0x379;
                harpoonGunAmmoOffset2 = 0x1E4F;
                mp5AmmoOffset = 0x375;
                mp5AmmoOffset2 = 0x1E5B;
                uziAmmoOffset = 0x371;
                uziAmmoOffset2 = 0x1E47;
            }
            else if (levelIndex == 15)  // Area 51
            {
                // Health offsets
                smallMedipackOffset = 0x3B0;
                largeMedipackOffset = 0x3B1;
                SetHealthOffsets(0xC45, 0xC57);

                // Misc offsets
                flaresOffset = 0x3B3;

                // Weapon offsets
                weaponsConfigNumOffset = 0x3B7;
                harpoonGunOffset = 0x3B8;

                // Ammo offsets
                shotgunAmmoOffset = 0x3A6;
                shotgunAmmoOffset2 = 0x210D;
                deagleAmmoOffset = 0x3A2;
                deagleAmmoOffset2 = 0x2105;
                grenadeLauncherAmmoOffset = 0x3AE;
                grenadeLauncherAmmoOffset2 = 0x2119;
                rocketLauncherAmmoOffset = 0x3AA;
                rocketLauncherAmmoOffset2 = 0x2115;
                harpoonGunAmmoOffset = 0x3AC;
                harpoonGunAmmoOffset2 = 0x2111;
                mp5AmmoOffset = 0x3A8;
                mp5AmmoOffset2 = 0x211D;
                uziAmmoOffset = 0x3A4;
                uziAmmoOffset2 = 0x2109;
            }
            else if (levelIndex == 5)   // Coastal Village
            {
                // Health offsets
                smallMedipackOffset = 0x1B2;
                largeMedipackOffset = 0x1B3;
                SetHealthOffsets(0x7BB);

                // Misc offsets
                flaresOffset = 0x1B5;

                // Weapon offsets
                weaponsConfigNumOffset = 0x1B9;
                harpoonGunOffset = 0x1BA;

                // Ammo offsets
                shotgunAmmoOffset = 0x1A8;
                shotgunAmmoOffset2 = 0x17B1;
                deagleAmmoOffset = 0x1A4;
                deagleAmmoOffset2 = 0x17A9;
                grenadeLauncherAmmoOffset = 0x1B0;
                grenadeLauncherAmmoOffset2 = 0x17BD;
                rocketLauncherAmmoOffset = 0x1AC;
                rocketLauncherAmmoOffset2 = 0x17B9;
                harpoonGunAmmoOffset = 0x1AE;
                harpoonGunAmmoOffset2 = 0x17B5;
                mp5AmmoOffset = 0x1AA;
                mp5AmmoOffset2 = 0x17C1;
                uziAmmoOffset = 0x1A6;
                uziAmmoOffset2 = 0x17AD;
            }
            else if (levelIndex == 6)   // Crash Site
            {
                // Health offsets
                smallMedipackOffset = 0x1E5;
                largeMedipackOffset = 0x1E6;
                SetHealthOffsets(0x1785, 0x1797, 0x17A9, 0x17BB);

                // Misc offsets
                flaresOffset = 0x1E8;

                // Weapon offsets
                weaponsConfigNumOffset = 0x1EC;
                harpoonGunOffset = 0x1ED;

                // Ammo offsets
                shotgunAmmoOffset = 0x1DB;
                shotgunAmmoOffset2 = 0x18D3;
                deagleAmmoOffset = 0x1D7;
                deagleAmmoOffset2 = 0x18CB;
                grenadeLauncherAmmoOffset = 0x1E3;
                grenadeLauncherAmmoOffset2 = 0x18DF;
                rocketLauncherAmmoOffset = 0x1DF;
                rocketLauncherAmmoOffset2 = 0x18DB;
                harpoonGunAmmoOffset = 0x1E1;
                harpoonGunAmmoOffset2 = 0x18D7;
                mp5AmmoOffset = 0x1DD;
                mp5AmmoOffset2 = 0x18E3;
                uziAmmoOffset = 0x1D9;
                uziAmmoOffset2 = 0x18CF;
            }
            else if (levelIndex == 7)   // Madubu Gorge
            {
                // Health offsets
                smallMedipackOffset = 0x218;
                largeMedipackOffset = 0x219;
                SetHealthOffsets(0xBE3, 0xBF5);

                // Misc offsets
                flaresOffset = 0x21B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x21F;
                harpoonGunOffset = 0x220;

                // Ammo offsets
                shotgunAmmoOffset = 0x20E;
                shotgunAmmoOffset2 = 0x141D;
                deagleAmmoOffset = 0x20A;
                deagleAmmoOffset2 = 0x1415;
                grenadeLauncherAmmoOffset = 0x216;
                grenadeLauncherAmmoOffset2 = 0x1429;
                rocketLauncherAmmoOffset = 0x212;
                rocketLauncherAmmoOffset2 = 0x1425;
                harpoonGunAmmoOffset = 0x214;
                harpoonGunAmmoOffset2 = 0x1421;
                mp5AmmoOffset = 0x210;
                mp5AmmoOffset2 = 0x142D;
                uziAmmoOffset = 0x20C;
                uziAmmoOffset2 = 0x1419;
            }
            else if (levelIndex == 8)   // Temple Of Puna
            {
                // Health offsets
                smallMedipackOffset = 0x24B;
                largeMedipackOffset = 0x24C;
                SetHealthOffsets(0x68F);

                // Misc offsets
                flaresOffset = 0x24E;

                // Weapon offsets
                weaponsConfigNumOffset = 0x252;
                harpoonGunOffset = 0x253;

                // Ammo offsets
                shotgunAmmoOffset = 0x241;
                shotgunAmmoOffset2 = 0x10F5;
                deagleAmmoOffset = 0x23D;
                deagleAmmoOffset2 = 0x10ED;
                grenadeLauncherAmmoOffset = 0x249;
                grenadeLauncherAmmoOffset2 = 0x1101;
                rocketLauncherAmmoOffset = 0x245;
                rocketLauncherAmmoOffset2 = 0x10FD;
                harpoonGunAmmoOffset = 0x247;
                harpoonGunAmmoOffset2 = 0x10F9;
                mp5AmmoOffset = 0x243;
                mp5AmmoOffset2 = 0x1105;
                uziAmmoOffset = 0x23F;
                uziAmmoOffset2 = 0x10F1;
            }
            else if (levelIndex == 9)   // Thames Wharf
            {
                // Health offsets
                smallMedipackOffset = 0x27E;
                largeMedipackOffset = 0x27F;
                SetHealthOffsets(0xB15, 0xB39);

                // Misc offsets
                flaresOffset = 0x281;

                // Weapon offsets
                weaponsConfigNumOffset = 0x285;
                harpoonGunOffset = 0x286;

                // Ammo offsets
                shotgunAmmoOffset = 0x274;
                shotgunAmmoOffset2 = 0x1873;
                deagleAmmoOffset = 0x270;
                deagleAmmoOffset2 = 0x186B;
                grenadeLauncherAmmoOffset = 0x27C;
                grenadeLauncherAmmoOffset2 = 0x187F;
                rocketLauncherAmmoOffset = 0x278;
                rocketLauncherAmmoOffset2 = 0x187B;
                harpoonGunAmmoOffset = 0x27A;
                harpoonGunAmmoOffset2 = 0x1877;
                mp5AmmoOffset = 0x276;
                mp5AmmoOffset2 = 0x1883;
                uziAmmoOffset = 0x272;
                uziAmmoOffset2 = 0x186F;
            }
            else if (levelIndex == 10)  // Aldwych
            {
                // Health offsets
                smallMedipackOffset = 0x2B1;
                largeMedipackOffset = 0x2B2;
                SetHealthOffsets(0x2135, 0x2147);

                // Misc offsets
                flaresOffset = 0x2B4;

                // Weapon offsets
                weaponsConfigNumOffset = 0x2B8;
                harpoonGunOffset = 0x2B9;

                // Ammo offsets
                shotgunAmmoOffset = 0x2A7;
                shotgunAmmoOffset2 = 0x22FF;
                deagleAmmoOffset = 0x2A3;
                deagleAmmoOffset2 = 0x22F7;
                grenadeLauncherAmmoOffset = 0x2AF;
                grenadeLauncherAmmoOffset2 = 0x230B;
                rocketLauncherAmmoOffset = 0x2AB;
                rocketLauncherAmmoOffset2 = 0x2307;
                harpoonGunAmmoOffset = 0x2AD;
                harpoonGunAmmoOffset2 = 0x2303;
                mp5AmmoOffset = 0x2A9;
                mp5AmmoOffset2 = 0x230F;
                uziAmmoOffset = 0x2A5;
                uziAmmoOffset2 = 0x22FB;
            }
            else if (levelIndex == 11)  // Lud's Gate
            {
                // Health offsets
                smallMedipackOffset = 0x2E4;
                largeMedipackOffset = 0x2E5;
                SetHealthOffsets(0xAB1, 0xAC3, 0xAD5);

                // Misc offsets
                flaresOffset = 0x2E7;

                // Weapon offsets
                weaponsConfigNumOffset = 0x2EB;
                harpoonGunOffset = 0x2EC;

                // Ammo offsets
                shotgunAmmoOffset = 0x2DA;
                shotgunAmmoOffset2 = 0x1D77;
                deagleAmmoOffset = 0x2D6;
                deagleAmmoOffset2 = 0x1D6F;
                grenadeLauncherAmmoOffset = 0x2E2;
                grenadeLauncherAmmoOffset2 = 0x1D83;
                rocketLauncherAmmoOffset = 0x2DE;
                rocketLauncherAmmoOffset2 = 0x1D7F;
                harpoonGunAmmoOffset = 0x2E0;
                harpoonGunAmmoOffset2 = 0x1D7B;
                mp5AmmoOffset = 0x2DC;
                mp5AmmoOffset2 = 0x1D87;
                uziAmmoOffset = 0x2D8;
                uziAmmoOffset2 = 0x1D73;
            }
            else if (levelIndex == 12)  // City
            {
                // Health offsets
                smallMedipackOffset = 0x317;
                largeMedipackOffset = 0x318;
                SetHealthOffsets(0x737);

                // Misc offsets
                flaresOffset = 0x31A;

                // Weapon offsets
                weaponsConfigNumOffset = 0x31E;
                harpoonGunOffset = 0x31F;

                // Ammo offsets
                shotgunAmmoOffset = 0x30D;
                shotgunAmmoOffset2 = 0xAF3;
                deagleAmmoOffset = 0x309;
                deagleAmmoOffset2 = 0xAEB;
                grenadeLauncherAmmoOffset = 0x315;
                grenadeLauncherAmmoOffset2 = 0xAFF;
                rocketLauncherAmmoOffset = 0x311;
                rocketLauncherAmmoOffset2 = 0xAFB;
                harpoonGunAmmoOffset = 0x313;
                harpoonGunAmmoOffset2 = 0xAF7;
                mp5AmmoOffset = 0x30F;
                mp5AmmoOffset2 = 0xB03;
                uziAmmoOffset = 0x30B;
                uziAmmoOffset2 = 0xAEF;
            }
            else if (levelIndex == 16)  // Antarctica
            {
                // Health offsets
                smallMedipackOffset = 0x3E3;
                largeMedipackOffset = 0x3E4;
                SetHealthOffsets(0x6C5);

                // Misc offsets
                flaresOffset = 0x3E6;

                // Weapon offsets
                weaponsConfigNumOffset = 0x3EA;
                harpoonGunOffset = 0x3EB;

                // Ammo offsets
                shotgunAmmoOffset = 0x3D9;
                shotgunAmmoOffset2 = 0x1995;
                deagleAmmoOffset = 0x3D5;
                deagleAmmoOffset2 = 0x198D;
                grenadeLauncherAmmoOffset = 0x3E1;
                grenadeLauncherAmmoOffset2 = 0x19A1;
                rocketLauncherAmmoOffset = 0x3DD;
                rocketLauncherAmmoOffset2 = 0x199D;
                harpoonGunAmmoOffset = 0x3DF;
                harpoonGunAmmoOffset2 = 0x1999;
                mp5AmmoOffset = 0x3DB;
                mp5AmmoOffset2 = 0x19A5;
                uziAmmoOffset = 0x3D7;
                uziAmmoOffset2 = 0x1991;
            }
            else if (levelIndex == 17)  // RX-Tech Mines
            {
                // Health offsets
                smallMedipackOffset = 0x416;
                largeMedipackOffset = 0x417;
                SetHealthOffsets(0xA65, 0xA77);

                // Misc offsets
                flaresOffset = 0x419;

                // Weapon offsets
                weaponsConfigNumOffset = 0x41D;
                harpoonGunOffset = 0x41E;

                // Ammo offsets
                shotgunAmmoOffset = 0x40C;
                shotgunAmmoOffset2 = 0x1957;
                deagleAmmoOffset = 0x408;
                deagleAmmoOffset2 = 0x194F;
                grenadeLauncherAmmoOffset = 0x414;
                grenadeLauncherAmmoOffset2 = 0x1963;
                rocketLauncherAmmoOffset = 0x410;
                rocketLauncherAmmoOffset2 = 0x195F;
                harpoonGunAmmoOffset = 0x412;
                harpoonGunAmmoOffset2 = 0x195B;
                mp5AmmoOffset = 0x40E;
                mp5AmmoOffset2 = 0x1967;
                uziAmmoOffset = 0x40A;
                uziAmmoOffset2 = 0x1953;
            }
            else if (levelIndex == 18)  // Lost City Of Tinnos
            {
                // Health offsets
                smallMedipackOffset = 0x449;
                largeMedipackOffset = 0x44A;
                SetHealthOffsets(0x711);

                // Misc offsets
                flaresOffset = 0x44C;

                // Weapon offsets
                weaponsConfigNumOffset = 0x450;
                harpoonGunOffset = 0x451;

                // Ammo offsets
                shotgunAmmoOffset = 0x43F;
                shotgunAmmoOffset2 = 0x1D97;
                deagleAmmoOffset = 0x43B;
                deagleAmmoOffset2 = 0x1D8F;
                grenadeLauncherAmmoOffset = 0x447;
                grenadeLauncherAmmoOffset2 = 0x1DA3;
                rocketLauncherAmmoOffset = 0x443;
                rocketLauncherAmmoOffset2 = 0x1D9F;
                harpoonGunAmmoOffset = 0x445;
                harpoonGunAmmoOffset2 = 0x1D9B;
                mp5AmmoOffset = 0x441;
                mp5AmmoOffset2 = 0x1DA7;
                uziAmmoOffset = 0x43D;
                uziAmmoOffset2 = 0x1D93;
            }
            else if (levelIndex == 19)  // Meteorite Cavern
            {
                // Health offsets
                smallMedipackOffset = 0x47C;
                largeMedipackOffset = 0x47D;
                SetHealthOffsets(0x68D);

                // Misc offsets
                flaresOffset = 0x47F;

                // Weapon offsets
                weaponsConfigNumOffset = 0x483;
                harpoonGunOffset = 0x484;

                // Ammo offsets
                shotgunAmmoOffset = 0x472;
                shotgunAmmoOffset2 = 0xAE9;
                deagleAmmoOffset = 0x46E;
                deagleAmmoOffset2 = 0xAE1;
                grenadeLauncherAmmoOffset = 0x47A;
                grenadeLauncherAmmoOffset2 = 0xAF5;
                rocketLauncherAmmoOffset = 0x476;
                rocketLauncherAmmoOffset2 = 0xAF1;
                harpoonGunAmmoOffset = 0x478;
                harpoonGunAmmoOffset2 = 0xAED;
                mp5AmmoOffset = 0x474;
                mp5AmmoOffset2 = 0xAF9;
                uziAmmoOffset = 0x470;
                uziAmmoOffset2 = 0xAE5;
            }
            else if (levelIndex == 20)  // All Hallows
            {
                // Health offsets
                smallMedipackOffset = 0x4AF;
                largeMedipackOffset = 0x4B0;
                SetHealthOffsets(0xAF5);

                // Misc offsets
                flaresOffset = 0x4B2;

                // Weapon offsets
                weaponsConfigNumOffset = 0x4B6;
                harpoonGunOffset = 0x4B7;

                // Ammo offsets
                shotgunAmmoOffset = 0x472;
                shotgunAmmoOffset2 = 0x102D;
                deagleAmmoOffset = 0x46E;
                deagleAmmoOffset2 = 0x1025;
                grenadeLauncherAmmoOffset = 0x47A;
                grenadeLauncherAmmoOffset2 = 0x1039;
                rocketLauncherAmmoOffset = 0x476;
                rocketLauncherAmmoOffset2 = 0x1035;
                harpoonGunAmmoOffset = 0x478;
                harpoonGunAmmoOffset2 = 0x1031;
                mp5AmmoOffset = 0x4A7;
                mp5AmmoOffset2 = 0x103D;
                uziAmmoOffset = 0x4A3;
                uziAmmoOffset2 = 0x1029;
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
            WriteNumFlares((byte)nudFlares.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);
            WriteSaveNumber((byte)nudSaveNumber.Value);

            WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
            WriteDeagleAmmo(chkDeagle.Checked, (UInt16)nudDeagleAmmo.Value);
            WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
            WriteRocketLauncherAmmo(chkRocketLauncher.Checked, (UInt16)nudRocketLauncherAmmo.Value);
            WriteHarpoonAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
            WriteMP5Ammo(chkMP5.Checked, (UInt16)nudMP5Ammo.Value);
            WriteUziAmmo(chkUzi.Checked, (UInt16)nudUziAmmo.Value);

            if (trbHealth.Enabled)
            {
                double newHealthPercentage = (double)trbHealth.Value;
                WriteHealthValue(newHealthPercentage);
            }
        }

        private readonly Dictionary<byte, Dictionary<int, List<int[]>>> ammoIndexData =
            new Dictionary<byte, Dictionary<int, List<int[]>>>
            {
                [1] = new Dictionary<int, List<int[]>>                          // Jungle
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1663, 0x1664, 0x1665, 0x1666 },
                        new int[] { 0x166D, 0x166E, 0x166F, 0x1670 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1675, 0x1676, 0x1677, 0x1678 },
                        new int[] { 0x167F, 0x1680, 0x1681, 0x1682 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1687, 0x1688, 0x1689, 0x168A },
                        new int[] { 0x1691, 0x1692, 0x1693, 0x1694 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1699, 0x169A, 0x169B, 0x169C },
                        new int[] { 0x16A3, 0x16A4, 0x16A5, 0x16A6 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x16AB, 0x16AC, 0x16AD, 0x16AE },
                        new int[] { 0x16B5, 0x16B6, 0x16B7, 0x16B8 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x16BD, 0x16BE, 0x16BF, 0x16C0 },
                        new int[] { 0x16C7, 0x16C8, 0x16C9, 0x16CA },
                    },
                },
                [2] = new Dictionary<int, List<int[]>>                          // Temple Ruins
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x23D3, 0x23D4, 0x23D5, 0x23D6 },
                        new int[] { 0x23DD, 0x23DE, 0x23DF, 0x23E0 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x23E5, 0x23E6, 0x23E7, 0x23E8 },
                        new int[] { 0x23EF, 0x23F0, 0x23F1, 0x23F2 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x23F7, 0x23F8, 0x23F9, 0x23FA },
                        new int[] { 0x2401, 0x2402, 0x2403, 0x2404 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x2409, 0x240A, 0x240B, 0x240C },
                        new int[] { 0x2413, 0x2414, 0x2415, 0x2416 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x241B, 0x241C, 0x241D, 0x241E },
                        new int[] { 0x2425, 0x2426, 0x2427, 0x2428 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x242D, 0x242E, 0x242F, 0x2430 },
                        new int[] { 0x2437, 0x2438, 0x2439, 0x243A },
                    },
                },
                [3] = new Dictionary<int, List<int[]>>                          // The River Ganges
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x181C, 0x181D, 0x181E, 0x181F },
                        new int[] { 0x1826, 0x1827, 0x1828, 0x1829 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x182E, 0x182F, 0x1830, 0x1831 },
                        new int[] { 0x1838, 0x1839, 0x183A, 0x183B },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1840, 0x1841, 0x1842, 0x1843 },
                        new int[] { 0x184A, 0x184B, 0x184C, 0x184D },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1852, 0x1853, 0x1854, 0x1855 },
                        new int[] { 0x185C, 0x185D, 0x185E, 0x185F },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1864, 0x1865, 0x1866, 0x1867 },
                        new int[] { 0x186E, 0x186F, 0x1870, 0x1871 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1876, 0x1877, 0x1878, 0x1879 },
                        new int[] { 0x1880, 0x1881, 0x1882, 0x1883 },
                    },
                },
                [4] = new Dictionary<int, List<int[]>>                          // Caves of Kaliya
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x0D37, 0x0D38, 0x0D39, 0x0D3A },
                        new int[] { 0x0D41, 0x0D42, 0x0D43, 0x0D44 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x0D49, 0x0D4A, 0x0D4B, 0x0D4C },
                        new int[] { 0x0D53, 0x0D54, 0x0D55, 0x0D56 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x0D5B, 0x0D5C, 0x0D5D, 0x0D5E },
                        new int[] { 0x0D65, 0x0D66, 0x0D67, 0x0D68 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x0D6D, 0x0D6E, 0x0D6F, 0x0D70 },
                        new int[] { 0x0D77, 0x0D78, 0x0D79, 0x0D7A },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x0D7F, 0x0D80, 0x0D81, 0x0D82 },
                        new int[] { 0x0D89, 0x0D8A, 0x0D8B, 0x0D8C },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x0D91, 0x0D92, 0x0D93, 0x0D94 },
                        new int[] { 0x0D9B, 0x0D9C, 0x0D9D, 0x0D9E },
                    },
                    [6] = new List<int[]>
                    {
                        new int[] { 0x0DA3, 0x0DA4, 0x0DA5, 0x0DA6 },
                        new int[] { 0x0DAD, 0x0DAE, 0x0DAF, 0x0DB0 },
                    },
                    [7] = new List<int[]>
                    {
                        new int[] { 0x0DB5, 0x0DB6, 0x0DB7, 0x0DB8 },
                        new int[] { 0x0DBF, 0x0DC0, 0x0DC1, 0x0DC2 },
                    },
                    [8] = new List<int[]>
                    {
                        new int[] { 0x0DC7, 0x0DC8, 0x0DC9, 0x0DCA },
                        new int[] { 0x0DD1, 0x0DD2, 0x0DD3, 0x0DD4 },
                    },
                    [9] = new List<int[]>
                    {
                        new int[] { 0x0DD9, 0x0DDA, 0x0DDB, 0x0DDC },
                        new int[] { 0x0DE3, 0x0DE4, 0x0DE5, 0x0DE6 },
                    },
                    [10] = new List<int[]>
                    {
                        new int[] { 0x0DEB, 0x0DEC, 0x0DED, 0x0DEE },
                        new int[] { 0x0DF5, 0x0DF6, 0x0DF7, 0x0DF8 },
                    },
                    [11] = new List<int[]>
                    {
                        new int[] { 0x0DFD, 0x0DFE, 0x0DFF, 0x0E00 },
                        new int[] { 0x0E07, 0x0E08, 0x0E09, 0x0E0A },
                    },
                    [12] = new List<int[]>
                    {
                        new int[] { 0x0E0F, 0x0E10, 0x0E11, 0x0E12 },
                        new int[] { 0x0E19, 0x0E1A, 0x0E1B, 0x0E1C },
                    },
                    [13] = new List<int[]>
                    {
                        new int[] { 0x0E21, 0x0E22, 0x0E23, 0x0E24 },
                        new int[] { 0x0E2B, 0x0E2C, 0x0E2D, 0x0E2E },
                    },
                },
                [13] = new Dictionary<int, List<int[]>>                         // Nevada Desert
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x17BC, 0x17BD, 0x17BE, 0x17BF },
                        new int[] { 0x17C6, 0x17C7, 0x17C8, 0x17C9 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x17CE, 0x17CF, 0x17D0, 0x17D1 },
                        new int[] { 0x17D8, 0x17D9, 0x17DA, 0x17DB },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x17E0, 0x17E1, 0x17E2, 0x17E3 },
                        new int[] { 0x17EA, 0x17EB, 0x17EC, 0x17ED },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x17F2, 0x17F3, 0x17F4, 0x17F5 },
                        new int[] { 0x17FC, 0x17FD, 0x17FE, 0x17FF },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1804, 0x1805, 0x1806, 0x1807 },
                        new int[] { 0x180E, 0x180F, 0x1810, 0x1811 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1816, 0x1817, 0x1818, 0x1819 },
                        new int[] { 0x1820, 0x1821, 0x1822, 0x1823 },
                    },
                },
                [14] = new Dictionary<int, List<int[]>>                         // High Security Compound
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1E63, 0x1E64, 0x1E65, 0x1E66 },
                        new int[] { 0x1E6D, 0x1E6E, 0x1E6F, 0x1E70 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1E75, 0x1E76, 0x1E77, 0x1E78 },
                        new int[] { 0x1E7F, 0x1E80, 0x1E81, 0x1E82 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1E87, 0x1E88, 0x1E89, 0x1E8A },
                        new int[] { 0x1E91, 0x1E92, 0x1E93, 0x1E94 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1E99, 0x1E9A, 0x1E9B, 0x1E9C },
                        new int[] { 0x1EA3, 0x1EA4, 0x1EA5, 0x1EA6 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1EAB, 0x1EAC, 0x1EAD, 0x1EAE },
                        new int[] { 0x1EB5, 0x1EB6, 0x1EB7, 0x1EB8 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1EBD, 0x1EBE, 0x1EBF, 0x1EC0 },
                        new int[] { 0x1EC7, 0x1EC8, 0x1EC9, 0x1ECA },
                    },
                    [6] = new List<int[]>
                    {
                        new int[] { 0x1ECF, 0x1ED0, 0x1ED1, 0x1ED2 },
                        new int[] { 0x1ED9, 0x1EDA, 0x1EDB, 0x1EDC },
                    },
                },
                [15] = new Dictionary<int, List<int[]>>                         // Area 51
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x2125, 0x2126, 0x2127, 0x2128 },
                        new int[] { 0x212F, 0x2130, 0x2131, 0x2132 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x2137, 0x2138, 0x2139, 0x213A },
                        new int[] { 0x2141, 0x2142, 0x2143, 0x2144 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x2149, 0x214A, 0x214B, 0x214C },
                        new int[] { 0x2153, 0x2154, 0x2155, 0x2156 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x215B, 0x215C, 0x215D, 0x215E },
                        new int[] { 0x2165, 0x2166, 0x2167, 0x2168 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x216D, 0x216E, 0x216F, 0x2170 },
                        new int[] { 0x2177, 0x2178, 0x2179, 0x217A },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x217F, 0x2180, 0x2181, 0x2182 },
                        new int[] { 0x2189, 0x218A, 0x218B, 0x218C },
                    },
                    [6] = new List<int[]>
                    {
                        new int[] { 0x2191, 0x2192, 0x2193, 0x2194 },
                        new int[] { 0x219B, 0x219C, 0x219D, 0x219E },
                    },
                    [7] = new List<int[]>
                    {
                        new int[] { 0x21A3, 0x21A4, 0x21A5, 0x21A6 },
                        new int[] { 0x21AD, 0x21AE, 0x21AF, 0x21B0 },
                    },
                    [8] = new List<int[]>
                    {
                        new int[] { 0x21B5, 0x21B6, 0x21B7, 0x21B8 },
                        new int[] { 0x21BF, 0x21C0, 0x21C1, 0x21C2 },
                    },
                    [9] = new List<int[]>
                    {
                        new int[] { 0x21C7, 0x21C8, 0x21C9, 0x21CA },
                        new int[] { 0x21D1, 0x21D2, 0x21D3, 0x21D4 },
                    },
                },
                [5] = new Dictionary<int, List<int[]>>                          // Coastal Village
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x17C9, 0x17CA, 0x17CB, 0x17CC },
                        new int[] { 0x17D3, 0x17D4, 0x17D5, 0x17D6 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x17DB, 0x17DC, 0x17DD, 0x17DE },
                        new int[] { 0x17E5, 0x17E6, 0x17E7, 0x17E8 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x17ED, 0x17EE, 0x17EF, 0x17F0 },
                        new int[] { 0x17F7, 0x17F8, 0x17F9, 0x17FA },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x17FF, 0x1800, 0x1801, 0x1802 },
                        new int[] { 0x1809, 0x180A, 0x180B, 0x180C },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1811, 0x1812, 0x1813, 0x1814 },
                        new int[] { 0x181B, 0x181C, 0x181D, 0x181E },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1823, 0x1824, 0x1825, 0x1826 },
                        new int[] { 0x182D, 0x182E, 0x182F, 0x1830 },
                    },
                },
                [6] = new Dictionary<int, List<int[]>>                          // Crash Site
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x18EB, 0x18EC, 0x18ED, 0x18EE },
                        new int[] { 0x18F5, 0x18F6, 0x18F7, 0x18F8 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x18FD, 0x18FE, 0x18FF, 0x1900 },
                        new int[] { 0x1907, 0x1908, 0x1909, 0x190A },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x190F, 0x1910, 0x1911, 0x1912 },
                        new int[] { 0x1919, 0x191A, 0x191B, 0x191C },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1921, 0x1922, 0x1923, 0x1924 },
                        new int[] { 0x192B, 0x192C, 0x192D, 0x192E },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1933, 0x1934, 0x1935, 0x1936 },
                        new int[] { 0x193D, 0x193E, 0x193F, 0x1940 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1945, 0x1946, 0x1947, 0x1948 },
                        new int[] { 0x194F, 0x1950, 0x1951, 0x1952 },
                    },
                },
                [7] = new Dictionary<int, List<int[]>>                          // Madubu Gorge
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1435, 0x1436, 0x1437, 0x1438 },
                        new int[] { 0x143F, 0x1440, 0x1441, 0x1442 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1447, 0x1448, 0x1449, 0x144A },
                        new int[] { 0x1451, 0x1452, 0x1453, 0x1454 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1459, 0x145A, 0x145B, 0x145C },
                        new int[] { 0x1463, 0x1464, 0x1465, 0x1466 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x146B, 0x146C, 0x146D, 0x146E },
                        new int[] { 0x1475, 0x1476, 0x1477, 0x1478 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x147D, 0x147E, 0x147F, 0x1480 },
                        new int[] { 0x1487, 0x1488, 0x1489, 0x148A },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x148F, 0x1490, 0x1491, 0x1492 },
                        new int[] { 0x1499, 0x149A, 0x149B, 0x149C },
                    },
                },
                [8] = new Dictionary<int, List<int[]>>                          // Temple of Puna
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x110D, 0x110E, 0x110F, 0x1110 },
                        new int[] { 0x1117, 0x1118, 0x1119, 0x111A },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x111F, 0x1120, 0x1121, 0x1122 },
                        new int[] { 0x1129, 0x112A, 0x112B, 0x112C },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1131, 0x1132, 0x1133, 0x1134 },
                        new int[] { 0x113B, 0x113C, 0x113D, 0x113E },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1143, 0x1144, 0x1145, 0x1146 },
                        new int[] { 0x114D, 0x114E, 0x114F, 0x1150 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1155, 0x1156, 0x1157, 0x1158 },
                        new int[] { 0x115F, 0x1160, 0x1161, 0x1162 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1167, 0x1168, 0x1169, 0x116A },
                        new int[] { 0x1171, 0x1172, 0x1173, 0x1174 },
                    },
                },
                [9] = new Dictionary<int, List<int[]>>                          // Thames Wharf
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x188B, 0x188C, 0x188D, 0x188E },
                        new int[] { 0x1895, 0x1896, 0x1897, 0x1898 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x189D, 0x189E, 0x189F, 0x18A0 },
                        new int[] { 0x18A7, 0x18A8, 0x18A9, 0x18AA },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x18AF, 0x18B0, 0x18B1, 0x18B2 },
                        new int[] { 0x18B9, 0x18BA, 0x18BB, 0x18BC },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x18C1, 0x18C2, 0x18C3, 0x18C4 },
                        new int[] { 0x18CB, 0x18CC, 0x18CD, 0x18CE },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x18D3, 0x18D4, 0x18D5, 0x18D6 },
                        new int[] { 0x18DD, 0x18DE, 0x18DF, 0x18E0 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x18E5, 0x18E6, 0x18E7, 0x18E8 },
                        new int[] { 0x18EF, 0x18F0, 0x18F1, 0x18F2 },
                    },
                },
                [10] = new Dictionary<int, List<int[]>>                         // Aldwych
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x2317, 0x2318, 0x2319, 0x231A },
                        new int[] { 0x2321, 0x2322, 0x2323, 0x2324 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x2329, 0x232A, 0x232B, 0x232C },
                        new int[] { 0x2333, 0x2334, 0x2335, 0x2336 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x233B, 0x233C, 0x233D, 0x233E },
                        new int[] { 0x2345, 0x2346, 0x2347, 0x2348 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x234D, 0x234E, 0x234F, 0x2350 },
                        new int[] { 0x2357, 0x2358, 0x2359, 0x235A },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x235F, 0x2360, 0x2361, 0x2362 },
                        new int[] { 0x2369, 0x236A, 0x236B, 0x236C },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x2371, 0x2372, 0x2373, 0x2374 },
                        new int[] { 0x237B, 0x237C, 0x237D, 0x237E },
                    },
                },
                [11] = new Dictionary<int, List<int[]>>                         // Lud's Gate
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1D8F, 0x1D90, 0x1D91, 0x1D92 },
                        new int[] { 0x1D99, 0x1D9A, 0x1D9B, 0x1D9C },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1DA1, 0x1DA2, 0x1DA3, 0x1DA4 },
                        new int[] { 0x1DAB, 0x1DAC, 0x1DAD, 0x1DAE },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1DB3, 0x1DB4, 0x1DB5, 0x1DB6 },
                        new int[] { 0x1DBD, 0x1DBE, 0x1DBF, 0x1DC0 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1DC5, 0x1DC6, 0x1DC7, 0x1DC8 },
                        new int[] { 0x1DCF, 0x1DD0, 0x1DD1, 0x1DD2 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1DD7, 0x1DD8, 0x1DD9, 0x1DDA },
                        new int[] { 0x1DE1, 0x1DE2, 0x1DE3, 0x1DE4 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1DE9, 0x1DEA, 0x1DEB, 0x1DEC },
                        new int[] { 0x1DF3, 0x1DF4, 0x1DF5, 0x1DF6 },
                    },
                },
                [12] = new Dictionary<int, List<int[]>>                         // City
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x0B0B, 0x0B0C, 0x0B0D, 0x0B0E },
                        new int[] { 0x0B15, 0x0B16, 0x0B17, 0x0B18 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x0B1D, 0x0B1E, 0x0B1F, 0x0B20 },
                        new int[] { 0x0B27, 0x0B28, 0x0B29, 0x0B2A },
                    },
                },
                [16] = new Dictionary<int, List<int[]>>                         // Antarctica
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x19AD, 0x19AE, 0x19AF, 0x19B0 },
                        new int[] { 0x19B7, 0x19B8, 0x19B9, 0x19BA },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x19BF, 0x19C0, 0x19C1, 0x19C2 },
                        new int[] { 0x19C9, 0x19CA, 0x19CB, 0x19CC },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x19D1, 0x19D2, 0x19D3, 0x19D4 },
                        new int[] { 0x19DB, 0x19DC, 0x19DD, 0x19DE },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x19E3, 0x19E4, 0x19E5, 0x19E6 },
                        new int[] { 0x19ED, 0x19EE, 0x19EF, 0x19F0 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x19F5, 0x19F6, 0x19F7, 0x19F8 },
                        new int[] { 0x19FF, 0x1A00, 0x1A01, 0x1A02 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1A07, 0x1A08, 0x1A09, 0x1A0A },
                        new int[] { 0x1A11, 0x1A12, 0x1A13, 0x1A14 },
                    },
                },
                [17] = new Dictionary<int, List<int[]>>                         // RX-Tech Mines
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x196F, 0x1970, 0x1971, 0x1972 },
                        new int[] { 0x1979, 0x197A, 0x197B, 0x197C },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1981, 0x1982, 0x1983, 0x1984 },
                        new int[] { 0x198B, 0x198C, 0x198D, 0x198E },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1993, 0x1994, 0x1995, 0x1996 },
                        new int[] { 0x199D, 0x199E, 0x199F, 0x199E },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x19A5, 0x19A6, 0x19A7, 0x19A8 },
                        new int[] { 0x19AF, 0x19B0, 0x19B1, 0x19B2 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x19B7, 0x19B8, 0x19B9, 0x19BA },
                        new int[] { 0x19C1, 0x19C2, 0x19C3, 0x19C4 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x19C9, 0x19CA, 0x19CB, 0x19CC },
                        new int[] { 0x19D3, 0x19D4, 0x19D5, 0x19D6 },
                    },
                },
                [18] = new Dictionary<int, List<int[]>>                         // Lost City of Tinnos
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2 },
                        new int[] { 0x1DB9, 0x1DBA, 0x1DBB, 0x1DBC },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1DC1, 0x1DC2, 0x1DC3, 0x1DC4 },
                        new int[] { 0x1DCB, 0x1DCC, 0x1DCD, 0x1DCE },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1DD3, 0x1DD4, 0x1DD5, 0x1DD6 },
                        new int[] { 0x1DDD, 0x1DDE, 0x1DDF, 0x1DE0 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x1DE5, 0x1DE6, 0x1DE7, 0x1DE8 },
                        new int[] { 0x1DEF, 0x1DF0, 0x1DF1, 0x1DF2 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x1DF7, 0x1DF8, 0x1DF9, 0x1DFA },
                        new int[] { 0x1E01, 0x1E02, 0x1E03, 0x1E04 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x1E09, 0x1E0A, 0x1E0B, 0x1E0C },
                        new int[] { 0x1E13, 0x1E14, 0x1E15, 0x1E16 },
                    },
                },
                [19] = new Dictionary<int, List<int[]>>                         // Meteorite Cavern
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x0B01, 0x0B02, 0x0B03, 0x0B04 },
                        new int[] { 0x0B0B, 0x0B0C, 0x0B0D, 0x0B0E },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x0B13, 0x0B14, 0x0B15, 0x0B16 },
                        new int[] { 0x0B1D, 0x0B1E, 0x0B1F, 0x0B20 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x0B25, 0x0B26, 0x0B27, 0x0B28 },
                        new int[] { 0x0B2F, 0x0B30, 0x0B31, 0x0B32 },
                    },
                    [3] = new List<int[]>
                    {
                        new int[] { 0x0B37, 0x0B38, 0x0B39, 0x0B3A },
                        new int[] { 0x0B41, 0x0B42, 0x0B43, 0x0B44 },
                    },
                    [4] = new List<int[]>
                    {
                        new int[] { 0x0B49, 0x0B4A, 0x0B4B, 0x0B4C },
                        new int[] { 0x0B53, 0x0B54, 0x0B55, 0x0B56 },
                    },
                    [5] = new List<int[]>
                    {
                        new int[] { 0x0B5B, 0x0B5C, 0x0B5D, 0x0B5E },
                        new int[] { 0x0B65, 0x0B66, 0x0B67, 0x0B68 },
                    },
                },
                [20] = new Dictionary<int, List<int[]>>                         // All Hallows
                {
                    [0] = new List<int[]>
                    {
                        new int[] { 0x1045, 0x1046, 0x1047, 0x1048 },
                        new int[] { 0x104F, 0x1050, 0x1051, 0x1052 },
                    },
                    [1] = new List<int[]>
                    {
                        new int[] { 0x1057, 0x1058, 0x1059, 0x105A },
                        new int[] { 0x1061, 0x1062, 0x1063, 0x1064 },
                    },
                    [2] = new List<int[]>
                    {
                        new int[] { 0x1069, 0x106A, 0x106B, 0x106C },
                        new int[] { 0x1073, 0x1074, 0x1075, 0x1076 },
                    },
                },
            };

        private int GetSecondaryAmmoIndex()
        {
            byte levelIndex = GetLevelIndex();
            int ammoIndex = -1;

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                Dictionary<int, List<int[]>> indexData = ammoIndexData[levelIndex];

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

            int currentAmmoIndex = GetSecondaryAmmoIndex();

            for (int i = 0; i < 14; i++)
            {
                secondaryOffsets.Add(baseSecondaryOffset + i * 0x12);
            }

            validOffsets.Add(primaryOffset);

            if (currentAmmoIndex != -1)
            {
                validOffsets.Add(secondaryOffsets[currentAmmoIndex]);
            }

            return validOffsets.ToArray();
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
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2);
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validShotgunAmmoOffsets[0], ammo);
                WriteUInt16(validShotgunAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validDeagleAmmoOffsets[0], ammo);
                WriteUInt16(validDeagleAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validGrenadeLauncherAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[0], ammo);
                WriteUInt16(validRocketLauncherAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validHarpoonGunAmmoOffsets[0], ammo);
                WriteUInt16(validHarpoonGunAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validMp5AmmoOffsets[0], ammo);
                WriteUInt16(validMp5AmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
            int ammoIndex = GetSecondaryAmmoIndex();

            if (isPresent && ammoIndex != -1)
            {
                WriteUInt16(validUziAmmoOffsets[0], ammo);
                WriteUInt16(validUziAmmoOffsets[1], ammo);
            }
            else if (!isPresent && ammoIndex != -1)
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
