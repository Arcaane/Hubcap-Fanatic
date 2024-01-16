using UnityEngine;
using UnityEngine.EventSystems;

public class TestShop : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] shopItems;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private ButtonsItems[] itemsHandler;
    private bool isShopActive;

    private void Start()
    {
        shopCanvas.SetActive(false);
        isShopActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartShopUI();
        }
    }

    private void StartShopUI()
    {
        Time.timeScale = 0;
        shopCanvas.SetActive(true);
        isShopActive = true;
        EventSystem.current.SetSelectedGameObject(itemsHandler[0].gameObject);
        SetupItemsInShop();
    }

    private void SetupItemsInShop()
    {
    }

    private void ExitShop()
    {
        Time.timeScale = 1;
    }
}
