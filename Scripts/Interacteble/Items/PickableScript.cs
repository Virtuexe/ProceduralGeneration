using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableScript : MonoBehaviour
{
    public ItemObject item;

    public bool pickingUp;
    public float pickupTime;
    public float currentPickupTime;
    public InteractableScript interactable;
    public List<string> listOfActions = new List<string>();
    void Start()
    {
        currentPickupTime = pickupTime;
    }
    public void PickUp(bool b)
    {
        pickingUp = b;
        interactable.properties.hudInteractBar.SetActive(b);
    }
    void Update()
    {
        if (interactable == null)
            return;
        if (pickingUp)
        {
            var slider = interactable.properties.hudInteractBarSlider.GetComponent<hudBarScript>();
            slider.ReverseScale((100 * currentPickupTime) / pickupTime);
            if (currentPickupTime <= 0)
            {
                Item thisItem = new Item(item);
                interactable.properties.inventory.AddItem(thisItem, 1);
                foreach (Action a in interactable.actions.actionList)
                {
                    a.action.Invoke(false);
                    interactable.properties.actions.actionList.Remove(a);
                }
                Destroy(this.gameObject);
            }
            currentPickupTime -= Time.deltaTime;
        }
        else
        {
            currentPickupTime = pickupTime;
        }
    }

}
