using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour {
	public static List<TrapDoor> trapDoors = new List<TrapDoor>();
	public static void Spawn(Vector3 coordinates) {
		GameObject obj = GameObject.Instantiate(GenerationProp.trapDoorPrefab, coordinates - new Vector3(0, GenerationProp.tileSize.y / 2, 0), new Quaternion());
		trapDoors.Add(obj.GetComponent<TrapDoor>());
	}
	public static void DestroyAll() {
		for (int i = trapDoors.Count - 1; i >= 0; i--) {
			GameObject.Destroy(trapDoors[i].gameObject);
			trapDoors.RemoveAt(trapDoors.Count - 1);
		}
	}
	public void Open(bool b) {
		if (!b) {
			return;
		}
		if (!GameEventsScript.playerFoundKey) {
			return;
		}
		GameEventsScript.playerFoundKey = false;
		GameEventsScript.StartLevel();
	}
}
