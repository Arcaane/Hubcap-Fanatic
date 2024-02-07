using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelectMenu : MonoBehaviour
{
    public SelectableCar[] cars;
    public GameObject[] carObjects;
    public TMP_Text[] locked, weaponName, weaponDesc,goldTexts;
    public TMP_Text nameText;
    public Image[] weaponImgs, nameImages;
<<<<<<< Updated upstream
    public GameObject startButton;
=======
    public GameObject startButton, unlockButton, garageSectionAnnouncer, powerUpSectionAnnouncer;
>>>>>>> Stashed changes
    public Color yellow, green, grey;
    
    public bool transition;

    public int index;
    
    public Animation anim;
    public Vector2 stickValue;
    public bool stickUsed;
    
    public Animation camAnim;
    public bool titleScreen;

    private void Start()
    {
        goldTexts[0].text = goldTexts[1].text = GameMaster.instance.PlayerGold.ToString();
    }

    public void LStick(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            stickValue = context.ReadValue<Vector2>();
            if (!stickUsed && stickValue.magnitude > 0.5f)
            {
                stickUsed = true;
                float angle = Vector2.SignedAngle(Vector2.up, stickValue);
                
                if (angle > 30 && angle < 150)
                {
                    LeftChoice();
                }
                else if (angle < -30 && angle > -150)
                {
                    RightChoice();
                }
                
            } 
            
        }
        else
        {
            stickValue = Vector2.zero;
            stickUsed = false;
        }
    }

    public async void AButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (titleScreen && !transition)
            {
                camAnim.Play("ToSelectMenu");
                anim.Play("SelectCanvasOn");
                transition = true;
                await Task.Delay(1000);
                titleScreen = false;
                transition = false;
                powerUpSectionAnnouncer.SetActive(true);
            }
            else if(!transition)
            {
                if (index == 0)
                {
                    StartGame();
                }
            }
        }
    }

<<<<<<< Updated upstream

    public async void StartGame()
=======
    public bool isInGarageSection, isInPowerUpSection;
    public async void LBButton(InputAction.CallbackContext context)
    {
        if (context.started && !transition)
        {
            if (isInGarageSection)
            {
                transition = true;
                garageSectionAnnouncer.SetActive(false);
                // Lancer les anims 
                // await le temps des anims
                powerUpSectionAnnouncer.SetActive(true);
            }
        }
    }
    
    public async void RBButton(InputAction.CallbackContext context)
    {
        if (context.started && !transition)
        {
            if (isInPowerUpSection)
            {
                transition = true;
                powerUpSectionAnnouncer.SetActive(false);
                // Lancer les anims 
                // await le temps des anims
                garageSectionAnnouncer.SetActive(true);
            }
        }
    }
    
    public async void StartGame(int index)
>>>>>>> Stashed changes
    {
        Debug.Log("StartGame");
        anim.Play("FadeToBlack");
        transition = true;
        await Task.Delay(600);
        SceneManager.LoadScene(1);
    }
    
    public void LeftChoice()
    {
        if (transition) return;
        anim.Play("LeftChoice");
        transition = true;
    }
    public void RightChoice()
    {
        if (transition) return;
        anim.Play("RightChoice");
        transition = true;
    }

    public void SetNextChoiceLeft()
    {
        
        index--;
        if (index < 0) index = cars.Length - 1;
        
        SetCarSelectable();
    }
    public void SetNextChoiceRight()
    {
        
        index = (index + 1) % cars.Length;
        
        SetCarSelectable();
    }

    public void SetCarSelectable()
    {
        for (int i = 0; i < carObjects.Length; i++)
        {
            if(i == index) carObjects[i].SetActive(true);
            else carObjects[i].SetActive(false);
        }

        if (cars[index].unlocked)
        {
            locked[0].text = locked[1].text = "Unlocked";
            locked[0].color = yellow;
            for (int i = 0; i < nameImages.Length; i++)
            {
                nameImages[i].color = yellow;
            }
            startButton.SetActive(true);
        }
        else
        {
            locked[0].text = locked[1].text = "Locked";
            locked[0].color = grey;
            for (int i = 0; i < nameImages.Length; i++)
            {
                nameImages[i].color = grey;
            }
            startButton.SetActive(false);
        }
        
        nameText.text = cars[index].name;
        weaponName[0].text = weaponName[1].text = cars[index].weapon;
        weaponDesc[0].text = weaponDesc[1].text = cars[index].description;
        weaponImgs[0].sprite = weaponImgs[1].sprite = cars[index].weaponSprite;
        
    }

    public void ResetTransition()
    {
        transition = false;
    }
}

[Serializable]
public struct SelectableCar
{
    public string name;
    public bool unlocked;
    public string weapon;
    [TextArea]public string description;
    public Sprite weaponSprite;
}
