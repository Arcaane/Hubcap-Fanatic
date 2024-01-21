using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OutOfBoundsManager : MonoBehaviour {
    [SerializeField] private List<GameObject> colGam = new();
    [SerializeField] private Volume outOfBoundVolume = null;
    [SerializeField] private float maxTimer = 15f;
    private float timer = 0;

    private void Start() {
        timer = maxTimer;
    }

    private void Update() {
        if (colGam.Count > 0) {
            timer -= Time.deltaTime;
            UIManager.instance.UpdateOutOfBoundsTxt(timer, maxTimer);
            outOfBoundVolume.weight = 1 - timer / maxTimer;

            if (timer <= 0) {
                GetComponent<CarHealthManager>().TakeDamage(1500);
                Destroy(this);
            }
        }
        else {
            UIManager.instance.CloseOutOfBounds();
            outOfBoundVolume.weight = 0;
            timer = maxTimer;
        }
    }

    public void RegisterCollider(GameObject col) {
        colGam.Add(col);
    }

    public void UnregisterCollider(GameObject col) {
        colGam.Remove(col);
    }
}
