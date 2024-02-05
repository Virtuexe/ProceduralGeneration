using Generation;
using PathFinding;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
	static public EntityScript entityScript;
	public void Go(TileCoordinates tileCoordinate) {
		PathFindingScript.FindPath(GenerationProp.RealCoordinatesToTileCoordinates(transform.position),tileCoordinate);
	}
}
