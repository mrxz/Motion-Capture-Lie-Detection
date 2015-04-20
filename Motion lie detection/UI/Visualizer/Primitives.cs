//-----------------------------------------------------------------------------
// SpherePrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Motion_lie_detection
{

	/// <summary>
	/// Geometric primitive class for drawing cylinders.
	/// </summary>
	public class CylinderPrimitive : GeometricPrimitive
	{
		/// <summary>
		/// Constructs a new cylinder primitive, using default settings.
		/// </summary>
		public CylinderPrimitive(GraphicsDevice graphicsDevice)
			: this(graphicsDevice, 1, 1, 32)
		{
		}


		/// <summary>
		/// Constructs a new cylinder primitive,
		/// with the specified size and tessellation level.
		/// </summary>
		public CylinderPrimitive(GraphicsDevice graphicsDevice,
			float height, float diameter, int tessellation)
		{
			if (tessellation < 3)
				throw new ArgumentOutOfRangeException("tessellation");

			height /= 2;

			float radius = diameter / 2;

			// Create a ring of triangles around the outside of the cylinder.
			for (int i = 0; i < tessellation; i++)
			{
				Vector3 normal = GetCircleVector(i, tessellation);

				AddVertex(normal * radius + Vector3.Up * height, normal);
				AddVertex(normal * radius + Vector3.Down * height, normal);

				AddIndex(i * 2);
				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 2) % (tessellation * 2));

				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 3) % (tessellation * 2));
				AddIndex((i * 2 + 2) % (tessellation * 2));
			}

			// Create flat triangle fan caps to seal the top and bottom.
			CreateCap(tessellation, height, radius, Vector3.Up);
			CreateCap(tessellation, height, radius, Vector3.Down);

			InitializePrimitive(graphicsDevice);
		}


		/// <summary>
		/// Helper method creates a triangle fan to close the ends of the cylinder.
		/// </summary>
		void CreateCap(int tessellation, float height, float radius, Vector3 normal)
		{
			// Create cap indices.
			for (int i = 0; i < tessellation - 2; i++)
			{
				if (normal.Y > 0)
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 1) % tessellation);
					AddIndex(CurrentVertex + (i + 2) % tessellation);
				}
				else
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 2) % tessellation);
					AddIndex(CurrentVertex + (i + 1) % tessellation);
				}
			}

			// Create cap vertices.
			for (int i = 0; i < tessellation; i++)
			{
				Vector3 position = GetCircleVector(i, tessellation) * radius +
					normal * height;

				AddVertex(position, normal);
			}
		}


		/// <summary>
		/// Helper method computes a point on a circle.
		/// </summary>
		static Vector3 GetCircleVector(int i, int tessellation)
		{
			float angle = i * MathHelper.TwoPi / tessellation;

			float dx = (float)Math.Cos(angle);
			float dz = (float)Math.Sin(angle);

			return new Vector3(dx, 0, dz);
		}
	}
    class SpherePrimitive : GeometricPrimitive
    {
         /// <summary>
        /// Constructs a new sphere primitive, using default settings.
        /// </summary>
        public SpherePrimitive(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, 0.5f, 16)
        {
        }


        /// <summary>
        /// Constructs a new sphere primitive,
        /// with the specified size and tessellation level.
        /// </summary>
        public SpherePrimitive(GraphicsDevice graphicsDevice,
                               float diameter, int tessellation)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException("tessellation");

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            float radius = diameter / 2;

            // Start with a single vertex at the bottom of the sphere.
            AddVertex(Vector3.Down * radius, Vector3.Down);

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * MathHelper.Pi /
                                            verticalSegments) - MathHelper.PiOver2;

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    AddVertex(normal * radius, normal);
                }
            }

            // Finish with a single vertex at the top of the sphere.
            AddVertex(Vector3.Up * radius, Vector3.Up);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(0);
                AddIndex(1 + (i + 1) % horizontalSegments);
                AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex(1 + i * horizontalSegments + j);
                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);

                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(CurrentVertex - 1);
                AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
                AddIndex(CurrentVertex - 2 - i);
            }

            InitializePrimitive(graphicsDevice);
        }
    }
}

