using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Inventory System/Inventories/Default")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public DefaultItemDatabaseObject database;
    public Inventory container;
    // Declare the delegate (if using non-generic pattern).
    public delegate void SampleEventHandler();

    // Declare the event.
    public event SampleEventHandler ItemChanged;

    void OnValidate()
    {
        container.CreateList();            
    }
    public bool TryAddItem(Item _item, Vector2Int slot)
    {
        if (slot.x < 0 || slot.y < 0) return false;
        for (int y2 = 0; y2 < _item.size.y; y2++)
        {
            if (container.size.y < slot.y + _item.size.y)
            {
                return false;
            }
            for (int x2 = 0; x2 < _item.size.x; x2++)
            {
                if (container.size.x < slot.x + _item.size.x)
                {
                    return false;
                }
                if (container.full[slot.x + x2][slot.y + y2])
                {
                    return false;
                }
            }
        }
        return true;
    }
    public bool AddItem(Item _item, int _amount)
    {
        //add item and automaticly put it in first slot avaiable
        for (int y = 0; y < container.size.y; y++)
        {
            if (container.size.y < y+_item.size.y)
            {
                return false;
            }
            for (int x = 0; x < container.size.x; x++)
            {
                if (container.size.x < x + _item.size.x)
                {
                    break;
                }
                if (TryAddItem(_item, new Vector2Int(x, y)))
                {
                    container.ItemListAdd(new InventorySlot(_item.id, _item, new Vector2Int(x, y), _amount));
                    ItemChanged.Invoke();
                    return true;
                }
            }
        }
        return false;
    }
    public bool AddItem(Item _item, int _amount, Vector2Int slot)
    {
        if(!TryAddItem(_item,slot)) return false;
        container.ItemListAdd(new InventorySlot(_item.id, _item, new Vector2Int(slot.x, slot.y), _amount));
        ItemChanged.Invoke();
        return true;
    }
    public bool TryMoveItem(InventorySlot slot, InventoryObject inventoryObj, Vector2Int moveTo)
    {
        if (inventoryObj != this)
        {
            Vector2Int moveBack = slot.slot;
            if (!MoveItem(slot, inventoryObj, moveTo))return false;
            if(!inventoryObj.MoveItem(slot, this, moveBack)) Debug.Log("fuck");
            return true;
        }
        return TryMoveItem(slot, moveTo);
    }
    public bool MoveItem(InventorySlot slot, InventoryObject inventoryObj,Vector2Int moveTo)
    {
        if (!container.ItemList().Contains(slot)) return false;
        if (inventoryObj != this)
        {
            if (inventoryObj.TryAddItem(slot.item, moveTo))
            {
                container.ItemListRemove(slot);
                slot.slot = moveTo;
                inventoryObj.container.ItemListAdd(slot);
                ItemChanged.Invoke();
                inventoryObj.ItemChanged.Invoke();
                return true;
            }
            return false;
        }
        return MoveItem(slot, moveTo);
    }
    public bool TryMoveItem(InventorySlot slot, Vector2Int moveTo)
    {
        if (moveTo == slot.slot) return true;
        Vector2Int moveBack = slot.slot;
        if(!MoveItem(slot, moveTo)) return false;
        MoveItem(slot, moveBack);
        return true;
    }
    public bool MoveItem(InventorySlot slot, Vector2Int moveTo)
    {
        if (!container.ItemList().Contains(slot)) return false;
        if (moveTo == slot.slot) return true;
        container.setFull(slot.slot, slot.item.size,false);
        if (TryAddItem(slot.item, moveTo))
        {
            slot.slot = moveTo;
            container.setFull(slot.slot,slot.item.size,true);
            ItemChanged.Invoke();
            return true;
        }
        container.setFull(slot.slot, slot.item.size, true);
        return false;
    }
    [ContextMenu("Save")]
    public void Save()
    {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        container = new Inventory();
        ItemChanged.Invoke();
    }

    internal void AddItem(InventorySlot slot, int v)
    {
        throw new NotImplementedException();
    }
}
[System.Serializable]
public class Inventory
{

    [SerializeField]private List<InventorySlot> items = new List<InventorySlot>();
    public Vector2Int size;
    public List<List<bool>> full = new List<List<bool>>();
    //ITEMS list
    public void ItemListAdd(InventorySlot s)
    {
        setFull(s.slot, s.item.size, true);
        items.Add(s);
    }
    public bool ItemListRemove(InventorySlot s)
    {
        if(items.Remove(s))
        {
            setFull(s.slot, s.item.size, false);
            return true;
        }
        return false;
    }
    public void ItemListClear()
    {
        items.Clear();
    }
    public List<InventorySlot> ItemList()
    {
        return items;
    }
    public void CreateList()
    {
        full.Clear();
        for (int x = 0; x < size.x; x++)
        {
            full.Add(new List<bool>());
            for (int y = 0; y < size.y; y++)
            {
                full[x].Add(false);
            }
        }
        ItemListCheck();
    }
    private void ItemListResize(Vector2Int _size)
    {
        ///will not work forgot to add new slots
        size = _size;
        ItemListCheck();
    }
    public void ItemListCheck()
    {
        //checks how many spaces are ocupited by item and puts it in their coordinates to list called full
        foreach (InventorySlot i in items)
        {
            for (int y = i.slot.y; y < i.item.size.y+ i.slot.y; y++)
            {
                for (int x = i.slot.x; x < i.item.size.x+ i.slot.x; x++)
                {
                    full[x][y] = true;
                }
            }
        }
    }
    public void setFull(Vector2Int position, Vector2Int size, bool add)
    {
        for (int y = position.y; y < size.y + position.y; y++)
        {
            for (int x = position.x; x < size.x + position.x; x++)
            {
                full[x][y] = add;
            }
        }
    }
}


[System.Serializable]
public class InventorySlot
{
    public int id;
    public Item item;
    public Vector2Int slot;
    public int amount;
    public InventorySlot(int _id, Item _item,Vector2Int _slot, int _amount)
    {
        id = _id;
        item = _item;
        slot = _slot;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}