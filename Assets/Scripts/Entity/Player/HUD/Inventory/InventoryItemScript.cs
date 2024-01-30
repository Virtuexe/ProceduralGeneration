using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemScript : MonoBehaviour
{
    public GameObject frame;
    public Image image;
    public InventorySlot slot;
    public void Slot(Vector2 size, InventorySlot slot)
    {
        size = GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * slot.item.size.x, size.y * slot.item.size.y);
        frame.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y);

        Vector2 image_size = slot.item.sprite.bounds.size;
        Vector2 image_scale;
        this.slot = slot;

        image.sprite = slot.item.sprite;
        if (slot.item.size.x >= slot.item.size.y)
        {
            if (image_size.x >= image_size.y)
            {
                image_scale = new Vector2(size.x, (image_size.y * size.x) / image_size.x);
            }
            else
            {
                image_scale = new Vector2((image_size.x * size.y) / image_size.y, size.y);
            }
        }
        else
        {
            if (image_size.x >= image_size.y)
            {
                image_scale = new Vector2((image_size.x * size.y) / image_size.y, size.y);
            }
            else
            {
                image_scale = new Vector2(size.x, (image_size.y * size.x) / image_size.x);
            }
        }

        image.GetComponent<RectTransform>().sizeDelta = image_scale;
    }
}
