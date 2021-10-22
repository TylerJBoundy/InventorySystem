using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Player player;
    private Items items;
    [SerializeField] private GameObject prefItemPopup;

    [Space]
    private Transform inventoryPanel;
    private Transform detailsPanel;
    private Transform dropAmountPanel;
    private GameObject inventory;

    [Space]
    [Header("Item Type Icons")]
    [SerializeField] private Sprite brokenIcon;
    [SerializeField] private Sprite swordIcon;
    [SerializeField] private Sprite armourIcon;
    [SerializeField] private Sprite shieldIcon;
    [SerializeField] private Sprite generalIcon;
    [SerializeField] private Sprite foodIcon;
    [SerializeField] private Sprite potionIcon;
    [SerializeField] private Sprite toolIcon;
    [SerializeField] private Sprite questIcon;

    [Space]
    private GameObject itemTemplate;
    private GameObject itemTemplate2;

    [SerializeField] private bool hasItemSelected=false;
    [SerializeField] private string selectedItem="";

    private void Start()
    {
        //Defining References
        player = FindObjectOfType<Player>();
        items = FindObjectOfType<Items>();

        inventoryPanel = transform.Find("InventorySystem/InventoryPanel");
        detailsPanel = transform.Find("InventorySystem/DetailsPanel");
        dropAmountPanel = transform.Find("InventorySystem/DropAmount");
        inventory = inventoryPanel.Find("AreaBox/Scroll View/Viewport/Content").gameObject;

        itemTemplate = inventory.transform.Find("ItemTemplate").gameObject;
        itemTemplate2 = inventory.transform.Find("ItemTemplate2").gameObject;

        //Subscribing to Events
        player.GetComponent<InventoryHandler>().OnItemAddedToInventory += InventoryHandler_OnItemAddedToInventory;
        player.GetComponent<InventoryHandler>().OnItemRemovedFromInventory += InventoryHandler_OnItemRemovedFromInventory;

        RefreshInventory();
    }

    private bool SetItemIcon(Item item, Image icon)
    {
        bool foundItem = false;
        if (item != null) foundItem = true; else return false;
        switch (item.itemType)
        {
            case ItemType.None: icon.sprite = brokenIcon; break;
            case ItemType.General: icon.sprite = generalIcon; break;
            case ItemType.Sword: icon.sprite = swordIcon; break;
            case ItemType.Armour: icon.sprite = armourIcon; break;
            case ItemType.Shield: icon.sprite = shieldIcon; break;
            case ItemType.Food: icon.sprite = foodIcon; break;
            case ItemType.Potion: icon.sprite = potionIcon; break;
            case ItemType.Tool: icon.sprite = toolIcon; break;
            case ItemType.Quest: icon.sprite = questIcon; break;
        }
        return foundItem;
    }

    private void InventoryHandler_OnItemRemovedFromInventory(string itemId) {
        RefreshInventory();
        HideDropAmountWindow();
        bool foundItem = player.GetComponent<InventoryHandler>().CheckForItem(itemId);
        if (!foundItem)
        {
            selectedItem = "";
            hasItemSelected = false;
            HideDetailsPanel();
        }
    }

    private void InventoryHandler_OnItemAddedToInventory(string itemId)
    {
        //Creates new Item Popup
        GameObject popup = Instantiate(prefItemPopup, Vector3.zero, prefItemPopup.transform.rotation);
        Image popupIcon = popup.transform.Find("Image").GetComponent<Image>();

        //Finds out which item was added
        Item item = player.GetComponent<InventoryHandler>().DeterminItem(itemId);

        //Changes Popup icon dependant on the item added
        bool foundItem = SetItemIcon(item, popupIcon);
        if (!foundItem)
            popupIcon.sprite = brokenIcon;

        //Enables the Popup
        popup.transform.SetParent(transform);
        popup.SetActive(true);

        RefreshInventory();
    }

    public void RefreshInventory()
    {
        //Clearing current Inventory UI
        foreach (Transform child in itemTemplate.transform.parent)
        {
            if (child.gameObject != itemTemplate.gameObject && 
                child.gameObject != itemTemplate2.gameObject)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        //Decoding Inventory
        ItemIds decInventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);

        //Generating Items in Inventory UI
        int amountOfItems = 0;
        List<string> currentItems = new List<string>();
        foreach (string item in decInventory.id)
        {
            bool itemAlreadyExists=false;
            foreach (var otherItem in currentItems)
            {
                if (otherItem == item) itemAlreadyExists = true;
            }
            switch (itemAlreadyExists)
            {
                case true:
                    //if item exists already
                    if (player.GetComponent<InventoryHandler>().DeterminItemType(item) != ItemType.General &&
                        player.GetComponent<InventoryHandler>().DeterminItemType(item) != ItemType.Food &&
                        player.GetComponent<InventoryHandler>().DeterminItemType(item) != ItemType.Potion &&
                        player.GetComponent<InventoryHandler>().DeterminItemType(item) != ItemType.None)
                    {
                        //if item isn't one of the above itemTypes else does nothing.
                        AddItemListing(item, amountOfItems);
                        amountOfItems++;
                    }
                    break;
                case false:
                    //if item doesn't already exist
                    AddItemListing(item, amountOfItems);
                    amountOfItems++;
                    break;
            }
            currentItems.Add(item);
        }
    }

    private void AddItemListing(string itemId, int amountOfItems)
    {
        if (player.GetComponent<InventoryHandler>().DeterminItemType(itemId) == ItemType.General ||
            player.GetComponent<InventoryHandler>().DeterminItemType(itemId) == ItemType.Food ||
            player.GetComponent<InventoryHandler>().DeterminItemType(itemId) == ItemType.Potion ||
            player.GetComponent<InventoryHandler>().DeterminItemType(itemId) == ItemType.None)
        {
            CreateNewItemListing(itemTemplate2, itemId, amountOfItems, true);
        } else
        {
            CreateNewItemListing(itemTemplate, itemId, amountOfItems, false);
        }
    }

    private void CreateNewItemListing(GameObject template, string itemId, int amountOfItems, bool stackable)
    {
        //Decoding inventory
        ItemIds decInventory = JsonUtility.FromJson<ItemIds>(player.jsonInventory);

        //Creating the listing
        GameObject newItem = Instantiate(template, Vector3.zero, template.transform.rotation, template.transform.parent);
        newItem.transform.localPosition = new Vector3(template.transform.localPosition.x, (amountOfItems * newItem.GetComponent<RectTransform>().sizeDelta.y) * (-1), 0);
        newItem.name = "Item_" + itemId;
        Image icon = newItem.transform.Find("Icon").GetComponent<Image>();

        //Setting the item icon and name
        Item item = player.GetComponent<InventoryHandler>().DeterminItem(itemId);
        bool foundItem = SetItemIcon(item, icon);
        if (!foundItem) {
            newItem.transform.Find("Icon").GetComponent<Image>().sprite = brokenIcon;
            newItem.transform.Find("Name").GetComponent<Text>().text = itemId;
        } else newItem.transform.Find("Name").GetComponent<Text>().text = item.itemName;

        //Checking if the item is stackable and if so setting the current amount of same items in listing
        if (stackable)
        {
            int itemAmount = 0;
            foreach (string otherItem in decInventory.id)
            {
                if (otherItem == itemId) itemAmount++;
            }
            newItem.transform.Find("Amount").GetComponent<Text>().text = "x " + itemAmount;
        }

        newItem.SetActive(true);
    }

    //Details Panel
    public void ItemSelected(GameObject itemSelected)
    {
        //Gets itemId from Item Listing name
        string itemId = itemSelected.name.Substring(5, itemSelected.name.Length - 5);

        detailsPanel.Find("AreaBox/Item/Weapon").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Armour").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Quest").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Food").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Potion").gameObject.SetActive(false);

        if (selectedItem == itemId)
        {
            selectedItem = "";
            hasItemSelected = false;
            HideDetailsPanel();
        }
        else
        {
            selectedItem = itemId;
            hasItemSelected = true;
            ShowDetailsPanel(itemId);
        }
    }
    public void ShowDetailsPanel(string itemId)
    {
        //Displays details panel
        detailsPanel.gameObject.SetActive(true);

        Transform panel = detailsPanel.Find("AreaBox/Item");

        //Determines what item the itemId is relative to and shows the correct values each itemType should
        ItemType itemType = player.GetComponent<InventoryHandler>().DeterminItemType(itemId);
        Item item = player.GetComponent<InventoryHandler>().DeterminItem(itemId);
        switch (itemType)
        {
            case ItemType.None: break;
            case ItemType.General: break;

            case ItemType.Sword:
                Transform weap = panel.Find("Weapon");
                weap.Find("Attack").GetComponent<Text>().text = "Attack: " + item.primaryStat.ToString();
                weap.gameObject.SetActive(true);
                break;
            case ItemType.Armour:
                Transform armr = panel.Find("Armour");
                armr.Find("Defence").GetComponent<Text>().text = "Defence: " + item.primaryStat.ToString();
                armr.gameObject.SetActive(true);
                break;
            case ItemType.Shield:
                Transform armr2 = panel.Find("Armour");
                armr2.Find("Defence").GetComponent<Text>().text = "Defence: " + item.primaryStat.ToString();
                armr2.gameObject.SetActive(true);
                break;
            case ItemType.Food:
                Transform food = panel.Find("Food");
                food.Find("Saturation").GetComponent<Text>().text = "Saturation: " + item.secondaryStat.ToString();
                food.gameObject.SetActive(true);
                break;
            case ItemType.Potion:
                Transform pot = panel.Find("Potion");
                pot.Find("Effect").GetComponent<Text>().text = "Effect: " + item.effectId;
                string addedBit;
                if (item.secondaryStat == 0) addedBit = "Instantaneous"; else addedBit = item.secondaryStat.ToString();
                pot.Find("Duration").GetComponent<Text>().text = "Duration: " + addedBit;
                pot.gameObject.SetActive(true);
                break;
            case ItemType.Quest:
                Transform que = panel.Find("Quest");
                que.gameObject.SetActive(true);
                break;
            case ItemType.Tool:
                //Transform que = panel.Find("Quest");
                //que.gameObject.SetActive(true);
                Debug.LogWarning("No panel exists yet!");
                break;
        }

        //Checks for item and sets item icon
        bool foundItem = SetItemIcon(item, panel.Find("Icon").GetComponent<Image>());
        if (!foundItem) {
            //if item is invalid or broken
            panel.Find("Icon").GetComponent<Image>().sprite = brokenIcon;
            panel.Find("Name").GetComponent<Text>().text = itemId;
            panel.Find("Description/Text").GetComponent<Text>().text = "This item either doesn't exist or is broken";
            panel.Find("SellPrice").GetComponent<Text>().text = "Sell Price: 0";
        } else {
            //if item is valid
            panel.Find("Name").GetComponent<Text>().text = item.itemName;
            panel.Find("Description/Text").GetComponent<Text>().text = item.itemDescription;
            panel.Find("SellPrice").GetComponent<Text>().text = "Sell Price: " + item.itemSellPrice.ToString();
        }

        panel.Find("Description/ItemId").GetComponent<Text>().text = itemId;
        panel.gameObject.SetActive(true);
    }
    public void HideDetailsPanel()
    {
        //Disables details panel and disables all itemType variants
        detailsPanel.gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Weapon").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Armour").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Quest").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Food").gameObject.SetActive(false);
        detailsPanel.Find("AreaBox/Item/Potion").gameObject.SetActive(false);
    }

    //Debugging
    public void GiveItem(InputField inputField)
    {
        string itemId = inputField.text;
        player.GetComponent<InventoryHandler>().AddItem(itemId);
    }

    public void ShowDropAmountWindow(string itemId)
    {
        dropAmountPanel.Find("AreaBox/Description/ItemId").GetComponent<Text>().text = itemId;
        dropAmountPanel.gameObject.SetActive(true);
    }
    public void HideDropAmountWindow()
    {
        dropAmountPanel.gameObject.SetActive(false);
        player.GetComponent<InventoryHandler>().dropAmount = 1;
        dropAmountPanel.Find("AreaBox/Description/Amount").GetComponent<Text>().text
            = player.GetComponent<InventoryHandler>().dropAmount.ToString();
    }
    public void DropAmountAdd(Text itemListing)
    {
        string itemId = itemListing.text;

        if (player.GetComponent<InventoryHandler>().dropAmount == player.GetComponent<InventoryHandler>().CheckHowManyItemsInInventory(itemId)) return;
        player.GetComponent<InventoryHandler>().dropAmount++;

        dropAmountPanel.Find("AreaBox/Description/Amount").GetComponent<Text>().text
            = player.GetComponent<InventoryHandler>().dropAmount.ToString();
    }
    public void DropAmountNegate()
    {
        if (player.GetComponent<InventoryHandler>().dropAmount == 1) return;
        player.GetComponent<InventoryHandler>().dropAmount--;

        dropAmountPanel.Find("AreaBox/Description/Amount").GetComponent<Text>().text
            = player.GetComponent<InventoryHandler>().dropAmount.ToString();
    }
}
