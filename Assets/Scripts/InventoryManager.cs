using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int inventoryCapacity = 10;
    [SerializeField] private Text inventoryText;

    private List<GameObject> inventory = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(GameObject item)
    {
        if (inventory.Count < inventoryCapacity)
        {
            inventory.Add(item);
            UpdateInventoryText();
            return true;
        }
        else
        {
            Debug.Log("Inventory is Full!!!");
            return false;
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            UpdateInventoryText();
        }
    }

    private void UpdateInventoryText()
    {
        if (inventoryText != null)
        {
            inventoryText.text = $"Inventory: {inventory.Count}/{inventoryCapacity}";
        }
    }
}