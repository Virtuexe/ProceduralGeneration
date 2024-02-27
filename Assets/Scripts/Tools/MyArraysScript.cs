using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

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
		public void Free() {
			System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)buffer);
		}
		public Pool<T> Copy() {
			Pool<T> copy = new Pool<T>(Length);
			copy.Count = Count;
			for(int i = 0; i < Count; i++) {
				copy.buffer[i] = buffer[i];
			}
			return copy;
		}
		public T this[int index] {
			get {
				return buffer[index];
			}
			set {
				buffer[index] = value;
			}
		}
		public void Add(T item) {
			if (Count >= Length) {
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
			if (index >= Count) {
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
		public T Last() {
			return buffer[Count - 1];
		}
		public T First() {
			return buffer[0];
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
		public Set(int length, T defaultValue) {
			Length = length;
			buffer = (T*)Marshal.AllocHGlobal(sizeof(T) * length);
			for(int i = 0; i < length; i++) {
				buffer[i] = defaultValue;
			}
		}
		public Set(T* array, int length) {
            Length = length;
            this.buffer = array;
        }
        public void Free() {
			Marshal.FreeHGlobal((IntPtr)buffer);
		}
		public T this[int index] {
            get {
				return buffer[index];
			}
			set {
				buffer[index] = value;
			}
		}
        public void Clear() {
            for (int i = 0; i < Length; i++) {
                buffer[i] = new T();
			}
        }
		public void Clear(in T item) {
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
#pragma warning disable CS0660
#pragma warning disable CS0661
    public unsafe struct Set3Int {
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
        private Set3<int> _value;
        public int x { get { return _value.x; } set { _value.x = value; } }
        public int y { get { return _value.y; } set { _value.y = value; } }
        public int z { get { return _value.z; } set { _value.z = value; } }
        public Set3Int(int x, int y, int z) {
            _value = new Set3<int>(x, y, z);
        }
        public int this[int index] {
            get {
                return _value[index];
            }
            set {
                _value[index] = value;
            }
        }
		//Simple Conversions
		public int AsMetrixToLength() {
			return x * y * z;
		}
		public Set3Int AsRadiusToMetrix() {
			return new Set3Int((x * 2) + 1, (y * 2) + 1, (z * 2) + 1);
		}
		public Set3Int IndexToMetrixIndex(int index) {
			Set3Int result = new Set3Int();
			result.z = index % z;
			index /= z;
			result.y = index % y;
			index /= y;
			result.x = index;
			return result;
		}
		public int AsMetrixIndexToIndex(Set3Int metrix) {
			return x * metrix.y * metrix.z + y * metrix.z + z;
		}
		//Nested Conversions
		public int AsRadiusToLength() {
			return new Set3Int((x * 2) + 1, (y * 2) + 1, (z * 2) + 1).AsMetrixToLength();
		}
		public int AsRadiusIndexToIndex(Set3Int radius) {
			return this.AsRadiusToMetrix().AsMetrixIndexToIndex(radius.AsRadiusToMetrix());
		}
		//Other
		public override string ToString() {
			return $"Set3({x},{y},{z})";
		}
		//Calculations
		public static Set3Int operator +(Set3Int v1, Set3Int v2) {
            return new Set3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}
        public static Set3Int operator -(Set3Int v1, Set3Int v2) {
            return new Set3Int(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Set3Int operator *(Set3Int v1, Set3Int v2) {
            return new Set3Int(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
        public static Set3Int operator *(Set3Int v1, int v2) {
            return new Set3Int(v1.x * v2, v1.y * v2, v1.z * v2);
        }
        public static Set3Int operator *(int v1, Set3Int v2) {
            return v2 * v1;
        }
        public static Set3Int operator /(Set3Int v1, Set3Int v2) {
            return new Set3Int(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }
        public static Set3Int operator /(Set3Int v1, int v2) {
            return new Set3Int(v1.x / v2, v1.y / v2, v1.z / v2);
        }
        public static Set3Int operator /(int v1, Set3Int v2) {
            return new Set3Int(v1 / v2.x, v1 / v2.y, v1 / v2.z);
        }
        public static Set3Int operator %(Set3Int v1, Set3Int v2) {
            return new Set3Int(v1.x % v2.x, v1.y % v2.y, v1.z % v2.z);
        }
        public static Set3Int operator -(Set3Int v1) {
            return new Set3Int(-v1.x, -v1.y, -v1.z);
        }
        public static bool operator ==(Set3Int v1, Set3Int v2) {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }
        public static bool operator !=(Set3Int v1, Set3Int v2) {
            return !(v1 == v2);
        }
        public static implicit operator Set3Int(Vector3Int value) {
            return new Set3Int(value.x, value.y, value.z);
		}
        public static explicit operator Vector3Int(Set3Int value) {
            return new Vector3Int(value.x, value.y, value.z);
        }
        public static implicit operator Set3Int(Set3<int> value) {
            return new Set3Int(value.x, value.y, value.z);
        }
		//DEFAULTS
		public static readonly Set3Int zero = new Set3Int(0, 0, 0);
		public static readonly Set3Int forward = new Set3Int(0, 0, 1);
		public static readonly Set3Int back = new Set3Int(0, 0, -1);
		public static readonly Set3Int right = new Set3Int(1, 0, 0);
		public static readonly Set3Int left = new Set3Int(-1, 0, 0);
		public static readonly Set3Int up = new Set3Int(0, 1, 0);
		public static readonly Set3Int down = new Set3Int(0, -1, 0);
        //LOOPS
        public delegate void Set3IntIndexDelegate(Set3Int index3);
        public void Loop(in Set3<int> lengths, in Set3IntIndexDelegate function) {
            Set3Int index = new Set3Int();
            for (index.y = 0; index.y < lengths.y; index.y++) {
                for (index.z = 0; index.z < lengths.z; index.z++) {
                    for (index.x = 0; index.x < lengths.x; index.x++) {
                        function(index);
                    }
                }
            }
        }
        public delegate void Set3IntFlatIndexDelegate(int index);
        public void FlatLoop(in Set3IntFlatIndexDelegate function) {
            int length = _value.x * _value.y * _value.z;
            for (int index = 0; index < length; index++) {
                function(index);
            }
        }
		public delegate void Set3IntPointDelegate(Set3Int index3);
		public void LoopRadius(in Set3IntIndexDelegate function) {
            Set3Int index3 = new Set3Int();
			for (index3.y = -_value.y; index3.y <= _value.y; index3.y++) {
				for (index3.z = -_value.z; index3.z <= _value.z; index3.z++) {
					for (index3.x = -_value.x; index3.x <= _value.x; index3.x++) {
						function(index3);
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
	public unsafe struct Ranges {
		public readonly int from;
		public readonly int to;
		private Pool<int> ranges;
		public int Length {
			get {
				return ranges.Length;
			}
		}
		public int Count {
			get {
				int count = to + 1;
				for (int i = 0; i < ranges.Count; i += 2) {
					count -= ranges[i + 1] + 1 - ranges[i];
				}
				return count;
			}
		}
		public int this[int index] {
			get {
				if (ranges.Count == 0) {
					return this.from + index;
				}
				int indexesTraveld = 0;
				int from = this.from;
				for (int i = 0; i < ranges.Count; i += 2) {
					indexesTraveld += ranges[i] - 1 - from;
					from = ranges[i + 1];
					if (indexesTraveld >= index) {
						return ranges[i] - 1 - (indexesTraveld - index);
					}
				}
				return ranges.Last() + (index - indexesTraveld);
			}
		}
		public Ranges(int min, int max) {
			int length = max - min + 1;
			ranges = new Pool<int>(length + 6);
			from = min;
			to = max;
		}
		public Ranges(int length) {
			ranges = new Pool<int>(length + 6);
			from = 0;
			to = length - 1;
		}
		private void Connect(int firstIndex) {
			bool left = false;
			bool right = false;
			if (firstIndex - 1 >= 0 && ranges[firstIndex] - 1 == ranges[firstIndex - 1]) {
				left = true;
			}
			if (firstIndex + 2 < ranges.Count && ranges[firstIndex + 1] + 1 == ranges[firstIndex + 2]) {
				right = true;
			}

			if (left) {
				ranges.Remove(firstIndex);
				ranges.Remove(firstIndex - 1);
				if (right) {
					ranges.Remove(firstIndex - 1);
					ranges.Remove(firstIndex - 1);
				}
				return;
			}
			if (right) {
				ranges.Remove(firstIndex + 1);
				ranges.Remove(firstIndex + 1);
			}
		}
		private void Disconnect(int firstIndex) {
			bool left = true;
			bool right = true;
			if (firstIndex - 1 >= 0 && ranges[firstIndex] - 1 == ranges[firstIndex - 1]) {
				left = false;
			}
			if (firstIndex + 2 < ranges.Count && ranges[firstIndex + 1] + 1 == ranges[firstIndex + 2]) {
				right = false;
			}

			if (left && right) {
				ranges.Remove(firstIndex);
				ranges.Remove(firstIndex);
			}
		}
		private void Increment(int index) {
			if (index % 2 == 0) {
				if (index + 1 < ranges.Count && ranges[index] == ranges[index + 1]) {
					ranges.Remove(index);
					ranges.Remove(index);
					return;
				}
				ranges[index]++;
				return;
			}
			if (index - 1 < 0 && ranges[index] == ranges[index - 1]) {
				ranges.Remove(index);
				ranges.Remove(index - 1);
				return;
			}
			ranges[index]--;
		}
		public static Ranges operator -(Ranges r1, int v1) {
			if (v1 < r1.from || v1 > r1.to) {
				throw new ArgumentException("value out of range");
			}
			for (int i = 0; i < r1.ranges.Count; i++) {
				if (r1.ranges[i] == v1) {
					goto finished;
				}
				if (r1.ranges[i] > v1) {
					r1.ranges.Insert(i, v1);
					r1.ranges.Insert(i, v1);
					r1.Connect(i);
					goto finished;
				}
			}
			r1.ranges.Add(v1);
			r1.ranges.Add(v1);
			r1.Connect(r1.ranges.LastIndex() - 1);
		finished:
			return r1;
		}
		public static Ranges operator +(Ranges r1, int v1) {
			if (v1 < r1.from || v1 > r1.to) {
				throw new ArgumentException("value out of range");
			}
			for (int i = 0; i < r1.ranges.Count; i++) {
				if (r1.ranges[i] == v1) {
					r1.Increment(i);
					break;
				}
				if (r1.ranges[i] > v1) {
					r1.ranges.Insert(i, v1 + 1);
					r1.ranges.Insert(i, v1 - 1);
					r1.Disconnect(i);
					break;
				}
			}
			return r1;
		}
		public override string ToString() {
			string s = "";
			for (int i = 0; i < ranges.Count; i++) {
				s += ranges[i] + " ";
			}
			return s;
		}
		public void Clear() {

			ranges.Clear();
		}
		public Ranges Copy() {
			Ranges copy = this;
			copy.ranges = ranges.Copy();
			return copy;
		}
		public void Free() {
			ranges.Free();
		}
	}
}
