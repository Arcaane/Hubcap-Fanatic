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
        SkipWaveCmd();
        NavigateToWaveCmd();
        DisableObjectsWithLayerCmd("Environnment");
        PaulCmd();
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
    
    /// <summary>
    /// Command to skip wave
    /// skipWave : Skips the current wave
    /// </summary>
    private void SkipWaveCmd()
    {
        CommandConsole SKIP_WAVE = new CommandConsole("skipWave", "skipWave : Skips the current wave", 
            new List<CommandClass>(), 
            (value) => { 
                WaveManager.instance.SkipWave();
            });
        AddCommand(SKIP_WAVE);
    }

    /// <summary waveNumber=": Navigates to the specified wave number">
    /// Command to navigate to wave
    /// </summary>
    private void NavigateToWaveCmd()
    {
        CommandConsole NAVIGATE_TO_WAVE = new CommandConsole("navigateToWave", "navigateToWave <waveNumber> : Navigates to the specified wave number", 
            new List<CommandClass>() { new CommandClass(typeof(int)) }, 
            (value) => { 
                if (int.TryParse(value[0], out int waveNumber)) {
                    WaveManager.instance.NavigateToWave(waveNumber);
                } else {
                    Debug.LogError("Invalid wave number provided.");
                }
            });

        AddCommand(NAVIGATE_TO_WAVE);
    }

    /// <summary LayerName=": Disables all objects with the specified layer">
    /// Command to disable objects with layer
    /// </summary>
    /// <param name="layerName"></param>
    private void DisableObjectsWithLayerCmd(string layerName)
    {
        CommandConsole DISABLE_OBJECTS_WITH_LAYER = new CommandConsole("disableObjectsWithLayer", "disableObjectsWithLayer <LayerName> : Disables all objects with the specified layer", 
            new List<CommandClass>() { new CommandClass(typeof(string)) }, 
            (value) => { 
                string layerName = value[0];
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.layer == 8)
                    {
                        obj.SetActive(false);
                    }
                }
            });

        AddCommand(DISABLE_OBJECTS_WITH_LAYER);
    }
    
    
    
    /// <summary>
    /// Command to terminate the game
    /// PAUL : Terminates the game
    /// </summary>
    private void PaulCmd()
    {
        CommandConsole ALT_F4 = new CommandConsole("PAUL", "PAUL : l'un des progs les plus forts de la terre, il peut tout faire", 
            new List<CommandClass>(), 
            (value) => { 
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            });

        AddCommand(ALT_F4);
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
