using System;
using Random = System.Random;
using UnityEngine;
using Generation;
using MyArrays;

public class CustomRandom {
	private Random rand = new Random();
	public double RandomValue() {
		return rand.NextDouble();
	}
	public bool Simple(int chanceInPercentages) {
		return chanceInPercentages > rand.Next(0,101);
	}
	public int Index(int length) {
		return rand.Next(length);
	}
	public int Number(int min, int max) {
		return rand.Next(min, max + 1);
	}
	public void SetSeed(params int[] numbers) {
		rand = new Random(HashCoordinates(numbers));
	}
	public int ChooseFromRange(Ranges ranges) {
		int index = Number(0, ranges.Count - 1);;
		return ranges[index];
	}
	private int HashCoordinates(params int[] state) {
		int hash = GenerationProp.seed;
		for(int i = 0; i < state.Length; i++) {
			hash = hash * 31 + state[i];
		}
		return hash;
	}
}
