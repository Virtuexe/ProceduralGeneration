using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(RectTransform))]
[System.Serializable]
public class ButtonScript : MonoBehaviour {
	public Action OnClick;
	public Action OnEnter;
	public Action OnExit;
	public new RectTransform transform;
	private TextMeshProUGUI text;
	public void Init() {
		UIManagerScript.buttons.Add(this);
		transform = gameObject.GetComponent<RectTransform>();
		text = gameObject.GetComponent<TextMeshProUGUI>();
		OnEnter = OnHoverDefault;
		OnExit = OnExitDefault;
	}
	public void Dispose() {
		UIManagerScript.buttons.Remove(this);
	}
	public bool IsMouseIntercapting(Vector2 mousePosition) {
		Vector2 leftBottomCorner = transform.TransformPoint(new Vector2(-transform.rect.width / 2, -transform.rect.height / 2));
		Vector2 rightTopCorner = transform.TransformPoint(new Vector2(transform.rect.width / 2, transform.rect.height / 2));
		return mousePosition.x > leftBottomCorner.x && mousePosition.y > leftBottomCorner.y && mousePosition.x < rightTopCorner.x && mousePosition.y < rightTopCorner.y;
	}
	public void CheckButtonClick() {
		if (OnClick == null || !IsMouseIntercapting(UIManagerScript.mousePosition)) {
			return;
		}
		OnClick();
	}
	private bool entered;
	public void CheckButtonHover() {
		if (IsMouseIntercapting(UIManagerScript.mousePosition)) {
			if (!entered && OnEnter != null) {
				OnEnter();
			}
			entered = true;
			return;
		}
		if (entered && OnExit != null) {
			OnExit();
		}
		entered = false;
	}
	public void ChangeColor(Color color) {
		text.color = color;
	}
	//Defaults
	public void OnHoverDefault() {
		ChangeColor(Color.yellow);
	}
	public void OnExitDefault() {
		ChangeColor(Color.white);
	}
}
