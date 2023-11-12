using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Weapon
}
public class ItemObject : ScriptableObject
{
    public new string name;
    [SerializeField]
    public Sprite sprite;
    public ItemType type;
    public int id;
    public int typeId;
    //size of 0,0 is 1x1
    public Vector2Int size;
    public GameObject prefab;
    [TextArea(15,20)]
    public string description;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int id;
    [SerializeField]
    public Sprite sprite;
    //size of 1,1 is 1x1
    public Vector2Int size;
    public GameObject prefab;
    public ItemType type;
    public int typeId;
    public Item(ItemObject item)
    {
        name = item.name;
        id = item.id;
        type = item.type;
        typeId = item.typeId;
        prefab = item.prefab;
        size = item.size;
        sprite = item.sprite;
    }
}