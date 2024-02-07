using System.Runtime.InteropServices;

namespace CustomVariables {
    public unsafe struct EntangledPInt {
        public int* overflow { get; private set; }
        private int number;
        private int maxInteger;
        public EntangledPInt(int maxInteger, int* overflowInteger) {
            number = 0;
            overflow = overflowInteger;
            this.maxInteger = maxInteger;
        }
        public void Set(int value) {
            number = value;
            Fix();
		}
        public static implicit operator int(EntangledPInt value) {
            return value.number;
        }
        private void Fix() {
            *overflow += number / maxInteger + (number < 0 ? -1 : 0);
            number += number % maxInteger + (number < 0 ? maxInteger : 0);
        }
    }
    public unsafe struct MultiEntangledPInt {
        public EntangledPInt* overflow { get; private set; }
        public int number { get; private set; }
        private int maxInteger;
        public MultiEntangledPInt(int maxInteger, EntangledPInt* overflowEntagledInteger) {
            number = 0;
            overflow = overflowEntagledInteger;
            this.maxInteger = maxInteger;
        }
        public void Set(int value) {
            number = value;
            Fix();
        }
        public static implicit operator int(MultiEntangledPInt value) {
            return value.number;
        }
        private void Fix() {
            overflow->Set(*overflow + (number / maxInteger) + (number < 0 ? -1 : 0));
            number += (number % maxInteger) + (number < 0 ? maxInteger : 0);
        }
    }
}
