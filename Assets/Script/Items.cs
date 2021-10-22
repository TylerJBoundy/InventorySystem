using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private Item[] allTheItems = { };

    private ItemBehaviour itemBehaviour;

    public Item[] GetReadOnlyAllTheItems()
    {
        return allTheItems;
    }

    private void Awake()
    {
        itemBehaviour = FindObjectOfType<ItemBehaviour>();

        Resources.LoadAll("Items/", typeof(Item));
        allTheItems = (Item[])Resources.FindObjectsOfTypeAll(typeof(Item));

        foreach (Item item in allTheItems)
        {
            item.OnUse.AddListener(() => itemBehaviour.UseItem(item));
        }
    }
}

public enum ItemType
{
    None,
    General,
    Quest,
    Tool,
    Sword,
    Shield,
    Armour,
    Food,
    Potion
};

[System.Serializable]
public class ItemIds
{
    public List<string> id = new List<string>();
}
