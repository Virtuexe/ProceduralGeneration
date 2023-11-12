using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory System/Databases/Weapon")]
public class WeaponItemDatabaseObject : ScriptableObject
{
    public string savePath;
    public Weapons container;
    public void AddItem(Item _item, int ammo, int mag)
    {
        WeaponDatabase weapon = new WeaponDatabase(ammo, mag);
        container.weaponItems.Add(weapon);
        _item.typeId = container.weaponItems.Count;
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
        container = new Weapons();
    }
}
[System.Serializable]
public class Weapons
{
    public List<WeaponDatabase> weaponItems = new List<WeaponDatabase>();
}
[System.Serializable]
public class WeaponDatabase
{
    public int ammo;
    public int mag;
    public WeaponDatabase(int _ammo, int _mag)
    {
        ammo = _ammo;
        mag = _mag;
    }
    public void AddAmmo(int value)
    {
        ammo += value;
    }
    public void AddMag(int value)
    {
        mag += value;
    }
}

