using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Items/Weapon")]
public class WeaponObject : ItemObject
{
    public int ammo;
    public int mag;
    public void Awake()
    {
        type = ItemType.Weapon;
    }
}
