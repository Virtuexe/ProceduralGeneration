using Generation;
using System;
using UnityEngine;
[RequireComponent(typeof(GeneratorManagerScript))]
[RequireComponent(typeof(MeshScript))]
[RequireComponent(typeof(ChunkScript))]
[RequireComponent(typeof(GenerationScript))]
[RequireComponent(typeof(GameEventsScript))]
public class GeneratorManagerScript : MonoBehaviour
{
	//SCRIPTS
	[HideInInspector]
	public GenerationScript generationS;
	[HideInInspector]
	public ChunkScript chunkS;
    [HideInInspector]
    public GameEventsScript gameEventS;
    [HideInInspector]
    public MeshScript meshS;

    //GENERATOR MANAGER
    //Game info
    public GameObject player;
    public void Start()
    {
        GenerationProp.transform = transform;

		MeshScript mesh = gameObject.GetComponent<MeshScript>();
		ChunkScript chunk = gameObject.GetComponent<ChunkScript>();
		GenerationScript generation = gameObject.GetComponent<GenerationScript>();
		GameEventsScript gameEvent = gameObject.GetComponent<GameEventsScript>();
        //manager
		meshS = mesh;
		chunkS = chunk;
		generationS = generation;
		//mesh
		//chunk
		//generation
		//gameEvent
		gameEventS = gameEvent;
        //pathFinding
		//Game info
		_playerChunk = PlayerChunk();
		ChunkArray.coordinates = PlayerChunk();
    }
    public void Update()
    {
        Vector3Int playerTravelDistance = PlayerTravelDistance();
        //chunks
        if (playerTravelDistance != Vector3Int.zero)
        {
            ChunkArray.MoveChunks(playerTravelDistance);
        }
        GenerateChunks();
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
    private void GenerateChunks() {
		Vector3Int locationGenerationDetail = Vector3Int.zero;
		for (locationGenerationDetail.y = 0; locationGenerationDetail.y < Layers.generationDetail.Length.y; locationGenerationDetail.y++)
			for (locationGenerationDetail.z = 0; locationGenerationDetail.z < Layers.generationDetail.Length.z; locationGenerationDetail.z++)
				for (locationGenerationDetail.x = 0; locationGenerationDetail.x < Layers.generationDetail.Length.x; locationGenerationDetail.x++) {
					if (Layers.generationDetail.created[locationGenerationDetail.x, locationGenerationDetail.y, locationGenerationDetail.z])
						continue;
					GenerationDetail.GenerateDetail(locationGenerationDetail);
					if (GameEventsScript.mainTask.OutOfTime())
						return;
				}
		Vector3Int locationGeneration = Vector3Int.zero;
        for (locationGeneration.y = 0; locationGeneration.y < Layers.generation.Length.y; locationGeneration.y++)
            for (locationGeneration.z = 0; locationGeneration.z < Layers.generation.Length.z; locationGeneration.z++)
                for (locationGeneration.x = 0; locationGeneration.x < Layers.generation.Length.x; locationGeneration.x++) {
					if (Layers.generation.created[locationGeneration.x, locationGeneration.y, locationGeneration.z])
                        continue;
                    generationS.GenerateChunk(locationGeneration);
                    if (GameEventsScript.mainTask.OutOfTime())
                        return;
                }
        Vector3Int locationRender = Vector3Int.zero;
        for (locationRender.y = 0; locationRender.y < Layers.render.Length.y; locationRender.y++)
            for (locationRender.z = 0; locationRender.z < Layers.render.Length.z; locationRender.z++)
                for (locationRender.x = 0; locationRender.x < Layers.render.Length.x; locationRender.x++) {
                    int renderChunk = Layers.render.LayerLocationToIndex(locationRender);
                    if (Layers.render.pendingsDestroy[locationRender.x,locationRender.y,locationRender.z]) {
                        Destroy(ChunkArray.gameObject[renderChunk]);
						Layers.render.pendingsDestroy[locationRender.x, locationRender.y, locationRender.z] = false;
					}
                    if (Layers.render.created[locationRender.x, locationRender.y, locationRender.z])
                        continue;
                    chunkS.RenderChunk(locationRender);
                    if (GameEventsScript.mainTask.OutOfTime())
                        return;
                }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        for (int y = -Layers.render.layerSize.y; y <= Layers.render.layerSize.y; y++)
        {
            for (int z = -Layers.render.layerSize.z; z <= Layers.render.layerSize.z; z++)
            {
                for (int x = -Layers.render.layerSize.x; x <= Layers.render.layerSize.x; x++)
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