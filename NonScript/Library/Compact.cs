using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public struct Compact<T>
{
	readonly T[] array;
	readonly int[] dimensions;
	public Compact(params int[] dimensions)
	{
		this.dimensions = dimensions;
		int size = 0;
		for(int i = 0; i < dimensions.Length; i++)
			size += dimensions[i];
		array = new T[size];
	}
	private int Get1DIndex(params int[] indices)
	{
		if (indices.Length != dimensions.Length)
			throw new ArgumentException("Invalid number of indices.");

		int index = 0;
		int multiplier = 1;

		for (int i = 0; i < indices.Length; i++)
		{
			if (indices[i] < 0 || indices[i] >= dimensions[i])
				throw new IndexOutOfRangeException();

			index += indices[i] * multiplier;
			multiplier *= dimensions[i];
		}
		return index;
	}
}
