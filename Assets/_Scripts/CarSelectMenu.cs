using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelectMenu : MonoBehaviour
{
    public SelectableCar[] cars;
    public GameObject[] carObjects;
    public TMP_Text[] locked, weaponName, weaponDesc, goldTexts;
    public TMP_Text nameText;
    public Image[] weaponImgs, nameImages;
    public GameObject startButton, unlockButton, garageSectionAnnouncer, powerUpSectionAnnouncer;
    public Color yellow, green, grey;

    public bool transition;

    public int index;
    private int carIndex;
    
    public Animation anim;
    public Vector2 stickValue;
    public bool stickUsed;

    public Animation camAnim;
    public bool titleScreen;

    private Camera cam;
    [SerializeField] private CanvasGroup garageCanvasGroup, powerUpCanvasGroup;
    [SerializeField] private RotatePlatform platform;
    
    public PowerUpMenu powersUps;

    public List<Material> lightsMat = new();
    public List<MeshRenderer> carsMeshRenderers = new();

    private void Start()
    {
        UpdateGold();

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].unlocked = GameMaster.instance.UnlockedCars[i];
        }

        garageSectionAnnouncer.SetActive(false);
        powerUpSectionAnnouncer.SetActive(false);

        cam = Camera.main;

        unlockButtonImage = unlockButton.transform.GetChild(1).GetComponent<Image>();
        
        carsMeshRenderers.Add(carObjects[0].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>());
        //carsMeshRenderers.Add(carObjects[2].GetComponent<MeshRenderer>());
        //carsMeshRenderers.Add(carObjects[1].transform.GetChild(0).GetChild(7).GetComponent<MeshRenderer>());
    }

    private void UpdateGold() => goldTexts[0].text = goldTexts[1].text = GameMaster.instance.PlayerGold.ToString();

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
                isInGarageSection = true;
            }
            else if (!transition && isInGarageSection)
            {
                if (cars[index].unlocked)
                {
                    StartGame(index);
                }
            }

            if (!transition && !titleScreen && isInGarageSection)
            {
                UnlockCar();
            }

            if (!transition && !titleScreen && isInPowerUpSection)
            {
                BuyPowerUp();
            }
        }
    }

    public async void BButton(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !transition && titleScreen == false)
        {
            camAnim.Play("ToMainMenu");
            anim.Play("SelectCanvasOff");
            transition = true;
            garageCanvasGroup.DOFade(0, 0.15f);
            await Task.Delay(1000);
            transition = false;
            titleScreen = true;
            isInPowerUpSection = false;
            isInGarageSection = false;
            powerUpSectionAnnouncer.SetActive(false);
            garageSectionAnnouncer.SetActive(false);
        }
    }
    
    public bool isInGarageSection, isInPowerUpSection;

    public async void LBButton(InputAction.CallbackContext context)
    {
        if (titleScreen) return;
        if (context.started && !transition && isInGarageSection)
        {
            anim.Play("ToPowerUpMenu");
;           carIndex = index;
            transition = true;
            garageSectionAnnouncer.SetActive(false);
            powerUpSectionAnnouncer.SetActive(false);
            isInGarageSection = false;
            cam.transform.DOMove(new Vector3(-6.38f, 28.662f, 19.275f), 1f).SetEase(Ease.OutCubic);
            cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(45.586f, 72.032f, 0.71f)), 1f).SetEase(Ease.OutCubic);
            garageCanvasGroup.DOFade(0, 0.185f);
            platform.RotateForward();
            await Task.Delay(1000);
            index = 0;
            SetPowerUpSelectable();
            garageCanvasGroup.DOFade(1, 0.225f);
            isInPowerUpSection = true;
            garageSectionAnnouncer.SetActive(true);
            transition = false;
        }
    }

    public async void RBButton(InputAction.CallbackContext context)
    {
        if (titleScreen) return;

        if (context.started && !transition && isInPowerUpSection)
        {
            transition = true;
            anim.Play("ToGarageMenu");
            garageSectionAnnouncer.SetActive(false);
            powerUpSectionAnnouncer.SetActive(false);
            isInPowerUpSection = false;
            cam.transform.DOMove(new Vector3(-2.74f, 26.77f, 14.74f), 1f).SetEase(Ease.OutCubic);
            cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(31f, 28.619f, 0f)), 1f).SetEase(Ease.OutCubic);
            garageCanvasGroup.DOFade(0, 0.225f);
            platform.isRotating = true;
            await Task.Delay(1000);
            index = carIndex;
            SetCarSelectable();
            garageCanvasGroup.DOFade(1, 0.225f);
            isInGarageSection = true;
            powerUpSectionAnnouncer.SetActive(true);
            transition = false;
        }
    }

    private async void StartGame(int index)
    {
        Debug.Log("StartGame");
        anim.Play("FadeToBlack");
        transition = true;
        await Task.Delay(600);
        SceneManager.LoadScene(1);
    }

    public RectTransform parentMask;
    private void LeftChoice()
    {
        if (transition) return;
        if (isInGarageSection)
        {
            anim.Play("LeftChoice");
            transition = true;
        }

        if (isInPowerUpSection)
        {
            transition = true;
            parentMask.DOAnchorPosX(-128.1f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                parentMask.gameObject.SetActive(false);
                parentMask.DOAnchorPosX(129.2f, 0.02f).OnComplete(() =>
                {
                    parentMask.DOAnchorPosX(2.313995f, 0.3f).SetEase(Ease.OutCubic);
                    parentMask.gameObject.SetActive(true);
                });
                SetNextChoiceLeft();
            });

            parentMask.DOScaleX(0.05f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                parentMask.DOScaleX(0.16166f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => transition = false);
            });
        }
    }

    private void RightChoice()
    {
        if (transition) return;
        if (isInGarageSection)
        {
            anim.Play("RightChoice");
            transition = true;
        }
        
        if (isInPowerUpSection)
        {
            transition = true;
            parentMask.DOAnchorPosX(129.2f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                parentMask.gameObject.SetActive(false);
                parentMask.DOAnchorPosX(-128.1f, 0.02f).OnComplete(() =>
                {
                    parentMask.DOAnchorPosX(2.313995f, 0.3f).SetEase(Ease.OutCubic);
                    parentMask.gameObject.SetActive(true);
                });
                SetNextChoiceRight();
            });

            parentMask.DOScaleX(0.05f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                parentMask.DOScaleX(0.16166f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => transition = false);
            });
        }
    }

    public void SetNextChoiceLeft()
    {
        index--;
        
        if (isInGarageSection)
        {
            if (index < 0) index = cars.Length - 1;
            SetCarSelectable();
        }
        else if (isInPowerUpSection)
        {
            if (index < 0) index = powersUps.items.Length - 1;
            SetPowerUpSelectable();
        }
    }
    
    public void SetNextChoiceRight()
    {
        if (isInGarageSection)
        {
            index = (index + 1) % cars.Length;
            SetCarSelectable();
        }
        else if (isInPowerUpSection)
        {
            index = (index + 1) % powersUps.items.Length;
            SetPowerUpSelectable();
        }
    }

    public void SetCarSelectable()
    {
        for (int i = 0; i < carObjects.Length; i++)
        {
            if (i == index) carObjects[i].SetActive(true);
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
            unlockButton.SetActive(false);
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
            unlockButton.SetActive(true);
            unlockButtonImage.color = GameMaster.instance.PlayerGold > cars[index].carPrice ? yellow : grey;
        }

        nameText.text = cars[index].name;
        weaponName[0].text = weaponName[1].text = cars[index].weapon;
        weaponDesc[0].text = weaponDesc[1].text = cars[index].description;
        weaponImgs[0].sprite = weaponImgs[1].sprite = cars[index].weaponSprite;
    }

    private void UnlockCar()
    {
        if (cars[index].unlocked) return;

        if (cars[index].carPrice > GameMaster.instance.PlayerGold)
        {
            // Pas assez d'argent
            // Son pas achetable
        }
        else
        {
            // Acheter voiture
            // Son voiture acheté
            GameMaster.instance.SubtractGold(cars[index].carPrice);
            cars[index].unlocked = true;
            GameMaster.instance.UnlockCar(index);
            SetCarSelectable();
            UpdateGold();
            GameMaster.instance.Save();
        }
    }

    public void ResetTransition()
    {
        transition = false;
    }
    
    private async void ToPosAndRot(Vector3 finalPos, Quaternion finalRot, Vector3 socleRot, bool stopRotation = false)
    {
        transition = true;
        cam.transform.DOMove(finalPos, 1f).SetEase(Ease.OutCubic);
        cam.transform.DOLocalRotateQuaternion(finalRot, 1f).SetEase(Ease.OutCubic);
        platform.isRotating = !stopRotation;
        platform.ToRotate(socleRot);
        await Task.Delay(1000);
        transition = false;
    }

    #region PowerUps

    private void BuyPowerUp()
    {
        if (powersUps.items[index].currentLevel == powersUps.items[index].price.Length) return;
        
        if (powersUps.items[index].price[powersUps.items[index].currentLevel] > GameMaster.instance.PlayerGold)
        {
            // Pas assez d'argent
            // Son pas achetable
        }
        else
        {
            GameMaster.instance.SubtractGold(powersUps.items[index].price[powersUps.items[index].currentLevel]);
            UpdateGold();
            powersUps.items[index].currentLevel++;
            GameMaster.instance.UnlockPUp(index);
            // Save Update
            SetPowerUpSelectable();
        }
    }

    private Image unlockButtonImage;
    
    private void SetPowerUpSelectable()
    {
        ToPosAndRot(powersUps.items[index].toPos, powersUps.items[index].toRot, powersUps.items[index].socleToRot, true);
        
        for (int i = 0; i < powersUps.items.Length; i++)
        {
            for (int j = 0; j < powersUps.items[i].objectsToActivateDuringFocus.Length; j++)
            {
                powersUps.items[i].objectsToActivateDuringFocus[j].SetActive(false);
            }
        }

        var m1 = carsMeshRenderers[0].materials;
        m1[5] = powersUps.baseheadLightMat;
        carsMeshRenderers[0].materials = m1;
        
        nameText.text = powersUps.items[index].name;
        weaponName[0].text = weaponName[1].text = powersUps.items[index].name;
        weaponDesc[0].text = weaponDesc[1].text = powersUps.items[index].description;
        weaponImgs[0].sprite = weaponImgs[1].sprite = powersUps.items[index].icon;
        
        locked[0].color = powersUps.items[index].currentLevel == powersUps.items[index].price.Length ? green : yellow;
        locked[0].text = locked[1].text = "Level   " + powersUps.items[index].currentLevel + " / " + powersUps.items[index].price.Length;
        
        startButton.SetActive(false);
        
        if (powersUps.items[index].currentLevel == powersUps.items[index].price.Length)
        {
            unlockButtonImage.color = grey;
            unlockButton.SetActive(false);
        }
        else
        {
            unlockButtonImage.color = GameMaster.instance.PlayerGold > powersUps.items[index].price[powersUps.items[index].currentLevel] ? green : grey;
            unlockButton.SetActive(true);
        }
        
        if (powersUps.items[index].item == MenuItem.Might)
        {
            var matsCar1 = carsMeshRenderers[0].materials;
            matsCar1[5] = powersUps.headLightMat;
            
            carsMeshRenderers[0].materials = matsCar1;
        }
        
        foreach (var t in powersUps.items[index].objectsToActivateDuringFocus) {
            t.SetActive(true);
        }
    }
    
    #endregion
}

[Serializable]
public struct SelectableCar
{
    public string name;
    public bool unlocked;
    public string weapon;
    public int carPrice;
    [TextArea] public string description;
    public Sprite weaponSprite;
}