using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlinkScript
{
	[SerializeField]
	RectTransform blinkUp;
	[SerializeField]
	RectTransform blinkDown;
	public float blinkSpeed;
	public bool blinking { get; private set; }
	public void Close() {
		Vector2 posUp = blinkUp.transform.position;
		blinkUp.anchorMin = new Vector2(0.5f, 0.5f);
		blinkUp.anchorMax = new Vector2(0.5f, 0.5f);
		blinkUp.transform.position = posUp;

		Vector2 posDown = blinkDown.transform.position;
		blinkDown.anchorMin = new Vector2(0.5f, 0.5f);
		blinkDown.anchorMax = new Vector2(0.5f, 0.5f);
		blinkDown.transform.position = posDown;

		blinking = true;
	}
	public void Open() {
		Vector2 posUp = blinkUp.transform.position;
		blinkUp.anchorMin = new Vector2(0.5f, 1);
		blinkUp.anchorMax = new Vector2(0.5f, 1);
		blinkUp.transform.position = posUp;

		Vector2 posDown = blinkDown.transform.position;
		blinkDown.anchorMin = new Vector2(0.5f, 0);
		blinkDown.anchorMax = new Vector2(0.5f, 0);
		blinkDown.transform.position = posDown;

		blinking = true;
	}
	public void Tick() {
		if (!blinking) {
			return;
		}
		if (blinkDown.anchoredPosition == Vector2.zero) {
			blinking = false;
			return;
		}
		float step = blinkSpeed * Time.deltaTime;
		blinkUp.anchoredPosition = Vector2.MoveTowards(blinkUp.anchoredPosition, Vector2.zero, step);
		blinkDown.anchoredPosition = Vector2.MoveTowards(blinkDown.anchoredPosition, Vector2.zero, step);
	}
}
