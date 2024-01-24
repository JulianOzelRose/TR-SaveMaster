using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR2GUtilities
    {
        // Offsets
        private const int saveNumOffset = 0x4B;
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

        private string GetCleanLvlName()
        {
            string lvlName = GetLvlName();

            if (lvlName.StartsWith("The Cold War") || lvlName.StartsWith("Der kalte Krieg")) return "The Cold War";
            else if (lvlName.StartsWith("Fool's Gold") || lvlName.StartsWith("Das Gold des Narren")) return "Fool's Gold";
            else if (lvlName.StartsWith("Furnace of the Gods") || lvlName.StartsWith("Hochofen der G~otter")) return "Furnace of the Gods";
            else if (lvlName.StartsWith("Kingdom") || lvlName.StartsWith("K~onigreich")) return "Kingdom";
            else if (lvlName.StartsWith("Nightmare In Vegas") || lvlName.StartsWith("Alptraum in Vegas")) return "Nightmare In Vegas";

            return null;
        }

        public void DetermineOffsets()
        {
            string lvlName = GetCleanLvlName();

            if (lvlName == "The Cold War")
            {
                automaticPistolsAmmoOffset = 0x7D;
                uziAmmoOffset = 0x7F;
                shotgunAmmoOffset = 0x81;
                m16AmmoOffset = 0x83;
                grenadeLauncherAmmoOffset = 0x85;
                harpoonGunAmmoOffset = 0x87;
                smallMedipackOffset = 0x89;
                largeMedipackOffset = 0x8A;
                flaresOffset = 0x8C;
                weaponsConfigNumOffset = 0x8F;

                SetHealthOffsets(0xE60, 0xE6C);
            }
            else if (lvlName == "Fool's Gold")
            {
                automaticPistolsAmmoOffset = 0xA9;
                uziAmmoOffset = 0xAB;
                shotgunAmmoOffset = 0xAD;
                m16AmmoOffset = 0xAF;
                grenadeLauncherAmmoOffset = 0xB1;
                harpoonGunAmmoOffset = 0xB3;
                smallMedipackOffset = 0xB5;
                largeMedipackOffset = 0xB6;
                flaresOffset = 0xB8;
                weaponsConfigNumOffset = 0xBB;

                SetHealthOffsets(0x12D6, 0x12E2);
            }
            else if (lvlName == "Furnace of the Gods")
            {
                automaticPistolsAmmoOffset = 0xD5;
                uziAmmoOffset = 0xD7;
                shotgunAmmoOffset = 0xD9;
                m16AmmoOffset = 0xDB;
                grenadeLauncherAmmoOffset = 0xDD;
                harpoonGunAmmoOffset = 0xDF;
                smallMedipackOffset = 0xE1;
                largeMedipackOffset = 0xE2;
                flaresOffset = 0xE4;
                weaponsConfigNumOffset = 0xE7;

                SetHealthOffsets(0x1490, 0x14B4, 0x14C0, 0x14CC);
            }
            else if (lvlName == "Kingdom")
            {
                automaticPistolsAmmoOffset = 0x101;
                uziAmmoOffset = 0x103;
                shotgunAmmoOffset = 0x105;
                m16AmmoOffset = 0x107;
                grenadeLauncherAmmoOffset = 0x109;
                harpoonGunAmmoOffset = 0x10B;
                smallMedipackOffset = 0x10D;
                largeMedipackOffset = 0x10E;
                flaresOffset = 0x110;
                weaponsConfigNumOffset = 0x113;

                SetHealthOffsets(0x600);
            }
            else if (lvlName == "Nightmare In Vegas")
            {
                automaticPistolsAmmoOffset = 0x12D;
                uziAmmoOffset = 0x12F;
                shotgunAmmoOffset = 0x131;
                m16AmmoOffset = 0x133;
                grenadeLauncherAmmoOffset = 0x135;
                harpoonGunAmmoOffset = 0x137;
                smallMedipackOffset = 0x139;
                largeMedipackOffset = 0x13A;
                flaresOffset = 0x13C;
                weaponsConfigNumOffset = 0x13F;

                SetHealthOffsets(0x8AE, 0x8BA);
            }

            SetSecondaryAmmoOffsets();
        }

        private readonly Dictionary<string, Dictionary<int, List<int[]>>> ammoIndexData =
            new Dictionary<string, Dictionary<int, List<int[]>>>
            {
                ["The Cold War"] = new Dictionary<int, List<int[]>>
                {
                    [0x1C3A] = new List<int[]>
                    {
                        new int[] { 0x1C44, 0x1C45, 0x1C46, 0x1C47 },
                    },
                    [0x1C46] = new List<int[]>
                    {
                        new int[] { 0x1C46, 0x1C47, 0x1C48, 0x1C49 },
                        new int[] { 0x1C50, 0x1C51, 0x1C52, 0x1C53 },
                    },
                    [0x1C50] = new List<int[]>
                    {
                        new int[] { 0x1C50, 0x1C51, 0x1C52, 0x1C53 },
                    },
                    [0x1C52] = new List<int[]>
                    {
                        new int[] { 0x1C5C, 0x1C5D, 0x1C5E, 0x1C5F },
                        new int[] { 0x1C52, 0x1C53, 0x1C54, 0x1C55 },
                    },
                    [0x1C58] = new List<int[]>
                    {
                        new int[] { 0x1C58, 0x1C59, 0x1C5A, 0x1C5B },
                    },
                    [0x1C5E] = new List<int[]>
                    {
                        new int[] { 0x1C68, 0x1C69, 0x1C6A, 0x1C6B },
                    },
                    [0x1C6A] = new List<int[]>
                    {
                        new int[] { 0x1C74, 0x1C75, 0x1C76, 0x1C77 },
                    },
                    [0x1C76] = new List<int[]>
                    {
                        new int[] { 0x1C76, 0x1C77, 0x1C78, 0x1C79 },
                        new int[] { 0x1C80, 0x1C81, 0x1C82, 0x1C83 },
                    },
                    [0x1C82] = new List<int[]>
                    {
                        new int[] { 0x1C82, 0x1C83, 0x1C84, 0x1C85 },
                        new int[] { 0x1C8C, 0x1C8D, 0x1C8E, 0x1C8F },
                    },
                    [0x1C8E] = new List<int[]>
                    {
                        new int[] { 0x1C8E, 0x1C8F, 0x1C90, 0x1C91 },
                    },
                    [0x1C9A] = new List<int[]>
                    {
                        new int[] { 0x1C9A, 0x1C9B, 0x1C9C, 0x1C9D },
                        new int[] { 0x1CA4, 0x1CA5, 0x1CA6, 0x1CA7 },
                    },
                },
                ["Fool's Gold"] = new Dictionary<int, List<int[]>>
                {
                    [0x1C44] = new List<int[]>
                    {
                        new int[] { 0x1C44, 0x1C45, 0x1C46, 0x1C47 },
                        new int[] { 0x1C4E, 0x1C4F, 0x1C50, 0x1C51 },
                    },
                    [0x1C50] = new List<int[]>
                    {
                        new int[] { 0x1C5A, 0x1C5B, 0x1C5C, 0x1C5D },
                    },
                    [0x1C56] = new List<int[]>
                    {
                        new int[] { 0x1C56, 0x1C57, 0x1C58, 0x1C59 },
                    },
                    [0x1C5C] = new List<int[]>
                    {
                        new int[] { 0x1C66, 0x1C67, 0x1C68, 0x1C69 },
                    },
                    [0x1C62] = new List<int[]>
                    {
                        new int[] { 0x1C62, 0x1C63, 0x1C64, 0x1C65 },
                    },
                    [0x1C68] = new List<int[]>
                    {
                        new int[] { 0x1C68, 0x1C69, 0x1C6A, 0x1C6B },
                        new int[] { 0x1C72, 0x1C73, 0x1C74, 0x1C75 },
                    },
                    [0x1C6E] = new List<int[]>
                    {
                        new int[] { 0x1C6E, 0x1C6F, 0x1C70, 0x1C71 },
                    },
                    [0x1C74] = new List<int[]>
                    {
                        new int[] { 0x1C74, 0x1C75, 0x1C76, 0x1C77 },
                        new int[] { 0x1C7E, 0x1C7F, 0x1C80, 0x1C81 },
                    },
                    [0x1C7A] = new List<int[]>
                    {
                        new int[] { 0x1C84, 0x1C85, 0x1C86, 0x1C87 },
                        new int[] { 0x1C7A, 0x1C7B, 0x1C7C, 0x1C7D },
                    },
                    [0x1C86] = new List<int[]>
                    {
                        new int[] { 0x1C86, 0x1C87, 0x1C88, 0x1C89 },
                    },
                    [0x1C92] = new List<int[]>
                    {
                        new int[] { 0x1C92, 0x1C93, 0x1C94, 0x1C95 },
                    },
                    [0x1C9E] = new List<int[]>
                    {
                        new int[] { 0x1C9E, 0x1C9F, 0x1CA0, 0x1CA1 },
                    },
                    [0x1CAA] = new List<int[]>
                    {
                        new int[] { 0x1CAA, 0x1CAB, 0x1CAC, 0x1CAD },
                    },
                    [0x1CB6] = new List<int[]>
                    {
                        new int[] { 0x1CB6, 0x1CB7, 0x1CB8, 0x1CB9 },
                    },
                },
                ["Furnace of the Gods"] = new Dictionary<int, List<int[]>>
                {
                    [0x1B34] = new List<int[]>
                    {
                        new int[] { 0x1B34, 0x1B35, 0x1B36, 0x1B37 },
                    },
                    [0x1B40] = new List<int[]>
                    {
                        new int[] { 0x1B40, 0x1B41, 0x1B42, 0x1B43 },
                    },
                    [0x1B4C] = new List<int[]>
                    {
                        new int[] { 0x1B4C, 0x1B4D, 0x1B4E, 0x1B4F },
                    },
                    [0x1B58] = new List<int[]>
                    {
                        new int[] { 0x1B58, 0x1B59, 0x1B5A, 0x1B5B },
                        new int[] { 0x1B62, 0x1B63, 0x1B64, 0x1B65 },
                    },
                    [0x1B64] = new List<int[]>
                    {
                        new int[] { 0x1B64, 0x1B65, 0x1B66, 0x1B67 },
                        new int[] { 0x1B6E, 0x1B6F, 0x1B70, 0x1B71 },
                    },
                    [0x1B70] = new List<int[]>
                    {
                        new int[] { 0x1B70, 0x1B71, 0x1B72, 0x1B73 },
                        new int[] { 0x1B7A, 0x1B7B, 0x1B7C, 0x1B7D },
                    },
                },
                ["Kingdom"] = new Dictionary<int, List<int[]>>
                {
                    [0x1392] = new List<int[]>
                    {
                        new int[] { 0x1392, 0x1393, 0x1394, 0x1395 },
                        new int[] { 0x139C, 0x139D, 0x139E, 0x139F },
                    },
                    [0x139E] = new List<int[]>
                    {
                        new int[] { 0x13A8, 0x13A9, 0x13AA, 0x13AB },
                    },
                    [0x13AA] = new List<int[]>
                    {
                        new int[] { 0x13AA, 0x13AB, 0x13AC, 0x13AD },
                        new int[] { 0x13B4, 0x13B5, 0x13B6, 0x13B7 },
                    },
                    [0x13B6] = new List<int[]>
                    {
                        new int[] { 0x13B6, 0x13B7, 0x13B8, 0x13B9 },
                        new int[] { 0x13C0, 0x13C1, 0x13C2, 0x13C3 },
                    },
                    [0x13C2] = new List<int[]>
                    {
                        new int[] { 0x13C2, 0x13C3, 0x13C4, 0x13C5 },
                        new int[] { 0x13CC, 0x13CD, 0x13CE, 0x13CF },
                    },
                    [0x13CE] = new List<int[]>
                    {
                        new int[] { 0x13D8, 0x13D9, 0x13DA, 0x13DB },
                    }
                },
                ["Nightmare In Vegas"] = new Dictionary<int, List<int[]>>
                {
                    [0x1584] = new List<int[]>
                    {
                        new int[] { 0x1584, 0x1585, 0x1586, 0x1587 },
                    },
                    [0x1590] = new List<int[]>
                    {
                        new int[] { 0x1590, 0x1591, 0x1592, 0x1593 },
                        new int[] { 0x159A, 0x159B, 0x159C, 0x159D },
                    },
                    [0x159C] = new List<int[]>
                    {
                        new int[] { 0x159C, 0x159D, 0x159E, 0x159F },
                        new int[] { 0x15A6, 0x15A7, 0x15A8, 0x15A9 },
                    },
                    [0x15A8] = new List<int[]>
                    {
                        new int[] { 0x15A8, 0x15A9, 0x15AA, 0x15AB },
                        new int[] { 0x15B2, 0x15B3, 0x15B4, 0x15B5 },
                    },
                    [0x15B4] = new List<int[]>
                    {
                        new int[] { 0x15BE, 0x15BF, 0x15C0, 0x15C1 },
                    },
                    [0x15C0] = new List<int[]>
                    {
                        new int[] { 0x15CA, 0x15CB, 0x15CC, 0x15CD },
                    },
                },
            };

        private int GetSecondaryAmmoIndexMarker()
        {
            string lvlName = GetCleanLvlName();
            int ammoIndexMarker = -1;

            if (ammoIndexData.ContainsKey(lvlName))
            {
                Dictionary<int, List<int[]>> indexData = ammoIndexData[lvlName];
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

        private void SetSecondaryAmmoOffsets()
        {
            int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

            m16AmmoOffset2 = secondaryAmmoIndexMarker - 8;
            grenadeLauncherAmmoOffset2 = secondaryAmmoIndexMarker - 12;
            harpoonGunAmmoOffset2 = secondaryAmmoIndexMarker - 16;
            shotgunAmmoOffset2 = secondaryAmmoIndexMarker - 20;
            uziAmmoOffset2 = secondaryAmmoIndexMarker - 24;
            automaticPistolsAmmoOffset2 = secondaryAmmoIndexMarker - 28;
        }

        private void WriteSmallMedipacks(byte value)
        {
            WriteByte(smallMedipackOffset, value);
        }

        private void WriteLargeMedipacks(byte value)
        {
            WriteByte(largeMedipackOffset, value);
        }

        private void WriteFlares(byte value)
        {
            WriteByte(flaresOffset, value);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumOffset, value);
        }

        private void WriteM16Ammo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(m16AmmoOffset, ammo);
                WriteUInt16(m16AmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(m16AmmoOffset, ammo);
                WriteUInt16(m16AmmoOffset2, 0);
            }
        }

        private void WriteGrenadeLauncherAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
                WriteUInt16(grenadeLauncherAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(grenadeLauncherAmmoOffset, ammo);
                WriteUInt16(grenadeLauncherAmmoOffset2, 0);
            }
        }

        private void WriteHarpoonGunAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(harpoonGunAmmoOffset, ammo);
                WriteUInt16(harpoonGunAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(harpoonGunAmmoOffset, ammo);
                WriteUInt16(harpoonGunAmmoOffset2, 0);
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

        private void WriteAutomaticPistolsAmmo(bool isPresent, UInt16 ammo)
        {
            if (isPresent)
            {
                WriteUInt16(automaticPistolsAmmoOffset, ammo);
                WriteUInt16(automaticPistolsAmmoOffset2, ammo);
            }
            else
            {
                WriteUInt16(automaticPistolsAmmoOffset, ammo);
                WriteUInt16(automaticPistolsAmmoOffset2, 0);
            }
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

        private void WriteHealthValue(double newHealthPercentage)
        {
            int healthOffset = GetHealthOffset();

            UInt16 newHealth = (UInt16)(newHealthPercentage / 100.0 * MAX_HEALTH_VALUE);
            WriteUInt16(healthOffset, newHealth);
        }

        private void WriteWeaponsConfigNum(byte value)
        {
            WriteByte(weaponsConfigNumOffset, value);
        }

        public void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
            }
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
            return ReadUInt16(saveNumOffset);
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

        public void DisplayGameInfo(TextBox txtLvlName, CheckBox chkPistols, CheckBox chkAutomaticPistols, CheckBox chkUzis,
            CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudAutomaticPistolsAmmo,
            CheckBox chkShotgun, NumericUpDown nudUziAmmo, NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo,
            NumericUpDown nudHarpoonGunAmmo, NumericUpDown nudShotgunAmmo, NumericUpDown nudSaveNumber, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, TrackBar trbHealth, Label lblHealth,
            Label lblHealthError)
        {
            txtLvlName.Text = GetCleanLvlName();

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

            if (GetCleanLvlName() == "Nightmare In Vegas")
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

        public void WriteChanges(CheckBox chkPistols, CheckBox chkAutomaticPistols, CheckBox chkUzis, CheckBox chkShotgun,
            CheckBox chkM16, CheckBox chkGrenadeLauncher, CheckBox chkHarpoonGun, NumericUpDown nudSaveNumber, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudAutomaticPistolsAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudM16Ammo, NumericUpDown nudGrenadeLauncherAmmo, NumericUpDown nudHarpoonGunAmmo,
            NumericUpDown nudShotgunAmmo, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteLargeMedipacks((byte)nudLargeMedipacks.Value);
            WriteFlares((byte)nudFlares.Value);

            if (GetSecondaryAmmoIndexMarker() != -1)
            {
                WriteM16Ammo(chkM16.Checked, (UInt16)nudM16Ammo.Value);
                WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
                WriteHarpoonGunAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
                WriteShotgunAmmo(chkShotgun.Checked, (UInt16)(nudShotgunAmmo.Value * 6));
                WriteUziAmmo(chkUzis.Checked, (UInt16)nudUziAmmo.Value);
                WriteAutomaticPistolsAmmo(chkAutomaticPistols.Checked, (UInt16)nudAutomaticPistolsAmmo.Value);
            }

            byte newWeaponsConfigNum = 1;

            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkAutomaticPistols.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;
            if (chkM16.Checked) newWeaponsConfigNum += 32;
            if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 64;
            if (chkHarpoonGun.Checked) newWeaponsConfigNum += 128;

            WriteWeaponsConfigNum(newWeaponsConfigNum);

            if (GetHealthOffset() != -1)
            {
                double newHealthPercentage = (double)trbHealth.Value;
                WriteHealthValue(newHealthPercentage);
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
