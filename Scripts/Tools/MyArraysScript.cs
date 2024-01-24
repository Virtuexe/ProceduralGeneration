using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyArrays {
    public struct Multi<T> {
        public int[] Lenghts { get; private set; }
        private T[] array;
        public int Length {
            get {
                return array.Length;
            }
        }
        public Multi(params int[] lengths) {
            Lenghts = lengths;
            int cumulativeMultiplier = 1;
            for (int i = 0; i < lengths.Length; i++) {
                cumulativeMultiplier *= lengths[i];
            }
            array = new T[cumulativeMultiplier];
        }
        public ref T this[params int[] indexes] {
            get {
                return ref array[GetFlatIndex(ref indexes)];
            }
        }
        public ref T this[Vector3Int vector] {
            get {
                int[] indexes = new int[] { vector.x, vector.y, vector.z };
                return ref array[GetFlatIndex(ref indexes)];
            }
        }
        private int GetFlatIndex(ref int[] indexes) {
            if (indexes.Length != Lenghts.Length) {
                throw new ArgumentException("Incorrect number of indexes.");
            }

            int flatIndex = 0;
            int cumulativeMultiplier = 1;
            for (int i = indexes.Length - 1; i >= 0; i--) {
                if (indexes[i] < 0 || indexes[i] >= Lenghts[i]) {
                    throw new IndexOutOfRangeException("Index out of range for dimension " + i);
                }
                flatIndex += indexes[i] * cumulativeMultiplier;
                cumulativeMultiplier *= Lenghts[i];
            }

            return flatIndex;
        }
    }
    public struct Pool<T> {
        public int Count { get; private set; }
        public int Length {
            get {
                return array.Length;
            }
        }
        private T[] array;
        public Pool(int length) {
            Count = 0;
            array = new T[length];
        }
        public ref T this[int index] {
            get {
                return ref array[index];
            }
        }
        public void Add(T item) {
            if(Count >= Length) {
                throw new ArgumentException("Array is out of space.");
            }
            array[Count] = item;
            Count++;
        }
        public void Remove() {
            if (Count <= 0) {
                throw new ArgumentException("Array is out of space.");
            }
            Count--;
        }
        public void Remove(int index) {
            if (index < 0) {
                throw new IndexOutOfRangeException("Index out of range " + index);
            }
            if(index >= Count) {
                throw new IndexOutOfRangeException("Index out of pool range " + index);
            }
            for (int i = index; i < Count; i++) {
                array[i] = array[i + 1];
            }
            Count--;
        }
        public void Clear() {
            Count = 0;
        }
        public void Insert(int index, T item) {
            if (Count >= Length) {
                throw new ArgumentException("Array is out of space.");
            }
            for (int i = Count - 1; i >= index; i--) {
                array[i + 1] = array[i];
            }
            array[index] = item;
            Count++;
        }
        public bool IsEmpty() {
            return Count == 0;
        }
        public T Last() {
            return array[Count - 1];
        }
        public T First() {
            return array[0];
        }
        public int LastIndex() {
            return Count - 1;
        }
    }
}
