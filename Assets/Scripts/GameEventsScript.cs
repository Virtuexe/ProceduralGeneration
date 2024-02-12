using Generation;
using MyArrays;
using PathFinding;
using UnityEngine;
using System.Collections.Generic;

public unsafe class GameEventsScript : MonoBehaviour{
#if UNITY_EDITOR
    public Matrix<PathFindingScript.Node> nodes;
    public Matrix<bool> called = new Matrix<bool>(Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z);
    public TileCoordinates startTileCoordinate;
    public bool findGizmos;
    public List<Vector3> gizmosList = new List<Vector3>();
    public Set<TileCoordinates> gizmosBestPath;
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
        Gizmos.color = Color.white;
        for (int i = 0; i < gizmosList.Count / 2; i++) {
            Gizmos.DrawLine(gizmosList[2 * i], gizmosList[2 * i + 1]);
        }
        if (gizmosBestPath.Length > 0) {
            Gizmos.color = Color.cyan;
            Vector3 lastGizmo = GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.array[0]);
            for (int i = 1; i < gizmosBestPath.Length; i++) {
                Gizmos.DrawLine(lastGizmo, GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.array[i]));
                lastGizmo = GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.array[i]);
            }
        }

        if (!findGizmos) {
            return;
        }
        findGizmos = false;
        for(int i = 0; i < called.Length; i++) {
            (*called[i]) = false;
        }
        gizmosList.Clear();
        int index = PathFindingScript.GetIndex(startTileCoordinate);
        (*called[index]) = true;
        GizmosSpread(startTileCoordinate);
    }
    private void GizmosSpread(TileCoordinates startTileCoordinate) {
        Queue<TileCoordinates> queue = new Queue<TileCoordinates>();
        queue.Enqueue(startTileCoordinate);

        while (queue.Count > 0) {
            TileCoordinates tileCoordinate = queue.Dequeue();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    for (int z = -1; z <= 1; z++) {
                        TileCoordinates currentTileCoordinates = new TileCoordinates(tileCoordinate.coordinates, tileCoordinate.tiles + new Vector3Int(x, y, z));

                        if (Layers.generation.IsLocationOutOfBounds(currentTileCoordinates.coordinates)) {
                            continue;
                        }
                        int index = PathFindingScript.GetIndex(currentTileCoordinates);
                        if (nodes.isOutOfBounds(index)) {
                            continue;
                        }
                        if (!*called[index]) {
                            *called[index] = true;
                            AddGizmos(index);

                            // Instead of calling GizmosSpread, enqueue the new tile coordinates
                            queue.Enqueue(currentTileCoordinates);
                        }
                    }
                }
            }
        }
    }
    private void AddGizmos(int index) {
        if (nodes.array[index].parentDirection == new Direction().Value) {
            return;
        }
        Vector3 realCoordinates = GenerationProp.TileCoordinatesToRealCoordinates(nodes.array[index].tileCoordinates);
        gizmosList.Add(realCoordinates);
        gizmosList.Add(realCoordinates + Vector3.Scale(nodes.array[index].parentDirection, GenerationProp.tileSize));
    }
#endif
}