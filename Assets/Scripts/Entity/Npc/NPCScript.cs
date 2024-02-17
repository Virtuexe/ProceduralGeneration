using Generation;
using MyArrays;
using PathFinding;
using Unity.VisualScripting;
using UnityEngine;

public unsafe class NPCScript {
	public static NPCScript Spawn(Vector3 coordinates) {
		Object npc = Object.Instantiate(GameEventsScript.EnemyPrefab, coordinates, new Quaternion());
		NPCScript npcScript = new NPCScript();
		npcScript.entityScript = npc.GetComponent<EntityScript>();
		Debug.Log("ligma: " + npcScript.entityScript);
		GameEventsScript.NPCs.Add(npcScript);
		return npcScript;
	} 
	public EntityScript entityScript;
	private Pool<TileCoordinates> path;
	private bool hasSomewhereToGo;
	public void Go(TileCoordinates tileCoordinate) {
		path = PathFindingScript.FindPath(GenerationProp.RealCoordinatesToTileCoordinates(entityScript.transform.position),tileCoordinate);
		hasSomewhereToGo = true;
	}
	public void Tick() {
		if (hasSomewhereToGo) {
			Move();
		}
	}
	private void Move() {
		TileCoordinates tileCoordinates = GenerationProp.RealCoordinatesToTileCoordinates(entityScript.transform.position);
		if(tileCoordinates == *path.Last()) {
			path.Remove();
			if(path.Count == 0) {
				hasSomewhereToGo = false;
				entityScript.Move(new Vector2(0, 0));
				return;
			}
		}
		Vector3 direction = (GenerationProp.TileCoordinatesToRealCoordinates(*path.Last()) - entityScript.transform.position).normalized;
		entityScript.Move(new Vector2(direction.x, direction.z));
	}
}
