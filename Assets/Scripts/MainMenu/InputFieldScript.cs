using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(RectTransform))]
public class InputFieldScript : MonoBehaviour
{
	[SerializeField]
	private InputType inputType;
	private ButtonScript button;
	private void Init() {
		button = gameObject.AddComponent<ButtonScript>();
		button.Init();
	}
}
[System.Serializable]
enum InputType {
	Text,
	Number
}
