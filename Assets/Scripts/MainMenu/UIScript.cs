using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class UIManagerScript
{
	public static Vector2 mousePosition { get; private set; }
	public static List<ButtonScript> buttons = new List<ButtonScript>();
	static public void Tick() {
		mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		CheckButtons();
	}
	static private void CheckButtons() {
		for (int index = 0; index < buttons.Count; index++) {
			if (Input.GetMouseButtonDown(0)) {
				buttons[index].CheckButtonClick();
			}
			buttons[index].CheckButtonHover();
		}
	}
}
