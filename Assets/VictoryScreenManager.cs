using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenManager : MonoBehaviour
{
    public TMP_Text label;
    public Image labelBG;
    public TMP_Text[] texts;
    public bool victory;
    public int waves;
    public Color red, yellow;
    public Animation anim;

    public void Start()
    {
        waves = MemoryForVictoryScreen.instance.waveCount;
        victory = MemoryForVictoryScreen.instance.victory;
        if (victory)
        {
            label.text = "VICTORY !";
            labelBG.color = yellow;
            texts[0].text = texts[1].text = "You survived all 15 waves !";
        }
        else
        {
            label.text = "YOU DIED...";
            labelBG.color = red;
            texts[0].text = texts[1].text = "You survived for " + waves + " waves.";
        }
    }

    public async void LoadMenu()
    {
        anim.Play("FTB");        
        await Task.Delay(600);
        SceneManager.LoadScene(0);
    }
}
