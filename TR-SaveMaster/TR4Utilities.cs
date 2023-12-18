using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    class TR4Utilities
    {
        // Offsets
        private const int saveNumOffset = 0x4B;
        private const int binocularsOffset = 0x170;
        private const int crowbarOffset = 0x171;
        private const int numSecretsOffset = 0x1FB;
        private const int smallMedipackOffset = 0x190;
        private const int largeMedipackOffset = 0x192;
        private const int flaresOffset = 0x194;
        private const int pistolsOffset = 0x169;
        private const int uziOffset = 0x16A;
        private const int shotgunOffset = 0x16B;
        private const int crossbowOffset = 0x16C;
        private const int grenadeGunOffset = 0x16D;
        private const int revolverOffset = 0x16E;
        private const int laserSightOffset = 0x16F;
        private const int uziAmmoOffset = 0x198;
        private const int revolverAmmoOffset = 0x19A;
        private const int grenadeGunNormalAmmoOffset = 0x1A0;
        private const int grenadeGunSuperAmmoOffset = 0x1A2;
        private const int grenadeGunFlashAmmoOffset = 0x1A4;
        private const int shotgunNormalAmmoOffset = 0x19C;
        private const int shotgunWideshotAmmoOffset = 0x19E;
        private const int crossbowNormalAmmoOffset = 0x1A6;
        private const int crossbowPoisonAmmoOffset = 0x1A8;
        private const int crossbowExplosiveAmmoOffset = 0x1AA;

        // Checksum
        private const int CHECKSUM_START_OFFSET = 0x57;
        private const int EOF_OFFSET = 0x3E51;

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

        public void SetLevelParams(CheckBox chkBinoculars, CheckBox chkLaserSight, CheckBox chkCrowbar)
        {
            string lvlName = GetLvlName();

            if (lvlName.StartsWith("Angkor Wat"))
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Race For The Iris"))
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("The Tomb Of Seth"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Burial Chambers"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Valley Of The Kings"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("KV5"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Temple Of Karnak"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("The Great Hypostyle Hall"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Sacred Lake"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Tomb Of Semerkhet"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Guardian Of Semerkhet"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName.StartsWith("Desert Railroad"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Alexandria"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Coastal Ruins"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Catacombs"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Temple Of Poseidon"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("The Lost Library"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Hall Of Demetrius"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Pharos, Temple Of Isis"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Cleopatra's Palaces"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("City Of The Dead"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Chambers Of Tulun"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Citadel Gate"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Trenches"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Street Bazaar"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Citadel"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("The Sphinx Complex"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Underneath The Sphinx"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Menkaure's Pyramid"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Inside Menkaure's Pyramid"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("The Mastabas"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("The Great Pyramid"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Khufu's Queens Pyramids"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Inside The Great Pyramid"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("Temple Of Horus"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName.StartsWith("The Times Exclusive"))
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
        }

        public void DisplayGameInfo(TextBox txtLvlName, NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudRevolverAmmo, NumericUpDown nudUziAmmo,
            NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo, NumericUpDown nudGrenadeGunNormalAmmo,
            NumericUpDown nudGrenadeGunSuperAmmo, NumericUpDown nudGrenadeGunFlashAmmo, NumericUpDown nudCrossbowNormalAmmo,
            NumericUpDown nudCrossbowPoisonAmmo, NumericUpDown nudCrossbowExplosiveAmmo, CheckBox chkBinoculars, CheckBox chkCrowbar,
            CheckBox chkLaserSight, CheckBox chkPistols, CheckBox chkRevolver, CheckBox chkUzis, CheckBox chkShotgun, CheckBox chkCrossbow,
            CheckBox chkGrenadeGun)
        {
            txtLvlName.Text = GetLvlName();

            nudSaveNumber.Value = GetSaveNumber();
            nudSecrets.Value = GetNumSecrets();
            nudFlares.Value = GetNumFlares();
            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();

            nudRevolverAmmo.Value = GetRevolverAmmo();
            nudUziAmmo.Value = GetUziAmmo();
            nudShotgunNormalAmmo.Value = GetShotgunNormalAmmo() / 6;
            nudShotgunWideshotAmmo.Value = GetShotgunWideshotAmmo() / 6;
            nudGrenadeGunNormalAmmo.Value = GetGrenadeGunNormalAmmo();
            nudGrenadeGunSuperAmmo.Value = GetGrenadeGunSuperAmmo();
            nudGrenadeGunFlashAmmo.Value = GetGrenadeGunFlashAmmo();
            nudCrossbowNormalAmmo.Value = GetCrossbowNormalAmmo();
            nudCrossbowPoisonAmmo.Value = GetCrossbowPoisonAmmo();
            nudCrossbowExplosiveAmmo.Value = GetCrossbowExplosiveAmmo();

            chkBinoculars.Checked = IsBinocularsPresent();
            chkCrowbar.Checked = IsCrowbarPresent();
            chkLaserSight.Checked = IsLaserSightPresent();
            chkPistols.Checked = GetPistolsValue() != 0;
            chkRevolver.Checked = GetRevolverValue() != 0;
            chkUzis.Checked = GetUziValue() != 0;
            chkShotgun.Checked = GetShotgunValue() != 0;
            chkCrossbow.Checked = GetCrossbowValue() != 0;
            chkGrenadeGun.Checked = GetGrenadeGunValue() != 0;
        }

        public void WriteChanges(NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudRevolverAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudGrenadeGunNormalAmmo, NumericUpDown nudGrenadeGunSuperAmmo,
            NumericUpDown nudGrenadeGunFlashAmmo, NumericUpDown nudCrossbowNormalAmmo, NumericUpDown nudCrossbowPoisonAmmo,
            NumericUpDown nudCrossbowExplosiveAmmo, NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo,
            CheckBox chkPistols, CheckBox chkUzis, CheckBox chkShotgun, CheckBox chkCrossbow, CheckBox chkGrenadeGun,
            CheckBox chkRevolver, CheckBox chkBinoculars, CheckBox chkCrowbar, CheckBox chkLaserSight)
        {
            byte prevPistolsValue = GetPistolsValue();
            byte prevUziValue = GetUziValue();
            byte prevShotgunValue = GetShotgunValue();
            byte prevCrossbowValue = GetCrossbowValue();
            byte prevGrenadeGunValue = GetGrenadeGunValue();
            byte prevRevolverValue = GetRevolverValue();

            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSecrets((byte)nudSecrets.Value);
            WriteFlares((UInt16)nudFlares.Value);
            WriteSmallMedipacks((UInt16)nudSmallMedipacks.Value);
            WriteLargeMedipacks((UInt16)nudLargeMedipacks.Value);

            WriteRevolverAmmo((UInt16)nudRevolverAmmo.Value);
            WriteUziAmmo((UInt16)nudUziAmmo.Value);
            WriteGrenadeGunNormalAmmo((UInt16)nudGrenadeGunNormalAmmo.Value);
            WriteGrenadeGunSuperAmmo((UInt16)nudGrenadeGunSuperAmmo.Value);
            WriteGrenadeGunFlashAmmo((UInt16)nudGrenadeGunFlashAmmo.Value);
            WriteCrossbowNormalAmmo((UInt16)nudCrossbowNormalAmmo.Value);
            WriteCrossbowPoisonAmmo((UInt16)nudCrossbowPoisonAmmo.Value);
            WriteCrossbowExplosiveAmmo((UInt16)nudCrossbowExplosiveAmmo.Value);
            WriteShotgunNormalAmmo((UInt16)(nudShotgunNormalAmmo.Value * 6));
            WriteShotgunWideshotAmmo((UInt16)(nudShotgunWideshotAmmo.Value * 6));

            WritePistolsPresent(chkPistols.Checked, prevPistolsValue);
            WriteUzisPresent(chkUzis.Checked, prevUziValue);
            WriteShotgunPresent(chkShotgun.Checked, prevShotgunValue);
            WriteCrossbowPresent(chkCrossbow.Checked, prevCrossbowValue);
            WriteGrenadeGunPresent(chkGrenadeGun.Checked, prevGrenadeGunValue);
            WriteRevolverPresent(chkRevolver.Checked, prevRevolverValue);

            if (chkBinoculars.Enabled)
            {
                WriteBinocularsPresent(chkBinoculars.Checked);
            }

            if (chkCrowbar.Enabled)
            {
                WriteCrowbarPresent(chkCrowbar.Checked);
            }

            if (chkLaserSight.Enabled)
            {
                WriteLaserSightPresent(chkLaserSight.Checked);
            }

            byte checksum = CalculateChecksum();
            WriteChecksum(checksum);
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

        private byte CalculateChecksum()
        {
            byte checksum = 0;

            using (FileStream fileStream = new FileStream(savegamePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Seek(CHECKSUM_START_OFFSET, SeekOrigin.Begin);

                while (fileStream.Position < EOF_OFFSET)
                {
                    int byteValue = fileStream.ReadByte();

                    if (byteValue == -1)
                    {
                        break;
                    }

                    checksum = (byte)((checksum - byteValue) % 256);
                }

                checksum = (byte)((checksum + 256) % 256);
            }

            return checksum;
        }


        private void WriteChecksum(byte checksum)
        {
            WriteByte(EOF_OFFSET, checksum);
        }

        private bool IsBinocularsPresent()
        {
            return ReadByte(binocularsOffset) != 0;
        }

        private bool IsCrowbarPresent()
        {
            return ReadByte(crowbarOffset) != 0;
        }

        private bool IsLaserSightPresent()
        {
            return ReadByte(laserSightOffset) != 0;
        }

        private byte GetNumSecrets()
        {
            return ReadByte(numSecretsOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumOffset);
        }

        private UInt16 GetNumFlares()
        {
            return ReadUInt16(flaresOffset);
        }

        private UInt16 GetNumSmallMedipacks()
        {
            return ReadUInt16(smallMedipackOffset);
        }

        private UInt16 GetNumLargeMedipacks()
        {
            return ReadUInt16(largeMedipackOffset);
        }

        private byte GetPistolsValue()
        {
            return ReadByte(pistolsOffset);
        }

        private byte GetUziValue()
        {
            return ReadByte(uziOffset);
        }

        private byte GetShotgunValue()
        {
            return ReadByte(shotgunOffset);
        }

        private byte GetCrossbowValue()
        {
            return ReadByte(crossbowOffset);
        }

        private byte GetGrenadeGunValue()
        {
            return ReadByte(grenadeGunOffset);
        }

        private byte GetRevolverValue()
        {
            return ReadByte(revolverOffset);
        }

        private UInt16 GetRevolverAmmo()
        {
            return ReadUInt16(revolverAmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private UInt16 GetShotgunNormalAmmo()
        {
            return ReadUInt16(shotgunNormalAmmoOffset);
        }

        private UInt16 GetShotgunWideshotAmmo()
        {
            return ReadUInt16(shotgunWideshotAmmoOffset);
        }

        private UInt16 GetCrossbowNormalAmmo()
        {
            return ReadUInt16(crossbowNormalAmmoOffset);
        }

        private UInt16 GetCrossbowPoisonAmmo()
        {
            return ReadUInt16(crossbowPoisonAmmoOffset);
        }

        private UInt16 GetCrossbowExplosiveAmmo()
        {
            return ReadUInt16(crossbowExplosiveAmmoOffset);
        }

        private UInt16 GetGrenadeGunNormalAmmo()
        {
            return ReadUInt16(grenadeGunNormalAmmoOffset);
        }

        private UInt16 GetGrenadeGunSuperAmmo()
        {
            return ReadUInt16(grenadeGunSuperAmmoOffset);
        }

        private UInt16 GetGrenadeGunFlashAmmo()
        {
            return ReadUInt16(grenadeGunFlashAmmoOffset);
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumOffset, value);
        }

        private void WriteNumSecrets(byte value)
        {
            WriteByte(numSecretsOffset, value);
        }

        private void WriteSmallMedipacks(UInt16 value)
        {
            WriteUInt16(smallMedipackOffset, value);
        }

        private void WriteLargeMedipacks(UInt16 value)
        {
            WriteUInt16(largeMedipackOffset, value);
        }

        private void WriteFlares(UInt16 value)
        {
            WriteUInt16(flaresOffset, value);
        }

        private void WriteRevolverAmmo(UInt16 ammo)
        {
            WriteUInt16(revolverAmmoOffset, ammo);
        }

        private void WriteUziAmmo(UInt16 ammo)
        {
            WriteUInt16(uziAmmoOffset, ammo);
        }

        private void WriteShotgunNormalAmmo(UInt16 ammo)
        {
            WriteUInt16(shotgunNormalAmmoOffset, ammo);
        }

        private void WriteShotgunWideshotAmmo(UInt16 ammo)
        {
            WriteUInt16(shotgunWideshotAmmoOffset, ammo);
        }

        private void WriteGrenadeGunNormalAmmo(UInt16 ammo)
        {
            WriteUInt16(grenadeGunNormalAmmoOffset, ammo);
        }

        private void WriteGrenadeGunSuperAmmo(UInt16 ammo)
        {
            WriteUInt16(grenadeGunSuperAmmoOffset, ammo);
        }

        private void WriteGrenadeGunFlashAmmo(UInt16 ammo)
        {
            WriteUInt16(grenadeGunFlashAmmoOffset, ammo);
        }

        private void WriteCrossbowNormalAmmo(UInt16 ammo)
        {
            WriteUInt16(crossbowNormalAmmoOffset, ammo);
        }

        private void WriteCrossbowExplosiveAmmo(UInt16 ammo)
        {
            WriteUInt16(crossbowExplosiveAmmoOffset, ammo);
        }

        private void WriteCrossbowPoisonAmmo(UInt16 ammo)
        {
            WriteUInt16(crossbowPoisonAmmoOffset, ammo);
        }

        private void WriteBinocularsPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(binocularsOffset, 0x1);
            }
            else
            {
                WriteByte(binocularsOffset, 0);
            }
        }

        private void WriteCrowbarPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(crowbarOffset, 0x1);
            }
            else
            {
                WriteByte(crowbarOffset, 0);
            }
        }

        private void WriteLaserSightPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(laserSightOffset, 0x1);
            }
            else
            {
                WriteByte(laserSightOffset, 0);
            }
        }

        private void WritePistolsPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(pistolsOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(pistolsOffset, 0x9);
            }
            else
            {
                WriteByte(pistolsOffset, 0);
            }
        }

        private void WriteShotgunPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(shotgunOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(shotgunOffset, 0x9);
            }
            else
            {
                WriteByte(shotgunOffset, 0);
            }
        }

        private void WriteUzisPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(uziOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(uziOffset, 0x9);
            }
            else
            {
                WriteByte(uziOffset, 0);
            }
        }

        private void WriteRevolverPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(revolverOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(revolverOffset, 0x9);
            }
            else
            {
                WriteByte(revolverOffset, 0);
            }
        }

        private void WriteCrossbowPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(crossbowOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(crossbowOffset, 0x9);
            }
            else
            {
                WriteByte(crossbowOffset, 0);
            }
        }

        private void WriteGrenadeGunPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(grenadeGunOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(grenadeGunOffset, 0x9);
            }
            else
            {
                WriteByte(grenadeGunOffset, 0);
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
            string lvlName = GetLvlName();

            if (lvlName.StartsWith("Angkor Wat")) return true;
            else if (lvlName.StartsWith("Race For The Iris")) return true;
            else if (lvlName.StartsWith("The Tomb Of Seth")) return true;
            else if (lvlName.StartsWith("Burial Chambers")) return true;
            else if (lvlName.StartsWith("Valley Of The Kings")) return true;
            else if (lvlName.StartsWith("KV5")) return true;
            else if (lvlName.StartsWith("Temple Of Karnak")) return true;
            else if (lvlName.StartsWith("The Great Hypostyle Hall")) return true;
            else if (lvlName.StartsWith("Sacred Lake")) return true;
            else if (lvlName.StartsWith("Tomb Of Semerkhet")) return true;
            else if (lvlName.StartsWith("Guardian Of Semerkhet")) return true;
            else if (lvlName.StartsWith("Desert Railroad")) return true;
            else if (lvlName.StartsWith("Alexandria")) return true;
            else if (lvlName.StartsWith("Coastal Ruins")) return true;
            else if (lvlName.StartsWith("Catacombs")) return true;
            else if (lvlName.StartsWith("Temple Of Poseidon")) return true;
            else if (lvlName.StartsWith("The Lost Library")) return true;
            else if (lvlName.StartsWith("Hall Of Demetrius")) return true;
            else if (lvlName.StartsWith("Pharos, Temple Of Isis")) return true;
            else if (lvlName.StartsWith("Cleopatra's Palaces")) return true;
            else if (lvlName.StartsWith("City Of The Dead")) return true;
            else if (lvlName.StartsWith("Chambers Of Tulun")) return true;
            else if (lvlName.StartsWith("Citadel Gate")) return true;
            else if (lvlName.StartsWith("Trenches")) return true;
            else if (lvlName.StartsWith("Street Bazaar")) return true;
            else if (lvlName.StartsWith("Citadel")) return true;
            else if (lvlName.StartsWith("The Sphinx Complex")) return true;
            else if (lvlName.StartsWith("Underneath The Sphinx")) return true;
            else if (lvlName.StartsWith("Menkaure's Pyramid")) return true;
            else if (lvlName.StartsWith("Inside Menkaure's Pyramid")) return true;
            else if (lvlName.StartsWith("The Mastabas")) return true;
            else if (lvlName.StartsWith("The Great Pyramid")) return true;
            else if (lvlName.StartsWith("Khufu's Queens Pyramids")) return true;
            else if (lvlName.StartsWith("Inside The Great Pyramid")) return true;
            else if (lvlName.StartsWith("Temple Of Horus")) return true;
            else if (lvlName.StartsWith("The Times Exclusive")) return true;

            return false;
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
