using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryForVictoryScreen : MonoBehaviour
{
   public static MemoryForVictoryScreen instance;

   public int waveCount;
   public bool victory;

   private void Awake()
   {
      instance = this;
   }
}
