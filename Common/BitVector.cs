using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public class BitVector
    {
        public int Length { get; private set; }
        public int Size { get; private set; }

        private int[] values;

        public BitVector(int size)
        {
            Size = size;
            Length = size >> 5;
            if ((size & 0x1F) != 0)
                Length++;
            values = new int[Length];
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BitVector))
                return false;

            var other = obj as BitVector;

            if (other.Size != this.Size)
                return false;

            for (int i = 0; i < Length; i++)
            {
                if (this.values[i] != other.values[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var ret = 0;

            foreach (var value in values)
                ret ^= value;

            return ret;
        }

        public bool this[int offset] { get { return (values[offset >> 5] & (1 << (offset & 0x1F))) != 0; } }

        public void Set(int offset)
        {
            values[offset >> 5] |= 1 << (offset & 0x1F);
        }

        public void Clear(int offset)
        {
            values[offset >> 5] &= ~(1 << (offset & 0x1F));
        }

        public bool IsAllClear()
        {
            for (int i = 0; i < Length; i++)
            {
                if (values[i] != 0)
                    return false;
            }

            return true;
        }

        public static BitVector operator &(BitVector lhs, BitVector rhs)
        {
            if (lhs.Size != rhs.Size)
                throw new InvalidOperationException("vector size is different");

            var ret = new BitVector(lhs.Size);

            for (int i = 0; i < lhs.Length; i++)
                ret.values[i] = lhs.values[i] & rhs.values[i];

            return ret;
        }

        public static BitVector operator |(BitVector lhs, BitVector rhs)
        {
            if (lhs.Size != rhs.Size)
                throw new InvalidOperationException("vector size is different");

            var ret = new BitVector(lhs.Size);

            for (int i = 0; i < lhs.Length; i++)
                ret.values[i] = lhs.values[i] | rhs.values[i];

            return ret;
        }
    }
}