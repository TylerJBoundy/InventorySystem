using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
public class ItemEditor : Editor
{
    public Item myTarget;

    void AddHeader(string title)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
    }

    public override void OnInspectorGUI()
    {
        myTarget = (Item)target;

        // Style
        myTarget.itemId = EditorGUILayout.TextField("Item ID", myTarget.itemId); // Item ID

        AddHeader("Inventory Variables");
        myTarget.itemName = EditorGUILayout.TextField("Name", myTarget.itemName); // Name

        EditorGUILayout.LabelField("Description"); // Description
        EditorStyles.textArea.wordWrap = true;
        myTarget.itemDescription = EditorGUILayout.TextArea(myTarget.itemDescription, EditorStyles.textArea, GUILayout.MaxHeight(100));

        AddHeader("Shop Variables");
        myTarget.itemBuyPrice = EditorGUILayout.IntField("Buy Price", myTarget.itemBuyPrice); // Buy Price
        myTarget.itemSellPrice = EditorGUILayout.IntField("Sell Price", myTarget.itemSellPrice); // Sell Price

        AddHeader("Type Variables");
        myTarget.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", myTarget.itemType); // Item Type

        switch (myTarget.itemType)
        {
            case ItemType.None: break;
            case ItemType.General: break;
            case ItemType.Quest: break;
            case ItemType.Tool: break;
            case ItemType.Sword: Sword(); break;
            case ItemType.Shield: Shield(); break;
            case ItemType.Armour: Armour(); break;
            case ItemType.Food: Food(); break;
            case ItemType.Potion: Potion(); break;
        }

        void Sword()
        {
            myTarget.primaryStat = EditorGUILayout.IntField("Damage", myTarget.primaryStat);
        }
        void Shield()
        {
            myTarget.primaryStat = EditorGUILayout.IntField("Defence", myTarget.primaryStat);
        }
        void Armour()
        {
            myTarget.primaryStat = EditorGUILayout.IntField("Defence", myTarget.primaryStat);
        }
        void Food()
        {
            myTarget.primaryStat = EditorGUILayout.IntField("Multiplier", myTarget.primaryStat);
            myTarget.secondaryStat = EditorGUILayout.IntField("Saturation", myTarget.secondaryStat);
        }
        void Potion()
        {
            myTarget.effectId = EditorGUILayout.TextField("Effect ID", myTarget.effectId);
            myTarget.primaryStat = EditorGUILayout.IntField("Multiplier", myTarget.primaryStat);
            myTarget.secondaryStat = EditorGUILayout.IntField("Duration", myTarget.secondaryStat);
        }

        // This guarantees variables being saved
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }
}