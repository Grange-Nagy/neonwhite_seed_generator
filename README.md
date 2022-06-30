## Neon White Level Rush Shuffle Seed Generator
---
Allows for the brute forcing of seeds for the level rush shuffle.
To modify the seed change "seedForLevelRushLevelOrder_NegativeValuesMeansRandomizeSeed" in powerprefs.txt in the game local data directory which should located at:
    
    C:\Users\<username>\AppData\LocalLow\Little Flag Software, LLC\Neon White\<some number>\powerprefs.txt

### Requirements
A somewhat recent C# compiler,
Python 3

### Compilation

`csc /optimize .\find_seed.cs`

### Finding a seed
Run `find_seed.exe` with no arguments for help info.

Caveats:
For the 8 level rushes, this programm only supports outputs of the level indexes and not the level names. -g will not work.
There are only 2.14e+9 seeds but over 1e+159 permutations of levels so you will not be able to get exactly what you want.
This may take up to 90 minutes to search the entire space, it is not very efficient.
If you are struggling to find a seed that fits your parameters, try running the program with a deeper allowed level depth and relaxing the order matters requirement.
TODO: Add a calculator for the odds of finding a seed for a given set of constraints.
For trying to get the first 7 levels to contain a set of 7 given levels with any order, there is a 43% chance of finding a seed.
For trying to get the first 8 levels to contain a set of 7 given levels with any order, there is a 99% chance of finding a seed.


### Updating splits
Only works for White/Mikey's level rush.
!!WARNING!!: This is programmed lazily and loses some precision.
Also make a backup of your splits.

If you have split times you want to carry over:
1. Run `find_seed.exe -g <seed>` with the seed you want to use. Check resulting new_order.txt.
2. Open your neonwhite splits in livesplit
3. Rightclick livesplit -> Edit Splits...
4. If your split names are subsplit formatted e.g. -01 Movement or have anything other than the map name
    4.1. Open clean_levelnames.txt in this directory, select all (Ctrl+A), and copy to clipboard
    4.2. Select the first split name cell in the Game Time table (for a normal order this would be "-01 Movement") and paste from clipboard
4. Select a cell in the Game Time table
5. Select all (Ctrl+A), and copy to clipboard
6. Open splitinfo.txt in this directory, select all (Ctrl+A), and paste clipboard
7. Run `python3 split_fixer.py`
8. Open splitinfo_shuffle.txt in this directory, select all (Ctrl+A), and copy to clipboard
9. Rightclick livesplit -> Edit Splits...
10. Select the first split name cell in the Game Time table (for a normal order this would be "Movement") and paste clipboard
11. Hit OK and rightclick livesplit -> Save Splits As.

If you do not have split times you want to carry over:
1. Run `find_seed.exe -g <seed>` with the seed you want to use. Check resulting new_order.txt.
2. Open splitinfo.txt in this directory and check that it only contains the map names.
3. Run `python3 split_fixer.py`
4. Open splitinfo_shuffle.txt in this directory, select all (Ctrl+A), and copy to clipboard
5. Rightclick livesplit -> Edit Splits...
6. Select the first split name cell in the Game Time table and paste clipboard
7. Hit OK and rightclick livesplit -> Save Splits As.

### Updating autosplitter
Only works for White/Mikey's level rush.

Do this after running `python3 split_fixer.py`:
1. Open your neonwhite splits in livesplit
2. Rightclick livesplit -> Edit Splits...
3. Deactivate the normal autosplitter if you have it on
4. Rightclick livesplit -> Edit Layout...
5. Add element Control -> Scriptable Auto Splitter
6. Open the settings for the scriptable autosplitter and change the script path to the generated neonwhite_shuffle.asl in this directory.
7. Save layout and restart livesplit


