using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    public class TRCUtilities
    {
        // Offsets
        private const int saveNumberOffset = 0x04B;
        private const int pistolsOffset = 0x16F;
        private const int uziOffset = 0x170;
        private const int shotgunOffset = 0x171;
        private const int grapplingGunOffset = 0x172;
        private const int hkGunOffset = 0x173;
        private const int revolverOffset = 0x174;
        private const int deagleOffset = 0x174;
        private const int laserSightOffset = 0x175;
        private const int binocularsOrHeadsetOffset = 0x177;
        private const int crowbarOffset = 0x178;
        private const int smallMedipackOffset = 0x194;
        private const int largeMedipackOffset = 0x196;
        private const int flaresOffset = 0x198;
        private const int uziAmmoOffset = 0x19C;
        private const int revolverAmmoOffset = 0x19E;
        private const int deagleAmmoOffset = 0x19E;
        private const int shotgunNormalAmmoOffset = 0x1A0;
        private const int shotgunWideshotAmmoOffset = 0x1A2;
        private const int hkAmmoOffset = 0x1A4;
        private const int grapplingGunAmmoOffset = 0x1A6;
        private const int secretsOffset = 0x1C3;
        private const int levelIndexOffset = 0x1EC;

        // Health
        private const UInt16 MIN_HEALTH_VALUE = 0;
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private int MIN_HEALTH_OFFSET = 0;
        private int MAX_HEALTH_OFFSET = 1;

        // Constants
        private const byte ITEM_PRESENT = 0x1;
        private const byte WEAPON_PRESENT = 0x9;
        private const byte WEAPON_PRESENT_WITH_SIGHT = 0xD;

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

        public void SetLevelParams(CheckBox chkRevolver, CheckBox chkDeagle, NumericUpDown nudRevolverAmmo,
            NumericUpDown nudDeagleAmmo, CheckBox chkUzi, NumericUpDown nudUziAmmo, CheckBox chkShotgun,
            NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo, CheckBox chkGrapplingGun,
            NumericUpDown nudGrapplingGunAmmo, CheckBox chkHkGun, NumericUpDown nudHkAmmo, CheckBox chkCrowbar,
            CheckBox chkPistols, NumericUpDown nudFlares, CheckBox chkLaserSight, CheckBox chkBinocularsOrHeadset)
        {
            byte levelIndex = GetLevelIndex();

            if (levelIndex == 1)        // Streets of Rome
            {
                chkRevolver.Enabled = true;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = true;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = true;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = true;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x4F4;
                MAX_HEALTH_OFFSET = 0x4F8;
            }
            else if (levelIndex == 2)   // Trajan's Markets
            {
                chkRevolver.Enabled = true;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = true;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = true;
                chkShotgun.Enabled = true;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = true;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x542;
                MAX_HEALTH_OFFSET = 0x5D7;
            }
            else if (levelIndex == 3)   // The Colosseum 
            {
                chkRevolver.Enabled = true;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = true;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = true;
                nudUziAmmo.Enabled = true;
                chkShotgun.Enabled = true;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = true;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x4D2;
                MAX_HEALTH_OFFSET = 0x7FF;
            }
            else if (levelIndex == 4)   // The Base
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = true;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = true;
                chkUzi.Enabled = true;
                nudUziAmmo.Enabled = true;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = true;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x556;
                MAX_HEALTH_OFFSET = 0x707;
            }
            else if (levelIndex == 5)   // The Submarine
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = true;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x520;
                MAX_HEALTH_OFFSET = 0x5D2;
            }
            else if (levelIndex == 6)   // Deepsea Dive
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = true;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x644;
                MAX_HEALTH_OFFSET = 0x6DE;
            }
            else if (levelIndex == 7)   // Sinking Submarine
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = true;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = true;
                chkUzi.Enabled = true;
                nudUziAmmo.Enabled = true;
                chkShotgun.Enabled = true;
                nudShotgunNormalAmmo.Enabled = true;
                nudShotgunWideshotAmmo.Enabled = true;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = true;
                nudFlares.Enabled = true;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x5CC;
                MAX_HEALTH_OFFSET = 0x66B;
            }
            else if (levelIndex == 8)   // Gallows Tree
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = false;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x4F0;
                MAX_HEALTH_OFFSET = 0x52D;
            }
            else if (levelIndex == 9)   // Labyrinth
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = false;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x538;
                MAX_HEALTH_OFFSET = 0x61A;
            }
            else if (levelIndex == 10)  // Old Mill
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = false;
                nudHkAmmo.Enabled = false;
                chkCrowbar.Enabled = true;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = false;
                chkBinocularsOrHeadset.Text = "Binoculars";
                MIN_HEALTH_OFFSET = 0x512;
                MAX_HEALTH_OFFSET = 0x624;
            }
            else if (levelIndex == 11)  // The 13th Floor
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = true;
                nudHkAmmo.Enabled = true;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Headset";
                MIN_HEALTH_OFFSET = 0x52A;
                MAX_HEALTH_OFFSET = 0x53A;
            }
            else if (levelIndex == 12)  // Escape with the Iris
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = false;
                nudGrapplingGunAmmo.Enabled = false;
                chkHkGun.Enabled = true;
                nudHkAmmo.Enabled = true;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Headset";
                MIN_HEALTH_OFFSET = 0x6F6;
                MAX_HEALTH_OFFSET = 0xC47;
            }
            else if (levelIndex == 14)  // Red Alert!
            {
                chkRevolver.Enabled = false;
                chkDeagle.Enabled = false;
                nudRevolverAmmo.Enabled = false;
                nudDeagleAmmo.Enabled = false;
                chkUzi.Enabled = false;
                nudUziAmmo.Enabled = false;
                chkShotgun.Enabled = false;
                nudShotgunNormalAmmo.Enabled = false;
                nudShotgunWideshotAmmo.Enabled = false;
                chkGrapplingGun.Enabled = true;
                nudGrapplingGunAmmo.Enabled = true;
                chkHkGun.Enabled = true;
                nudHkAmmo.Enabled = true;
                chkCrowbar.Enabled = false;
                chkPistols.Enabled = false;
                nudFlares.Enabled = false;
                chkLaserSight.Enabled = false;
                chkBinocularsOrHeadset.Enabled = true;
                chkBinocularsOrHeadset.Text = "Headset";
                MIN_HEALTH_OFFSET = 0x52C;
                MAX_HEALTH_OFFSET = 0x5D6;
            }
        }

        public void DisplayGameInfo(TextBox txtLvlName, NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks,
            NumericUpDown nudFlares, NumericUpDown nudHkAmmo, NumericUpDown nudSecrets, NumericUpDown nudSaveNumber,
            NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo, NumericUpDown nudUziAmmo,
            NumericUpDown nudGrapplingGunAmmo, NumericUpDown nudRevolverAmmo, NumericUpDown nudDeagleAmmo, CheckBox chkPistols,
            CheckBox chkBinocularsOrHeadset, CheckBox chkLaserSight, CheckBox chkCrowbar, CheckBox chkRevolver, CheckBox chkDeagle,
            CheckBox chkShotgun, CheckBox chkUzi, CheckBox chkHkGun, CheckBox chkGrapplingGun, TrackBar trbHealth, Label lblHealth,
            Label lblHealthError)
        {
            txtLvlName.Text = GetLvlName();

            nudSmallMedipacks.Value = GetNumSmallMedipacks();
            nudLargeMedipacks.Value = GetNumLargeMedipacks();
            nudFlares.Value = GetNumFlares();
            nudHkAmmo.Value = GetHKAmmo();
            nudSecrets.Value = GetNumSecrets();
            nudSaveNumber.Value = GetSaveNumber();
            nudShotgunNormalAmmo.Value = GetShotgunNormalAmmo() / 6;
            nudShotgunWideshotAmmo.Value = GetShotgunWideshotAmmo() / 6;
            nudUziAmmo.Value = GetUziAmmo();
            nudGrapplingGunAmmo.Value = GetGrapplingGunAmmo();

            chkPistols.Checked = IsPistolsPresent();
            chkBinocularsOrHeadset.Checked = IsBinocularsOrHeadsetPresent();
            chkLaserSight.Checked = IsLaserSightPresent();
            chkCrowbar.Checked = IsCrowbarPresent();
            chkShotgun.Checked = IsShotgunPresent();
            chkUzi.Checked = IsUziPresent();
            chkHkGun.Checked = IsHKGunPresent();
            chkGrapplingGun.Checked = IsGrapplingGunPresent();

            if (chkRevolver.Enabled)
            {
                chkRevolver.Checked = IsRevolverPresent();
                nudRevolverAmmo.Value = GetRevolverAmmo();

                chkDeagle.Checked = false;
                nudDeagleAmmo.Value = 0;
            }
            else if (chkDeagle.Enabled)
            {
                chkDeagle.Checked = IsDeaglePresent();
                nudDeagleAmmo.Value = GetDeagleAmmo();

                chkRevolver.Checked = false;
                nudRevolverAmmo.Value = 0;
            }
            else
            {
                chkRevolver.Checked = false;
                nudRevolverAmmo.Value = 0;

                chkDeagle.Checked = false;
                nudDeagleAmmo.Value = 0;
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

        public void WriteChanges(NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudSmallMedipacks,
            NumericUpDown nudLargeMedipacks, NumericUpDown nudFlares, NumericUpDown nudRevolverAmmo, NumericUpDown nudDeagleAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudHkAmmo, NumericUpDown nudGrapplingGunAmmo, NumericUpDown nudShotgunNormalAmmo,
            NumericUpDown nudShotgunWideshotAmmo, CheckBox chkPistols, CheckBox chkUzi, CheckBox chkRevolver, CheckBox chkDeagle,
            CheckBox chkShotgun, CheckBox chkHkGun, CheckBox chkGrapplingGun, CheckBox chkBinocularsOrHeadset,
            CheckBox chkCrowbar, CheckBox chkLaserSight, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSecrets((byte)nudSecrets.Value);
            WriteNumSmallMedipacks((UInt16)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((UInt16)nudLargeMedipacks.Value);
            WriteNumFlares((UInt16)nudFlares.Value);
            WriteUziAmmo((UInt16)nudUziAmmo.Value);
            WriteHKAmmo((UInt16)nudHkAmmo.Value);
            WriteGrapplingGunAmmo((UInt16)nudGrapplingGunAmmo.Value);
            WriteShotgunNormalAmmo((UInt16)nudShotgunNormalAmmo.Value);
            WriteShotgunWideshotAmmo((UInt16)nudShotgunWideshotAmmo.Value);
            WriteCrowbarPresent(chkCrowbar.Checked);
            WritePistolsPresent(chkPistols.Checked);
            WriteShotgunPresent(chkShotgun.Checked);
            WriteUziPresent(chkUzi.Checked);
            WriteHKGunPresent(chkHkGun.Checked);
            WriteGrapplingGunPresent(chkGrapplingGun.Checked);

            if (chkRevolver.Enabled)
            {
                byte prevRevolverValue = GetRevolverValue();
                WriteRevolverPresent(chkRevolver.Checked, prevRevolverValue);
            }
            else if (chkDeagle.Enabled)
            {
                byte prevDeagleValue = GetDeagleValue();
                WriteDeaglePresent(chkDeagle.Checked, prevDeagleValue);
            }

            if (nudRevolverAmmo.Enabled)
            {
                WriteRevolverAmmo((UInt16)nudRevolverAmmo.Value);
            }
            else if (nudDeagleAmmo.Enabled)
            {
                WriteDeagleAmmo((UInt16)nudDeagleAmmo.Value);
            }

            if (chkBinocularsOrHeadset.Enabled)
            {
                WriteBinocularsOrHeadsetPresent(chkBinocularsOrHeadset.Checked);
            }

            if (chkCrowbar.Enabled)
            {
                WriteCrowbarPresent(chkCrowbar.Checked);
            }

            if (chkLaserSight.Enabled)
            {
                WriteLaserSightPresent(chkLaserSight.Checked);
            }

            if (trbHealth.Enabled)
            {
                WriteHealthValue(trbHealth.Value);
            }
        }

        private int GetHealthOffset()
        {
            for (int offset = MIN_HEALTH_OFFSET; offset <= MAX_HEALTH_OFFSET; offset++)
            {
                UInt16 value = ReadUInt16(offset);

                if (value > MIN_HEALTH_VALUE && value <= MAX_HEALTH_VALUE)
                {
                    byte byteFlag1 = ReadByte(offset - 7);
                    byte byteFlag2 = ReadByte(offset - 6);

                    if (IsKnownByteFlagPattern(byteFlag1, byteFlag2))
                    {
                        return offset;
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

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2)
        {
            if (byteFlag1 == 0x01 && byteFlag2 == 0x02) return true;    // Finishing running
            if (byteFlag1 == 0x02 && byteFlag2 == 0x02) return true;    // Standing
            if (byteFlag1 == 0x03 && byteFlag2 == 0x47) return true;    // Running jump
            if (byteFlag1 == 0x09 && byteFlag2 == 0x09) return true;    // Freefalling
            if (byteFlag1 == 0x13 && byteFlag2 == 0x13) return true;    // Climbing up a ledge
            if (byteFlag1 == 0x17 && byteFlag2 == 0x02) return true;    // Crouch-rolling
            if (byteFlag1 == 0x18 && byteFlag2 == 0x18) return true;    // Sliding down a ledge
            if (byteFlag1 == 0x19 && byteFlag2 == 0x19) return true;    // Doing a backflip
            if (byteFlag1 == 0x21 && byteFlag2 == 0x21) return true;    // In water but not underwater
            if (byteFlag1 == 0x47 && byteFlag2 == 0x47) return true;    // Crouching
            if (byteFlag1 == 0x47 && byteFlag2 == 0x57) return true;    // Squatting
            if (byteFlag1 == 0x49 && byteFlag2 == 0x49) return true;    // Sprinting
            if (byteFlag1 == 0x0D && byteFlag2 == 0x12) return true;    // Swimming
            if (byteFlag1 == 0x12 && byteFlag2 == 0x12) return true;    // Swimming (with suit)
            if (byteFlag1 == 0x0D && byteFlag2 == 0x0D) return true;    // Underwater
            if (byteFlag1 == 0x50 && byteFlag2 == 0x50) return true;    // Crouching forward
            if (byteFlag1 == 0x59 && byteFlag2 == 0x16) return true;    // Searching a container
            if (byteFlag1 == 0x59 && byteFlag2 == 0x15) return true;    // Searching a cabinet
            if (byteFlag1 == 0x59 && byteFlag2 == 0x10) return true;    // About to search a container
            if (byteFlag1 == 0x27 && byteFlag2 == 0x10) return true;    // Picking up an item
            if (byteFlag1 == 0x29 && byteFlag2 == 0x00) return true;    // Pulling a lever
            if (byteFlag1 == 0x28 && byteFlag2 == 0x10) return true;    // Pushing a button
            if (byteFlag1 == 0x23 && byteFlag2 == 0x11) return true;    // Diving
            if (byteFlag1 == 0x1C && byteFlag2 == 0x0F) return true;    // In air or jumping straight up
            if (byteFlag1 == 0x51 && byteFlag2 == 0x50) return true;    // Crawling
            if (byteFlag1 == 0x2B && byteFlag2 == 0x16) return true;    // Placing an item in a recepticle
            if (byteFlag1 == 0x62 && byteFlag2 == 0x15) return true;    // Activating a switch

            return false;
        }

        private UInt16 GetNumSmallMedipacks()
        {
            return ReadUInt16(smallMedipackOffset);
        }

        private UInt16 GetNumLargeMedipacks()
        {
            return ReadUInt16(largeMedipackOffset);
        }

        private UInt16 GetNumFlares()
        {
            return ReadUInt16(flaresOffset);
        }

        private UInt16 GetHKAmmo()
        {
            return ReadUInt16(hkAmmoOffset);
        }

        private UInt16 GetUziAmmo()
        {
            return ReadUInt16(uziAmmoOffset);
        }

        private UInt16 GetGrapplingGunAmmo()
        {
            return ReadUInt16(grapplingGunAmmoOffset);
        }

        private UInt16 GetRevolverAmmo()
        {
            return ReadUInt16(revolverAmmoOffset);
        }

        private UInt16 GetDeagleAmmo()
        {
            return ReadUInt16(deagleAmmoOffset);
        }

        private UInt16 GetShotgunNormalAmmo()
        {
            return ReadUInt16(shotgunNormalAmmoOffset);
        }

        private UInt16 GetShotgunWideshotAmmo()
        {
            return ReadUInt16(shotgunWideshotAmmoOffset);
        }

        private byte GetLevelIndex()
        {
            return ReadByte(levelIndexOffset);
        }

        private byte GetNumSecrets()
        {
            return ReadByte(secretsOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumberOffset);
        }

        private byte GetRevolverValue()
        {
            return ReadByte(revolverOffset);
        }

        private byte GetDeagleValue()
        {
            return ReadByte(deagleOffset);
        }

        private bool IsHKGunPresent()
        {
            return ReadByte(hkGunOffset) != 0;
        }

        private bool IsRevolverPresent()
        {
            return ReadByte(revolverOffset) != 0;
        }

        private bool IsDeaglePresent()
        {
            return ReadByte(deagleOffset) != 0;
        }

        private bool IsUziPresent()
        {
            return ReadByte(uziOffset) != 0;
        }

        private bool IsShotgunPresent()
        {
            return ReadByte(shotgunOffset) != 0;
        }

        private bool IsGrapplingGunPresent()
        {
            return ReadByte(grapplingGunOffset) != 0;
        }

        private bool IsPistolsPresent()
        {
            return ReadByte(pistolsOffset) != 0;
        }

        private bool IsCrowbarPresent()
        {
            return ReadByte(crowbarOffset) != 0;
        }

        private bool IsBinocularsOrHeadsetPresent()
        {
            return ReadByte(binocularsOrHeadsetOffset) != 0;
        }

        private bool IsLaserSightPresent()
        {
            return ReadByte(laserSightOffset) != 0;
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumberOffset, value);
        }

        private void WriteNumSecrets(byte value)
        {
            WriteByte(secretsOffset, value);
        }

        private void WriteNumSmallMedipacks(UInt16 value)
        {
            WriteUInt16(smallMedipackOffset, value);
        }

        private void WriteNumLargeMedipacks(UInt16 value)
        {
            WriteUInt16(largeMedipackOffset, value);
        }

        private void WriteNumFlares(UInt16 value)
        {
            WriteUInt16(flaresOffset, value);
        }

        private void WriteCrowbarPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(crowbarOffset, ITEM_PRESENT);
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
                WriteByte(laserSightOffset, ITEM_PRESENT);
            }
            else
            {
                WriteByte(laserSightOffset, 0);
            }
        }

        private void WriteBinocularsOrHeadsetPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(binocularsOrHeadsetOffset, ITEM_PRESENT);
            }
            else
            {
                WriteByte(binocularsOrHeadsetOffset, 0);
            }
        }

        private void WritePistolsPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(pistolsOffset, WEAPON_PRESENT);
            }
            else
            {
                WriteByte(pistolsOffset, 0);
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
                WriteByte(revolverOffset, WEAPON_PRESENT_WITH_SIGHT);
            }
            else
            {
                WriteByte(revolverOffset, 0);
            }
        }

        private void WriteDeaglePresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(deagleOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(deagleOffset, WEAPON_PRESENT_WITH_SIGHT);
            }
            else
            {
                WriteByte(deagleOffset, 0);
            }
        }

        private void WriteUziPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(uziOffset, WEAPON_PRESENT);
            }
            else
            {
                WriteByte(uziOffset, 0);
            }
        }

        private void WriteHKGunPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(hkGunOffset, WEAPON_PRESENT);
            }
            else
            {
                WriteByte(hkGunOffset, 0);
            }
        }

        private void WriteGrapplingGunPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(grapplingGunOffset, WEAPON_PRESENT_WITH_SIGHT);
            }
            else
            {
                WriteByte(grapplingGunOffset, 0);
            }
        }

        private void WriteShotgunPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(shotgunOffset, WEAPON_PRESENT);
            }
            else
            {
                WriteByte(shotgunOffset, 0);
            }
        }

        private void WriteRevolverAmmo(UInt16 ammo)
        {
            WriteUInt16(revolverAmmoOffset, ammo);
        }

        private void WriteDeagleAmmo(UInt16 ammo)
        {
            WriteUInt16(deagleAmmoOffset, ammo);
        }

        private void WriteUziAmmo(UInt16 ammo)
        {
            WriteUInt16(uziAmmoOffset, ammo);
        }

        private void WriteHKAmmo(UInt16 ammo)
        {
            WriteUInt16(hkAmmoOffset, ammo);
        }

        private void WriteGrapplingGunAmmo(UInt16 ammo)
        {
            WriteUInt16(grapplingGunAmmoOffset, ammo);
        }

        private void WriteShotgunNormalAmmo(UInt16 ammo)
        {
            WriteUInt16(shotgunNormalAmmoOffset, (UInt16)(ammo * 6));
        }

        private void WriteShotgunWideshotAmmo(UInt16 ammo)
        {
            WriteUInt16(shotgunWideshotAmmoOffset, (UInt16)(ammo * 6));
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
            return (levelIndex >= 1 && levelIndex != 13 && levelIndex <= 14);
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
