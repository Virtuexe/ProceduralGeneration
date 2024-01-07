using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	[SerializeField]
	public ButtonScript startButton;
	private void Start() {
		startButton.Init();
		startButton.OnClick += StartGame;
		//GenerationButton.OnClick += StartGame;
	}
	private void Update() {
		UIManagerScript.Tick();
	}
	public void StartGame() {
		SceneManager.UnloadScene(0);
		SceneManager.LoadScene(1);
	}
}
