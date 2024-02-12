using System;
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
            carIndex = index;
            transition = true;
            garageSectionAnnouncer.SetActive(false);
            powerUpSectionAnnouncer.SetActive(false);
            isInGarageSection = false;
            cam.transform.DOMove(new Vector3(-6.38f, 28.662f, 19.275f), 1f).SetEase(Ease.OutCubic);
            cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(45.586f, 72.032f, 0.71f)), 1f)
                .SetEase(Ease.OutCubic);
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
            garageSectionAnnouncer.SetActive(false);
            powerUpSectionAnnouncer.SetActive(false);
            isInPowerUpSection = false;
            // Lancer les anims 
            // await le temps des anims
            cam.transform.DOMove(new Vector3(-2.74f, 26.77f, 14.74f), 1f).SetEase(Ease.OutCubic);
            cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(31f, 28.619f, 0f)), 1f).SetEase(Ease.OutCubic);
            garageCanvasGroup.DOFade(0, 0.225f);
            //await ToPosAndRot(new Vector3(-2.74f, 26.77f, 14.74f), Quaternion.Euler(new Vector3(31f, 28.619f, 0f)));
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
            unlockButton.transform.GetChild(1).GetComponent<Image>().color =
                GameMaster.instance.PlayerGold > cars[index].carPrice ? yellow : grey;
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

    private Vector3 QuadraticBeziersCurve(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p4 = Vector3.Lerp(p1, p2, t);
        Vector3 p5 = Vector3.Lerp(p2, p3, t);
        return Vector3.Lerp(p4, p5, t);
    }

    public Vector3 CubicBeziersCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        Vector3 p5 = QuadraticBeziersCurve(p1, p2, p3, t);
        Vector3 p6 = QuadraticBeziersCurve(p2, p3, p4, t);

        return Vector3.Lerp(p5, p6, t);
    }

    private async Task ToPosAndRot(Vector3 finalPos, Quaternion finalRot, bool stopRotation = false)
    {
        var cMainTransform = cam.transform;
        var initPos = cMainTransform.position;
        var initRot = cMainTransform.rotation;
        transition = true;

        if (stopRotation)
        {
        }

        float timer = 0;
        while (timer < 1)
        {
            cam.transform.position = CubicBeziersCurve(initPos, initPos, finalPos, finalPos, timer);
            cam.transform.rotation = Quaternion.Lerp(initRot, finalRot, timer / 1);
            await Task.Yield();
            timer += Time.deltaTime;
        }

        cam.transform.position = finalPos;
        cam.transform.rotation = finalRot;
        transition = false;
    }

    #region PowerUps

    private void BuyPowerUp()
    {
        throw new NotImplementedException();
    }

    private void SetPowerUpSelectable()
    {
        // Index
        // powersUps.items[index].currentLevel;
        nameText.text = powersUps.items[index].name;
        weaponName[0].text = weaponName[1].text = powersUps.items[index].name;
        weaponDesc[0].text = weaponDesc[1].text = powersUps.items[index].description;
        weaponImgs[0].sprite = weaponImgs[1].sprite = powersUps.items[index].icon;
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