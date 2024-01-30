using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    public GameObject muzzle;
    public InteractableScript interactable;
    private bool lastInteractPrimary;
    public void Shoot(bool b)
    {
        if (!b)
            return;
        var cam = interactable.properties.fpsCamera;
        var ray = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(ray, Camera.main.transform.forward, out hit))
        {
            var selection = hit.transform;
            //var selectedObject = selection.GetComponent<EnemyScript>();
            //if (selectedObject != null)
            //{
            //   selectedObject.health.damage(10);
            //}

        }
    }
}
