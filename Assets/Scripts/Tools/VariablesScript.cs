namespace MyVariables {
	public unsafe struct OverflowingInt {
		private int _value;
		public int value {
			get {
				return _value;
			}
			set {
				_value = value;
				FixValue();
			}
		}
		private readonly int* maxValue;
		public int parentValue;

		public OverflowingInt(int* maxValue) {
			this._value = 0;
			this.maxValue = maxValue;
			this.parentValue = 0;
		}
		private void FixValue() {
			parentValue += _value / *maxValue;
			_value = _value % *maxValue;
			if (_value < 0) {
				parentValue--;
				_value *= -1;
			}
		}
	}
}