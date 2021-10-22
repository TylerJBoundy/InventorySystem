using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class InstantHealthPotion : UnityEvent<int> { };

    [SerializeField]
    public InstantHealthPotion OnInstantHealthPotionUsed;

    public void UseItem(Item item)
    {
        if (item.itemType == ItemType.Potion)
        {
            switch (item.effectId)
            {
                case "Instant Health":
                    if (OnInstantHealthPotionUsed != null) OnInstantHealthPotionUsed.Invoke(item.primaryStat);
                    break;
                case "Healing":

                    break;
            }
        }
    }
}