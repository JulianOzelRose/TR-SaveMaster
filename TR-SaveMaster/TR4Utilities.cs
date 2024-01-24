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

        private string GetCleanLvlName()
        {
            string lvlName = GetLvlName();
            lvlName = lvlName.Trim();

            if (lvlName.StartsWith("Angkor Wat")) return "Angkor Wat";
            else if (lvlName.StartsWith("Race For The Iris") || lvlName.StartsWith("Die Jagd nach der Iris")) return "Race For The Iris";
            else if (lvlName.StartsWith("The Tomb Of Seth") || lvlName.StartsWith("Das Grabmal des Seth")) return "The Tomb Of Seth";
            else if (lvlName.StartsWith("Burial Chambers") || lvlName.StartsWith("Die Grabkammer")) return "Burial Chambers";
            else if (lvlName.StartsWith("Valley Of The Kings") || lvlName.StartsWith("Im Tal der K~onige")) return "Valley Of The Kings";
            else if (lvlName.StartsWith("KV5") || lvlName.StartsWith("Das 5. Grabmal im Tal der K~onige")) return "KV5";
            else if (lvlName.StartsWith("Temple Of Karnak") || lvlName.StartsWith("Der Tempel von Karnak")) return "Temple Of Karnak";
            else if (lvlName.StartsWith("The Great Hypostyle Hall") || lvlName.StartsWith("Die grosse Hypostyle Halle")) return "The Great Hypostyle Hall";
            else if (lvlName.StartsWith("Sacred Lake") || lvlName.StartsWith("Der heilige See")) return "Sacred Lake";
            else if (lvlName.StartsWith("Tomb Of Semerkhet") || lvlName.StartsWith("Das Grabmal des Semerkhet")) return "Tomb Of Semerkhet";
            else if (lvlName.StartsWith("Guardian Of Semerkhet") || lvlName.StartsWith("Der W~achter des Semerkhet")) return "Guardian Of Semerkhet";
            else if (lvlName.StartsWith("Desert Railroad") || lvlName.StartsWith("Ein Zug in der W~uste")) return "Desert Railroad";
            else if (lvlName.StartsWith("Alexandria")) return "Alexandria";
            else if (lvlName.StartsWith("Coastal Ruins") || lvlName.StartsWith("Die K~ustenruinen")) return "Coastal Ruins";
            else if (lvlName.StartsWith("Catacombs") || lvlName.StartsWith("Die Katakomben")) return "Catacombs";
            else if (lvlName.StartsWith("Temple Of Poseidon") || lvlName.StartsWith("Der Poseidontempel")) return "Temple Of Poseidon";
            else if (lvlName.StartsWith("The Lost Library") || lvlName.StartsWith("Die verschwundene Bibliothek")) return "The Lost Library";
            else if (lvlName.StartsWith("Hall Of Demetrius") || lvlName.StartsWith("Die Hallen des Demetrius")) return "Hall Of Demetrius";
            else if (lvlName.StartsWith("Pharos, Temple Of Isis") || lvlName.StartsWith("Der Isistempel von Pharos")) return "Pharos, Temple Of Isis";
            else if (lvlName.StartsWith("Cleopatra's Palaces") || lvlName.StartsWith("Der Palast der Kleopatra")) return "Cleopatra's Palaces";
            else if (lvlName.StartsWith("City Of The Dead") || lvlName.StartsWith("Die Stadt der Toten")) return "City Of The Dead";
            else if (lvlName.StartsWith("Chambers Of Tulun") || lvlName.StartsWith("Die Tulun-Moschee")) return "Chambers Of Tulun";
            else if (lvlName.StartsWith("Citadel Gate") || lvlName.StartsWith("Das Tor zur Zitadelle")) return "Citadel Gate";
            else if (lvlName.StartsWith("Trenches") || lvlName.StartsWith("Die Gr~aben")) return "Trenches";
            else if (lvlName.StartsWith("Street Bazaar") || lvlName.StartsWith("Der Stra=enbasar")) return "Street Bazaar";
            else if (lvlName.StartsWith("Citadel") || lvlName.StartsWith("Die Zitadelle")) return "Citadel";
            else if (lvlName.StartsWith("The Sphinx Complex") || lvlName.StartsWith("Der Sphinx Komplex")) return "The Sphinx Complex";
            else if (lvlName.StartsWith("Underneath The Sphinx") || lvlName.StartsWith("Der Tempel im Tal")) return "Underneath The Sphinx";
            else if (lvlName.StartsWith("Menkaure's Pyramid") || lvlName.StartsWith("Die Pyramide des Menkaure")) return "Menkaure's Pyramid";
            else if (lvlName.StartsWith("Inside Menkaure's Pyramid") || lvlName.StartsWith("In der Pyramide des Menkaure")) return "Inside Menkaure's Pyramid";
            else if (lvlName.StartsWith("The Mastabas") || lvlName.StartsWith("Die Mastabas")) return "The Mastabas";
            else if (lvlName.StartsWith("The Great Pyramid") || lvlName.StartsWith("Die gro=e Pyramide")) return "The Great Pyramid";
            else if (lvlName.StartsWith("Khufu's Queens Pyramids") || lvlName.StartsWith("Die K~onigspyramiden von Khufu")) return "Khufu's Queens Pyramids";
            else if (lvlName.StartsWith("Inside The Great Pyramid") || lvlName.StartsWith("In der gro=en Pyramide")) return "Inside The Great Pyramid";
            else if (lvlName.StartsWith("Temple Of Horus") || lvlName.StartsWith("Der Tempel des Horus")) return "Temple Of Horus";
            else if (lvlName.StartsWith("The Times Exclusive")) return "The Times Exclusive";

            return null;
        }

        public void SetLevelParams(CheckBox chkBinoculars, CheckBox chkLaserSight, CheckBox chkCrowbar)
        {
            string lvlName = GetCleanLvlName();

            if (lvlName == "Angkor Wat")
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Race For The Iris")
            {
                chkBinoculars.Enabled = false;
                chkLaserSight.Enabled = false;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "The Tomb Of Seth")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Burial Chambers")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Valley Of The Kings")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "KV5")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Temple Of Karnak")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "The Great Hypostyle Hall")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Sacred Lake")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Tomb Of Semerkhet")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Guardian Of Semerkhet")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = false;
            }
            else if (lvlName == "Desert Railroad")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Alexandria")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Coastal Ruins")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Catacombs")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Temple Of Poseidon")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "The Lost Library")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Hall Of Demetrius")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Pharos, Temple Of Isis")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Cleopatra's Palaces")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "City Of The Dead")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Chambers Of Tulun")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Citadel Gate")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Trenches")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Street Bazaar")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Citadel")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "The Sphinx Complex")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Underneath The Sphinx")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Menkaure's Pyramid")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Inside Menkaure's Pyramid")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "The Mastabas")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "The Great Pyramid")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Khufu's Queens Pyramids")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Inside The Great Pyramid")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "Temple Of Horus")
            {
                chkBinoculars.Enabled = true;
                chkLaserSight.Enabled = true;
                chkCrowbar.Enabled = true;
            }
            else if (lvlName == "The Times Exclusive")
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
            txtLvlName.Text = GetCleanLvlName();

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
