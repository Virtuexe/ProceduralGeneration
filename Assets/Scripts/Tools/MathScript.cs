using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMath {
	public static class Functions {
		public static int ClosestNumber(int number, int round) {
			int quotient = number / round;

			int closestNum1 = round * quotient;
			int closestNum2 = (number * round) > 0 ? (round * (quotient + 1)) : (round * (quotient - 1));

			if (Math.Abs(number - closestNum1) < Math.Abs(number - closestNum2))
				return closestNum1;
			return closestNum2;
		}
		public static float ScaleDown(float number, float scale) {
			return (number / scale) + (number < 0 ? -1f : 0f);
		}
		public static int PercentageToAmount(int maxAmount, int percentege) {
			return (maxAmount * percentege) / 100;
		}
	}  
}
