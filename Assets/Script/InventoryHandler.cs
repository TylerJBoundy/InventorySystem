using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    private Player player;
    private Items items;

    public delegate void ItemAdded(string itemId);
    public event ItemAdded OnItemAddedToInventory;
    public event ItemAdded OnItemRemovedFromInventory;

    public int dropAmount = 1;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        items = FindObjectOfType<Items>();
    }

    public ItemType DeterminItemType(string itemId)
    {
        if (DeterminItem(itemId) != null)
            return DeterminItem(itemId).itemType;
        else
            return ItemType.None;
    }

    public Item DeterminItem(string itemId)
    {
        Item correctItem = null;
        for (int i = 0; i < items.GetReadOnlyAllTheItems().Length; i++)
        {
            if (items.GetReadOnlyAllTheItems()[i].itemId == itemId)
            {
                correctItem = items.GetReadOnlyAllTheItems()[i];
            }
        }
        return correctItem;
    }

    public int CheckHowManyItemsInInventory(string itemId)
    {
        int amountOfItems = 0;

        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);

        foreach (string item in inventory.id)
        {
            if (item == itemId)
            {
                amountOfItems++;
            }
        }

        return amountOfItems;
    }

    public void AddItem(string itemId)
    {
        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);
        inventory.id.Add(itemId);
        player.jsonInventory = JsonUtility.ToJson(inventory);

        if (OnItemAddedToInventory != null) OnItemAddedToInventory(itemId);
    }
    public void RemoveItem(Text itemListing)
    {
        string itemId = itemListing.text;

        //Check for itemType
        if (DeterminItemType(itemId) == ItemType.Food
            || DeterminItemType(itemId) == ItemType.General
            || DeterminItemType(itemId) == ItemType.Potion
            || DeterminItemType(itemId) == ItemType.None)
        {
            Debug.LogWarning("Sending to drop amount");
            FindObjectOfType<UIManager>().ShowDropAmountWindow(itemId);
            return;
        }

        RemoveOneItem(itemListing);
    }
    public void RemoveOneItem(Text itemListing)
    {
        string itemId = itemListing.text;

        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);

        inventory.id.Remove(itemId);
        player.jsonInventory = JsonUtility.ToJson(inventory);

        if (OnItemRemovedFromInventory != null) OnItemRemovedFromInventory(itemId);
    }
    public void RemoveItemBulk(Text itemListing)
    {
        string itemId = itemListing.text;
        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);
        List<string> itemsToDrop = new List<string>();

        int amountToDrop = dropAmount;
        foreach (string item in inventory.id)
        {
            if (amountToDrop == 0)
            {
                Debug.LogWarning("Stopped dropping items");
                break;
            }
            if (item == itemId)
            {
                itemsToDrop.Add(item);
                amountToDrop--;
            }
        }

        foreach(string item in itemsToDrop)
        {
            inventory.id.Remove(item);
            Debug.Log("removed " + item + " from inventory.");
        }

        player.jsonInventory = JsonUtility.ToJson(inventory);

        if (OnItemRemovedFromInventory != null) OnItemRemovedFromInventory(itemId);
    }

    public bool CheckForItem(string itemId)
    {
        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);
        foreach (string item in inventory.id)
        {
            if (item == itemId)
            {
                return true;
            }
        }

        return false;
    }

    public void EquipItem(Text itemListing)
    {
        string itemId = itemListing.text;
        Debug.LogWarning("Equiping " + itemId);
    }

    public void UseItem(Text itemListing)
    {
        string itemId = itemListing.text;
        Item item = DeterminItem(itemId);

        if (item.OnUse != null) item.OnUse.Invoke();

        RemoveOneItem(itemListing);
    }
}
