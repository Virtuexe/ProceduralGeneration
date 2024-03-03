using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour {
	public BlinkScript blink;
	private void Update() {
		blink.Tick();
	}
}
