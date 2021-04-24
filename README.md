# MegamixMapConverter
A tool for converting vgmap layouts of Mega Man 1-6, 9, and 10 to Megamix-style rooms, utilizing pre-existing tilesets. Particularly, those by Revned.

Features:
* Creates a room in GMS containing all tiles from the level, using a base tileset in megamix rather than needing to auto-generate one from the map itself.
  * Even if it's the wrong palette, the tool will conform the level to the tileset's palette in the creation process.
* Places an auto-tiler ready to be used to set collision in the level (sometimes buggy if the first 'screen' isn't a black void on the image)
* Sets all screen boundaries and vertical scrolling borders (horizontal scrolling with stops not set due to having no easy way to determine).
* Moves defaultspawn to the proper spawn point.
* Grants a 'score' for what percentage of tiles the generator was able to properly identify on the map (A sorted score for 1.0 is at the root of this project).
* 8x8 tile mode.
* Auto-inserts music from most stages into the room CC
* Auto-sets NES and Wii Modes (Based on the image height/file name)

Arguments:
* -i (--individualMode): Choose a specific file in maps to create a room from. Default will create *all* rooms not already in the rooms folder.
* -e (--exactTileCheck): Do not estimate colors when checking the simplified map to the tileset.
* -o (--overwriteExisting): Overwrite existing rooms even if they already exist.
* -n (--namesExact): Disable name estimation used to match vgmap names with megamix tileset names. Default is on.
* -s (--singleThread): Disables multithreading when individual mode is not on. Prevents high CPU usage, but will take much longer.
* -m (--skipMega): Skips searching for Mega Man in the room and keeps the original spawn of lvlCopyThisRoom.
* -b (--skipBorders): Skips auto-creating borders based on the white edges provided by vgmaps.
* -h (--halfTiles): Treats tilesets as 8x8 rather than 16x16 (except for border tiles). Can make some maps more accurate, but slower and requires a modified auto-tiler for auto-tiling to work without causing a lot of lag. Highly experimental and increases file size almost twofold.

Building:
Compile like any VS project, then paste the contents of the resources folder into the root folder of the output.

Project should be compatibile with Mono, but is untested.
