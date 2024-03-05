using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	[SerializeField]
	public ButtonScript startButton;
	private void Start() {
		UIManagerScript.Reset();

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		startButton.Init();
		startButton.OnClick += StartGame;
		//GenerationButton.OnClick += StartGame;
	}
	private void Update() {
		UIManagerScript.Tick();
	}
	public void StartGame() {
		SceneManager.LoadScene(0);
	}
}
