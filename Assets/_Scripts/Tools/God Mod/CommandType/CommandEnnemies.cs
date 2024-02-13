using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagerNameSpace;
using UnityEngine;

public class EnemyCommandManager : CommandManager {
    public EnemyCommandManager(CommandConsoleRuntime commandConsole) : base(commandConsole) {
        InitializeCommands();
    }

    private void InitializeCommands() {
        SpawnEnemiesCmd();
        ClearEnemiesCmd();
    }


    #region Command Enemies
    /// <summary>
    /// Command to spawn enemies
    /// spawnEnemies <TYPE> <int> : Spawns the specified number of enemies
    /// </summary>
    private void SpawnEnemiesCmd()
    {
        CommandConsole SPAWN_ENEMIES = new CommandConsole("spawnEnemies", "spawnEnemies <TYPE> <int> : Spawns the specified number of enemies", 
            new List<CommandClass>() { new CommandClass(typeof(Key)), new CommandClass(typeof(int)) }, 
            (value) => { 
                if (Enum.TryParse(value[0], out Key entityType)) {
                    if (int.TryParse(value[1], out int numberOfEnemies)) {
                        WaveManager.instance.SpawnNewEntity(entityType, numberOfEnemies);
                    } else {
                        Debug.LogError("Invalid number of enemies provided.");
                    }
                } else {
                    Debug.LogError("Invalid entity type provided.");
                }
            });

        AddCommand(SPAWN_ENEMIES);
    }

    /// <summary>
    /// Command to clear enemies
    /// clearEnemies : Clears all enemies from the scene
    /// </summary>
    private void ClearEnemiesCmd()
    {
        Debug.Log("ClearEnnemiesCmd() method called.");
        CommandConsole CLEAR_ENEMIES = new CommandConsole("clearEnemies", "clearEnemies : Clears all enemies from the scene", 
            new List<CommandClass>(), 
            (value) => { 
                ClearEnemies();
            });
        AddCommand(CLEAR_ENEMIES);
    }

    

    #endregion

    private void ClearEnemies()
    {
        if (PoliceCarBehavior.policeCars.Count <= 0) return;
        int i = 0;
        while (i < PoliceCarBehavior.policeCars.Count)
        {
            var pc = PoliceCarBehavior.policeCars[i];
            pc.gameObject.SetActive(false);
            PoliceCarBehavior.policeCars.RemoveAt(i);
        }
    }
}
