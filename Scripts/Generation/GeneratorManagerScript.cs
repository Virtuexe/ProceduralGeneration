using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneratorManagerScript : MonoBehaviour
{
	[SerializeField]
	public GenerationProp prop;
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
    public void Init()
    {
        //Game info
        _playerChunk = PlayerChunk();
		prop.chunkArray.coordinates = PlayerChunk();
    }
    public void Tick()
    {
        Vector3Int playerTravelDistance = PlayerTravelDistance();
        //chunks
        if (playerTravelDistance != Vector3Int.zero)
        {
            prop.chunkArray.MoveChunks(playerTravelDistance);
        }
        GenerateChunks();
    }

    Vector3Int playerChunk;
    public Vector3Int PlayerChunk()
    {
        Vector3 relativePosition = player.transform.position - transform.position;
        int xOffset = Mathf.RoundToInt(relativePosition.x / prop.chunkSize.x);
        int yOffset = Mathf.RoundToInt(relativePosition.y / prop.chunkSize.y);
        int zOffset = Mathf.RoundToInt(relativePosition.z / prop.chunkSize.z);
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
		Vector3Int locationGeneration = Vector3Int.zero;
        for (locationGeneration.y = 0; locationGeneration.y < prop.chunkArray.chunksGeneration.layer.Length.y; locationGeneration.y++)
            for (locationGeneration.z = 0; locationGeneration.z < prop.chunkArray.chunksGeneration.layer.Length.z; locationGeneration.z++)
                for (locationGeneration.x = 0; locationGeneration.x < prop.chunkArray.chunksGeneration.layer.Length.x; locationGeneration.x++) {
                    int chunkGeneration = prop.chunkArray.chunksGeneration.layer.GetIndex(locationGeneration);
					if (prop.chunkArray.chunksGeneration.genereted[chunkGeneration])
                        continue;
                    generationS.GenerateChunk(locationGeneration);
                    if (GameEventsScript.mainTask.OutOfTime())
                        return;
                }
        Vector3Int locationRender = Vector3Int.zero;
        for (locationRender.y = 0; locationRender.y < prop.chunkArray.chunksRender.layer.Length.y; locationRender.y++)
            for (locationRender.z = 0; locationRender.z < prop.chunkArray.chunksRender.layer.Length.z; locationRender.z++)
                for (locationRender.x = 0; locationRender.x < prop.chunkArray.chunksRender.layer.Length.x; locationRender.x++) {
                    int renderChunk = prop.chunkArray.chunksRender.layer.GetIndex(locationRender);
                    if (prop.chunkArray.chunksRender.destroy[renderChunk]) {
                        Destroy(prop.chunkArray.chunksRender.gameObject[renderChunk]);
                        prop.chunkArray.chunksRender.destroy[renderChunk] = false;
					}
                    if (prop.chunkArray.chunksRender.rendered[renderChunk])
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
        for (int y = -prop.layers.finallLayerSize.y; y <= prop.layers.finallLayerSize.y; y++)
        {
            for (int z = -prop.layers.finallLayerSize.z; z <= prop.layers.finallLayerSize.z; z++)
            {
                for (int x = -prop.layers.finallLayerSize.x; x <= prop.layers.finallLayerSize.x; x++)
                {
                    Gizmos.color = Color.red;
                    //leftDownBack is -1/2,-1/2,-1/2
                    Vector3 leftDownBack = new Vector3(pos.x + x * prop.tileAmmount.x * prop.tileSize.x - (prop.tileAmmount.x * prop.tileSize.x / 2), pos.y + y * prop.tileAmmount.y * prop.tileSize.y - (prop.tileAmmount.y * prop.tileSize.y / 2), pos.z + z * prop.tileAmmount.z * prop.tileSize.z - (prop.tileAmmount.z * prop.tileSize.z / 2));
                    Vector3 rightDownBack = leftDownBack + new Vector3(prop.tileAmmount.x * prop.tileSize.x, 0, 0);
                    Vector3 leftUpBack = leftDownBack + new Vector3(0, prop.tileAmmount.y * prop.tileSize.y, 0);
                    Vector3 leftDownFront = leftDownBack + new Vector3(0, 0, prop.tileAmmount.z * prop.tileSize.z);

                    Vector3 rightUpFront = leftDownBack + new Vector3(prop.tileAmmount.x * prop.tileSize.x, prop.tileAmmount.y * prop.tileSize.y, prop.tileAmmount.z * prop.tileSize.z);
                    Vector3 leftUpFront = rightUpFront - new Vector3(prop.tileAmmount.x * prop.tileSize.x, 0, 0);
                    Vector3 rightDownFront = rightUpFront - new Vector3(0, prop.tileAmmount.y * prop.tileSize.y, 0);
                    Vector3 rightUpBack = rightUpFront - new Vector3(0, 0, prop.tileAmmount.z * prop.tileSize.z);

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