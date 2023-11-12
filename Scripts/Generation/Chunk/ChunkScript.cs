using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    public GeneratorManagerScript manager;

    public RoomScript room;
    int doorSeedModifier = 1;
    void Start()
    {
    }
    public IEnumerator GenerateChunk(GenerationChunk chunk)
    {
        //generate doors TEST
        List<GenerationDoor> doors = new List<GenerationDoor>();
        doors.Add(new GenerationDoor(GenerateDoors(chunk.coordinates, new Vector3Int(chunk.coordinates.x, chunk.coordinates.y, chunk.coordinates.z + 1)) + new Vector3Int(0, 0, manager.chunkTilesWidth - 1), Position.Front));
        doors.Add(new GenerationDoor(GenerateDoors(new Vector3Int(chunk.coordinates.x, chunk.coordinates.y, chunk.coordinates.z - 1), chunk.coordinates), Position.Back));

        doors.Add(new GenerationDoor(GenerateDoors(chunk.coordinates, new Vector3Int(chunk.coordinates.x + 1, chunk.coordinates.y, chunk.coordinates.z)) + new Vector3Int(manager.chunkTilesWidth - 1, 0, 0), Position.Right));
        doors.Add(new GenerationDoor(GenerateDoors(new Vector3Int(chunk.coordinates.x - 1, chunk.coordinates.y, chunk.coordinates.z), chunk.coordinates), Position.Left));

        doors.Add(new GenerationDoor(GenerateDoors(chunk.coordinates, new Vector3Int(chunk.coordinates.x, chunk.coordinates.y + 1, chunk.coordinates.z)) + new Vector3Int(0, manager.chunkTilesHeight - 1, 0), Position.Top));
        doors.Add(new GenerationDoor(GenerateDoors(new Vector3Int(chunk.coordinates.x, chunk.coordinates.y - 1, chunk.coordinates.z), chunk.coordinates), Position.Buttom));
        //instance of Room
        bool[,,] roomsPos = new bool[manager.chunkTilesWidth, manager.chunkTilesHeight, manager.chunkTilesWidth];
        GenerationRoom roomObject = new GenerationRoom(roomsPos, doors);

        ///TEST:
        //instatiate gameobject
        var g = new GameObject("Chunk " + chunk.coordinates.x + "," + chunk.coordinates.y + "," + chunk.coordinates.z);
        g.AddComponent<RoomScript>();
        g.transform.parent = transform;
        g.transform.localPosition = new Vector3(chunk.coordinates.x * manager.tileWidth * manager.chunkTilesWidth, chunk.coordinates.y * manager.tileHeight * manager.chunkTilesHeight, chunk.coordinates.z * manager.tileWidth * manager.chunkTilesWidth);
        chunk.gameObject = g;
        //generate room
        for (int x = 0; x < roomObject.tiles.GetLength(0); x += 1)
        {
            for (int y = 0; y < roomObject.tiles.GetLength(1); y += 1)
            {
                for (int z = 0; z < roomObject.tiles.GetLength(2); z += 1)
                {
                    roomObject.tiles[x, y, z] = true;
                }
            }
        }
        roomObject.tiles[5, 1, 5] = false;
        roomObject.tiles[6, 1, 5] = false;
        var roomGameObject = room.CreateRoom(roomObject);
        roomGameObject.transform.parent = g.transform;
        roomGameObject.transform.localPosition = Vector3.zero;
        yield return null;
    }
    public Vector3Int GenerateDoors(Vector3Int pos1, Vector3Int pos2)
    {
        //random
        CustomRandom random = new CustomRandom(manager.seed);
        int[] array = { doorSeedModifier, pos1.x, pos1.y, pos1.z };
        random.Modifier(array);
        //door pos
        Vector3Int availableVecotrs = pos1 - pos2 + new Vector3Int(1, 1, 1);
        return
            new Vector3Int(random.random.Next(0, manager.chunkTilesWidth) * availableVecotrs.x, random.random.Next(0, manager.chunkTilesHeight) * availableVecotrs.y, random.random.Next(0, manager.chunkTilesWidth) * availableVecotrs.z);

    }
    public void KillChunk(GenerationChunk c)
    {
        Destroy(c.gameObject);
    }

    public class GenerationChunk
    {
        public Vector3Int coordinates;
        public GameObject gameObject;

        public GenerationChunk(Vector3Int coordinates, GameObject gameObject)
        {
            this.coordinates = coordinates;
            this.gameObject = gameObject;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (manager == null) return;
        Vector3 pos = transform.position;
        int y = 0;
        for(int z = -5;z<=5;z++)
        {
            for (int x = -5; x <= 5; x++)
            {
                Gizmos.color = Color.red;
                //leftDownBack is 0,0,0
                Vector3 leftDownBack = new Vector3(pos.x+x*manager.chunkTilesWidth*manager.tileWidth,pos.y+y * manager.chunkTilesHeight * manager.tileHeight, pos.z+z * manager.chunkTilesWidth * manager.tileWidth);
                Vector3 rightDownBack = leftDownBack+new Vector3(manager.chunkTilesWidth * manager.tileWidth,0,0);
                Vector3 leftUpBack = leftDownBack+new Vector3(0, manager.chunkTilesHeight * manager.tileHeight, 0);
                Vector3 leftDownFront = leftDownBack+new Vector3(0, 0, manager.chunkTilesWidth * manager.tileWidth);

                Vector3 rightUpFront = leftDownBack + new Vector3(manager.chunkTilesWidth * manager.tileWidth, manager.chunkTilesHeight * manager.tileHeight, manager.chunkTilesWidth * manager.tileWidth);
                Vector3 leftUpFront = rightUpFront - new Vector3(manager.chunkTilesWidth * manager.tileWidth, 0,0);
                Vector3 rightDownFront = rightUpFront - new Vector3(0, manager.chunkTilesHeight * manager.tileHeight, 0);
                Vector3 rightUpBack = rightUpFront - new Vector3(0, 0, manager.chunkTilesWidth * manager.tileWidth);

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
#endif
}
