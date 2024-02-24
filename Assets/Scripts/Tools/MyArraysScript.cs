using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyArrays {
    public unsafe struct Matrix<T> where T : unmanaged {
        public int[] Lenghts { get; private set; }
		[MarshalAs(UnmanagedType.R4)]
		public T* buffer;
		public int Length { get; private set; }
		public Matrix(params int[] lengths) {
            Lenghts = lengths;
            int cumulativeMultiplier = 1;
            for (int i = 0; i < lengths.Length; i++) {
                cumulativeMultiplier *= lengths[i];
            }
            buffer = (T*)Marshal.AllocHGlobal(sizeof(T) * cumulativeMultiplier);
            Length = cumulativeMultiplier;
		}
		public void Free() {
			Marshal.FreeHGlobal((IntPtr)buffer);
		}
		public T* this[int index] {
			get{
				return &buffer[index];
			}
		}
		public T* this[params int[] indexes] {
            get {
                return &buffer[GetFlatIndex(ref indexes)];
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
                return ref buffer[GetFlatIndex(ref indexes)];
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
        public bool isOutOfBounds(params int[] indexes) {
            int index = GetFlatIndex(ref indexes);
            return isOutOfBounds(index);
        }
        public bool isOutOfBounds(int index) {
            if(index < 0) {
                return true;
            }
            int cumulativeMultiplier = 1;
            for (int i = 0; i < Lenghts.Length; i++) {
                cumulativeMultiplier *= Lenghts[i];
            }
            if(index >= cumulativeMultiplier) {
                return true;
            }
            return false;
        }
    }
    public unsafe struct Pool<T> where T : unmanaged {
        public int Count { get; private set; }
        public int Length { get; private set; }
        public T* buffer;
        public Pool(int length) {
            Count = 0;
            Length = length;
			buffer = (T*)Marshal.AllocHGlobal(sizeof(T) * length);
		}
		public Pool(int length, T* buffer, int count) {
			Count = count;
			Length = length;
			this.buffer = buffer;
		}
		public void Free(){
			System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)buffer);
		}
        public T* this[int index] {
            get {
                return &buffer[index];
            }
        }
        public void Add(T item) {
            if(Count >= Length) {
                throw new ArgumentException("Array is out of space.");
            }
            buffer[Count] = item;
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
                buffer[i] = buffer[i + 1];
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
                buffer[i + 1] = buffer[i];
            }
            buffer[index] = item;
            Count++;
        }
        public bool IsEmpty() {
            return Count == 0;
        }
        public T* Last() {
            return &buffer[Count - 1];
        }
        public T* First() {
            return &buffer[0];
        }
        public int LastIndex() {
            return Count - 1;
        }
    }
	public unsafe struct Set<T> where T : unmanaged {
		public int Length { get; private set; }
        public T* buffer;
		public Set(int length) {
			Length = length;
			buffer = (T*)Marshal.AllocHGlobal(sizeof(T) * length);
		}
        public Set(T* array, int length) {
            Length = length;
            this.buffer = array;
        }
        public void Free() {
			Marshal.FreeHGlobal((IntPtr)buffer);
		}
		public T* this[int index] {
            get {
				return &buffer[index];
			}
		}
        public void Clear() {
            for (int i = 0; i < Length; i++) {
                buffer[i] = new T();
			}
        }
		public void Clear(T item) {
			for (int i = 0; i < Length; i++) {
				buffer[i] = item;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Set3<T> where T : unmanaged {
        public T x;
		public T y;
		public T z;
        public Set3(T x, T y, T z) {
            this.x = x;
            this.y = y;
            this.z = z;
		}
        public T this[int index] {
            get {
                fixed (T* ptr = &x) {
					return ptr[index];
				}
			}
			set {
				fixed (T* ptr = &x) {
					ptr[index] = value;
				}
			}
		}
	}
    public static class Set3Int {
        //LOOPS
        public delegate void Set3IntIndexDelegate(int x, int y, int z);
        public static void Loop(in Set3<int> lengths, in Set3IntIndexDelegate function) {
            for (int y = 0; y < lengths.y; y++) {
                for (int z = 0; z < lengths.z; z++) {
                    for (int x = 0; x < lengths.x; x++) {
                        function(x, y, z);
                    }
                }
            }
        }
        public delegate void Set3IntFlatIndexDelegate(int index);
        public static void FlatLoop(in Set3<int> lengths, in Set3IntFlatIndexDelegate function) {
            int length = lengths.x * lengths.y * lengths.z;
            for (int index = 0; index < length; index++) {
                function(index);
            }
        }
		public delegate void Set3IntPointDelegate(int x, int y, int z);
		public static void LoopCenter(in Set3<int> lengthsFromCenter, in Set3IntIndexDelegate function) {
			for (int y = -lengthsFromCenter.y; y <= lengthsFromCenter.y; y++) {
				for (int z = -lengthsFromCenter.z; z <= lengthsFromCenter.z; z++) {
					for (int x = -lengthsFromCenter.x; x <= lengthsFromCenter.x; x++) {
						function(x, y, z);
					}
				}
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
    public unsafe struct Range<T> where T : unmanaged {
        public T min;
        public T max;
        public Range(T min, T max) {
            this.min = min;
            this.max = max;
        }
        public T this[int index] {
            get {
                fixed (T* ptr = &min) {
                    return ptr[index];
                }
            }
            set {
                fixed (T* ptr = &min) {
                    ptr[index] = value;
                }
            }
        }
    }
}
