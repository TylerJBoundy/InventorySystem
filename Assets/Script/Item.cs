using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemId;
    [Space]
    public string itemName;
    [TextArea(10, 10)]
    public string itemDescription;
    [Space]
    public ItemType itemType;
    [Space]
    public int itemBuyPrice;
    public int itemSellPrice;

    public int primaryStat;
    public int secondaryStat;

    public string effectId;

    public UnityEvent OnUse;
}
