using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public class SmallFraction : IComparable<SmallFraction>
    {
        public long Numerator { get; private set; }
        public long Denominator { get; private set; }

        public SmallFraction(long numerator, long denominator)
        {
            long factor;

            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }

            if (numerator >= 0)
                factor = Factor.GetCommonFactor(numerator, denominator);
            else
                factor = Factor.GetCommonFactor(-numerator, denominator);

            Numerator = numerator / factor;
            Denominator = denominator / factor;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SmallFraction))
                return false;

            var other = obj as SmallFraction;

            return this.Numerator == other.Numerator && this.Denominator == other.Denominator;
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode() ^ Denominator.GetHashCode();
        }

        public override string ToString()
        {
            return Numerator.ToString() + "/" + Denominator.ToString();
        }

        public int CompareTo(SmallFraction other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            else
                return 1;
        }

        public static implicit operator SmallFraction(long value)
        {
            return new SmallFraction(value, 1);
        }

        public static SmallFraction operator +(SmallFraction lhs, SmallFraction rhs)
        {
            return new SmallFraction(lhs.Numerator * rhs.Denominator + rhs.Numerator * lhs.Denominator,
                lhs.Denominator * rhs.Denominator);
        }

        public static SmallFraction operator -(SmallFraction lhs, SmallFraction rhs)
        {
            return new SmallFraction(lhs.Numerator * rhs.Denominator - rhs.Numerator * lhs.Denominator,
                lhs.Denominator * rhs.Denominator);
        }

        public static SmallFraction operator *(SmallFraction lhs, SmallFraction rhs)
        {
            return new SmallFraction(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator);
        }

        public static SmallFraction operator /(SmallFraction lhs, SmallFraction rhs)
        {
            return new SmallFraction(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator);
        }

        public static bool operator ==(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator == rhs.Numerator && lhs.Denominator == rhs.Denominator;
        }

        public static bool operator !=(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator != rhs.Numerator || lhs.Denominator != rhs.Denominator;
        }

        public static bool operator >(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator * rhs.Denominator > rhs.Numerator * lhs.Denominator;
        }

        public static bool operator <(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator * rhs.Denominator < rhs.Numerator * lhs.Denominator;
        }

        public static bool operator >=(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator * rhs.Denominator >= rhs.Numerator * lhs.Denominator;
        }

        public static bool operator <=(SmallFraction lhs, SmallFraction rhs)
        {
            return lhs.Numerator * rhs.Denominator <= rhs.Numerator * lhs.Denominator;
        }
    }
}