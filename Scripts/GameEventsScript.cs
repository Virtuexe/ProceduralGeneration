using Generation;
using MyArrays;
using PathFinding;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameEventsScript : MonoBehaviour{
#if UNITY_EDITOR
    public Matrix<PathFindingScript.Node> nodes;
    public Matrix<bool> called = new Matrix<bool>(Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z);
    public Vector3Int startTileCoordinate;
    public bool findGizmos;
    public List<Vector3> gizmosList = new List<Vector3>();
#endif
    public static Task mainTask;
    public void Awake()
    {
        mainTask.minFPS = 100f;
#if UNITY_EDITOR
        PathFindingScript.gameEvent = this;
#endif
    }
    private void Update()
    {
        mainTask.Start();
    }
#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < gizmosList.Count / 2; i++) {
            Gizmos.DrawLine(gizmosList[2 * i], gizmosList[2 * i + 1]);
        }
        if (!findGizmos) {
            return;
        }
        findGizmos = false;
        for(int i = 0; i < called.Length; i++) {
            called[i] = false;
        }
        gizmosList.Clear();
        int index = PathFindingScript.GetIndex(startTileCoordinate);
        called[index] = true;
        GizmosSpread(startTileCoordinate);
    }
    private void GizmosSpread(Vector3Int tileCoordinate) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                for (int z = -1; z <= 1; z++) {
                    Vector3Int currentTileCoordinates = tileCoordinate + new Vector3Int(x, y, z);
                    if (Layers.generation.IsLocationOutOfBounds(GenerationProp.TileCoordinatesToCoordinates(currentTileCoordinates))) {
                        continue;
                    }
                    //Debug.Log("got here");
                    int index = PathFindingScript.GetIndex(currentTileCoordinates);
                    if (nodes.isOutOfBounds(index)) {
                        continue;
                    }
                    //Debug.Log("im so close");
                    if (!called[index]) {
                        called[index] = true;
                        AddGizmos(index);
                        GizmosSpread(currentTileCoordinates);
                    }
                }
            }
        }
    }
    private void AddGizmos(int index) {
        Vector3 realCoordinates = GenerationProp.TileCoordinatesToRealCoordinates(nodes[index].tileCoordinates);
        gizmosList.Add(realCoordinates + (GenerationProp.tileSize / 2));
        gizmosList.Add(realCoordinates + (GenerationProp.tileSize / 2) + Vector3.Scale(nodes[index].parentDirection, GenerationProp.tileSize));
    }
#endif
}