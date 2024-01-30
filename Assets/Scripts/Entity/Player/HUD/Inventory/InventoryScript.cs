using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public anchorPositon anchor;
    public float cellSize;


    public InventoryObject inventory;
    public InventoryManagerScript manager;
    public GameObject slot;
    public GameObject item;
    public GameObject itemDrag;
    public List<List<GameObject>> slots = new List<List<GameObject>>();
    public List<GameObject> items = new List<GameObject>();
    Vector2 resolution;
    public enum anchorPositon
    {
        right,
        left,
        middle
    }
    void Awake()
    {
        resolution = new Vector2(Screen.width, Screen.height);
        inventory.ItemChanged += RedrawItems;
        manager.inventories.Add(transform);
        ChangeResolution();
        RedrawItems();
        Redraw();
    }
    void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            ChangeResolution();
        }
    }
    public void ChangeResolution()
    {
        resolution.x = Screen.width;
        resolution.y = Screen.height;
        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(resolution.x / 3, resolution.y);
        switch (anchor)
        {
            case anchorPositon.right:
                transform.position = new Vector2(resolution.x -(transform.sizeDelta.x/2), resolution.y / 2);
                break;
            case anchorPositon.left:
                transform.position = new Vector2(0+ (transform.sizeDelta.x / 2), resolution.y / 2);
                break;
            case anchorPositon.middle:
                transform.position = new Vector2(resolution.x/2, resolution.y / 2);
                break;
        }
            
        Redraw();
        RedrawItems();
    }

    public void Redraw()
    {

        foreach (List<GameObject> list in slots.ToList())
        {
            foreach (GameObject slot in list.ToList())
            {
                list.Remove(slot);
                Destroy(slot);
            }
        }
        RectTransform rt = GetComponent(typeof(RectTransform)) as RectTransform;
        var pos = rt.transform.position;
        var scale = rt.sizeDelta;

        Vector2 scale2 = new Vector2(cellSize, cellSize);
        Vector2 Scale2 = new Vector2(cellSize / 100, cellSize / 100);
        if (cellSize * inventory.container.size.x > scale.x)
        {
            scale2 = new Vector2(scale.x / inventory.container.size.x, scale.x / inventory.container.size.x);
            Scale2 = new Vector2((1*scale2.x)/ cellSize, (1 * scale2.y) / cellSize);
        }

        for (int y = 0; y < inventory.container.size.y; y++)
        {
            slots.Add(new List<GameObject>());
            for (int x = 0; x < inventory.container.size.x; x++)
            {
                var g = Instantiate(slot, new Vector2(((pos.x - (scale.x / 2) + (scale2.x / 2)) + x * scale2.x), ((pos.y + (scale.y / 2) - (scale2.y / 2)) - y * scale2.y)), Quaternion.identity);
                slots[y].Add(g);
                g.GetComponent<RectTransform>().localScale = Scale2;
                
                g.transform.SetParent(transform);
            }
        }
    }
    public Vector2 scale2;
    public void RedrawItems()
    {
        //delete all items
        foreach (GameObject item in items.ToList())
        {
            items.Remove(item);
            Destroy(item);
        }
        RectTransform rt = GetComponent(typeof(RectTransform)) as RectTransform;
        var pos = rt.transform.position;
        var scale = rt.sizeDelta;

        RectTransform rt2 = slot.GetComponent<RectTransform>();
        scale2 = new Vector2(cellSize, cellSize);
        Vector2 Scale2 = new Vector2();
        if (cellSize * inventory.container.size.x > scale.x)
        {
            scale2 = new Vector2(scale.x / inventory.container.size.x, scale.x / inventory.container.size.x);
            Scale2 = new Vector2((1 * scale2.x) / cellSize, (1 * scale2.y) / cellSize);
        }
        foreach (InventorySlot i in inventory.container.ItemList())
        {
            //XY of first cell
            float x = ((pos.x - (scale.x / 2) + (scale2.x / 2)) + i.slot.x * scale2.x);
            float y = ((pos.y + (scale.y / 2) - (scale2.y / 2)) - i.slot.y * scale2.y);

            var g = Instantiate(item, new Vector2(x + ((i.item.size.x * scale2.x) / 2 - (scale2.x / 2)), y - ((i.item.size.y * scale2.y) / 2 - (scale2.y / 2))), Quaternion.identity);
            g.transform.SetParent(this.gameObject.transform);
            g.GetComponent<InventoryItemScript>().Slot(scale2, i);
            items.Add(g);
        }
    }

}
