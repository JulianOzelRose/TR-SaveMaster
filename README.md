# Tomb Raider - Savegame Editor
This is a fully-featured savegame editor for the classic Tomb Raider series, compatible with Tomb Raider 1-5, including the expansion packs and bonus levels.
It is designed to function seamlessly with both the Steam versions and the CD/multi-patched versions of the games. For installation and usage instructions,
please scroll down to the next section of this README. Additionally, technical details on reverse engineering the Tomb Raider classic series and offset tables are included.

![TR-SaveMaster-UI](https://github.com/JulianOzelRose/TR-SaveMaster/assets/95890436/d95b20d0-ea26-431f-ab39-74c9166d791b)

# Installation and use
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


