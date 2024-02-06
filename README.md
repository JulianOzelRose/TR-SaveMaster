# Tomb Raider - Savegame Editor
This is a fully-featured savegame editor for the classic Tomb Raider series. It works with Tomb Raider 1-5, including the 3 expansion packs and bonus levels.
This editor is compatible with the original CD savegames as well as the Steam, ATI, GOG, and multi-patched savegames. For installation and usage instructions,
please scroll down to the next section of this README. Additionally, technical details on reverse engineering the Tomb Raider are included, as well as
a complete list of offsets.

![TR-SaveMaster-UI](https://github.com/JulianOzelRose/TR-SaveMaster/assets/95890436/d95b20d0-ea26-431f-ab39-74c9166d791b)

## Table of contents
- [**Installation and use**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#installation-and-use)
   - [Common game directories](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#common-game-directories)
   - [Hidden game directories](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#hidden-game-directories)
   - [Dealing with protected directories](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#dealing-with-protected-directories)
- [**Reverse engineering the Tomb Raider series**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#reverse-engineering-the-tomb-raider-series)
   - [Using bitwise to extract weapons information](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-bitwise-to-extract-weapons-information)
   - [Using heuristics to determine the health offset](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-heuristics-to-determine-the-health-offset)
   - [**Tomb Raider I**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#tomb-raider-i)
      - [Weapons](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#weapons)
      - [Ammunition](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#ammunition)
   - [**Tomb Raider II**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#tomb-raider-ii)
      - [Weapons](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#weapons-1)
      - [Ammunition](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#ammunition-1)
      - [Health](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#health)
   - [**Tomb Raider III**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#tomb-raider-iii)
      - [Ammunition](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#ammunition-2)
      - [Health](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#health-1)
   - [**Tomb Raider: The Last Revelation**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#tomb-raider-the-last-revelation)
      - [Checksum algorithm](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#checksum-algorithm)
   - [**Tomb Raider: Chronicles**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#tomb-raider-chronicles)
      - [Health offsets](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#health-offsets)
      - [Weapons](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#weapons)
- [**Offsets**](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md)
   - [Tomb Raider I](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-i)
   - [Tomb Raider I: Unfinished Business](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-i-unfinished-business)
   - [Tomb Raider II](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-ii)
   - [Tomb Raider II: The Golden Mask](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-ii-the-golden-mask)
   - [Tomb Raider III](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-iii)
   - [Tomb Raider III: The Lost Artifact](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-iii-the-lost-artifact)
   - [Tomb Raider: The Last Revelation](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-the-last-revelation)
   - [Tomb Raider: Chronicles](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md#tomb-raider-chronicles)


## Installation and use
To use this savegame editor, simply navigate to the [Release](https://github.com/JulianOzelRose/TR-SaveMaster/tree/master/TR-SaveMaster/bin/x64/Release) folder, then
download `TR-SaveMaster.exe`, then open it. There is no need to install anything, and it can be run from anywhere on your computer. To toggle between the different
Tomb Raider games, click the appopriate tab on the tab control on the top. To begin savegame editing, you must first set your game directory. To do this, click "Browse",
the navigate to your game directory on your computer. The game directory depends on whether you have a Steam installation, a CD installation, or a GOG installation.
For GOG and CD installations, the actual directory the savegames are stored in varies based on your Windows setup. It seems that for most modern Windows installations,
the savegames are stored in the hidden directory. Both variations are listed below. To find the hidden directories, you will need to enable the "Show hidden files, folders, and drives"
option in Windows Explorer.

### Common game directories
- **Tomb Raider I**
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider I\TOMBRAID\`
  - GOG: `C:\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 1\TOMBRAID`
  - Steam: `C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)\TOMBRAID\`
- **Tomb Raider I: Unfinished Business**
  - Steam (Patched): `C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)\`
- **Tomb Raider II**
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider II\`
  - GOG: `C:\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 2\`
  - Steam: `C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (II)\`
- **Tomb Raider II: The Golden Mask**
  - CD: `C:\Program Files (x86)\Tomb Raider II Gold (Full Net)\`
- **Tomb Raider III** 
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider III\`
  - GOG: `C:\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 3\`
  - Steam: `C:\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\`
- **Tomb Raider III: The Lost Artifact**
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact\`
- **Tomb Raider: The Last Revelation**
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider - The Last Revelation\`
  - Steam: `C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (IV) The Last Revelation\`
- **Tomb Raider: Chronicles**
  - CD: `C:\Program Files (x86)\Core Design\Tomb Raider Chronicles\`
  - Steam: `C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (V) Chronicles\`

### Hidden game directories
- **Tomb Raider I**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider I\TOMBRAID\`
   - GOG: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 1\TOMBRAID`
- **Tomb Raider II**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider II\`
   - GOG: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 2\`
- **Tomb Raider II: The Golden Mask**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Tomb Raider II Gold (Full Net)\`
- **Tomb Raider III**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider III\`
   - GOG: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 3\`
- **Tomb Raider III: The Lost Artifact**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact\`
- **Tomb Raider: The Last Revelation**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Last Revelation\`
- **Tomb Raider: Chronicles**
   - CD: `C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider Chronicles\`

Once your game directory is selected, the editor will populate with the savegames found. To toggle between them, simply click the dropdown box labled "Savegame". You
can then change ammunition, weapons, health, and items. Click "Save" when you are done, and your changes will be applied. This editor automatically creates backups
of savegame files, which is enabled by default. You can toggle auto backups off, but it is highly recommended that you leave it enabled. To find this option, click
"File" and then look for "Create backups". Under the "View" menu, you can also change the UI theme and hide the status bar, if you desire a simpler interface.
This savegame editor will remember your game directories, so there is no need to re-enter them each time you launch it.

For Tomb Raider: Chronicles and The Last Revelation, ammunition will be represented in the editor exactly as displayed in the game. However, for Tomb Raider 1-3 and their
expansions, the ammunition for weapons not currently equipped may appear differently than in-game. Specifically, Harpoons are grouped in bundles of 2, Desert Eagle clips
equate to 5 bullets, MP5 clips equate to 30 bullets, Uzi clips equate to 20 bullets, and a single box of grenades is equivalent to 2 rounds of Grenade Launcher ammunition.

### Dealing with protected directories
If you did a CD or GOG install with one of the original games or expansions on Windows 7 or later, the savegame files are likely located in a hidden and protected directory.
This savegame editor should be able to override the read-only protection. However, if you get an "Access is denied" error when attempting to patch a savegame, you may have
to modify the folder permissions manually to allow modification. To do this, first right-click on the Start menu, then click "Windows PowerShell (Admin)". Then, use the following commands:

```
takeown /f "C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact" /r /d y

icacls "C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact" /grant:r %USERNAME%:F /t
```

Of course, be sure to replace the directory with the one that you are encountering permission issues with, and be sure to replace "USERNAME" with your actual username.
Run the `takeown` command first, then run the `icals` command second. You should then be able to modify files within that directory and begin savegame editing.


# Reverse engineering the Tomb Raider series
This section details the technical aspects of reverse engineering the savegame files for the classic Tomb Raider series. You can find the offset tables for each game
[here](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md). In general, there are many similarities between the games, as they are all built on the same engine.
Tomb Raider 1-3 use a very similar engine, and so reversing those games involves a similar process.

Tomb Raider 4 and 5 use a markedly different engine than 1-3, but involve a simpler process nonetheless.
This is because most of the offsets in Tomb Raider 1-3 are dynamic, while all offsets in Tomb Raider 4 and 5 are static, with the exception of the health offset.
Interestingly, Tomb Raider 4 uses a file checksum as an anti-reverse engineering technique, while Tomb Raider 5 does not.

## Using bitwise to extract weapons information
In Tomb Raider 1-3, all weapons information is stored on a single offset, which is called `weaponsConfigNum` in the editor's code. The weapons configuration variable has a base number of 1,
which indicates no weapons present in inventory. Each weapon adds a unique number to the variable. To determine which weapons are present in inventory, you can use
bitwise operators. Here is an example of using bitwise to extract weapons information from Tomb Raider II savegames:

```
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
```

When writing a new weapons configuration, perform the same operation, but in reverse. Start with the base number of 1,
then increment the new weapons config number based on which weapons are checkmarked in the editor's UI. Here is an example
of how this is implemented for Tomb Raider II:

```
byte newWeaponsConfigNum = 1;

if (chkPistols.Checked) newWeaponsConfigNum += 2;
if (chkAutomaticPistols.Checked) newWeaponsConfigNum += 4;
if (chkUzis.Checked) newWeaponsConfigNum += 8;
if (chkShotgun.Checked) newWeaponsConfigNum += 16;
if (chkM16.Checked) newWeaponsConfigNum += 32;
if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 64;
if (chkHarpoonGun.Checked) newWeaponsConfigNum += 128;

WriteWeaponsConfigNum(newWeaponsConfigNum);
```

## Using heuristics to determine the health offset
Health is stored in a similar fashion for each game. Health is represented as a UInt16 number ranging from 0 (dead) to
1000 (full health). In Tomb Raider 1-3, there are several offsets that store health interchangably. In Tomb Raider 5
there is a health offset range. In other words, health is dynamically allocated. Since writing to the incorrect health
offset may cause the game to crash, it is important to determine the health offset with an accurate method. Since health
is stored right next to character movement data, is is possible to find the correct health offset by checking the surrounding
data for character movement byte flags. Here is an example of how to implement this algorithm for Tomb Raider 5 savegames:

```
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
```

## Calculating the secondary ammo index
The secondary ammo offsets in Tomb Raider II and Tomb Raider III are dynamically stored, so they must be calculated dynamically.
While the primary offset remains constant throughout a level, the secondary offset shifts based on the number of active entities in the game.
If there are no other entities present, the secondary ammo index is 0. The there are 2 entities, the secondary ammo index is 2, etc. 

There is a 4-byte array of `{0xFF, 0xFF, 0xFF, 0xFF}` that is stored just before the null padding of the savegame file.
Since this array shifts consistently along with the secondary ammo offsets, it can be used as a marker to determine the secondary ammo index.
Each index corresponds with two possible offset ranges of this array. The second one is 0xA bytes from the first.
For example, if the offset range that represents index 0 begins at 0xFE0, then the offset range that begins at 0xFEC
also corresponds to index 0.

So, when pulling the ammo index based on this dictionary, you must account for this. Here is an example of how to pull the
secondary ammo index for Tomb Raider II. Note that the 0x6 iterator value is specific to it, and Tomb Raider III uses a 0x12 iterator:

```
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
```

## Tomb Raider I
Reverse engineering Tomb Raider I savegames is relatively straightforward. The biggest convenience is that most of the file offsets are static. Meaning, they are the same
across levels. The only exception is the secondary ammunition offsets, which are still static, but different on each level.

### Weapons
Weapons information can be extracted using
bitwise methods, as outlined in the [section above](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-bitwise-to-extract-weapons-information).
Here are the weapon byte flags specific to Tomb Raider I:

###                                         ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Magnums                 | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |

### Ammunition
Ammunition is stored on up to two offsets; a primary offset, and a secondary offset. If a weapon is not equipped,
it is only stored on the primary offset. If the weapon is equipped, the ammo is stored on both offsets. So when
removing a weapon from inventory, you should also zero the secondary ammo offset to free its address space:

```
private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
{
    WriteUInt16(shotgunAmmoOffset, ammo);

    if (isPresent)
    {
        WriteUInt16(shotgunAmmoOffset2, ammo);
    }
    else
    {
        WriteUInt16(shotgunAmmoOffset2, 0);
    }
}
```

## Tomb Raider II
Reverse engineering Tomb Raider II savegames is slightly different compared to Tomb Raider I. The file offsets
differ on each level, and are dynamically allocated. You can calculate the dynamic offsets based on the level
index:

```
byte levelIndex = GetLevelIndex();

automaticPistolsAmmoOffset = 0x51 + (levelIndex * 0x2C);
uziAmmoOffset = 0x53 + (levelIndex * 0x2C);
shotgunAmmoOffset = 0x55 + (levelIndex * 0x2C);
m16AmmoOffset = 0x57 + (levelIndex * 0x2C);
grenadeLauncherAmmoOffset = 0x59 + (levelIndex * 0x2C);
harpoonGunAmmoOffset = 0x5B + (levelIndex * 0x2C);
smallMedipackOffset = 0x5D + (levelIndex * 0x2C);
largeMedipackOffset = 0x5E + (levelIndex * 0x2C);
flaresOffset = 0x60 + (levelIndex * 0x2C);
weaponsConfigNumOffset = 0x63 + (levelIndex * 0x2C);
```

### Weapons
Similar to Tomb Raider I, weapons information is also stored on a single offset in Tomb Raider II.
You can use the same methodology to extract weapons information outlined in the
[above section](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-bitwise-to-extract-weapons-information).
Here are the weapon byte flags specific to Tomb Raider II:

###                                         ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Automatic Pistols       | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |
| M16                     | 32                |
| Grenade Launcher        | 64                |
| Harpoon Gun             | 128               |

### Ammunition
Ammunition is stored on both a primary and a secondary offset, similar to Tomb Raider I. When the weapon
is not equipped, ammo is only stored on the primary offset. When the weapon is equipped, ammo is stored
on both offsets. The primary ammo offsets are static per level. However, the secondary ammo offsets are
dynamically allocated throughout each level. To be able to dynamically calculate the secondary ammo offsets,
you must store the base location of the `{0xFF, 0xFF, 0xFF, 0xFF}` array that precedes the null padding.
Here is an example of how this can be done for Tomb Raider II:

```
private readonly Dictionary<byte, int[]> ammoIndexData = new Dictionary<byte, int[]>
{
    {  1, new int[] { 0x0FE0, 0x0FE1, 0x0FE2, 0x0FE3 } },   // The Great Wall
    {  2, new int[] { 0x10EC, 0x10ED, 0x10EE, 0x10EF } },   // Venice
    {  3, new int[] { 0x12B4, 0x12B5, 0x12B6, 0x12B7 } },   // Bartoli's Hideout
    {  4, new int[] { 0x19EE, 0x19EF, 0x19F0, 0x19F1 } },   // Opera House
    {  5, new int[] { 0x1020, 0x1021, 0x1022, 0x1023 } },   // Offshore Rig
    {  6, new int[] { 0x1274, 0x1275, 0x1276, 0x1277 } },   // Diving Area
    {  7, new int[] { 0x0C0E, 0x0C0F, 0x0C10, 0x0C11 } },   // 40 Fathoms
    {  8, new int[] { 0x16F4, 0x16F5, 0x16F6, 0x16F7 } },   // Wreck of the Maria Doria
    {  9, new int[] { 0x0EA4, 0x0EA5, 0x0EA6, 0x0EA7 } },   // Living Quarters
    { 10, new int[] { 0x11C8, 0x11C9, 0x11CA, 0x11CB } },   // The Deck
    { 11, new int[] { 0x1402, 0x1403, 0x1404, 0x1405 } },   // Tibetan Foothills
    { 12, new int[] { 0x1972, 0x1973, 0x1974, 0x1975 } },   // Barkhang Monastery
    { 13, new int[] { 0x1522, 0x1523, 0x1524, 0x1525 } },   // Catacombs of the Talion
    { 14, new int[] { 0x122A, 0x122B, 0x122C, 0x122D } },   // Ice Palace
    { 15, new int[] { 0x1A6A, 0x1A6B, 0x1A6C, 0x1A6D } },   // Temple of Xian
    { 16, new int[] { 0x1204, 0x1205, 0x1206, 0x1207 } },   // Floating Islands
    { 17, new int[] { 0x0D30, 0x0D31, 0x0D32, 0x0D33 } },   // The Dragon's Lair
    { 18, new int[] { 0x1020, 0x1021, 0x1022, 0x1023 } },   // Home Sweet Home
};
```

With each index marker array's location stored on the array, you can then calculate both
the secondary ammo index, as well as the base secondary ammo offsets. To see how to
calculate the secondary ammo index, see the section above. Here is how to calculate the
base secondary ammo offsets based on the index data:

```
int baseSecondaryAmmoIndexOffset = ammoIndexData[levelIndex][0];

automaticPistolsAmmoOffset2 = baseSecondaryAmmoIndexOffset - 28;
uziAmmoOffset2 = baseSecondaryAmmoIndexOffset - 24;
shotgunAmmoOffset2 = baseSecondaryAmmoIndexOffset - 20;
harpoonGunAmmoOffset2 = baseSecondaryAmmoIndexOffset - 16;
grenadeLauncherAmmoOffset2 = baseSecondaryAmmoIndexOffset - 12;
m16AmmoOffset2 = baseSecondaryAmmoIndexOffset - 8;
```

Once you have the ammo index and the base secondary ammo offset, the next step is to
is calculate the secondary ammo offsets based on the index. This can be achieved with a simple for loop:

```
private int GetSecondaryAmmoOffset(int baseOffset)
{
    List<int> secondaryAmmoOffsets = new List<int>();

    for (int i = 0; i < 20; i++)
    {
        secondaryAmmoOffsets.Add(baseOffset + i * 0x6);
    }

    return secondaryAmmoOffsets[secondaryAmmoIndex];
}
```

Finally, once you have the dynamically allocated secondary ammo offset, all that is left to do is to write
to the it. Because there are edge cases where the secondary ammo index may not be found, this should be
accounted for when writing ammo to avoid a savegame that causes a crash:

```
private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
{
    WriteUInt16(shotgunAmmoOffset, ammo);

    if (isPresent && secondaryAmmoIndex != -1)
    {
        WriteUInt16(shotgunAmmoOffset2, ammo);
    }
    else if (!isPresent && secondaryAmmoIndex != -1)
    {
        WriteUInt16(shotgunAmmoOffset2, 0);
    }
}
```

### Health
Health is stored dynamically on anywhere from 1-6 different offsets per level. Since the health offsets shift
throughout a level, it is best to use the heuristic method to determine the correct health offset outlined
in the [above section](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-heuristics-to-determine-the-health-offset).
In the case of Tomb Raider II, the health offsets are stored 8, 9, and 10 bytes away from the character movement data.

## Tomb Raider III
The savegame file structure of Tomb Raider III is very similar to that of Tomb Raider II, so the processes involved
in reversing it are very similar. Similar to Tomb Raider II, the file offsets differ on each level, and are dynamically allocated.
You can calculate the dynamic offsets based on the level index:

```
byte levelIndex = GetLevelIndex();

smallMedipackOffset = 0xB3 + (levelIndex * 0x33);
largeMedipackOffset = 0xB4 + (levelIndex * 0x33);
flaresOffset = 0xB6 + (levelIndex * 0x33);
weaponsConfigNumOffset = 0xBA + (levelIndex * 0x33);
harpoonGunOffset = 0xBB + (levelIndex * 0x33);
deagleAmmoOffset = 0xA5 + (levelIndex * 0x33);
uziAmmoOffset = 0xA7 + (levelIndex * 0x33);
shotgunAmmoOffset = 0xA9 + (levelIndex * 0x33);
mp5AmmoOffset = 0xAB + (levelIndex * 0x33);
rocketLauncherAmmoOffset = 0xAD + (levelIndex * 0x33);
harpoonGunAmmoOffset = 0xAF + (levelIndex * 0x33);
grenadeLauncherAmmoOffset = 0xB1 + (levelIndex * 0x33);
```

### Weapons
Similar to the previous two titles, Tomb Raider III also stores weapons information on a single offset - with the exception
of the Harpoon Gun, which is stored as a boolean on its own offset, 1 byte away from the weapons config number. Bitwise
can be used to determine which weapons are present in inventory -- see the [section above](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-bitwise-to-extract-weapons-information)
on how to do this.

###                                         ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Desert Eagle            | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |
| MP5                     | 32                |
| Rocket Launcher         | 64                |
| Grenade Launcher        | 128               |

### Ammunition
Tomb Raider III also stores ammunition on a primary and secondary offset, with the same logic as the previous two titles;
an equipped weapon stores ammo on both offsets, while a non-equipped weapon only stores ammo in the primary offset. The
secondary ammo indices are dynamically allocated. The methods for writing to ammunition are almost identical to Tomb Raider II.
Here is the dictionary for the base secondary ammo index arrays for Tomb Raider III:

```
private readonly Dictionary<byte, int[]> ammoIndexData = new Dictionary<byte, int[]>
{
    { 1,  new int[] { 0x1663, 0x1664, 0x1665, 0x1666 } },   // Jungle
    { 2,  new int[] { 0x23D3, 0x23D4, 0x23D5, 0x23D6 } },   // Temple Ruins
    { 3,  new int[] { 0x181C, 0x181D, 0x181E, 0x181F } },   // The River Ganges
    { 4,  new int[] { 0x0D37, 0x0D38, 0x0D39, 0x0D3A } },   // Caves of Kaliya
    { 13, new int[] { 0x17BC, 0x17BD, 0x17BE, 0x17BF } },   // Nevada Desert
    { 14, new int[] { 0x1E63, 0x1E64, 0x1E65, 0x1E66 } },   // High Security Compound
    { 15, new int[] { 0x2125, 0x2126, 0x2127, 0x2128 } },   // Area 51
    { 5,  new int[] { 0x17C9, 0x17CA, 0x17CB, 0x17CC } },   // Coastal Village
    { 6,  new int[] { 0x18EB, 0x18EC, 0x18ED, 0x18EE } },   // Crash Site
    { 7,  new int[] { 0x1435, 0x1436, 0x1437, 0x1438 } },   // Madubu Gorge
    { 8,  new int[] { 0x110D, 0x110E, 0x110F, 0x1110 } },   // Temple of Puna
    { 9,  new int[] { 0x188B, 0x188C, 0x188D, 0x188E } },   // Thames Wharf
    { 10, new int[] { 0x2317, 0x2318, 0x2319, 0x231A } },   // Aldwych
    { 11, new int[] { 0x1D8F, 0x1D90, 0x1D91, 0x1D92 } },   // Lud's Gate
    { 12, new int[] { 0x0B0B, 0x0B0C, 0x0B0D, 0x0B0E } },   // City
    { 16, new int[] { 0x19AD, 0x19AE, 0x19AF, 0x19B0 } },   // Antarctica
    { 17, new int[] { 0x196F, 0x1970, 0x1971, 0x1972 } },   // RX-Tech Mines
    { 18, new int[] { 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2 } },   // Lost City of Tinnos
    { 19, new int[] { 0x0B01, 0x0B02, 0x0B03, 0x0B04 } },   // Meteorite Cavern
    { 20, new int[] { 0x1045, 0x1046, 0x1047, 0x1048 } },   // All Hallows
 };

```

With the base secondary ammo indices, you can then calculate both the current secondary ammo index
as well as the base secondary ammo offset. To see how to calculate the secondary ammo index, see
the section above. Here is how to calculate the secondary offset from the base offset for Tomb Raider III:

```
private int GetSecondaryAmmoOffset(int baseOffset)
{
    List<int> secondaryAmmoOffsets = new List<int>();

    for (int i = 0; i < 15; i++)
    {
        secondaryAmmoOffsets.Add(baseOffset + i * 0x12);
    }

    return secondaryAmmoOffsets[secondaryAmmoIndex];
}
```

The last step is to write the ammo. Again, just as with Tomb Raider II, since there may be edge cases where the
secondary ammo index could not be found, it is best to ensure that the secondary ammo index was found before
attempting to write to the secondary offset:

```
private void WriteShotgunAmmo(bool isPresent, UInt16 ammo)
{
    WriteUInt16(shotgunAmmoOffset, ammo);

    if (isPresent && secondaryAmmoIndex != -1)
    {
        WriteUInt16(shotgunAmmoOffset2, ammo);
    }
    else if (!isPresent && secondaryAmmoIndex != -1)
    {
        WriteUInt16(shotgunAmmoOffset2, 0);
    }
}
```

### Health
Health information is also stored dynamically. It can be stored on anywhere from 1-4 unique offsets per level. To avoid
writing to the incorrect health offset, it is neccessary to use the heuristic algorithm outlined in the
[above section](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-heuristics-to-determine-the-health-offset).
In the case of Tomb Raider III, the character movement data is stored 8 bytes away from the current health offset.

## Tomb Raider: The Last Revelation
Tomb Raider 4 is built on an engine that utilizes efficient memory management. Unlike the previous titles, Tomb Raider 4 uses
static offsets for every variable across all levels. Also unlike the previous titles, weapons information in Tomb Raider 4 is
not stored on a single offset; instead, it is stored on individual offsets. For weapons, a value of 0x9 indicates the weapon is
present. A value of 0xD indicates the weapon is present with a LaserSight attached. For items such as binoculars, crowbar, and
LaserSight, a value of 0x1 indicates it is present in inventory.

### Checksum algorithm
The biggest challenge in reversing the Tomb Raider 4 savegames is the fact that it uses a file checksum to determine if a
savegame is valid or not. If a savegame's checksum does not match the game's calculation, it will show up as "Empty Slot".
So, when modifying a savegame, you must calculate the new checksum after writing your changes. The checksum is a simple
modulo-256 checksum. The calculation begins on offset 0x57, and ends where the file ends.

```
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
```

## Tomb Raider: Chronicles
Similar to Tomb Raider 4, most of the savegame data in Tomb Raider 5 is stored statically. The only exception is the health data,
which is dynamically stored on a range that varies on each level. To determine the correct health offset, it is necessary to use
the heuristic algorithm outlined in the [above section](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#using-heuristics-to-determine-the-health-offset).
For Tomb Raider 5, the character animation data is located between 6 bytes away from the health offset.

### Health offsets
| **Level**           	   | **Offset range** |
| :---                	   | :---             |
| Streets of Rome     	   | 0x4F4 - 0x4F8	 |
| Trajan's Markets    	   | 0x542 - 0x5D7	 |
| The Colosseum	      	| 0x4D2 - 0x7FF	 |
| The Base		            | 0x556 - 0x707	 |
| The Submarine		      | 0x520 - 0x5D2	 |
| Deepsea Dive		         | 0x644 - 0x6DE	 |
| Sinking Submarine	      | 0x5CC - 0x66B	 |
| Gallows Tree		         | 0x4F0 - 0x52D	 |
| Labyrinth		            | 0x538 - 0x61A	 |
| Old Mill		            | 0x512 - 0x624	 |
| The 13th Floor	         | 0x52A - 0x53A	 |
| Escape with the Iris	   | 0x6F6 - 0xC47	 |
| Red Alert!		         | 0x52C - 0x5D6	 |

### Weapons
Weapons information is also stored similar to Tomb Raider 4; each weapon is stored on its own unique offset. A value of 0x9
indicates the weapon is present, a value of 0xD indicates the weapon is present with a LaserSight attached, and a value of 0
indicates the weapon is not present. Ammunition data is also stored statically, and on its own individual offsets.
