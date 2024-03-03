using Generation;
using MyArrays;
using PathFinding;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public unsafe class NPCScript {
	public static GameObject EnemyPrefab;
	public static List<NPCScript> NPCs = new List<NPCScript>();
	public static NPCScript Spawn(Vector3 coordinates) {
		GameObject npc = GameObject.Instantiate(EnemyPrefab, coordinates, new Quaternion());
		NPCScript npcScript = new NPCScript();
		npcScript.entityScript = npc.GetComponent<EntityScript>();
		NPCs.Add(npcScript);
		return npcScript;
	} 

	public EntityScript entityScript;
	private Pool<TileCoordinates> path;
	private bool hasSomewhereToGo;
	public void Go(TileCoordinates tileCoordinate) {
		path = PathFindingScript.FindPath(GenerationProp.RealCoordinatesToTileCoordinates(entityScript.transform.position),tileCoordinate);
		hasSomewhereToGo = true;
	}
	private float elapsedTime = 0.0f;
	public void Tick() {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= 0.1f) {
			if (TileCoordinates.Distance(GenerationProp.playerTileCoordinates, GenerationProp.RealCoordinatesToTileCoordinates(entityScript.transform.position)) < 10) {
				Go(GenerationProp.playerTileCoordinates);
				elapsedTime = 0;
			}
		}
		if (hasSomewhereToGo) {
			Look();
			Move();
		}
	}
	private void Move() {
		TileCoordinates tileCoordinates = GenerationProp.RealCoordinatesToTileCoordinates(entityScript.transform.position);
		if (path.Count == 0) {
			return;
		}
		if (tileCoordinates == path.Last()) {
			path.Remove();
			if(path.Count == 0) {
				hasSomewhereToGo = false;
				entityScript.Move(new Vector2(0, 0));
				return;
			}
		}
		Vector3 globalDirection = (GenerationProp.TileCoordinatesToRealCoordinates(path.Last()) - entityScript.transform.position).normalized;
		Vector3 localDirection = entityScript.transform.InverseTransformDirection(globalDirection);
		entityScript.Move(new Vector2(localDirection.x, localDirection.z));
	}
	private void Look() {
		Vector3 direction = GenerationProp.TileCoordinatesToRealCoordinates(GenerationProp.playerTileCoordinates) - entityScript.transform.position;
		direction.y = 0;
		entityScript.transform.forward = direction;
	}
}
