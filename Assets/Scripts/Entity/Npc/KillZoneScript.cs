using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZoneScript : MonoBehaviour {
	private int npcId;
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			PlayerScript player = other.GetComponent<PlayerScript>();
			player.Hurt();
		}
	}
}
