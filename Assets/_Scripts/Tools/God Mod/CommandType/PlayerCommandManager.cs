using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommandManager : CommandManager
{
    public PlayerCommandManager(CommandConsoleRuntime commandConsole) : base(commandConsole) {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        DownLevelCmd();
        NextLevelCmd();
        IncreaseLevelAboutCmd();
        GODModeCmd();
    }

    #region Command Player
    //TODO : Increase Statistic of Players
    //TODO : Add a new ability to the player
    //TODO : Upgrade a ability to the player
    //TODO : GOD Mode
    
    /// <summary>
    /// Command to increase the level of the player
    /// </summary>
    private void IncreaseLevelAboutCmd()
    {
        CommandConsole INCREASE_LEVEL = new CommandConsole("increaseLevel", "increaseLevel <int> : Increase the level of the player", 
            new List<CommandClass>() { new CommandClass(typeof(int)) }, 
            (value) => { 
                if (int.TryParse(value[0], out int level)) {
                    CarExperienceManager.Instance.playerLevel += level;
                } else {
                    Debug.LogError("Invalid level provided.");
                }
            });

        AddCommand(INCREASE_LEVEL);
    }
    
    /// <summary>
    /// Command to go to the next level
    /// </summary>
    private void NextLevelCmd()
    {
        CommandConsole NEXT_LEVEL = new CommandConsole("nextLevel", "nextLevel : Go to the next level", 
            new List<CommandClass>(), 
            (value) => { 
                CarExperienceManager.Instance.playerLevel += 1;
            });

        AddCommand(NEXT_LEVEL);
    }
    
    /// <summary>
    /// Command to go to the previous level
    /// </summary>
    private void DownLevelCmd()
    {
        CommandConsole DOWN_LEVEL = new CommandConsole("downLevel", "downLevel : Go to the previous level", 
            new List<CommandClass>(), 
            (value) => {
                if (CarExperienceManager.Instance.playerLevel > 0)
                {
                    CarExperienceManager.Instance.playerLevel -= 1;
                }
                else
                {
                    Debug.LogError("You are already at the lowest level.");
                }
            });

        AddCommand(DOWN_LEVEL);
    }

    /// <summary>
    /// Command to enable GOD mode
    /// </summary>
    //TODO : GOD Mode Turn to a parameter
    private void GODModeCmd()
    {
        CommandConsole GOD_MODE_CMD = new CommandConsole("godMode", "godMode <state> : Enables or disables god mode",
            new List<CommandClass>() { new CommandClass(typeof(BooleanState)) },
            (value) => {
                BooleanState mode = (BooleanState)Enum.Parse(typeof(BooleanState), value[0], true);
            });
        CommandConsoleRuntime.Instance.AddCommand(GOD_MODE_CMD);
    }
    #endregion
}
