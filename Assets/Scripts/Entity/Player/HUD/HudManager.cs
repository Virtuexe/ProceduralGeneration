using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour {
	public BlinkScript blink;
	public static GameObject key;
	public GameObject keyIcon;
	public void Awake() {
		key = keyIcon;
	}
	private void Update() {
		blink.Tick();
	}
	public static void hasKey(bool b) {
		key.SetActive(b);
	}
}
