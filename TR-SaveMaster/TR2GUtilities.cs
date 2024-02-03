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

        // Ammo index
        private int secondaryAmmoIndex = -1;

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

            if (levelIndex == 1)        // The Cold War
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

                m16AmmoOffset2 = 0x1C32;
                grenadeLauncherAmmoOffset2 = 0x1C2E;
                harpoonGunAmmoOffset2 = 0x1C2A;
                shotgunAmmoOffset2 = 0x1C26;
                uziAmmoOffset2 = 0x1C22;
                automaticPistolsAmmoOffset2 = 0x1C1E;

                SetHealthOffsets(0xE54, 0xE60, 0xE6C, 0xE84);
            }
            else if (levelIndex == 2)   // Fool's Gold
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

                m16AmmoOffset2 = 0x1C3C;
                grenadeLauncherAmmoOffset2 = 0x1C38;
                harpoonGunAmmoOffset2 = 0x1C34;
                shotgunAmmoOffset2 = 0x1C30;
                uziAmmoOffset2 = 0x1C2C;
                automaticPistolsAmmoOffset2 = 0x1C28;

                SetHealthOffsets(0x12D6, 0x12E2, 0x12EE, 0x12FA);
            }
            else if (levelIndex == 3)   // Furnace of the Gods
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

                m16AmmoOffset2 = 0x1B2C;
                grenadeLauncherAmmoOffset2 = 0x1B28;
                harpoonGunAmmoOffset2 = 0x1B24;
                shotgunAmmoOffset2 = 0x1B20;
                uziAmmoOffset2 = 0x1B1C;
                automaticPistolsAmmoOffset2 = 0x1B18;

                SetHealthOffsets(0x1490, 0x14B4, 0x14C0, 0x14CC);
            }
            else if (levelIndex == 4)   // Kingdom
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

                m16AmmoOffset2 = 0x138A;
                grenadeLauncherAmmoOffset2 = 0x1386;
                harpoonGunAmmoOffset2 = 0x1382;
                shotgunAmmoOffset2 = 0x137E;
                uziAmmoOffset2 = 0x137A;
                automaticPistolsAmmoOffset2 = 0x1376;

                SetHealthOffsets(0x600);
            }
            else if (levelIndex == 5)   // Nightmare In Vegas
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

                m16AmmoOffset2 = 0x157C;
                grenadeLauncherAmmoOffset2 = 0x1578;
                harpoonGunAmmoOffset2 = 0x1574;
                shotgunAmmoOffset2 = 0x1570;
                uziAmmoOffset2 = 0x156C;
                automaticPistolsAmmoOffset2 = 0x1568;

                SetHealthOffsets(0x8AE, 0x8BA, 0x8C6);
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

        private int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
        {
            List<int> secondaryOffsets = new List<int>();
            List<int> validOffsets = new List<int>();

            for (int i = 0; i < 20; i++)
            {
                secondaryOffsets.Add(baseSecondaryOffset + i * 0x6);
            }

            validOffsets.Add(primaryOffset);

            if (secondaryAmmoIndex != -1)
            {
                validOffsets.Add(secondaryOffsets[secondaryAmmoIndex]);
            }

            return validOffsets.ToArray();
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

        private void WriteM16Ammo(bool isPresent, UInt16 ammo)
        {
            int[] validM16AmmoOffsets = GetValidAmmoOffsets(m16AmmoOffset, m16AmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validM16AmmoOffsets[0], ammo);
                WriteUInt16(validM16AmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validM16AmmoOffsets[0], ammo);
                WriteUInt16(validM16AmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validM16AmmoOffsets[0], ammo);
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

        private void WriteHarpoonGunAmmo(bool isPresent, UInt16 ammo)
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

        private void WriteAutomaticPistolsAmmo(bool isPresent, UInt16 ammo)
        {
            int[] validAutomaticPistolsAmmoOffsets = GetValidAmmoOffsets(automaticPistolsAmmoOffset, automaticPistolsAmmoOffset2);

            if (isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validAutomaticPistolsAmmoOffsets[0], ammo);
                WriteUInt16(validAutomaticPistolsAmmoOffsets[1], ammo);
            }
            else if (!isPresent && secondaryAmmoIndex != -1)
            {
                WriteUInt16(validAutomaticPistolsAmmoOffsets[0], ammo);
                WriteUInt16(validAutomaticPistolsAmmoOffsets[1], 0);
            }
            else
            {
                WriteUInt16(validAutomaticPistolsAmmoOffsets[0], ammo);
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
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSmallMedipacks((byte)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((byte)nudLargeMedipacks.Value);
            WriteNumFlares((byte)nudFlares.Value);

            secondaryAmmoIndex = GetSecondaryAmmoIndex();

            WriteM16Ammo(chkM16.Checked, (UInt16)nudM16Ammo.Value);
            WriteGrenadeLauncherAmmo(chkGrenadeLauncher.Checked, (UInt16)nudGrenadeLauncherAmmo.Value);
            WriteHarpoonGunAmmo(chkHarpoonGun.Checked, (UInt16)nudHarpoonGunAmmo.Value);
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
                WriteHealthValue((double)trbHealth.Value);
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

        private double GetHealthPercentage(int healthOffset)
        {
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
