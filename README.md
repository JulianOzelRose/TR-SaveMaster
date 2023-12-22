# Tomb Raider - Savegame Editor
This is a fully-featured savegame editor for the classic Tomb Raider series, compatible with Tomb Raider 1-5, including the expansion packs and bonus levels.
It is designed to function seamlessly with both the Steam versions and the CD/multi-patched versions of the games. For installation and usage instructions,
please scroll down to the next section of this README. Additionally, technical details on reverse engineering the Tomb Raider classic series and offset tables are included.

![TR-SaveMaster-UI](https://github.com/JulianOzelRose/TR-SaveMaster/assets/95890436/d95b20d0-ea26-431f-ab39-74c9166d791b)

## Installation and use
To use this savegame editor, simply navigate to the [Release](https://github.com/JulianOzelRose/TR-SaveMaster/tree/master/TR-SaveMaster/bin/x64/Release) folder, then
download ```TR-SaveMaster.exe```, then open it. There is no need to install anything, and it can be run from anywhere on your computer. To toggle between the different
Tomb Raider games, click the appopriate tab on the tab control above. To begin savegame editing, you must first set your game's directory. To do this, click ```Browse```,
the navigate to your game's directory on your computer. The exact directory depends on whether you have a Steam installation or a CD installation. Here are some common
directories for each game:

- **Tomb Raider I**
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)\TOMBRAID```
- **Tomb Raider I: Unfinished Business**
  - Multi-patched: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (I)```
- **Tomb Raider II**
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (II)```
- **Tomb Raider II: The Golden Mask**
  - CD: ```C:\Users\YourUserName\AppData\Local\VirtualStore\Program Files (x86)\Tomb Raider II Gold (Full Net)```
- **Tomb Raider III**
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\TombRaider (III)```
- **Tomb Raider III: The Lost Artifact**
  - CD: ```C:\Users\YourUserName\AppData\Local\VirtualStore\Program Files (x86)\Core Design\Tomb Raider - The Lost Artifact```
- **Tomb Raider: The Last Revelation**
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (IV) The Last Revelation```
- **Tomb Raider: Chronicles**
  - Steam: ```C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (V) Chronicles```

Once your game directory is selected, the editor will populate with the savegames found. To toggle between them, simply click the dropdown box labled "Savegame". You
can then change ammunition, weapons, health, and items. Click ```Save``` when you are done, and your changes will be applied. This editor automatically creates backups
of savegame files, which is enabled by default. You can toggle auto backups off, but it is highly reccommended that you leave it enabled. To find this option, click
```File``` and then look for ```Create backups```. Under the ```View``` menu, you can also change the UI theme and hide the status bar, if you desire a simpler interface.
This savegame editor will remember your game directories, so there is no need to re-enter them each time you launch it.

# Reverse engineering the Tomb Raider series
This section details the technical aspects of reverse engineering the savegame files for the classic Tomb Raider series. The offset tables for each game are included
at the end of the README. In general, there are many similarities between the games, as they are all built on the same engine. Tomb Raider 1-3 use a very similar engine,
and so reversing those games involve a similar process. Tomb Raider 4 and 5 use a markedly different engine than 1-3, but involve a simpler process nonetheless.
Interestingly enough, Tomb Raider 4 uses a file checksum as an anti-reverse engineering technique. However, Tomb Raider 5 does not employ this method.

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
then increment the new weapons config number based on which weapons are checkmarked in the UI. Here is an example of this
for Tomb Raider II:

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
1,000 (full health). In Tomb Raider 1-3, there are several offsets that store health interchangably. In Tomb Raider 5
there is a health offset range. In other words, health is dynamically allocated. Since writing to the incorrect health
offset may cause the game to crash, it is important to determine the health offset with an accurate method. Since health
is stored right next to character movement data, is is possible to find the correct health offset by checking the surrounding
data for character movement byte flags. Here is an example of this algorithm for Tomb Raider III savegames:

```
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
```

## Tomb Raider I
Reverse engineering Tomb Raider I savegames is relatively straightforward. The biggest convenience is that most of the file offsets are static. Meaning, they are the same
across levels. The only exception is the secondary ammunition offsets, which are still static, but different on each level. Weapons information can be extracted using
bitwise methods, as outlined in the section above. Here are the weapon codes unique to Tomb Raider I:

###                                         ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Magnums                 | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |

Ammunition is stored on up to two offsets; a primary offset, and a secondary offset. If a weapon is not equipped,
it is only stored on the primary offset. If the weapon is equipped, the ammo is stored on both offsets. So when
removing a weapon from inventory, you should also zero the secondary ammo offset. Health is stored on
one offset per level.

## Tomb Raider II
Reverse engineering Tomb Raider II savegames is slightly different compared to Tomb Raider I. The file offsets
differ on each level, and are dynamically allocated. Similar to Tomb Raider I, weapons information is also stored
on a single offset in Tomb Raider II. You can use the same methodology to extract weapons information outlined in
the above section. Here are the weapon numbers:

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
can be used to extract the weapons present in inventory -- see the section above for how to do this. Here are the unique
weapon byte codes for Tomb Raier III:

###                                         ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Deagle                  | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |
| MP5                     | 32                |
| Rocket Launcher         | 64                |
| Grenade Launcher        | 128               |

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
    validOffsets.Add(secondaryOffsets[currentAmmoIndex]);

    return validOffsets.ToArray();
}
```

Health information is also stored dynamically. It can be stored on anywhere from 1-4 unique offsets per level. To avoid
writing to the incorrect health offset, it is neccessary to use the heuristic algorithm outlined in the above section.
In the case of Tomb Raider III, the character movement data is stored between 8-10 bytes away from the current health offset.

## Tomb Raider: The Last Revelation
Tomb Raider 4 is built on an engine that utilizes efficient memory management. Unlike the previous titles, Tomb Raider 4 uses
static offsets for every variable across all levels. Also unlike the previous titles, weapons information in Tomb Raider 4 is
not stored on a single offset; instead, it is stored on individual offsets. For weapons, a value of 0x9 indicates the weapon is
present. A value of 0xD indicates the weapon is present with a LaserSight attached. For items such as binoculars, crowbar, and
LaserSight, a value of 0x1 indicates it is present in inventory.

The biggest challenge in reversing the Tomb Raider 4 savegames is the fact that it uses a file checksum to determine if a
savegame is valid or not. If a savegame's checksum does not match the game's calculation, it will show up as "Empty Slot".
So, when modifying a savegame, you must calculate the new checksum after writing your changes. The checksum is a simple
modulo-256 checksum. The calculation begins on offset 0x57, and ends where the file ends. Here is the algorithm used to
calculate the checksum:

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
the heuristic algorithm outlined in the above section. For Tomb Raider 5, the character animation data is located between 6-7
bytes away from the health offset. Below is a table of the health offset ranges:

| **Level**           	| **Offset range** |
| :---                	| :---             |
| Streets of Rome     	| 0x4F4 - 0x4F8		 |
| Trajan's Markets    	| 0x542 - 0x5D7		 |
| The Colosseum	      	| 0x4D2 - 0x7FF		 |
| The Base		          | 0x556 - 0x707		 |
| The Submarine		      | 0x520 - 0x5D2		 |
| Deepsea Dive		      | 0x644 - 0x6DE		 |
| Sinking Submarine	    | 0x5CC - 0x66B		 |
| Gallows Tree		      | 0x4F0 - 0x52D		 |
| Labyrinth		          | 0x538 - 0x61A		 |
| Old Mill		          | 0x512 - 0x624		 |
| The 13th Floor	      | 0x52A - 0x53A		 |
| Escape with the Iris	| 0x6F6 - 0xC47		 |
| Red Alert!		        | 0x52C - 0x5D6		 |

Weapons information is also stored similar to Tomb Raider 4; each weapon is stored on its own unique offset. A value of 0x9
indicates the weapon is present, a value of 0xD indicates the weapon is present with a LaserSight attached, and a value of 0
indicates the weapon is not present. Ammunition data is also stored statically, and on its own individual offsets.

# Offset tables
## Tomb Raider I
### Common ###
| **File offset**        | **Type**         | **Variable**            |
| :---                	 | :---             | :---                    |
| 0x0000                 | String           | Level Name              |
| 0x004B                 | UInt16           | Save Number             |
| 0x018C                 | UInt16           | Magnum Ammo             |
| 0x018E                 | UInt16           | Uzi Ammo                |
| 0x0190                 | UInt16           | Shotgun Ammo            |
| 0x0192                 | BYTE             | Small Medipack          |
| 0x0193                 | BYTE             | Large Medipack          |
| 0x0197                 | BYTE             | Weapons Config Num      |

### Caves ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0641                   |
| Uzi Ammo              | UInt16           | 0x064D                   |
| Shotgun Ammo          | UInt16           | 0x0659                   |

### City of Vilcabamba ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0C24                   |
| Uzi Ammo              | UInt16           | 0x0C30                   |
| Shotgun Ammo          | UInt16           | 0x0C3C                   |

### Lost Valley ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0598                   |
| Uzi Ammo              | UInt16           | 0x0C30                   |
| Shotgun Ammo          | UInt16           | 0x0C3C                   |

### Tomb of Qualopec ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x084A                   |
| Uzi Ammo              | UInt16           | 0x0856                   |
| Shotgun Ammo          | UInt16           | 0x0862                   |

### St. Francis' Folly ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0D70                   |
| Uzi Ammo              | UInt16           | 0x0D7C                   |
| Shotgun Ammo          | UInt16           | 0x0D88                   |

### Colosseum ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0A56                   |
| Uzi Ammo              | UInt16           | 0x0A62                   |
| Shotgun Ammo          | UInt16           | 0x0A6E                   |

### Palace Midas ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0D48                   |
| Uzi Ammo              | UInt16           | 0x0D54                   |
| Shotgun Ammo          | UInt16           | 0x0D60                   |

### The Cistern ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0C8A                   |
| Uzi Ammo              | UInt16           | 0x0C96                   |
| Shotgun Ammo          | UInt16           | 0x0CA2                   |

### Tomb of Tihocan ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x093D                   |
| Uzi Ammo              | UInt16           | 0x0949                   |
| Shotgun Ammo          | UInt16           | 0x0955                   |

### City of Khamoon ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x085D                   |
| Uzi Ammo              | UInt16           | 0x0869                   |
| Shotgun Ammo          | UInt16           | 0x0955                   |

### Obelisk of Khamoon ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x08A3                   |
| Uzi Ammo              | UInt16           | 0x08AF                   |
| Shotgun Ammo          | UInt16           | 0x08BB                   |

### Sanctuary of the Scion ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0718                   |
| Uzi Ammo              | UInt16           | 0x0724                   |
| Shotgun Ammo          | UInt16           | 0x0730                   |

### Natla's Mines ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x08A8                   |
| Uzi Ammo              | UInt16           | 0x08B4                   |
| Shotgun Ammo          | UInt16           | 0x08C0                   |

### Atlantis ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x0FFA                   |
| Uzi Ammo              | UInt16           | 0x1006                   |
| Shotgun Ammo          | UInt16           | 0x1012                   |

### The Great Pyramid ###
| **File offset**       | **Type**         | **Variable**             |
| :---                	| :---             | :---                     |
| Magnum Ammo           | UInt16           | 0x08D2                   |
| Uzi Ammo              | UInt16           | 0x08DE                   |
| Shotgun Ammo          | UInt16           | 0x08EA                   |

## Tomb Raider I: Unfinished Business
### Common ###
| **File Offset**       | **Type**         | **Variable**            |
| :---                	| :---             | :---                    |
| 0x000                 | String           | Level Name              |
| 0x04B                 | UInt16           | Save Number             |
| 0x09C                 | UInt16           | Magnum Ammo             |
| 0x09E                 | UInt16           | Uzi Ammo                |
| 0x0A0                 | UInt16           | Shotgun Ammo            |
| 0x0A2                 | BYTE             | Small Medipack          |
| 0x0A3                 | BYTE             | Large Medipack          |
| 0x0A7                 | BYTE             | Weapons Config Num      |

## Tomb Raider III
#### Jungle ####
| **File offset** | **Type** | **Variable**            |
| :---            | :---     | :---                    |
| 0x0000          | String   | Level Name              |
| 0x004B          | UInt16   | Save Number             |
| 0x00D8          | UInt16   | Deagle Ammo 1           |
| 0x00DA          | UInt16   | Uzi Ammo 1              |
| 0x00DC          | UInt16   | Shotgun Ammo 1          |
| 0x00DE          | UInt16   | MP5 Ammo 1              |
| 0x00E0          | UInt16   | Rocket Launcher Ammo 1  |
| 0x00E2          | UInt16   | Harpoon Ammo 1          |
| 0x00E4          | UInt16   | Grenade Launcher Ammo 1 |
| 0x00E6          | BYTE     | Small Medipack          |
| 0x00E7          | BYTE     | Large Medipack          |
| 0x00E9          | BYTE     | Flares                  |
| 0x00ED          | BYTE     | Weapons Config Number   |
| 0x00EE          | BYTE     | Harpoon Gun             |
| 0x1643          | UInt16   | Deagle Ammo 2	       |
| 0x1647          | UInt16   | Uzi Ammo 2              |
| 0x164B          | UInt16   | Shotgun Ammo 2          |
| 0x164F          | UInt16   | Harpoon Ammo 2          |
| 0x1653          | UInt16   | Rocket Launcher Ammo 2  |
| 0x1657          | UInt16   | Grenade Launcher Ammo 2 |
| 0x165B          | UInt16   | MP5 Ammo 2              |

#### Temple Ruins ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x010B            | UInt16   | Deagle Ammo 1           |
| 0x010D            | UInt16   | Uzi Ammo 1              |
| 0x010F            | UInt16   | Shotgun Ammo 1          |
| 0x0111            | UInt16   | MP5 Ammo 1              |
| 0x0113            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0115            | UInt16   | Harpoon Ammo 1          |
| 0x0117            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0119            | BYTE     | Small Medipack          |
| 0x011A            | BYTE     | Large Medipack          |
| 0x011C            | BYTE     | Flares                  |
| 0x0120            | BYTE     | Weapons Config Number   |
| 0x0121            | BYTE     | Harpoon Gun             |
| 0x23B3            | UInt16   | Deagle Ammo 2           |
| 0x23B7            | UInt16   | Uzi Ammo 2              |
| 0x23BB            | UInt16   | Shotgun Ammo 2          |
| 0x23BF            | UInt16   | Harpoon Ammo 2          |
| 0x23C3            | UInt16   | Rocket Launcher Ammo 2  |
| 0x23C7            | UInt16   | Grenade Launcher Ammo 2 |
| 0x23CB            | UInt16   | MP5 Ammo 2              |

#### The River Ganges ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x013E            | UInt16   | Deagle Ammo 1           |
| 0x0140            | UInt16   | Uzi Ammo 1              |
| 0x0142            | UInt16   | Shotgun Ammo 1          |
| 0x0144            | UInt16   | MP5 Ammo 1              |
| 0x0146            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0148            | UInt16   | Harpoon Ammo 1          |
| 0x014A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x014C            | BYTE     | Small Medipack          |
| 0x014D            | BYTE     | Large Medipack          |
| 0x014F            | BYTE     | Flares                  |
| 0x0153            | BYTE     | Weapons Config Number   |
| 0x0154            | BYTE     | Harpoon Gun             |
| 0x17FC            | UInt16   | Deagle Ammo 2           |
| 0x1800            | UInt16   | Uzi Ammo 2              |
| 0x1804            | UInt16   | Shotgun Ammo 2          |
| 0x1808            | UInt16   | Harpoon Ammo 2          |
| 0x180C            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1810            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1814            | UInt16   | MP5 Ammo 2              |

#### Caves of Kaliya ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0171            | UInt16   | Deagle Ammo 1           |
| 0x0173            | UInt16   | Uzi Ammo 1              |
| 0x0175            | UInt16   | Shotgun Ammo 1          |
| 0x0177            | UInt16   | MP5 Ammo 1              |
| 0x0179            | UInt16   | Rocket Launcher Ammo 1  |
| 0x017B            | UInt16   | Harpoon Ammo 1          |
| 0x017D            | UInt16   | Grenade Launcher Ammo 1 |
| 0x017F            | BYTE     | Small Medipack          |
| 0x0180            | BYTE     | Large Medipack          |
| 0x0182            | BYTE     | Flares                  |
| 0x0186            | BYTE     | Weapons Config Number   |
| 0x0187            | BYTE     | Harpoon Gun             |
| 0x0D17            | UInt16   | Deagle Ammo 2           |
| 0x0D1B            | UInt16   | Uzi Ammo 2              |
| 0x0D1F            | UInt16   | Shotgun Ammo 2          |
| 0x0D23            | UInt16   | Harpoon Ammo 2          |
| 0x0D27            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0D2B            | UInt16   | Grenade Launcher Ammo 2 |
| 0x0D2F            | UInt16   | MP5 Ammo 2              |

#### Nevada Desert ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x033C            | UInt16   | Deagle Ammo 1           |
| 0x033E            | UInt16   | Uzi Ammo 1              |
| 0x0340            | UInt16   | Shotgun Ammo 1          |
| 0x0342            | UInt16   | MP5 Ammo 1              |
| 0x0344            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0346            | UInt16   | Harpoon Ammo 1          |
| 0x0348            | UInt16   | Grenade Launcher Ammo 1 |
| 0x034A            | BYTE     | Small Medipack          |
| 0x034B            | BYTE     | Large Medipack          |
| 0x034A            | BYTE     | Flares                  |
| 0x0351            | BYTE     | Weapons Config Number   |
| 0x0352            | BYTE     | Harpoon Gun             |
| 0x179C            | UInt16   | Deagle Ammo 2           |
| 0x17A0            | UInt16   | Uzi Ammo 2              |
| 0x17A4            | UInt16   | Shotgun Ammo 2          |
| 0x17A8            | UInt16   | Harpoon Ammo 2          |
| 0x17AC            | UInt16   | Rocket Launcher Ammo 2  |
| 0x17B0            | UInt16   | Grenade Launcher Ammo 2 |
| 0x17B4            | UInt16   | MP5 Ammo 2              |

#### High Security Compound ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x036F            | UInt16   | Deagle Ammo 1           |
| 0x0371            | UInt16   | Uzi Ammo 1              |
| 0x0373            | UInt16   | Shotgun Ammo 1          |
| 0x0375            | UInt16   | MP5 Ammo 1              |
| 0x0377            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0379            | UInt16   | Harpoon Ammo 1          |
| 0x037B            | UInt16   | Grenade Launcher Ammo 1 |
| 0x037D            | BYTE     | Small Medipack          |
| 0x037E            | BYTE     | Large Medipack          |
| 0x0380            | BYTE     | Flares                  |
| 0x0384            | BYTE     | Weapons Config Number   |
| 0x0385            | BYTE     | Harpoon Gun             |
| 0x1E43            | UInt16   | Deagle Ammo 2           |
| 0x1E47            | UInt16   | Uzi Ammo 2              |
| 0x1E4B            | UInt16   | Shotgun Ammo 2          |
| 0x1E4F            | UInt16   | Harpoon Ammo 2          |
| 0x1E53            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1E57            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1E5B            | UInt16   | MP5 Ammo 2              |

#### Area 51 ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x03A2            | UInt16   | Deagle Ammo 1           |
| 0x03A4            | UInt16   | Uzi Ammo 1              |
| 0x03A6            | UInt16   | Shotgun Ammo 1          |
| 0x03A8            | UInt16   | MP5 Ammo 1              |
| 0x03AA            | UInt16   | Rocket Launcher Ammo 1  |
| 0x03AC            | UInt16   | Harpoon Ammo 1          |
| 0x03AE            | UInt16   | Grenade Launcher Ammo 1 |
| 0x03B0            | BYTE     | Small Medipack          |
| 0x03B1            | BYTE     | Large Medipack          |
| 0x03B3            | BYTE     | Flares                  |
| 0x03B7            | BYTE     | Weapons Config Number   |
| 0x03B8            | BYTE     | Harpoon Gun             |
| 0x2105            | UInt16   | Deagle Ammo 2           |
| 0x2109            | UInt16   | Uzi Ammo 2              |
| 0x210D            | UInt16   | Shotgun Ammo 2          |
| 0x2111            | UInt16   | Harpoon Ammo 2          |
| 0x2115            | UInt16   | Rocket Launcher Ammo 2  |
| 0x2119            | UInt16   | Grenade Launcher Ammo 2 |
| 0x211D            | UInt16   | MP5 Ammo 2              |

#### Coastal Village ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x01A4            | UInt16   | Deagle Ammo 1           |
| 0x01A6            | UInt16   | Uzi Ammo 1              |
| 0x01A8            | UInt16   | Shotgun Ammo 1          |
| 0x01AA            | UInt16   | MP5 Ammo 1              |
| 0x01AC            | UInt16   | Rocket Launcher Ammo 1  |
| 0x01AE            | UInt16   | Harpoon Ammo 1          |
| 0x01B0            | UInt16   | Grenade Launcher Ammo 1 |
| 0x01B2            | BYTE     | Small Medipack          |
| 0x01B3            | BYTE     | Large Medipack          |
| 0x01B5            | BYTE     | Flares                  |
| 0x01B9            | BYTE     | Weapons Config Number   |
| 0x01BA            | BYTE     | Harpoon Gun             |
| 0x17A9            | UInt16   | Deagle Ammo 2           |
| 0x17AD            | UInt16   | Uzi Ammo 2              |
| 0x17B1            | UInt16   | Shotgun Ammo 2          |
| 0x17B5            | UInt16   | Harpoon Ammo 2          |
| 0x17B9            | UInt16   | Rocket Launcher Ammo 2  |
| 0x17BD            | UInt16   | Grenade Launcher Ammo 2 |
| 0x17C1            | UInt16   | MP5 Ammo 2              |

#### Crash Site ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x01D7            | UInt16   | Deagle Ammo 1           |
| 0x01D9            | UInt16   | Uzi Ammo 1              |
| 0x01DB            | UInt16   | Shotgun Ammo 1          |
| 0x01DD            | UInt16   | MP5 Ammo 1              |
| 0x01DF            | UInt16   | Rocket Launcher Ammo 1  |
| 0x01E1            | UInt16   | Harpoon Ammo 1          |
| 0x01E3            | UInt16   | Grenade Launcher Ammo 1 |
| 0x01E5            | BYTE     | Small Medipack          |
| 0x01E6            | BYTE     | Large Medipack          |
| 0x01EC            | BYTE     | Weapons Config Number   |
| 0x01ED            | BYTE     | Harpoon Gun             |
| 0x18CB            | UInt16   | Deagle Ammo 2           |
| 0x18CF            | UInt16   | Uzi Ammo 2              |
| 0x18D3            | UInt16   | Shotgun Ammo 2          |
| 0x18D7            | UInt16   | Harpoon Ammo 2          |
| 0x18DB            | UInt16   | Rocket Launcher Ammo 2  |
| 0x18DF            | UInt16   | Grenade Launcher Ammo 2 |
| 0x18E3            | UInt16   | MP5 Ammo 2              |

#### Madubu Gorge ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x020A            | UInt16   | Deagle Ammo 1           |
| 0x020C            | UInt16   | Uzi Ammo 1              |
| 0x020E            | UInt16   | Shotgun Ammo 1          |
| 0x0210            | UInt16   | MP5 Ammo 1              |
| 0x0212            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0214            | UInt16   | Harpoon Ammo 1          |
| 0x0216            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0218            | BYTE     | Small Medipack          |
| 0x0219            | BYTE     | Large Medipack          |
| 0x021F            | BYTE     | Weapons Config Number   |
| 0x0220            | BYTE     | Harpoon Gun             |
| 0x1415            | UInt16   | Deagle Ammo 2           |
| 0x1419            | UInt16   | Uzi Ammo 2              |
| 0x141D            | UInt16   | Shotgun Ammo 2          |
| 0x1421            | UInt16   | Harpoon Ammo 2          |
| 0x1425            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1429            | UInt16   | Grenade Launcher Ammo 2 |
| 0x142D            | UInt16   | MP5 Ammo 2              |

#### Temple of Puna ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x023D            | UInt16   | Deagle Ammo 1           |
| 0x023F            | UInt16   | Uzi Ammo 1              |
| 0x0241            | UInt16   | Shotgun Ammo 1          |
| 0x0243            | UInt16   | MP5 Ammo 1              |
| 0x0245            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0247            | UInt16   | Harpoon Ammo 1          |
| 0x0249            | UInt16   | Grenade Launcher Ammo 1 |
| 0x024B            | BYTE     | Small Medipack          |
| 0x024C            | BYTE     | Large Medipack          |
| 0x024E            | BYTE     | Flares                  |
| 0x0252            | BYTE     | Weapons Config Number   |
| 0x0253            | BYTE     | Harpoon Gun             |
| 0x10ED            | UInt16   | Deagle Ammo 2           |
| 0x10F1            | UInt16   | Uzi Ammo 2              |
| 0x10F5            | UInt16   | Shotgun Ammo 2          |
| 0x10F9            | UInt16   | Harpoon Ammo 2          |
| 0x10FD            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1101            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1105            | UInt16   | MP5 Ammo 2              |

#### Thames Wharf ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0270            | UInt16   | Deagle Ammo 1           |
| 0x0272            | UInt16   | Uzi Ammo 1              |
| 0x0274            | UInt16   | Shotgun Ammo 1          |
| 0x0276            | UInt16   | MP5 Ammo 1              |
| 0x0278            | UInt16   | Rocket Launcher Ammo 1  |
| 0x027A            | UInt16   | Harpoon Ammo 1          |
| 0x027C            | UInt16   | Grenade Launcher Ammo 1 |
| 0x027E            | BYTE     | Small Medipack          |
| 0x027F            | BYTE     | Large Medipack          |
| 0x0281            | BYTE     | Flares                  |
| 0x0285            | BYTE     | Weapons Config Number   |
| 0x0286            | BYTE     | Harpoon Gun             |
| 0x186B            | UInt16   | Deagle Ammo 2           |
| 0x186F            | UInt16   | Uzi Ammo 2              |
| 0x1873            | UInt16   | Shotgun Ammo 2          |
| 0x1877            | UInt16   | Harpoon Ammo 2          |
| 0x187B            | UInt16   | Rocket Launcher Ammo 2  |
| 0x187F            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1883            | UInt16   | MP5 Ammo 2              |

#### Aldwych ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x02A3            | UInt16   | Deagle Ammo 1           |
| 0x02A5            | UInt16   | Uzi Ammo 1              |
| 0x02A7            | UInt16   | Shotgun Ammo 1          |
| 0x02A9            | UInt16   | MP5 Ammo 1              |
| 0x02AB            | UInt16   | Rocket Launcher Ammo 1  |
| 0x02AD            | UInt16   | Harpoon Ammo 1          |
| 0x02AF            | UInt16   | Grenade Launcher Ammo 1 |
| 0x02B1            | BYTE     | Small Medipack          |
| 0x02B2            | BYTE     | Large Medipack          |
| 0x02B4            | BYTE     | Flares                  |
| 0x02B8            | BYTE     | Weapons Config Number   |
| 0x02B9            | BYTE     | Harpoon Gun             |
| 0x22F7            | UInt16   | Deagle Ammo 2           |
| 0x22FB            | UInt16   | Uzi Ammo 2              |
| 0x22FF            | UInt16   | Shotgun Ammo 2          |
| 0x2303            | UInt16   | Harpoon Ammo 2          |
| 0x2307            | UInt16   | Rocket Launcher Ammo 2  |
| 0x230B            | UInt16   | Grenade Launcher Ammo 2 |
| 0x230F            | UInt16   | MP5 Ammo 2              |

#### Lud's Gate ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x02D6            | UInt16   | Deagle Ammo 1           |
| 0x02D8            | UInt16   | Uzi Ammo 1              |
| 0x02DA            | UInt16   | Shotgun Ammo 1          |
| 0x02DC            | UInt16   | MP5 Ammo 1              |
| 0x02DE            | UInt16   | Rocket Launcher Ammo 1  |
| 0x02E0            | UInt16   | Harpoon Ammo 1          |
| 0x02E2            | UInt16   | Grenade Launcher Ammo 1 |
| 0x02E4            | BYTE     | Small Medipack          |
| 0x02E5            | BYTE     | Large Medipack          |
| 0x02E7            | BYTE     | Flares                  |
| 0x02EB            | BYTE     | Weapons Config Number   |
| 0x02EC            | BYTE     | Harpoon Gun             |
| 0x1D6F            | UInt16   | Deagle Ammo 2           |
| 0x1D73            | UInt16   | Uzi Ammo 2              |
| 0x1D77            | UInt16   | Shotgun Ammo 2          |
| 0x1D7B            | UInt16   | Harpoon Ammo 2          |
| 0x1D7F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1D83            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1D87            | UInt16   | MP5 Ammo 2              |

#### City ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0309            | UInt16   | Deagle Ammo 1           |
| 0x030B            | UInt16   | Uzi Ammo 1              |
| 0x030D            | UInt16   | Shotgun Ammo 1          |
| 0x030F            | UInt16   | MP5 Ammo 1              |
| 0x0311            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0313            | UInt16   | Harpoon Ammo 1          |
| 0x0315            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0317            | BYTE     | Small Medipack          |
| 0x0318            | BYTE     | Large Medipack          |
| 0x031A            | BYTE     | Flares                  |
| 0x031E            | BYTE     | Weapons Config Number   |
| 0x031F            | BYTE     | Harpoon Gun             |
| 0x0AEB            | UInt16   | Deagle Ammo 2           |
| 0x0AEF            | UInt16   | Uzi Ammo 2              |
| 0x0AF3            | UInt16   | Shotgun Ammo 2          |
| 0x0AF7            | UInt16   | Harpoon Ammo 2          |
| 0x0AFB            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0AFF            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1B03            | UInt16   | MP5 Ammo 2              |

#### Antarctica ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x03D5            | UInt16   | Deagle Ammo 1           |
| 0x03D7            | UInt16   | Uzi Ammo 1              |
| 0x03D9            | UInt16   | Shotgun Ammo 1          |
| 0x03DB            | UInt16   | MP5 Ammo 1              |
| 0x03DD            | UInt16   | Rocket Launcher Ammo 1  |
| 0x03DF            | UInt16   | Harpoon Ammo 1          |
| 0x03E1            | UInt16   | Grenade Launcher Ammo 1 |
| 0x03E3            | BYTE     | Small Medipack          |
| 0x03E4            | BYTE     | Large Medipack          |
| 0x03E6            | BYTE     | Flares                  |
| 0x03EA            | BYTE     | Weapons Config Number   |
| 0x03EB            | BYTE     | Harpoon Gun             |
| 0x198D            | UInt16   | Deagle Ammo 2           |
| 0x1991            | UInt16   | Uzi Ammo 2              |
| 0x1995            | UInt16   | Shotgun Ammo 2          |
| 0x1999            | UInt16   | Harpoon Ammo 2          |
| 0x199D            | UInt16   | Rocket Launcher Ammo 2  |
| 0x19A1            | UInt16   | Grenade Launcher Ammo 2 |
| 0x19A5            | UInt16   | MP5 Ammo 2              |

#### RX-Tech Mines ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0408            | UInt16   | Deagle Ammo 1           |
| 0x040A            | UInt16   | Uzi Ammo 1              |
| 0x040C            | UInt16   | Shotgun Ammo 1          |
| 0x040E            | UInt16   | MP5 Ammo 1              |
| 0x0410            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0412            | UInt16   | Harpoon Ammo 1          |
| 0x0414            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0416            | BYTE     | Small Medipack          |
| 0x0417            | BYTE     | Large Medipack          |
| 0x0419            | BYTE     | Flares                  |
| 0x041D            | BYTE     | Weapons Config Number   |
| 0x041E            | BYTE     | Harpoon Gun             |
| 0x194F            | UInt16   | Deagle Ammo 2           |
| 0x1953            | UInt16   | Uzi Ammo 2              |
| 0x1957            | UInt16   | Shotgun Ammo 2          |
| 0x195B            | UInt16   | Harpoon Ammo 2          |
| 0x195F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1963            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1967            | UInt16   | MP5 Ammo 2              |

#### Lost City of Tinnos ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x043B            | UInt16   | Deagle Ammo 1           |
| 0x043D            | UInt16   | Uzi Ammo 1              |
| 0x043F            | UInt16   | Shotgun Ammo 1          |
| 0x0441            | UInt16   | MP5 Ammo 1              |
| 0x0443            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0445            | UInt16   | Harpoon Ammo 1          |
| 0x0447            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0449            | BYTE     | Small Medipack          |
| 0x044A            | BYTE     | Large Medipack          |
| 0x044C            | BYTE     | Flares                  |
| 0x0450            | BYTE     | Weapons Config Number   |
| 0x0451            | BYTE     | Harpoon Gun             |
| 0x1D8F            | UInt16   | Deagle Ammo 2           |
| 0x1D93            | UInt16   | Uzi Ammo 2              |
| 0x1D97            | UInt16   | Shotgun Ammo 2          |
| 0x1D9B            | UInt16   | Harpoon Ammo 2          |
| 0x1D9F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1DA3            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1DA7            | UInt16   | MP5 Ammo 2              |

#### Meteorite Cavern ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x046E            | UInt16   | Deagle Ammo 1           |
| 0x0470            | UInt16   | Uzi Ammo 1              |
| 0x0472            | UInt16   | Shotgun Ammo 1          |
| 0x0474            | UInt16   | MP5 Ammo 1              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0478            | UInt16   | Harpoon Ammo 1          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x047C            | BYTE     | Small Medipack          |
| 0x047D            | BYTE     | Large Medipack          |
| 0x047F            | BYTE     | Flares                  |
| 0x0483            | BYTE     | Weapons Config Number   |
| 0x0484            | BYTE     | Harpoon Gun             |
| 0x0AE1            | UInt16   | Deagle Ammo 2           |
| 0x0AE5            | UInt16   | Uzi Ammo 2              |
| 0x0AE9            | UInt16   | Shotgun Ammo 2          |
| 0x0AED            | UInt16   | Harpoon Ammo 2          |
| 0x0AF1            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0AF5            | UInt16   | Grenade Launcher Ammo 2 |
| 0x0AF9            | UInt16   | MP5 Ammo 2              |

#### All Hallows ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x046E            | UInt16   | Deagle Ammo 1           |
| 0x0470            | UInt16   | Uzi Ammo 1              |
| 0x0472            | UInt16   | Shotgun Ammo 1          |
| 0x0474            | UInt16   | MP5 Ammo 1              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0478            | UInt16   | Harpoon Ammo 1          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0472            | UInt16   | Shotgun Ammo 2          |
| 0x0474            | UInt16   | MP5 Ammo 2              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0478            | UInt16   | Harpoon Ammo 2          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 2 |
| 0x047C            | BYTE     | Small Medipack          |
| 0x047D            | BYTE     | Large Medipack          |
| 0x047F            | BYTE     | Flares                  |
| 0x0483            | BYTE     | Weapons Config Number   |
| 0x0484            | BYTE     | Harpoon Gun             |
| 0x1025            | UInt16   | Deagle Ammo 2           |
| 0x1029            | UInt16   | Uzi Ammo 2              |
| 0x102D            | UInt16   | Shotgun Ammo 2          |
| 0x1031            | UInt16   | Harpoon Ammo 2          |
| 0x1035            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1039            | UInt16   | Grenade Launcher Ammo 2 |
| 0x103D            | UInt16   | MP5 Ammo 2              |

## Tomb Raider: The Last Revelation
| **File offset**       | **Type**         | **Variable**            |
| :---                	| :---             | :---                    |
| 0x000                 | String           | Level Name              |
| 0x04B                 | UInt16           | Save Number             |
| 0x169                 | BYTE             | Pistols                 |
| 0x16A                 | BYTE             | Uzi                     |
| 0x16B                 | BYTE             | Shotgun                 |
| 0x16C                 | BYTE             | Crossbow                |
| 0x16D                 | BYTE             | Grenade Gun             |
| 0x16E                 | BYTE             | Revolver                |
| 0x16F                 | BYTE             | LaserSight              |
| 0x170                 | BYTE             | Binoculars              |
| 0x171                 | BYTE             | Crowbar                 |
| 0x1FB                 | BYTE             | Secrets                 |
| 0x190                 | UInt16           | Small Medipack          |
| 0x192                 | UInt16           | Large Medipack          |
| 0x194                 | UInt16           | Flares                  |
| 0x198                 | UInt16           | Uzi Ammo                |
| 0x19A                 | UInt16           | Revolver Ammo           |
| 0x19C                 | UInt16           | Shotgun Normal Ammo     |
| 0x1A0                 | UInt16           | Grenade Gun Normal Ammo |
| 0x1A2                 | UInt16           | Grenade Gun Super Ammo  |
| 0x1A4                 | UInt16           | Grenade Gun Flash Ammo  |
| 0x1A6                 | UInt16           | Crossbow Normal Ammo    |
| 0x1A8                 | UInt16           | Crossbow Poison Ammo    |
| 0x1AA                 | UInt16           | Crossbow Explosive Ammo |

## Tomb Raider: Chronicles
| **File offset**       | **Type**         | **Variable**          |
| :---                	| :---             | :---                  |
| 0x000     	          | String      		 | Level Name            |
| 0x04B                 | UInt16           | Save Number           |
| 0x16F     	          | BYTE        		 | Pistols               |
| 0x170     	          | BYTE  		       | Uzi                   |
| 0x171     	          | BYTE  		       | Shotgun               |
| 0x172     	          | BYTE  		       | Grappling Gun         |
| 0x173     	          | BYTE  		       | HK Gun                |
| 0x174     	          | BYTE  		       | Revolver/Deagle       |
| 0x175     	          | BYTE  		       | LaserSight            |
| 0x177     	          | BYTE  		       | Binoculars/Headset    |
| 0x178     	          | BYTE  		       | Crowbar               |
| 0x194     	          | UInt16  		     | Small Medipack        |
| 0x196     	          | UInt16  		     | Large Medipack        |
| 0x198     	          | UInt16  		     | Flares                |
| 0x19C     	          | UInt16  		     | Uzi Ammo              |
| 0x19E     	          | UInt16  		     | Revolver/Deagle Ammo  |
| 0x1A0     	          | UInt16  		     | Shotgun Normal Ammo   |
| 0x1A2     	          | UInt16  		     | Shotgun Wideshot Ammo |
| 0x1A4     	          | UInt16  		     | HK Ammo               |
| 0x1A6     	          | UInt16  		     | Grappling Gun Ammo    |
| 0x1C3     	          | BYTE    		     | Secrets               |
