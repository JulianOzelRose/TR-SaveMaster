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
        private const int saveNumberOffset = 0x4B;
        private const int pistolsOffset = 0x169;
        private const int uziOffset = 0x16A;
        private const int shotgunOffset = 0x16B;
        private const int crossbowOffset = 0x16C;
        private const int grenadeGunOffset = 0x16D;
        private const int revolverOffset = 0x16E;
        private const int laserSightOffset = 0x16F;
        private const int binocularsOffset = 0x170;
        private const int crowbarOffset = 0x171;
        private const int smallMedipackOffset = 0x190;
        private const int largeMedipackOffset = 0x192;
        private const int flaresOffset = 0x194;
        private const int uziAmmoOffset = 0x198;
        private const int revolverAmmoOffset = 0x19A;
        private const int shotgunNormalAmmoOffset = 0x19C;
        private const int shotgunWideshotAmmoOffset = 0x19E;
        private const int grenadeGunNormalAmmoOffset = 0x1A0;
        private const int grenadeGunSuperAmmoOffset = 0x1A2;
        private const int grenadeGunFlashAmmoOffset = 0x1A4;
        private const int crossbowNormalAmmoOffset = 0x1A6;
        private const int crossbowPoisonAmmoOffset = 0x1A8;
        private const int crossbowExplosiveAmmoOffset = 0x1AA;
        private const int levelIndexOffset = 0x1E7;
        private const int secretsOffset = 0x1FB;

        // Constants
        private const int ITEM_PRESENT = 0x1;
        private const int WEAPON_PRESENT = 0x9;

        // Checksum
        private const int CHECKSUM_START_OFFSET = 0x57;
        private const int EOF_OFFSET = 0x3E51;

        // Strings
        private string savegamePath;

        // Health
        private const UInt16 MAX_HEALTH_VALUE = 1000;
        private const UInt16 MIN_HEALTH_VALUE = 1;
        private const byte BASE_HEALTH_FLAG_FULL = 0x80;      // Baseline flag for full health (not stored)
        private const byte BASE_HEALTH_FLAG_PARTIAL = 0xC0;     // Baseline flag for partial health (stored)
        private const byte HEALTH_FLAG_DELTA = 0x40;            // Difference between full and partial health flags

        private int MAX_HEALTH_OFFSET;
        private int MIN_HEALTH_OFFSET;

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
            byte levelIndex = GetLevelIndex();

            if (levelIndex == 1)        // Angkor Wat
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x34D;
                MAX_HEALTH_OFFSET = 0x34F;
            }
            else if (levelIndex == 2)   // Race For The Iris (also The Times Exclusive)
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x70E;
                MAX_HEALTH_OFFSET = 0x70E;
            }
            else if (levelIndex == 3)   // The Tomb Of Seth
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x3FF;
                MAX_HEALTH_OFFSET = 0x70E;
            }
            else if (levelIndex == 4)   // Burial Chambers
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x3B7;
                MAX_HEALTH_OFFSET = 0x5EB;
            }
            else if (levelIndex == 5)   // Valley Of The Kings
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x2F9;
                MAX_HEALTH_OFFSET = 0x33E;
            }
            else if (levelIndex == 6)   // KV5
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0x5D9;
                MAX_HEALTH_OFFSET = 0x627;
            }
            else if (levelIndex == 7)   // Temple Of Karnak
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (levelIndex == 8)   // The Great Hypostyle Hall
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (levelIndex == 9)   // Sacred Lake
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (levelIndex == 11)  // Tomb Of Semerkhet
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
                MIN_HEALTH_OFFSET = 0xF1B;
                MAX_HEALTH_OFFSET = 0xFE2;
            }
            else if (levelIndex == 12)  // Guardian Of Semerkhet
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (levelIndex == 13)  // Desert Railroad
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 14)  // Alexandria
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 15)  // Coastal Ruins
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 18)  // Catacombs
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 19)  // Temple Of Poseidon
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 20)  // The Lost Library
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 21)  // Hall Of Demetrius
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 16)  // Pharos, Temple Of Isis
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 17)  // Cleopatra's Palaces
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 22)  // City Of The Dead
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 24)  // Chambers Of Tulun
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 26)  // Citadel Gate
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 23)  // Trenches
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 25)  // Street Bazaar
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 27)  // Citadel
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 28)  // The Sphinx Complex
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 30)  // Underneath The Sphinx
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 31)  // Menkaure's Pyramid
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 32)  // Inside Menkaure's Pyramid
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 33)  // The Mastabas
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 34)  // The Great Pyramid
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 35)  // Khufu's Queens Pyramids
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 36)  // Inside The Great Pyramid
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (levelIndex == 38)  // Temple Of Horus
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
        }

        public void DisplayGameInfo(TextBox txtLvlName, NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudRevolverAmmo, NumericUpDown nudUziAmmo,
            NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo, NumericUpDown nudGrenadeGunNormalAmmo,
            NumericUpDown nudGrenadeGunSuperAmmo, NumericUpDown nudGrenadeGunFlashAmmo, NumericUpDown nudCrossbowNormalAmmo,
            NumericUpDown nudCrossbowPoisonAmmo, NumericUpDown nudCrossbowExplosiveAmmo, CheckBox chkBinoculars, CheckBox chkCrowbar,
            CheckBox chkLaserSight, CheckBox chkPistols, CheckBox chkRevolver, CheckBox chkUzi, CheckBox chkShotgun, CheckBox chkCrossbow,
            CheckBox chkGrenadeGun, TrackBar trbHealth, Label lblHealth, Label lblHealthError)
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
            chkUzi.Checked = GetUziValue() != 0;
            chkShotgun.Checked = GetShotgunValue() != 0;
            chkCrossbow.Checked = GetCrossbowValue() != 0;
            chkGrenadeGun.Checked = GetGrenadeGunValue() != 0;

            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                UInt16 health = GetHealthValue(healthOffset);
                double healthPercentage = ((double)health / MAX_HEALTH_VALUE) * 100;
                trbHealth.Value = health;
                trbHealth.Enabled = true;

                lblHealth.Text = healthPercentage.ToString("0.0") + "%";
                lblHealthError.Visible = false;
                lblHealth.Visible = true;
            }
            else
            {
                trbHealth.Enabled = false;
                trbHealth.Value = 1;
                lblHealthError.Visible = true;
                lblHealth.Visible = false;
            }
        }

        public void WriteChanges(NumericUpDown nudSaveNumber, NumericUpDown nudSecrets, NumericUpDown nudFlares,
            NumericUpDown nudSmallMedipacks, NumericUpDown nudLargeMedipacks, NumericUpDown nudRevolverAmmo,
            NumericUpDown nudUziAmmo, NumericUpDown nudGrenadeGunNormalAmmo, NumericUpDown nudGrenadeGunSuperAmmo,
            NumericUpDown nudGrenadeGunFlashAmmo, NumericUpDown nudCrossbowNormalAmmo, NumericUpDown nudCrossbowPoisonAmmo,
            NumericUpDown nudCrossbowExplosiveAmmo, NumericUpDown nudShotgunNormalAmmo, NumericUpDown nudShotgunWideshotAmmo,
            CheckBox chkPistols, CheckBox chkUzi, CheckBox chkShotgun, CheckBox chkCrossbow, CheckBox chkGrenadeGun,
            CheckBox chkRevolver, CheckBox chkBinoculars, CheckBox chkCrowbar, CheckBox chkLaserSight, TrackBar trbHealth)
        {
            byte prevPistolsValue = GetPistolsValue();
            byte prevUziValue = GetUziValue();
            byte prevShotgunValue = GetShotgunValue();
            byte prevCrossbowValue = GetCrossbowValue();
            byte prevGrenadeGunValue = GetGrenadeGunValue();
            byte prevRevolverValue = GetRevolverValue();

            WriteSaveNumber((UInt16)nudSaveNumber.Value);
            WriteNumSecrets((byte)nudSecrets.Value);
            WriteNumFlares((UInt16)nudFlares.Value);
            WriteNumSmallMedipacks((UInt16)nudSmallMedipacks.Value);
            WriteNumLargeMedipacks((UInt16)nudLargeMedipacks.Value);

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
            WriteUziPresent(chkUzi.Checked, prevUziValue);
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

            if (trbHealth.Enabled)
            {
                WriteHealthValue((UInt16)trbHealth.Value);
            }

            byte checksum = CalculateChecksum();
            WriteChecksum(checksum);
        }

        private void WriteHealthValue(UInt16 newHealth)
        {
            int healthOffset = GetHealthOffset();

            if (healthOffset != -1)
            {
                byte currentFlag = ReadByte(healthOffset - 0x11);
                // Determine current mode by checking the top two bits:
                // Full: 0x80–0x8F (bits: 10xxxxxx)
                // Partial: 0xC0–0xCF (bits: 11xxxxxx)
                bool currentlyFull = ((currentFlag & BASE_HEALTH_FLAG_PARTIAL) == BASE_HEALTH_FLAG_FULL);
                bool currentlyPartial = ((currentFlag & BASE_HEALTH_FLAG_PARTIAL) == BASE_HEALTH_FLAG_PARTIAL);

                // New mode is determined by whether newHealth is less than MAX_HEALTH_VALUE.
                bool newIsPartial = newHealth < MAX_HEALTH_VALUE;

                Console.WriteLine($"Packed Byte at 0x{healthOffset - 0x11:X}: {currentFlag:X2}");

                if (currentlyFull && newIsPartial)
                {
                    // Transition from full -> partial:
                    // Adjust flag by adding 0x40. For example, 0x80 becomes 0xC0, 0x82 becomes 0xC2, etc.
                    byte newFlag = (byte)(currentFlag + HEALTH_FLAG_DELTA);
                    WriteByte(healthOffset - 0x11, newFlag); // Enable health storage
                    WriteUInt16(healthOffset, newHealth);
                    ShiftBytesRight(healthOffset);
                }
                else if (currentlyPartial && !newIsPartial)
                {
                    // Transition from partial -> full:
                    // Adjust flag by subtracting 0x40. For example, 0xC6 becomes 0x86.
                    byte newFlag = (byte)(currentFlag - HEALTH_FLAG_DELTA);
                    WriteByte(healthOffset - 0x11, newFlag); // Disable health storage
                    WriteUInt16(healthOffset, 0x0);  // Zero out stored health
                    ShiftBytesLeft(healthOffset);
                }
                else
                {
                    // Same mode – update the stored health value (if partial)
                    WriteUInt16(healthOffset, newHealth);
                }
            }
        }

        private void ShiftBytesLeft(int healthOffset)
        {
            Console.WriteLine("Shifting bytes by -0x2...");
            string filePath = savegamePath;

            byte[] saveData = File.ReadAllBytes(filePath);

            for (int i = healthOffset + 2; i < saveData.Length - 2; i++)
            {
                saveData[i] = saveData[i + 2]; // Shift bytes left
            }

            Array.Resize(ref saveData, saveData.Length - 2);

            File.WriteAllBytes(filePath, saveData);
        }

        private void ShiftBytesRight(int healthOffset)
        {
            Console.WriteLine("Shifting bytes by +0x2...");
            string filePath = savegamePath;

            byte[] saveData = File.ReadAllBytes(filePath);

            Array.Resize(ref saveData, saveData.Length + 2);

            for (int i = saveData.Length - 3; i >= healthOffset + 2; i--)
            {
                saveData[i + 2] = saveData[i]; // Shift bytes right
            }

            File.WriteAllBytes(filePath, saveData);
        }

        private UInt16 GetHealthValue(int healthOffset)
        {
            UInt16 rawHealth = ReadUInt16(healthOffset);

            if (rawHealth != 0)
            {
                return rawHealth;
            }

            return MAX_HEALTH_VALUE;
        }

        private int GetHealthOffset()
        {
            for (int offset = MIN_HEALTH_OFFSET; offset <= MAX_HEALTH_OFFSET; offset++)
            {
                UInt16 value = ReadUInt16(offset);

                if (value >= MIN_HEALTH_VALUE && value < MAX_HEALTH_VALUE || value == 0)
                {
                    byte byteFlag1 = ReadByte(offset - 7);
                    byte byteFlag2 = ReadByte(offset - 6);
                    byte byteFlag3 = ReadByte(offset - 5);
                    byte byteFlag4 = ReadByte(offset - 4);

                    bool isKnownByteFlagPattern = IsKnownByteFlagPattern(byteFlag1, byteFlag2, byteFlag3, byteFlag4);

                    if (isKnownByteFlagPattern && value != 0)
                    {
                        Console.WriteLine($"Health offset: 0x{offset:X}, Packed byte offset: 0x{(offset - 0x11):X}");
                        return offset;
                    }
                    else if (isKnownByteFlagPattern && value == 0)
                    {
                        // Read only the first byte.
                        byte packedByteFlag = ReadByte(offset - 0x11);

                        // Full health is indicated by the top two bits being 10 (i.e. 0x80 to 0x8F)
                        if ((packedByteFlag & BASE_HEALTH_FLAG_PARTIAL) == BASE_HEALTH_FLAG_FULL)
                        {
                            Console.WriteLine($"Health offset: 0x{offset:X}, Packed byte offset: 0x{(offset - 0x11):X}");
                            return offset;
                        }
                    }
                }
            }

            return -1;
        }

        private bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3, byte byteFlag4)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x02 && byteFlag3 == 0x00 && byteFlag4 == 0x52) return true;
            if (byteFlag1 == 0x02 && byteFlag2 == 0x02 && byteFlag3 == 0x00 && byteFlag4 == 0x67) return true;
            if (byteFlag1 == 0x02 && byteFlag2 == 0x02 && byteFlag3 == 0x47 && byteFlag4 == 0x67) return true;
            if (byteFlag1 == 0x50 && byteFlag2 == 0x50 && byteFlag3 == 0x00 && byteFlag4 == 0x07) return true;
            if (byteFlag1 == 0x50 && byteFlag2 == 0x50 && byteFlag3 == 0x47 && byteFlag4 == 0x07) return true;
            if (byteFlag1 == 0x47 && byteFlag2 == 0x47 && byteFlag3 == 0x00 && byteFlag4 == 0xDE) return true;

            return false;
        }

        private string GetLvlName()
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
                WriteByte(binocularsOffset, ITEM_PRESENT);
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

        private void WritePistolsPresent(bool isPresent, byte previousValue)
        {
            if (isPresent)
            {
                WriteByte(pistolsOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(pistolsOffset, WEAPON_PRESENT);
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
                WriteByte(shotgunOffset, WEAPON_PRESENT);
            }
            else
            {
                WriteByte(shotgunOffset, 0);
            }
        }

        private void WriteUziPresent(bool isPresent, byte previousValue)
        {
            if (isPresent && previousValue != 0)
            {
                WriteByte(uziOffset, previousValue);
            }
            else if (isPresent && previousValue == 0)
            {
                WriteByte(uziOffset, WEAPON_PRESENT);
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
                WriteByte(revolverOffset, WEAPON_PRESENT);
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
                WriteByte(crossbowOffset, WEAPON_PRESENT);
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
                WriteByte(grenadeGunOffset, WEAPON_PRESENT);
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

            byte levelIndex = GetLevelIndex();
            return (levelIndex >= 1 && levelIndex <= 38);
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
