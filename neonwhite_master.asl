state("Neon White") {
    long levelPlaythroughMicroseconds : "UnityPlayer.dll", 0x199CDC0, 0x18, 0x40, 0x28, 0x48, 0x20;
    long levelRushMicroseconds : "UnityPlayer.dll", 0x1930010, 0x10, 0xD0, 0x8, 0x60, 0x50, 0x0, 0x1C0, 0x10, 0x20;
    string255 levelId : "UnityPlayer.dll", 0x199CDC0, 0x18, 0x40, 0x28, 0x30, 0x20, 0x14;
    string255 levelScene : "UnityPlayer.dll", 0x1A058E0, 0x48, 0x10, 0x18;
}

startup {
    vars.LEVEL_RUSH_MENU_SCENE = "nu.unity";
    vars.FIRST_LEVEL_IDS = new string[1]{
        "STAGE_NAME", //Shuffle start
    };
    vars.LevelIdToScene = (Func<string, string>)((string levelId) => {
        return "id/" + levelId + ".unity";
    });
    vars.IsFirstLevelId = (Func<string, bool>)((string levelId) => {
        foreach (string firstLevelId in vars.FIRST_LEVEL_IDS) {
            if (firstLevelId == levelId) {
                return true;
            }
        }
        return false;
    });
    vars.IsFirstLevelScene = (Func<string, bool>)((string levelScene) => {
        foreach (string firstLevelId in vars.FIRST_LEVEL_IDS) {
            string firstLevelScene = vars.LevelIdToScene(firstLevelId);
            if (firstLevelScene == levelScene) {
                return true;
            }
        }
        return false;
    });

    vars.includeCurrentLevel = true;  // flag which prevents doubling the time of the final rush level
}

update {
    // level ID can randomly be set to a null string for a single frame, causing false splits 
    if (string.IsNullOrEmpty(current.levelId)) {
        current.levelId = old.levelId;
    }

    // levelRushMicroseconds is set to 0 when loading; suppress this for a clean timer,
    // unless levelRushMicroseconds is actually zero. (i.e. we are on the first level)
    if (current.levelRushMicroseconds == 0 && !vars.IsFirstLevelScene(current.levelScene)) {
        current.levelRushMicroseconds = old.levelRushMicroseconds;
    }

    // levelRushMicroseconds is incremented by levelPlaythroughMicroseconds every level;
    // if levelPlaythroughMicroseconds hasn't reset yet, suppress this change.
    if (
        current.levelRushMicroseconds > old.levelRushMicroseconds &&
        current.levelPlaythroughMicroseconds > 0
    ) {
        vars.includeCurrentLevel = false;
    }

    // if we have started a new LevelPlaythrough then include the current level's timer
    if (current.levelPlaythroughMicroseconds == -1 && !vars.includeCurrentLevel) {
        vars.includeCurrentLevel = true;
    }

    if (!vars.includeCurrentLevel) {
        current.levelPlaythroughMicroseconds = 0;
    }
}

isLoading {
    return true;
}

gameTime {
    long totalMicroseconds = current.levelRushMicroseconds + current.levelPlaythroughMicroseconds;
    long totalMilliseconds = totalMicroseconds / 1000;
    return TimeSpan.FromMilliseconds(totalMilliseconds);
}

split {
    return (
        (  // normal split
            old.levelId != current.levelId &&
            !vars.IsFirstLevelId(current.levelId)  // suppress split when restarting a Level Rush
        ) ||  // or, split when returning to the Level Rush menu
        (old.levelScene != current.levelScene && current.levelScene == vars.LEVEL_RUSH_MENU_SCENE)
    );
}

// levelId does not update when exiting to the main menu, so use levelScene to detect when to start
// or reset the timer
start {
    return old.levelScene != current.levelScene && vars.IsFirstLevelScene(current.levelScene);
}

reset {
    return old.levelScene != current.levelScene && vars.IsFirstLevelScene(current.levelScene);
}