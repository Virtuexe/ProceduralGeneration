using MyArrays;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
namespace MyVariables {

	//public static class MyVariables{
	//	public unsafe struct OverflowingInt {
	//		private int _value;
	//		public static OverflowingInt* selectedValue;
	//		public static int value {
	//			get { return selectedValue->_value; }
	//			set {
	//				selectedValue->_value = value;
	//				selectedValue->FixValue();
	//			}
	//		}
	//		private readonly int maxValue;
	//		public int parentValue;

	//		public OverflowingInt(in int maxValue) {
	//			this._value = 0;
	//			this.maxValue = maxValue;
	//			this.parentValue = 0;
	//		}

	//		private void FixValue() {
	//			parentValue += _value / maxValue;
	//			_value = _value % maxValue;
	//			if (_value < 0) {
	//				parentValue--;
	//				_value *= -1;
	//			}
	//		}
	//	}
	//	public unsafe struct GetSetter<T> where T : unmanaged {
	//		public readonly delegate*<T> getter;
	//		public readonly delegate*<T,void> setter;
	//		public GetSetter(delegate*<T> getter, delegate*<T, void> setter) {
	//			this.getter = getter;
	//			this.setter = setter;
	//		}
	//	}
	//	public struct TEST {
	//		private Set3<OverflowingInt> tilesCoordinate;
	//		public Set3<GetSetter<int>> tiles;
	//		public TEST() {

	//		}
	//	}
	//}


}
