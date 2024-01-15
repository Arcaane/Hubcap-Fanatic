using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CollectibleGroup : MonoBehaviour
{
    [SerializeField] private List<CollectibleEntity> entities = new();
    [SerializeField] private float pickAllEntitiesDuration;
    [SerializeField] private float timer;
    private bool isCollecting;
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            entities.Add(transform.GetChild(i).GetComponent<CollectibleEntity>());
        }
    }

    private void Update()
    {
        if (isCollecting)
        {
            timer += Time.deltaTime;
            if (timer > pickAllEntitiesDuration)
            {
                Debug.Log("Temps écoulé !");
                if (!IsAllCollected())
                { 
                   ResetSpawner();
                }
            }
        }
    }

    public void TriggeredEntity()
    {
        isCollecting = true;
        if (IsAllCollected())
        {
            Debug.Log("All Bubbles Collected !");
            ResetSpawner();
        }
    }

    private bool IsAllCollected() => entities.TrueForAll(entities => entities.isTrigger);

    private async void ResetSpawner()
    {
        isCollecting = false;
        timer = 0;
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].gameObject.SetActive(false);
        }

        await Task.Delay(10 * 1000);
        
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].isTrigger = false;
            entities[i].gameObject.SetActive(true);
        }
    }
}
