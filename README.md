# Tomb Raider - Savegame Editor
This is a fully-featured savegame editor for the classic Tomb Raider series. It works with Tomb Raider 1-5, including the 3 expansion packs and bonus levels.
This editor is compatible with the original CD savegames as well as the Steam, ATI, GOG, and multi-patched savegames. For installation and usage instructions,
please scroll down to the next section of this README. Additionally, technical details on reverse engineering the Tomb Raider series as well as offset tables are included.

![TR-SaveMaster-UI](https://github.com/JulianOzelRose/TR-SaveMaster/assets/95890436/d95b20d0-ea26-431f-ab39-74c9166d791b)

## Table of contents
- [**Installation and use**](https://github.com/JulianOzelRose/TR-SaveMaster?tab=readme-ov-file#installation-and-use)
   - [Common game directories](https://github.com/JulianOzelRose/TR-SaveMaster#common-game-directories)
   - [Dealing with protected directories](https://github.com/JulianOzelRose/TR-SaveMaster#dealing-with-protected-directories)
- [**Reverse engineering the Tomb Raider series**](https://github.com/JulianOzelRose/TR-SaveMaster#reverse-engineering-the-tomb-raider-series)
   - [Using bitwise to extract weapons information](https://github.com/JulianOzelRose/TR-SaveMaster#using-bitwise-to-extract-weapons-information)
   - [Using heuristics to determine the health offset](https://github.com/JulianOzelRose/TR-SaveMaster#using-heuristics-to-determine-the-health-offset)
   - [**Tomb Raider I**](https://github.com/JulianOzelRose/TR-SaveMaster#tomb-raider-i)
      - [Weapon byte flags](https://github.com/JulianOzelRose/TR-SaveMaster#weapon-byte-flags)
      - [Ammunition](https://github.com/JulianOzelRose/TR-SaveMaster#ammunition)
   - [**Tomb Raider II**](https://github.com/JulianOzelRose/TR-SaveMaster#tomb-raider-ii)
      - [Weapon byte flags](https://github.com/JulianOzelRose/TR-SaveMaster#weapon-byte-flags-1) 
      - [Calculating secondary ammo offsets](https://github.com/JulianOzelRose/TR-SaveMaster#calculating-secondary-ammo-offsets) 
   - [**Tomb Raider III**](https://github.com/JulianOzelRose/TR-SaveMaster#tomb-raider-iii)
      - [Weapon byte flags](https://github.com/JulianOzelRose/TR-SaveMaster#weapon-byte-flags-2) 
      - [Calculating secondary ammo offsets](https://github.com/JulianOzelRose/TR-SaveMaster#calculating-secondary-ammo-offsets-1)
      - [Health](https://github.com/JulianOzelRose/TR-SaveMaster#health)
   - [**Tomb Raider: The Last Revelation**](https://github.com/JulianOzelRose/TR-SaveMaster#tomb-raider-the-last-revelation)
      - [Checksum algorithm](https://github.com/JulianOzelRose/TR-SaveMaster#checksum-algorithm)
   - [**Tomb Raider: Chronicles**](https://github.com/JulianOzelRose/TR-SaveMaster#tomb-raider-chronicles)
      - [Health offsets](https://github.com/JulianOzelRose/TR-SaveMaster#health-offsets)
      - [Weapons](https://github.com/JulianOzelRose/TR-SaveMaster#weapons)
- [**Offset tables**](https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/OFFSETS.md)
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
download ```TR-SaveMaster.exe```, then open it. There is no need to install anything, and it can be run from anywhere on your computer. To toggle between the different
Tomb Raider games, click the appopriate tab on the tab control on the top. To begin savegame editing, you must first set your game's directory. To do this, click "Browse",
the navigate to your game's directory on your computer. The exact directory depends on whether you have a Steam installation, a CD installation, or a GOG installation.
Below are some common game directories. Note that the ```\AppData\``` directory is hidden, so you will need to either set Windows to show hidden files and folders,
or manually enter the directory.

### Common game directories
- **Tomb Raider I**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider I\TOMBRAID\```
  - GOG: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 1\TOMBRAID```
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)\TOMBRAID\```
- **Tomb Raider I: Unfinished Business**
  - Steam (Patched): ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)\```
- **Tomb Raider II**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider II\```
  - GOG: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 2\```
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (II)\```
- **Tomb Raider II: The Golden Mask**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Tomb Raider II Gold (Full Net)\```
- **Tomb Raider III** 
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider III\```
  - GOG: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\GOG.com\Tomb Raider 1 2 3\Tomb Raider 3\```
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\```
- **Tomb Raider III: The Lost Artifact**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact\```
- **Tomb Raider: The Last Revelation**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Last Revelation\```
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (IV) The Last Revelation\```
- **Tomb Raider: Chronicles**
  - CD: ```C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider Chronicles\```
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (V) Chronicles\```

Once your game directory is selected, the editor will populate with the savegames found. To toggle between them, simply click the dropdown box labled "Savegame". You
can then change ammunition, weapons, health, and items. Click "Save" when you are done, and your changes will be applied. This editor automatically creates backups
of savegame files, which is enabled by default. You can toggle auto backups off, but it is highly recommended that you leave it enabled. To find this option, click
"File" and then look for "Create backups". Under the "View" menu, you can also change the UI theme and hide the status bar, if you desire a simpler interface.
This savegame editor will remember your game directories, so there is no need to re-enter them each time you launch it.

For Tomb Raider: Chronicles and The Last Revelation, ammunition will be represented in the editor exactly as displayed in the game. However, for Tomb Raider 1-3 and their
expansions, the ammunition for weapons not currently equipped may appear differently than in-game. Specifically, Harpoons are grouped in bundles of 2, Desert Eagle clips
equate to 5 bullets, MP5 clips equate to 30 bullets, Uzi clips equate to 20 bullets, and a single box of grenades is equivalent to 2 rounds of Grenade Launcher ammunition.

### Dealing with protected directories
If you did a CD install with one of the original games or expansions, the savegame files are likely located in a protected directory, and modifications to them
will not be possible. To circumvent this issue, you will have to modify the folder permissions to allow modification. First, right-click on the Start menu, then click
"Windows PowerShell (Admin)". Then, use the following commands:

```
takeown /f "C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact" /r /d y

icacls "C:\Users\USERNAME\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact" /grant:r %USERNAME%:F /t
```

Of course, be sure to replace the directory with the one that you are encountering permission issues with, and be sure to replace "USERNAME" with your actual username.
Run the `takeown` command first, then run the `icals` command second. You should then be able to modify files within that directory and begin savegame editing.


# Reverse engineering the Tomb Raider series
This section details the technical aspects of reverse engineering the savegame files for the classic Tomb Raider series. The offset tables for each game are included
at the end of the README. In general, there are many similarities between the games, as they are all built on the same engine. Tomb Raider 1-3 use a very similar engine,
and so reversing those games involve a similar process. Tomb Raider 4 and 5 use a markedly different engine than 1-3, but involve a simpler process nonetheless.
Interestingly, Tomb Raider 4 uses a file checksum as an anti-reverse engineering technique, while Tomb Raider 5 does not.

## Using bitwise to extract weapons information
In Tomb Raider 1-3, all weapons information is stored on a single offset, which I call the weapons configuration number. The weapons configuration variable has a base number of 1,
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
```

## Tomb Raider I
Reverse engineering Tomb Raider I savegames is relatively straightforward. The biggest convenience is that most of the file offsets are static. Meaning, they are the same
across levels. The only exception is the secondary ammunition offsets, which are still static, but different on each level. Weapons information can be extracted using
bitwise methods, as outlined in the [section above](https://github.com/JulianOzelRose/TR-SaveMaster#using-bitwise-to-extract-weapons-information).

### Weapon byte flags
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
removing a weapon from inventory, you should also zero the secondary ammo offset.

## Tomb Raider II
Reverse engineering Tomb Raider II savegames is slightly different compared to Tomb Raider I. The file offsets
differ on each level, and are dynamically allocated. Similar to Tomb Raider I, weapons information is also stored
on a single offset in Tomb Raider II. You can use the same methodology to extract weapons information outlined in
the [above section](https://github.com/JulianOzelRose/TR-SaveMaster#using-bitwise-to-extract-weapons-information).

### Weapon byte flags
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

### Calculating secondary ammo offsets
Ammunition is also stored on a primary and secondary offset, this aspect works the same as in Tomb Raider I. However,
the secondary ammo offsets are dynamically allocated and must therefore be calculated before writing. The location
of the secondary ammo offsets seem to depend on the number of active entities in the game. When there are no entities,
the secondary ammo is stored on lower offsets. As more entities are rendered, the secondary ammo offsets shift upwards.
There is a line in the savegame files that shifts along with the secondary ammo offsets consistently. It's a 4-byte line
consisting of 0xFF, 4 times. It's somewhat of an EOF marker. You can store the locations of the 0xFF markers using
a dictionary. Then, using the location of the first byte of the 0xFF line, you can calculate the secondary ammo offsets.

```
private void SetSecondaryAmmoOffsets()
{
    int secondaryAmmoIndexMarker = GetSecondaryAmmoIndexMarker();

    automaticPistolsAmmoOffset2 = secondaryAmmoIndexMarker - 28;
    uziAmmoOffset2 = secondaryAmmoIndexMarker - 24;
    shotgunAmmoOffset2 = secondaryAmmoIndexMarker - 20;
    harpoonGunAmmoOffset2 = secondaryAmmoIndexMarker - 16;
    grenadeLauncherAmmoOffset2 = secondaryAmmoIndexMarker - 12;
    m16AmmoOffset2 = secondaryAmmoIndexMarker - 8;
}
```

## Tomb Raider III
Similar to the previous two titles, Tomb Raider III also stores weapons information on a single offset - with the exception
of the Harpoon Gun, which is stored as a boolean on its own offset, 1 byte away from the weapons config number. Bitwise
can be used to determine which weapons are present in inventory -- see the [section above](https://github.com/JulianOzelRose/TR-SaveMaster#using-bitwise-to-extract-weapons-information)
on how to do this.

### Weapon byte flags
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

### Calculating secondary ammo offsets
Tomb Raider III also stores ammunition on a primary and secondary offset, with the same logic as the previous two titles;
an equipped weapon stores ammo on both offsets, while a non-equipped weapon only stores ammo in the primary offset. The
secondary ammo indices are dynamically allocated. The pattern is more consistent than Tomb Raider II, as the offsets
are always 18 bytes apart. Storing the index data as a dictionary, you can use the following algorithm to determine
the dynamically allocated ammo offsets:

```
private int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
{
    List<int> secondaryOffsets = new List<int>();
    List<int> validOffsets = new List<int>();

    int currentAmmoIndex = GetAmmoIndex();

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
```

### Health
Health information is also stored dynamically. It can be stored on anywhere from 1-4 unique offsets per level. To avoid
writing to the incorrect health offset, it is neccessary to use the heuristic algorithm outlined in the
[above section](https://github.com/JulianOzelRose/TR-SaveMaster#using-heuristics-to-determine-the-health-offset).
In the case of Tomb Raider III, the character movement data is stored between 8-10 bytes away from the current health offset.

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
the heuristic algorithm outlined in the [above section](https://github.com/JulianOzelRose/TR-SaveMaster#using-heuristics-to-determine-the-health-offset).
For Tomb Raider 5, the character animation data is located between 6-7 bytes away from the health offset.

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
