using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManagerScript : MonoBehaviour
{
    [SerializeField] InputActionAsset playerInput;
    InputAction selecting;
    InputAction moving;

    public InventorySlot slot;
    public InventoryObject slot_inventoryObject;
    public GameObject dragedItem;

    public List<Transform> inventories;
    public InventoryScript inventory;

    void Awake()
    {
        var inventoryActionMap = playerInput.FindActionMap("Inventory");
        selecting = inventoryActionMap.FindAction("Select");
        selecting.performed += OnSelectPreformed;
        selecting.canceled += OnSelectCanceled;
        moving = inventoryActionMap.FindAction("Move");
        moving.performed += OnMoveChanged;
        moving.Enable();
        slot = null;
    }
    void OnEnable()
    {
        if(inventory != null)
            selecting.Enable();
        moving.Enable();
    }
    void OnDisable()
    {
        selecting.Disable();
        moving.Disable();
    }
    void OnSelectPreformed(InputAction.CallbackContext context)
    {
        //Debug.Log(Mouse.current.position.x.ReadValue()+","+ Mouse.current.position.y.ReadValue()+"::"+transform.position.x+","+ transform.position.y);
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 size = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, transform.GetComponent<RectTransform>().sizeDelta.y);
        Vector2 corner = new Vector2(transform.position.x - (size.x / 2), transform.position.y - (size.y / 2));
        //check if mouse is in inventory
        //check if mouse is in any of items in inventory
        foreach (GameObject item in inventory.items)
        {
            size = new Vector2(item.transform.GetComponent<RectTransform>().sizeDelta.x, item.transform.GetComponent<RectTransform>().sizeDelta.y);
            corner = new Vector2(item.transform.position.x - (size.x / 2), item.transform.position.y - (size.y / 2));
            if (mouse.x >= corner.x && mouse.x < corner.x + size.x && mouse.y >= corner.y && mouse.y < corner.y + size.y)
            {
                var slot = item.GetComponent<InventoryItemScript>();
                this.slot = slot.slot;
                slot_inventoryObject = inventory.inventory;
                //create new instance of item and put it in manager
                var i = Instantiate(inventory.itemDrag);
                i.transform.SetParent(transform.parent);
                i.GetComponent<InventoryItemScript>().Slot(inventory.scale2, this.slot);
                dragedItem = i;
                //make item sprite see-through
                ///add item redraw
                /*
                Image image = slot.image;
                slot.image.color = new Color(image.color.r, image.color.b, image.color.g, image.color.a/2);
                break;
                */
            }
        }
    }
    Vector2Int placeSlot;
    InventoryObject placeInventory;
    void OnSelectCanceled(InputAction.CallbackContext context)
    {
        //GetComponent<InventoryItemScript>().Slot(scale2, i);
        if (slot == null||slot_inventoryObject == null||inventory==null||placeInventory==null)
            return;
        slot_inventoryObject.MoveItem(slot, placeInventory, placeSlot);
        //remove drag item
        slot = null;
        slot_inventoryObject = null;
        Destroy(dragedItem);
    }
    void OnMoveChanged(InputAction.CallbackContext context)
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 point = new Vector2(mouse.x, mouse.y);
        Vector2 size;
        Vector2 corner;
        foreach (Transform t in inventories)
        {
            if (t.GetComponent<InventoryScript>() != inventory)
            {
                size = new Vector2(t.GetComponent<RectTransform>().sizeDelta.x, t.transform.GetComponent<RectTransform>().sizeDelta.y);
                corner = new Vector2(t.position.x - (size.x / 2), t.position.y - (size.y / 2));
                //check if mouse is in
                if (point.x >= corner.x && point.x < corner.x + size.x && point.y >= corner.y && point.y < corner.y + size.y)
                {
                    selecting.Enable();
                    inventory = t.GetComponent<InventoryScript>();
                    break;
                }


            }
        }
        if (inventory == null || slot_inventoryObject == null || slot == null) return;
        point += new Vector2(-(int)(slot.item.size.x * inventory.scale2.x / 2), (int)(slot.item.size.y * inventory.scale2.y / 2));

        //check if mouse is in any of items in inventory
        int y = 0; int x = 0;
        foreach (List<GameObject> list in inventory.slots)
        {
            foreach (GameObject slot in list)
            {
                Vector2 scale = slot.transform.GetComponent<RectTransform>().localScale;
                size = new Vector2(slot.transform.GetComponent<RectTransform>().sizeDelta.x * scale.x, slot.transform.GetComponent<RectTransform>().sizeDelta.y * scale.y);
                corner = new Vector2(slot.transform.position.x - (size.x / 2), slot.transform.position.y - (size.y / 2));
                if (point.x >= corner.x && point.x < corner.x + size.x && point.y >= corner.y && point.y < corner.y + size.y)
                {
                    if (!slot_inventoryObject.TryMoveItem(this.slot, inventory.inventory,new Vector2Int(x, y))) return;
                    placeSlot = new Vector2Int(x, y);
                    placeInventory = inventory.inventory;
                    return;
                }
                x++;
            }
            y++;
            x = 0;

        }
        //set size of draged item
        if (dragedItem != null)
        {
            //manager.dragedItem.GetComponent<InventoryItemScript>().Slot(new Vector2)
        }
    }
}
