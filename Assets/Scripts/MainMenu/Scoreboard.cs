using Generation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public unsafe class Scoreboard : MonoBehaviour {
	public bool highScore;
	string text;
	private void Start() {
		text = GetComponent<TextMeshProUGUI>().text;
		int score = highScore ? GenerationProp.highScore : GenerationProp.score;
		GetComponent<TextMeshProUGUI>().text = text + " " + score;
	}
	public void ShowScore() {
		
	}
}
