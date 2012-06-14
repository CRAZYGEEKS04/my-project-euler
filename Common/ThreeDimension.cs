using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public static class ThreeDimension
    {
        public class BaseValue
        {
            public double X;
            public double Y;
            public double Z;

            public BaseValue(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public class Point : BaseValue
        {
            public Point(double x, double y, double z) : base(x, y, z) { }

            public static Vector operator +(Point lhs, Point rhs)
            {
                return new Vector(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            }

            public static Vector operator -(Point lhs, Point rhs)
            {
                return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
            }
        }

        public class Vector : BaseValue
        {
            public Vector(double x, double y, double z) : base(x, y, z) { }

            public static Vector operator *(Vector lhs, Vector rhs)
            {
                return new Vector(lhs.Y * rhs.Z - lhs.Z - rhs.Y, lhs.Z * rhs.X - lhs.X - rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);
            }

            public static double operator ^(Vector lhs, Vector rhs)
            {
                return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
            }
        }
    }
}