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
        private const int saveNumOffset = 0x04B;
        private const int pistolsOffset = 0x16F;
        private const int uziOffset = 0x170;
        private const int shotgunOffset = 0x171;
        private const int grapplingGunOffset = 0x172;
        private const int hkOffset = 0x173;
        private const int revolverOrDeagleOffset = 0x174;
        private const int laserSightOffset = 0x175;
        private const int binocularsOffset = 0x177;
        private const int crowbarOffset = 0x178;
        private const int smallMedipackOffset = 0x194;
        private const int lrgMedipackOffset = 0x196;
        private const int numFlaresOffset = 0x198;
        private const int uziAmmoOffset = 0x19C;
        private const int revolverAmmoOffset = 0x19E;
        private const int shotgunNormalAmmoOffset = 0x1A0;
        private const int shotgunWideshotAmmoOffset = 0x1A2;
        private const int hkAmmoOffset = 0x1A4;
        private const int grapplingGunAmmoOffset = 0x1A6;
        private const int numSecretsOffset = 0x1C3;

        // Health
        private const UInt16 MIN_HEALTH_VALUE = 0;
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private int MIN_HEALTH_OFFSET = 0;
        private int MAX_HEALTH_OFFSET = 1;

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

        public void SetLevelParams(CheckBox chkRevolverOrDeagle, NumericUpDown nudRevolverOrDeagleAmmo, CheckBox chkUzi,
            NumericUpDown nudUziAmmo, CheckBox chkShotgun, NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo,
            CheckBox chkGrapplingGun, NumericUpDown nudGrapplingGunAmmo, CheckBox chkHkGun, NumericUpDown nudHkAmmo, CheckBox chkCrowbar,
            CheckBox chkPistols, NumericUpDown nudFlares, CheckBox chkLaserSight, CheckBox chkBinocularsOrHeadset)
        {
            string lvlName = GetLvlName();

            if (lvlName.StartsWith("Streets of Rome"))
            {
                chkRevolverOrDeagle.Enabled = true;
                nudRevolverOrDeagleAmmo.Enabled = true;
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
                chkRevolverOrDeagle.Text = "Revolver";
                MIN_HEALTH_OFFSET = 0x4F4;
                MAX_HEALTH_OFFSET = 0x4F8;
            }
            else if (lvlName.StartsWith("Trajan`s markets"))
            {
                chkRevolverOrDeagle.Enabled = true;
                nudRevolverOrDeagleAmmo.Enabled = true;
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
                chkRevolverOrDeagle.Text = "Revolver";
                MIN_HEALTH_OFFSET = 0x542;
                MAX_HEALTH_OFFSET = 0x5D7;
            }
            else if (lvlName.StartsWith("The Colosseum"))
            {
                chkRevolverOrDeagle.Enabled = true;
                nudRevolverOrDeagleAmmo.Enabled = true;
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
                chkRevolverOrDeagle.Text = "Revolver";
                MIN_HEALTH_OFFSET = 0x4D2;
                MAX_HEALTH_OFFSET = 0x7FF;
            }
            else if (lvlName.StartsWith("The base"))
            {
                chkRevolverOrDeagle.Enabled = true;
                nudRevolverOrDeagleAmmo.Enabled = true;
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
                chkRevolverOrDeagle.Text = "Desert Eagle";
                MIN_HEALTH_OFFSET = 0x556;
                MAX_HEALTH_OFFSET = 0x707;
            }
            else if (lvlName.StartsWith("The submarine"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Deepsea dive"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Sinking submarine"))
            {
                chkRevolverOrDeagle.Enabled = true;
                nudRevolverOrDeagleAmmo.Enabled = true;
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
            else if (lvlName.StartsWith("Gallows tree"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Labyrinth"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Old mill"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("The 13th floor"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Escape with the iris"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            else if (lvlName.StartsWith("Red alert!"))
            {
                chkRevolverOrDeagle.Enabled = false;
                nudRevolverOrDeagleAmmo.Enabled = false;
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
            NumericUpDown nudGrapplingGunAmmo, NumericUpDown nudRevolverOrDeagleAmmo, CheckBox chkPistols,
            CheckBox chkBinocularsOrHeadset, CheckBox chkLaserSight, CheckBox chkCrowbar, CheckBox chkRevolverOrDeagle,
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
            nudRevolverOrDeagleAmmo.Value = GetRevolverOrDeagleAmmo();

            chkPistols.Checked = IsPistolsPresent();
            chkBinocularsOrHeadset.Checked = IsBinocularsOrHeadsetPresent();
            chkLaserSight.Checked = IsLaserSightPresent();
            chkCrowbar.Checked = IsCrowbarPresent();
            chkRevolverOrDeagle.Checked = IsRevolverPresent();
            chkShotgun.Checked = IsShotgunPresent();
            chkUzi.Checked = IsUziPresent();
            chkHkGun.Checked = IsHKPresent();
            chkGrapplingGun.Checked = IsGrapplingGunPresent();

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

        public void WriteChanges(NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudSmallMedipacks,
            NumericUpDown nudLargeMedipacks, NumericUpDown nudFlares, NumericUpDown nudRevolverOrDeagleAmmo, NumericUpDown nudUziAmmo,
            NumericUpDown nudHkAmmo, NumericUpDown nudGrapplingGunAmmo, NumericUpDown nudShotgunNormalAmmo,
            NumericUpDown nudShotgunWideshotAmmo, CheckBox chkPistols, CheckBox chkUzi, CheckBox chkRevolverOrDeagle,
            CheckBox chkShotgun, CheckBox chkHkGun, CheckBox chkGrapplingGun, CheckBox chkBinocularsOrHeadset,
            CheckBox chkCrowbar, CheckBox chkLaserSight, TrackBar trbHealth)
        {
            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteSecrets((byte)nudSecrets.Value);
            WriteSmallMedipacks((UInt16)nudSmallMedipacks.Value);
            WriteLargeMedipacks((UInt16)nudLargeMedipacks.Value);
            WriteFlares((UInt16)nudFlares.Value);
            WriteRevolverOrDeagleAmmo((UInt16)nudRevolverOrDeagleAmmo.Value);
            WriteUziAmmo((UInt16)nudUziAmmo.Value);
            WriteHKAmmo((UInt16)nudHkAmmo.Value);
            WriteGrapplingGunAmmo((UInt16)nudGrapplingGunAmmo.Value);
            WriteShotgunNormalAmmo((UInt16)nudShotgunNormalAmmo.Value);
            WriteShotgunWideshotAmmo((UInt16)nudShotgunWideshotAmmo.Value);

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

            byte prevRevolverValue = GetRevolverValue();

            WriteCrowbarPresent(chkCrowbar.Checked);
            WritePistolsPresent(chkPistols.Checked);
            WriteRevolverOrDeaglePresent(chkRevolverOrDeagle.Checked, prevRevolverValue);
            WriteShotgunPresent(chkShotgun.Checked);
            WriteUziPresent(chkUzi.Checked);
            WriteHKPresent(chkHkGun.Checked);
            WriteGrapplingGunPresent(chkGrapplingGun.Checked);

            if (GetHealthOffset() != -1)
            {
                WriteHealthValue(trbHealth.Value);
            }
        }

        private int GetHealthOffset()
        {
            for (int offset = MIN_HEALTH_OFFSET; offset <= MAX_HEALTH_OFFSET; offset++)
            {
                byte byteFlag1 = ReadByte(offset - 7);
                byte byteFlag2 = ReadByte(offset - 6);

                if (IsKnownByteFlagPattern(byteFlag1, byteFlag2))
                {
                    UInt16 value = ReadUInt16(offset);

                    if (value > MIN_HEALTH_VALUE && value <= MAX_HEALTH_VALUE)
                    {
                        return offset;
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
            return ReadUInt16(lrgMedipackOffset);
        }

        private UInt16 GetNumFlares()
        {
            return ReadUInt16(numFlaresOffset);
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

        private UInt16 GetRevolverOrDeagleAmmo()
        {
            return ReadUInt16(revolverAmmoOffset);
        }

        private UInt16 GetShotgunNormalAmmo()
        {
            return ReadUInt16(shotgunNormalAmmoOffset);
        }

        private UInt16 GetShotgunWideshotAmmo()
        {
            return ReadUInt16(shotgunWideshotAmmoOffset);
        }

        private byte GetNumSecrets()
        {
            return ReadByte(numSecretsOffset);
        }

        private UInt16 GetSaveNumber()
        {
            return ReadUInt16(saveNumOffset);
        }

        private byte GetRevolverValue()
        {
            return ReadByte(revolverOrDeagleOffset);
        }

        private bool IsHKPresent()
        {
            return ReadByte(hkOffset) != 0;
        }

        private bool IsRevolverPresent()
        {
            return ReadByte(revolverOrDeagleOffset) != 0;
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
            return ReadByte(binocularsOffset) != 0;
        }

        private bool IsLaserSightPresent()
        {
            return ReadByte(laserSightOffset) != 0;
        }

        private void WriteSaveNumber(UInt16 value)
        {
            WriteUInt16(saveNumOffset, value);
        }

        private void WriteSecrets(byte value)
        {
            WriteByte(numSecretsOffset, value);
        }

        private void WriteSmallMedipacks(UInt16 value)
        {
            WriteUInt16(smallMedipackOffset, value);
        }

        private void WriteLargeMedipacks(UInt16 value)
        {
            WriteUInt16(lrgMedipackOffset, value);
        }

        private void WriteFlares(UInt16 value)
        {
            WriteUInt16(numFlaresOffset, value);
        }

        private void WriteCrowbarPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(crowbarOffset, 0x9);
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

        private void WriteBinocularsOrHeadsetPresent(bool isPresent)
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

        private void WritePistolsPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(pistolsOffset, 0x9);
            }
            else
            {
                WriteByte(pistolsOffset, 0);
            }
        }

        private void WriteRevolverOrDeaglePresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(revolverOrDeagleOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(revolverOrDeagleOffset, 0x9);
            }
            else
            {
                WriteByte(revolverOrDeagleOffset, 0);
            }
        }

        private void WriteUziPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(uziOffset, 0x9);
            }
            else
            {
                WriteByte(uziOffset, 0);
            }
        }

        private void WriteHKPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(hkOffset, 0x9);
            }
            else
            {
                WriteByte(hkOffset, 0);
            }
        }

        private void WriteGrapplingGunPresent(bool isPresent)
        {
            if (isPresent)
            {
                WriteByte(grapplingGunOffset, 0xD);
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
                WriteByte(shotgunOffset, 0x9);
            }
            else
            {
                WriteByte(shotgunOffset, 0);
            }
        }

        private void WriteRevolverOrDeagleAmmo(UInt16 ammo)
        {
            WriteUInt16(revolverAmmoOffset, ammo);
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

            UInt16 newHealth = (UInt16)(newHealthPercentage / 100.0 * MAX_HEALTH_VALUE);
            WriteUInt16(healthOffset, newHealth);
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

            if (lvlName.StartsWith("Streets of Rome")) return true;
            else if (lvlName.StartsWith("Trajan`s markets")) return true;
            else if (lvlName.StartsWith("The Colosseum")) return true;
            else if (lvlName.StartsWith("The base")) return true;
            else if (lvlName.StartsWith("The submarine")) return true;
            else if (lvlName.StartsWith("Deepsea dive")) return true;
            else if (lvlName.StartsWith("Sinking submarine")) return true;
            else if (lvlName.StartsWith("Gallows tree")) return true;
            else if (lvlName.StartsWith("Labyrinth")) return true;
            else if (lvlName.StartsWith("Old mill")) return true;
            else if (lvlName.StartsWith("The 13th floor")) return true;
            else if (lvlName.StartsWith("Escape with the iris")) return true;
            else if (lvlName.StartsWith("Red alert!")) return true;

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
