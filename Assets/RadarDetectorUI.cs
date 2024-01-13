using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarDetectorUI : MonoBehaviour
{
   public Image[] wifiParts;

   public Color activeColor,innactiveColor;

   public void SetActivation(int activeNb)
   {
      for (int i = 0; i < wifiParts.Length; i++)
      {
         if (i < activeNb) wifiParts[i].color = activeColor;
         else wifiParts[i].color = innactiveColor;
      }
   }
}
