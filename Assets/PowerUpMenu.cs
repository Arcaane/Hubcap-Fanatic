using System;
using UnityEngine;

public class PowerUpMenu : MonoBehaviour
{
    public MacroShopItem[] items;
    [SerializeField] public Material headLightMat;
    [SerializeField] public Material baseheadLightMat;

    private void Start()
    {
        for (int i = 0; i < items.Length; i++) {
            items[i].currentLevel = GameMaster.instance.UnlockedPowerUps[i];
        }
    }
}

[Serializable]
public class MacroShopItem
{
    // --- 
    [Header("InShopSetup")]
    public string name;
    [TextArea] public string description;
    public Sprite icon;
    public int[] price;
    public int currentLevel;
    public MenuItem item;

    [Header("VisualSetup")] 
    public Vector3 toPos;
    public Quaternion toRot;
    public Vector3 socleToRot;
    public GameObject[] objectsToActivateDuringFocus;
}

public enum MenuItem
{
    Might,
    Recovery,
    MoveSpeed
};
