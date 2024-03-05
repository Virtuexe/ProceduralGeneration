using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour {
	public static List<KeyPickup> instances = new List<KeyPickup>();
	static KeyPickup() {
		GameEventsScript.GameEnd += End;
	}
	static void End() {
		instances.Clear();
	}
	public static void Spawn(Vector3 coordinates) {
		if (GameEventsScript.playerFoundKey) {
			return;
		}
		GameObject obj = Object.Instantiate(GenerationProp.keyPrefab, coordinates, new Quaternion());
		instances.Add(obj.GetComponent<KeyPickup>());
	}
	public static void DestroyAll() {
		for (int i = 0; i < instances.Count; i++) {
			Destroy(instances[i].gameObject);
			instances.RemoveAt(i);
		}
	}
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			GameEventsScript.playerFoundKey = true;
			DestroyAll();
		}
	}
}
