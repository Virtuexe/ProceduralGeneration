using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.FilePathAttribute;

public delegate void LayerFunction(int chunk);
[System.Serializable]
public class Layer {
	[HideInInspector]
	public Vector3Int size;
	[HideInInspector]
	public Vector3Int coordinatesOffset;
	[HideInInspector]
	public CreateChunkFunction SetCreateChunk { private get; set; }
	[HideInInspector]
	public DestroyChunkFunction SetDestroyChunk { private get; set; }

	private int[,,] indexes;

	public LayerFunction function;
	public Vector3Int lowerLayerSize; // modifier how many lower layers should be created around edges this
	public Vector3Int Length => size * 2 + Vector3Int.one;
	public int LengthInt => Length.x * Length.y * Length.z;
	public Vector3Int FirstIndex => coordinatesOffset;
	public Vector3Int LastIndex => coordinatesOffset + Length - Vector3Int.one;
	public void Init() {
		indexes = new int[Length.x, Length.y, Length.z];
		for (int z = 0; z < Length.z; z++)
			for (int y = 0; y < Length.y; y++)
				for (int x = 0; x < Length.x; x++) {
					indexes[x, y, z] = x + y * Length.x + z * Length.x * Length.y;
				}
	}
	public void CallCreateChunk(Vector3Int location) {
		int index = indexes[location.x, location.y, location.z];
		SetCreateChunk(index);
	}
	public void CallDestroyChunk(Vector3Int location) {
		int index = indexes[location.x, location.y, location.z];
		SetCreateChunk(index);
	}
	public void MoveChunk(Vector3Int location, Vector3Int targetLocation) {
		int index = indexes[location.x, location.y, location.z];
		int targetIndex = indexes[targetLocation.x, targetLocation.y, targetLocation.z];

		indexes[targetLocation.x, targetLocation.y, targetLocation.z] = index;
		indexes[location.x, location.y, location.z] = targetIndex;
	}

	public int GetIndex(Vector3Int location) {
		
		try {
			return indexes[location.x, location.y, location.z];
		}
		catch {
			Debug.Log("err: " + location);
			return indexes[location.x, location.y, location.z];
		}
		
	}
	public int GetIndex(Vector3Int location, Layer layer) {
		Debug.Log("offset here is " + coordinatesOffset + "so coordinates are " + ToCoordinates(location) + " and location in other layer is" + layer.ToLocation(ToCoordinates(location)));
		return layer.GetIndex(layer.ToLocation(ToCoordinates(location)));
	}
	public Vector3Int ToCoordinates(Vector3Int location) {
		return location + coordinatesOffset;
	}
	public Vector3Int ToLocation(Vector3Int coordinates) {
		return coordinates - coordinatesOffset;
	}
	public bool IsLocationIncluded(Vector3Int location) {
		return location.x >= FirstIndex.x && location.y >= FirstIndex.y && location.z >= FirstIndex.z && location.x <= LastIndex.x && location.y <= LastIndex.y && location.z <= LastIndex.z;
	}
}
[System.Serializable]
public class Layers
{
	public Vector3Int finallLayerSize; //fake lowerLayerSize above finalle layer
	public Layer render;
	public Layer path;
	public Layer generation;
	public Layer[] hierarchy { get; private set; }
	public void Init() {
		hierarchy = new Layer[] { new Layer(),generation, path, render }; //atleast one item

		
		hierarchy[hierarchy.Length - 1].size = finallLayerSize;
		for (int layer = hierarchy.Length - 1; layer > 0; layer--) {
			hierarchy[layer - 1].size += hierarchy[layer].lowerLayerSize + hierarchy[layer].size;
		}
		Vector3Int size = hierarchy[0].size;
		hierarchy[0].Init();
		for (int layer = 1; layer < hierarchy.Length; layer++) {
			hierarchy[layer].coordinatesOffset = size - hierarchy[layer].size;
			hierarchy[layer].Init();
		}
	}
}