using JetBrains.Annotations;

namespace MyVariables {
	public unsafe struct OverflowingInt {
		private int _value;
		public GetSet<int> value;
		private static void SetValue(int* pointer, int value) {
			*pointer = value;
			
		}
		private static void SetValue(int* pointer, int value) {
			*pointer = value;
		}
		private readonly int* maxValue;
		public int parentValue;

		public OverflowingInt(in int* maxValue) {
			fixed(int* p = &_value) {
				this._value = 0;
				this.value = new GetSet<int>(p);
				this.maxValue = maxValue;
				this.parentValue = 0;
			}
		}
		
		private static void FixValue() {
			parentValue += _value / *maxValue;
			_value = _value % *maxValue;
			if (_value < 0) {
				parentValue--;
				_value *= -1;
			}
		}
	}
	public unsafe struct Point<T> where T : unmanaged {
		public readonly T* pointer;
		public Point(T* pointer) {
			this.pointer = pointer;
		}
	}

	public unsafe struct GetSet<T> where T : unmanaged  {
		public readonly T* pointer;
		private readonly delegate*<T*, T> getter;
		private readonly delegate*<T*, T, void> setter;
		public GetSet(T* pointer, delegate*<T*, T> getter, delegate*<T*, T, void> setter) {
			this.pointer = pointer;
			this.getter = getter;
			this.setter = setter;
		}
		public T Get() {
			return getter(pointer);
		}
		public void Set(T value) {
			setter(pointer, value);
		}
	}
}