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
                SetHealthOffsets(0x1797, 0x1785, 0x17A9, 0x17BB);

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
                SetHealthOffsets(0xB15);

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
                SetHealthOffsets(0x2135);

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
                SetHealthOffsets(0xAB1, 0xAD5);

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

        private readonly Dictionary<byte, Dictionary<int, int[]>> ammoIndexData = new Dictionary<byte, Dictionary<int, int[]>>
        {
            [1] = new Dictionary<int, int[]>                            // Jungle
            {
                [0] = new int[] { 0x1663, 0x1664, 0x1665, 0x1666 },
                [1] = new int[] { 0x1675, 0x1676, 0x1677, 0x1678 },
                [2] = new int[] { 0x1687, 0x1688, 0x1689, 0x168A },
                [3] = new int[] { 0x1699, 0x169A, 0x169B, 0x169C },
                [4] = new int[] { 0x16AB, 0x16AC, 0x16AD, 0x16AE },
                [5] = new int[] { 0x16BD, 0x16BE, 0x16BF, 0x16C0 }
            },
            [2] = new Dictionary<int, int[]>                            // Temple Ruins
            {
                [0] = new int[] { 0x23D3, 0x23D4, 0x23D5, 0x23D6 },
                [1] = new int[] { 0x23E5, 0x23E6, 0x23E7, 0x23E8 },
            },
            [3] = new Dictionary<int, int[]>                            // The River Ganges
            {
                [0] = new int[] { 0x181C, 0x181D, 0x181E, 0x181F },
                [1] = new int[] { 0x182E, 0x182F, 0x1830, 0x1831 },
            },
            [4] = new Dictionary<int, int[]>                            // Caves Of Kaliya
            {
                [0] = new int[] { 0xD37, 0xD38, 0xD39, 0xD3A },
                [1] = new int[] { 0xD53, 0xD54, 0xD55, 0xD56 }
            },
            [13] = new Dictionary<int, int[]>                            // Nevada Desert
            {
                [0] = new int[] { 0x17BC, 0x17BD, 0x17BE, 0x17BF },
                [1] = new int[] { 0x17CE, 0x17CF, 0x17D0, 0x17D1 },
                [2] = new int[] { 0x17E0, 0x17E1, 0x17E2, 0x17E3 },
                [3] = new int[] { 0x17F2, 0x17F3, 0x17F4, 0x17F5 }
            },
            [14] = new Dictionary<int, int[]>                           // High Security Compound
            {
                [0] = new int[] { 0x1E63, 0x1E64, 0x1E65, 0x1E66 },
                [1] = new int[] { 0x1E75, 0x1E76, 0x1E77, 0x1E78 },
                [2] = new int[] { 0x1E91, 0x1E92, 0x1E93, 0x1E94 },
                [3] = new int[] { 0x1EA3, 0x1EA4, 0x1EA5, 0x1EA6 },
                [4] = new int[] { 0x1EB5, 0x1EB6, 0x1EB7, 0x1EB8 },
                [5] = new int[] { 0x1EBD, 0x1EBE, 0x1EBF, 0x1EC0 },
                [6] = new int[] { 0x1ECF, 0x1ED0, 0x1ED1, 0x1ED2 }
            },
            [15] = new Dictionary<int, int[]>                           // Area 51
            {
                [0] = new int[] { 0x2125, 0x2126, 0x2127, 0x2128 },
                [1] = new int[] { 0x2137, 0x2138, 0x2139, 0x213A },
                [2] = new int[] { 0x2149, 0x214A, 0x214B, 0x214C },
                [3] = new int[] { 0x215B, 0x215C, 0x215D, 0x215E },
                [4] = new int[] { 0x216D, 0x216E, 0x216F, 0x2170 },
                [5] = new int[] { 0x217F, 0x2180, 0x2181, 0x2182 },
                [6] = new int[] { 0x219B, 0x219C, 0x219D, 0x219E },
                [7] = new int[] { 0x21A3, 0x21A4, 0x21A5, 0x21A6 },
                [8] = new int[] { 0x21BF, 0x21C0, 0x21C1, 0x21C2 },
                [9] = new int[] { 0x21C7, 0x21C8, 0x21C9, 0x21CA }
            },
            [5] = new Dictionary<int, int[]>                            // Coastal Village
            {
                [0] = new int[] { 0x17C9, 0x17CA, 0x17CB, 0x17CC },
                [1] = new int[] { 0x17DB, 0x17DC, 0x17DD, 0x17DE },
                [2] = new int[] { 0x17ED, 0x17EE, 0x17EF, 0x17F0 }
            },
            [6] = new Dictionary<int, int[]>                            // Crash Site
            {
                [0] = new int[] { 0x18EB, 0x18EC, 0x18ED, 0x18EE },
                [1] = new int[] { 0x18FD, 0x18FE, 0x18FF, 0x1900 },
                [2] = new int[] { 0x190F, 0x1910, 0x1911, 0x1912 },
                [3] = new int[] { 0x1921, 0x1922, 0x1923, 0x1924 },
                [4] = new int[] { 0x1933, 0x1934, 0x1935, 0x1936 },
                [5] = new int[] { 0x1945, 0x1946, 0x1947, 0x1948 }
            },
            [7] = new Dictionary<int, int[]>                            // Madubu Gorge
            {
                [0] = new int[] { 0x1435, 0x1436, 0x1437, 0x1438 },
                [1] = new int[] { 0x1447, 0x1448, 0x1449, 0x144A },
                [2] = new int[] { 0x1459, 0x145A, 0x145B, 0x145C },
                [3] = new int[] { 0x146B, 0x146C, 0x146D, 0x146E },
                [4] = new int[] { 0x1487, 0x1488, 0x1489, 0x148A },
                [5] = new int[] { 0x148F, 0x1490, 0x1491, 0x1492 }
            },
            [8] = new Dictionary<int, int[]>                            // Temple Of Puna
            {
                [0] = new int[] { 0x110D, 0x110E, 0x110F, 0x1110 },
                [1] = new int[] { 0x105D, 0x105E, 0x105F, 0x1060 }
            },
            [9] = new Dictionary<int, int[]>                            // Thames Wharf
            {
                [0] = new int[] { 0x188B, 0x188C, 0x188D, 0x188E },
                [1] = new int[] { 0x189D, 0x189E, 0x189F, 0x18A0 },
                [2] = new int[] { 0x18AF, 0x18B0, 0x18B1, 0x18B2 }
            },
            [10] = new Dictionary<int, int[]>                           // Aldwych
            {
                [0] = new int[] { 0x2317, 0x2318, 0x2319, 0x231A },
                [1] = new int[] { 0x2329, 0x232A, 0x232B, 0x232C }
            },
            [11] = new Dictionary<int, int[]>                           // Lud's Gate
            {
                [0] = new int[] { 0x1D8F, 0x1D90, 0x1D91, 0x1D92 },
                [1] = new int[] { 0x1DA1, 0x1DA2, 0x1DA3, 0x1DA4 },
                [2] = new int[] { 0x1DB3, 0x1DB4, 0x1DB5, 0x1DB6 },
                [3] = new int[] { 0x1D03, 0x1D04, 0x1D05, 0x1D06 }
            },
            [12] = new Dictionary<int, int[]>                           // City
            {
                [0] = new int[] { 0xB0B, 0xB0C, 0xB0D, 0xB0E },
                [1] = new int[] { 0xB1D, 0xB1E, 0xB1F, 0xB20 }
            },
            [16] = new Dictionary<int, int[]>                           // Antarctica
            {
                [0] = new int[] { 0x19AD, 0x19AE, 0x19AF, 0x19B0 },
                [1] = new int[] { 0x19BF, 0x19C0, 0x19C1, 0x19C2 }
            },
            [17] = new Dictionary<int, int[]>                           // RX-Tech Mines
            {
                [0] = new int[] { 0x196F, 0x1970, 0x1971, 0x1972 },
                [1] = new int[] { 0x1981, 0x1982, 0x1983, 0x1984 },
                [2] = new int[] { 0x1993, 0x1994, 0x1995, 0x1996 },
                [3] = new int[] { 0x19A5, 0x19A6, 0x19A7, 0x19A8 }
            },
            [18] = new Dictionary<int, int[]>                           // Lost City Of Tinnos
            {
                [0] = new int[] { 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2 },
                [1] = new int[] { 0x1DC1, 0x1DC2, 0x1DC3, 0x1DC4 },
                [2] = new int[] { 0x1DD3, 0x1DD4, 0x1DD5, 0x1DD6 },
            },
            [19] = new Dictionary<int, int[]>                           // Meteorite Cavern
            {
                [0] = new int[] { 0xB01, 0xB02, 0xB03, 0xB04 },
                [1] = new int[] { 0xB13, 0xB14, 0xB15, 0xB16 },
            },
            [20] = new Dictionary<int, int[]>                           // All Hallows
            {
                [0] = new int[] { 0x1045, 0x1046, 0x1047, 0x1048 },
                [1] = new int[] { 0x1057, 0x1058, 0x1059, 0x105A },
                [2] = new int[] { 0x1609, 0x106A, 0x106B, 0x106C }
            }
        };

        private int GetSecondaryAmmoIndex()
        {
            byte levelIndex = GetLevelIndex();
            int ammoIndex = -1;

            if (ammoIndexData.ContainsKey(levelIndex))
            {
                Dictionary<int, int[]> indexData = ammoIndexData[levelIndex];

                for (int i = 0; i < indexData.Count; i++)
                {
                    int key = indexData.ElementAt(i).Key;
                    int[] offsets = indexData.ElementAt(i).Value;

                    if (offsets.All(offset => ReadByte(offset) == 0xFF))
                    {
                        ammoIndex = key;
                        break;
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

            for (int i = 0; i < 10; i++)
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
