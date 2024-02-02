using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    public TMP_Text text1, text2;
    public Vector3 rot;

    public void SetDamageText(int amount)
    {
        transform.rotation = Quaternion.Euler(rot);
        text1.text = text2.text = "-" + amount;
    }

    public void SetGoldText(int amount)
    {
        transform.rotation = Quaternion.Euler(rot);
        text1.text = text2.text = "+" + amount;
    }
}
