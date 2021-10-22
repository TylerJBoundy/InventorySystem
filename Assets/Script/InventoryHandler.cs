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

    /// <summary>
    /// Finds an itemType relative to an itemID.
    /// </summary>
    /// <param name="itemId">The itemID to check for.</param>
    /// <returns>The itemType relative to itemID.</returns>
    public ItemType DeterminItemType(string itemId)
    {
        if (DeterminItem(itemId) != null)
            return DeterminItem(itemId).itemType;
        else
            return ItemType.None;
    }

    /// <summary>
    /// Finds an item relative to an itemID.
    /// </summary>
    /// <param name="itemId">The itemID to check for.</param>
    /// <returns>An item relative to itemID.</returns>
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

    /// <summary>
    /// Checks the inventory for an item.
    /// </summary>
    /// <param name="itemId">The itemId of the item to check for.</param>
    /// <returns>The amount of said item.</returns>
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

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="itemId">The item to add.</param>
    public void AddItem(string itemId)
    {
        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);
        inventory.id.Add(itemId);
        player.jsonInventory = JsonUtility.ToJson(inventory);

        if (OnItemAddedToInventory != null) OnItemAddedToInventory(itemId);
    }

    /// <summary>
    /// Removes items.
    /// </summary>
    /// <param name="itemListing">The item to remove.</param>
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

    /// <summary>
    /// Removes 1 item.
    /// </summary>
    /// <param name="itemListing">The item to remove.</param>
    public void RemoveOneItem(Text itemListing)
    {
        string itemId = itemListing.text;

        ItemIds inventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);

        inventory.id.Remove(itemId);
        player.jsonInventory = JsonUtility.ToJson(inventory);

        if (OnItemRemovedFromInventory != null) OnItemRemovedFromInventory(itemId);
    }

    /// <summary>
    /// Removes a bulk amount of items.
    /// </summary>
    /// <param name="itemListing">The item to remove.</param>
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

    /// <summary>
    /// Checks for the item.
    /// </summary>
    /// <param name="itemId">Item to check for.</param>
    /// <returns>True if the item was found. False if the item was not found.</returns>
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

    /// <summary>
    /// Equips the item.
    /// </summary>
    /// <param name="itemListing">Item to equip.</param>
    public void EquipItem(Text itemListing)
    {
        string itemId = itemListing.text;
        Debug.LogWarning("Equiping " + itemId);
    }

    /// <summary>
    /// Uses the item.
    /// </summary>
    /// <param name="itemListing">The item to use.</param>
    public void UseItem(Text itemListing)
    {
        string itemId = itemListing.text;
        Item item = DeterminItem(itemId);

        if (item.OnUse != null) item.OnUse.Invoke();

        RemoveOneItem(itemListing);
    }
}
