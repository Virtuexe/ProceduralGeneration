using Generation;
using System;
using UnityEngine;
[RequireComponent(typeof(GameEventsScript))]
public class GeneratorManagerScript : MonoBehaviour
{
	//SCRIPTS
    [HideInInspector]
    public GameEventsScript gameEventS;

    //GENERATOR MANAGER
    //Game info
    public GameObject player;
    public GameObject npcPrefab;
    public GameObject keyPrefab;
    public GameObject trapDoorPrefab;
    public Material material;
    private bool gameStarted;
    public void Start()
    {
        NPCScript.EnemyPrefab = npcPrefab;
        GenerationProp.player = player.GetComponent<PlayerScript>();
        GenerationProp.keyPrefab = keyPrefab;
        GenerationProp.trapDoorPrefab = trapDoorPrefab;

        GenerationProp.transform = transform;
		MeshScript.mat = material;

		GameEventsScript gameEvent = gameObject.GetComponent<GameEventsScript>();

		gameEventS = gameEvent;

		//Game info
		_playerChunk = PlayerChunk();
		ChunkArray.coordinates = PlayerChunk();

        GameEventsScript.StartLevel();
    }
    public void Update() {
        Vector3Int playerTravelDistance = PlayerTravelDistance();
        //chunks
        if (playerTravelDistance != Vector3Int.zero)
        {
            ChunkArray.MoveChunks(playerTravelDistance);
        }
		GenerationProp.playerTileCoordinates = GenerationProp.RealCoordinatesToTileCoordinates(player.transform.position);
        GenerationProp.GenerateChunks();
    }

    Vector3Int playerChunk;
    public Vector3Int PlayerChunk()
    {
        Vector3 relativePosition = player.transform.position - transform.position;
        int xOffset = Mathf.RoundToInt(relativePosition.x / GenerationProp.chunkSize.x);
        int yOffset = Mathf.RoundToInt(relativePosition.y / GenerationProp.chunkSize.y);
        int zOffset = Mathf.RoundToInt(relativePosition.z / GenerationProp.chunkSize.z);
        playerChunk = new Vector3Int(xOffset, yOffset, zOffset);
        return playerChunk;
    }
    Vector3Int _playerChunk;
    public Vector3Int PlayerTravelDistance()
    {
        playerChunk = PlayerChunk();
        Vector3Int playerTravelDistance = playerChunk - _playerChunk;
        _playerChunk = playerChunk;
        return playerTravelDistance;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        for (int y = -Layers.render.radius.y; y <= Layers.render.radius.y; y++)
        {
            for (int z = -Layers.render.radius.z; z <= Layers.render.radius.z; z++)
            {
                for (int x = -Layers.render.radius.x; x <= Layers.render.radius.x; x++)
                {
                    Gizmos.color = Color.red;
                    //leftDownBack is -1/2,-1/2,-1/2
                    Vector3 leftDownBack = new Vector3(pos.x + x * GenerationProp.tileAmount.x * GenerationProp.tileSize.x - (GenerationProp.tileAmount.x * GenerationProp.tileSize.x / 2), pos.y + y * GenerationProp.tileAmount.y * GenerationProp.tileSize.y - (GenerationProp.tileAmount.y * GenerationProp.tileSize.y / 2), pos.z + z * GenerationProp.tileAmount.z * GenerationProp.tileSize.z - (GenerationProp.tileAmount.z * GenerationProp.tileSize.z / 2));
                    Vector3 rightDownBack = leftDownBack + new Vector3(GenerationProp.tileAmount.x * GenerationProp.tileSize.x, 0, 0);
                    Vector3 leftUpBack = leftDownBack + new Vector3(0, GenerationProp.tileAmount.y * GenerationProp.tileSize.y, 0);
                    Vector3 leftDownFront = leftDownBack + new Vector3(0, 0, GenerationProp.tileAmount.z * GenerationProp.tileSize.z);

                    Vector3 rightUpFront = leftDownBack + new Vector3(GenerationProp.tileAmount.x * GenerationProp.tileSize.x, GenerationProp.tileAmount.y * GenerationProp.tileSize.y, GenerationProp.tileAmount.z * GenerationProp.tileSize.z);
                    Vector3 leftUpFront = rightUpFront - new Vector3(GenerationProp.tileAmount.x * GenerationProp.tileSize.x, 0, 0);
                    Vector3 rightDownFront = rightUpFront - new Vector3(0, GenerationProp.tileAmount.y * GenerationProp.tileSize.y, 0);
                    Vector3 rightUpBack = rightUpFront - new Vector3(0, 0, GenerationProp.tileAmount.z * GenerationProp.tileSize.z);

                    Gizmos.DrawLine(leftDownBack, rightDownBack);
                    Gizmos.DrawLine(leftDownBack, leftUpBack);
                    Gizmos.DrawLine(leftDownBack, leftDownFront);

                    Gizmos.DrawLine(rightUpFront, leftUpFront);
                    Gizmos.DrawLine(rightUpFront, rightDownFront);
                    Gizmos.DrawLine(rightUpFront, rightUpBack);

                    Gizmos.DrawLine(leftDownFront, leftUpFront);
                    Gizmos.DrawLine(leftUpFront, leftUpBack);
                    Gizmos.DrawLine(rightDownFront, rightDownBack);
                    Gizmos.DrawLine(leftUpBack, rightUpBack);
                    Gizmos.DrawLine(rightUpBack, rightDownBack);
                    Gizmos.DrawLine(leftDownFront, rightDownFront);
                }
            }
        }
    }
#endif
}