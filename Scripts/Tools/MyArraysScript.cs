using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyArrays {
    public unsafe struct Matrix<T> where T : unmanaged {
        public int[] Lenghts { get; private set; }
        private T* array;
		public int Length { get; private set; }
		public Matrix(params int[] lengths) {
            Lenghts = lengths;
            int cumulativeMultiplier = 1;
            for (int i = 0; i < lengths.Length; i++) {
                cumulativeMultiplier *= lengths[i];
            }
            array = (T*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(T) * cumulativeMultiplier);
            Length = cumulativeMultiplier;
		}
		void Dispose() {
			System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)array);
		}
		public ref T this[int index] {
			get{
				return ref array[index];
			}
		}
		public ref T this[params int[] indexes] {
            get {
                return ref array[GetFlatIndex(ref indexes)];
            }
        }
        public ref T this[params Vector3Int[] vectors] {
            get {
                int[] indexes = new int[vectors.Length * 3];
                for(int i = 0; i < vectors.Length; i++) {
                    indexes[i * 3 + 0] = vectors[i].x;
					indexes[i * 3 + 1] = vectors[i].y;
					indexes[i * 3 + 2] = vectors[i].z;
				}
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
    public unsafe struct Pool<T> where T : unmanaged {
        public int Count { get; private set; }
        public int Length { get; private set; }
		private T* array;
        public Pool(int length) {
            Count = 0;
            Length = length;
			array = (T*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(T) * length);
		}
		public void Dispose(){
			System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)array);
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
