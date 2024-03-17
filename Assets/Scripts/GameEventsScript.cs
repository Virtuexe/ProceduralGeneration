using Generation;
using MyArrays;
using PathFinding;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public unsafe class GameEventsScript : MonoBehaviour{
#if UNITY_EDITOR
    public Matrix<PathFindingScript.Node> nodes;
    public Matrix<bool> called = new Matrix<bool>(Layers.generation.lengthInt, GenerationProp.tileAmount.x, GenerationProp.tileAmount.y, GenerationProp.tileAmount.z);
    public TileCoordinates startTileCoordinate;
    public bool findGizmos;
    public List<Vector3> gizmosList = new List<Vector3>();
    public Set<TileCoordinates> gizmosBestPath;
#endif
	public delegate void GameEndEvent();
	public static GameEndEvent GameEnd;
	public static Task mainTask;
    public static bool playerFoundKey;

    public void Awake()
    {
        GameEnd += End;
		mainTask.minFPS = 100f;
        
#if UNITY_EDITOR
        PathFindingScript.gameEvent = this;
#endif
    }
	public void Start() {
        HudManager.hasKey(false);
    }
	private void Update()
    {
        mainTask.Start();
        for(int i = 0; i < NPCScript.npcs.Count; i++) {
            NPCScript.npcs[i].Tick();
        }
    }
    public static void End() {
        HudManager.hasKey(false);
        NPCScript.npcs.Clear();
        TrapDoor.trapDoors.Clear();
        KeyPickup.instances.Clear();
        ChunkArray.coordinates = Vector3Int.zero;
		NPCScript.chasingNpcAmount = 0;
        if(GenerationProp.score > GenerationProp.highScore) {
            GenerationProp.highScore = GenerationProp.score;
        }
        playerFoundKey = false;
        
    }
    public static void MainMenu() {
        GameEnd?.Invoke();
        SceneManager.LoadScene(1);
    }
    public static void StartLevel() {
        HudManager.hasKey(false);
        CustomRandom rand = new CustomRandom();
        GenerationProp.score += 1;
		GenerationProp.seed += 1;
		rand.SetSeed(GenerationProp.seed);
		MeshScript.mat.color = new Color(rand.Float(0.2f, 0.5f), rand.Float(0.2f, 0.5f), rand.Float(0.2f, 0.5f));

        KeyPickup.DestroyAll();
        NPCScript.DestroyAll();
        TrapDoor.DestroyAll();

		Layers.Regenerate();
		GenerationProp.ForceGenerateChunks();
	}
#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        for (int i = 0; i < gizmosList.Count / 2; i++) {
            Gizmos.DrawLine(gizmosList[2 * i], gizmosList[2 * i + 1]);
        }
        if (gizmosBestPath.Length > 0) {
            Gizmos.color = Color.cyan;
            Vector3 lastGizmo = GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.buffer[0]);
            for (int i = 1; i < gizmosBestPath.Length; i++) {
                Gizmos.DrawLine(lastGizmo, GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.buffer[i]));
                lastGizmo = GenerationProp.TileCoordinatesToRealCoordinates(gizmosBestPath.buffer[i]);
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

                        if (Layers.generation.IsLocationOutOfBounds(Layers.generation.CoordinatesToLayerLocation(currentTileCoordinates.coordinates))) {
                            continue;
                        }
                        try {
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
                        catch {
                            Debug.Log("Err -> " + currentTileCoordinates);
                        }
                    }
                }
            }
        }
    }
    private void AddGizmos(int index) {
        if (nodes.buffer[index].parentDirection == new Direction().Value) {
            return;
        }
        Vector3 realCoordinates = GenerationProp.TileCoordinatesToRealCoordinates(nodes.buffer[index].tileCoordinates);
        gizmosList.Add(realCoordinates);
        gizmosList.Add(realCoordinates + Vector3.Scale(nodes.buffer[index].parentDirection, GenerationProp.tileSize));
    }
#endif
}