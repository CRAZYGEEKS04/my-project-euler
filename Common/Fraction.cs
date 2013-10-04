using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common
{
    public class Fraction : IComparable<Fraction>
    {
        public BigInteger Numerator { get; private set; }
        public BigInteger Denominator { get; private set; }

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            BigInteger factor;

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
            if (!(obj is Fraction))
                return false;

            var other = obj as Fraction;

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

        public int CompareTo(Fraction other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            else
                return 1;
        }

        public static implicit operator Fraction(long value)
        {
            return new Fraction(value, 1);
        }

        public static implicit operator Fraction(BigInteger value)
        {
            return new Fraction(value, 1);
        }

        public static Fraction operator +(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.Numerator * rhs.Denominator + rhs.Numerator * lhs.Denominator,
                lhs.Denominator * rhs.Denominator);
        }

        public static Fraction operator -(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.Numerator * rhs.Denominator - rhs.Numerator * lhs.Denominator,
                lhs.Denominator * rhs.Denominator);
        }

        public static Fraction operator *(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator);
        }

        public static Fraction operator /(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator);
        }

        public static bool operator ==(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator == rhs.Numerator && lhs.Denominator == rhs.Denominator;
        }

        public static bool operator !=(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator != rhs.Numerator || lhs.Denominator != rhs.Denominator;
        }

        public static bool operator >(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator * rhs.Denominator > rhs.Numerator * lhs.Denominator;
        }

        public static bool operator <(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator * rhs.Denominator < rhs.Numerator * lhs.Denominator;
        }

        public static bool operator >=(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator * rhs.Denominator >= rhs.Numerator * lhs.Denominator;
        }

        public static bool operator <=(Fraction lhs, Fraction rhs)
        {
            return lhs.Numerator * rhs.Denominator <= rhs.Numerator * lhs.Denominator;
        }
    }
}