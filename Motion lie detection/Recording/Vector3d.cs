﻿using System;

namespace Motion_lie_detection
{
    /**
     * Simple 3d vector struct using double precision.
     */
    public struct Vector3d
    {
        /**
         * The X component of the vector.
         */
        public double X;
        /**
         * The Y component of the vector.
         */
        public double Y;
        /**
         * The Z component of the vector.
         */
        public double Z;

        public Vector3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double LengthSquared
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public static double Distance(Vector3d a, Vector3d b)
        {
            double dX = a.X - b.X;
            double dY = a.Y - b.Y;
            double dZ = a.Z - b.Z;
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3d operator *(Vector3d a, double scalar)
        {
            return new Vector3d(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        public static Vector3d operator /(Vector3d a, double scalar)
        {
            return new Vector3d(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }

        /**
         * Convenience method for converting a Vector3d to an XNA Vector3.
         * @return The XNA Vector3.
         */
        public Microsoft.Xna.Framework.Vector3 ToXNAVec()
        {
            return new Microsoft.Xna.Framework.Vector3((float)X, (float)Y, (float)Z);
        }

    }
}
