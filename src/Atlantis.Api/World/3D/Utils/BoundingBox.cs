namespace Atlantis.Api.World._3D
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public struct BoundingBox
    {
        /// <summary>
        /// Gets or sets the center of the bounding box.
        /// </summary>
        [DataMember]
        public Vector3 Center { get; set; }

        /// <summary>
        /// Gets or sets the extents of the bounding box. This is always half of the <see cref="Size"/>.
        /// </summary>
        [DataMember]
        public Vector3 Extents { get; set; }

        public float ExtentsSqrMagnitude => Extents.LengthSquared();

        /// <summary>
        /// Gets or sets the size of the bounding box. This is always twice as large as the <see cref="Extents"/>.
        /// </summary>
        public Vector3 Size
        {
            get { return Extents * 2; }
            set { Extents = value * 0.5f; }
        }

        /// <summary>
        /// Gets or sets the minimal point of the box.
        /// </summary>
        /// <remarks>
        /// This is always equal to <c>center-extents</c>.
        /// </remarks>
        public Vector3 Min
        {
            get { return Center - Extents; }
            set { SetMinMax(value, Max); }
        }

        /// <summary>
        /// Gets or sets the maximal point of the box.
        /// </summary>
        /// <remarks>
        /// This is always equal to <c>center+extents</c>.
        /// </remarks>
        public Vector3 Max
        {
            get { return Center + Extents; }
            set { SetMinMax(Min, value); }
        }

        /// <summary>
        /// Creates a new bounding box.
        /// </summary>
        /// <param name="center">The center of the box.</param>
        /// <param name="size">The size of the box.</param>
        public BoundingBox(Vector3 center, Vector3 size)
        {
            Center = center;
            Extents = size * 0.5f;
        }

        /// <summary>
        /// Sets the bounds to the min and max value of the box.
        /// </summary>
        /// <param name="min">The minimal point.</param>
        /// <param name="max">The maximal point.</param>
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            Extents = (max - min) * 0.5f;
            Center = min + Extents;
        }

        /// <summary>
        /// Grows the bounding box include the point.
        /// </summary>
        /// <param name="point">The specified point to include.</param>
        public void Encapsulate(Vector3 point)
        {
            SetMinMax(Vector3.Min(Min, point), Vector3.Max(Max, point));
        }

        /// <summary>
        /// Grows the bounding box include the other box.
        /// </summary>
        /// <param name="box">The specified box to include.</param>
        public void Encapsulate(BoundingBox box)
        {
            Encapsulate(box.Center - box.Extents);
            Encapsulate(box.Center + box.Extents);
        }

        /// <summary>
        /// Expands the bounds by increasing its <see cref="Size"/> by <paramref name="amount"/> along each side.
        /// </summary>
        /// <param name="amount">The expansions for each dimension.</param>
        public void Expand(float amount)
        {
            amount *= 0.5f;
            Extents += new Vector3(amount, amount, amount);
        }

        /// <summary>
        /// Expands the bounds by increasing its <see cref="Size"/> by <paramref name="amount"/> along each side.
        /// </summary>
        /// <param name="amount">The expansions for each dimension in order.</param>
        public void Expand(Vector3 amount)
        {
            Extents += amount * 0.5f;
        }

        /// <summary>
        /// Determines whether the box contains the point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns><c>true</c> if the box contains the point; otherwise, <c>false</c>.</returns>
        public bool Contains(Vector3 point)
        {
            return
                Min.X <= point.X && Max.X >= point.X &&
                Min.Y <= point.Y && Max.Y >= point.Y &&
                Min.Z <= point.Z && Max.Z >= point.Z;
        }

        public bool Contains(BoundingBox box)
        {
            return Contains(box.Min) && Contains(box.Max);
        }

        /// <summary>
        /// Determines whether the bounding box intersects with another box.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns><c>true</c> if the bounding box intersects with another box, <c>false</c> otherwise.</returns>
        public bool Intersects(BoundingBox box)
        {
            return
                Min.X <= box.Max.X && Max.X >= box.Min.X &&
                Min.Y <= box.Max.Y && Max.Y >= box.Min.Y &&
                Min.Z <= box.Max.Z && Max.Z >= box.Min.Z;
        }

        /// <summary>
        /// Determines whether the bounding box intersects with a ray.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <returns><c>true</c> if the box intersects with the ray, <c>false</c> otherwise.</returns>
        public bool IntersectsRay(Ray ray)
        {
            float distance;
            return IntersectsRay(ray, out distance);
        }

        public bool IntersectsRay(Ray ray, out float distance)
        {
            // 1. Calculate inverse directions safely (IEEE 754 handles 1f / 0f as Infinity)
            Vector3 dirFrac = new Vector3(
                1f / ray.Direction.X,
                1f / ray.Direction.Y,
                1f / ray.Direction.Z
            );

            // 2. Compute the min and max bounds of the bounding box using Center and Extents
            Vector3 min = Center - Extents;
            Vector3 max = Center + Extents;

            // 3. X-Axis: Calculate intervals and explicitly sort them to handle negative zero/infinity
            float tx1 = (min.X - ray.Origin.X) * dirFrac.X;
            float tx2 = (max.X - ray.Origin.X) * dirFrac.X;
            float tmin = Math.Min(tx1, tx2);
            float tmax = Math.Max(tx1, tx2);

            // 4. Y-Axis: Calculate, sort, and intersect with the X-axis window
            float ty1 = (min.Y - ray.Origin.Y) * dirFrac.Y;
            float ty2 = (max.Y - ray.Origin.Y) * dirFrac.Y;
            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            // 5. Z-Axis: Calculate, sort, and intersect with the existing window
            float tz1 = (min.Z - ray.Origin.Z) * dirFrac.Z;
            float tz2 = (max.Z - ray.Origin.Z) * dirFrac.Z;
            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            // 6. Handle the NaN corner-case (ray moving parallel to and exactly lying on a box face)
            if (float.IsNaN(tmin) || float.IsNaN(tmax))
            {
                distance = 0f;
                return false;
            }

            // 7. Check if the ray misses the box entirely (intervals do not overlap)
            if (tmin > tmax)
            {
                distance = 0f;
                return false;
            }

            // 8. Determine the first positive intersection point:
            // - If tmin >= 0, the ray is outside and hits the entry wall first.
            // - If tmin < 0 and tmax >= 0, the ray is INSIDE and hits the exit wall first.
            if (tmin >= 0f)
            {
                distance = tmin;
                return true;
            }
            else if (tmax >= 0f)
            {
                distance = tmax; // Inside the box, returning the forward exit time
                return true;
            }

            // Both tmin and tmax are negative; the box is completely behind the ray
            distance = 0f;
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ Extents.GetHashCode() << 2;
        }

        /// <summary>
        /// Determines whether the specified object as a <see cref="BoundingBox" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BoundingBox" /> object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified box is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            bool result;
            if (!(other is BoundingBox))
            {
                result = false;
            }
            else
            {
                BoundingBox box = (BoundingBox)other;
                result = (Center.Equals(box.Center) && Extents.Equals(box.Extents));
            }
            return result;
        }

        /// <summary>
        /// Returns a nicely formatted string for this bounding box.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Center: {0}, Extents: {1}",
                Center,
                Extents
            );
        }

        /// <summary>
        /// Returns a nicely formatted string for this bounding box.
        /// </summary>
        /// <param name="format">The format for the center and the extent.</param>
        public string ToString(string format)
        {
            return String.Format("Center: {0}, Extents: {1}",
                Center.ToString(format),
                Extents.ToString(format)
            );
        }

        /// <summary>
        /// Determines whether two bounding boxes are equal.
        /// </summary>
        /// <param name="lhs">The first box.</param>
        /// <param name="rhs">The second box.</param>
        public static bool operator ==(BoundingBox lhs, BoundingBox rhs)
        {
            return lhs.Center == rhs.Center && lhs.Extents == rhs.Extents;
        }

        /// <summary>
        /// Determines whether two bounding boxes are different.
        /// </summary>
        /// <param name="lhs">The first box.</param>
        /// <param name="rhs">The second box.</param>
        public static bool operator !=(BoundingBox lhs, BoundingBox rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Checks if any part of the bounding box is to the right (positive side) of a plane defined by 3 points.
        /// </summary>
        public bool IsAnyPartRightOfPlane(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // 1. Calculate plane normal using the Right-Hand Rule
            Vector3 v0 = p1 - p0;
            Vector3 v1 = p2 - p0;
            Vector3 normal = Vector3.Cross(v0, v1);

            // Normalize to ensure distance math is exact (important for dot products)
            float length = normal.Length();
            if (length < 1e-6f) return false; // Degenerate plane (points are collinear)
            normal /= length;

            // 2. Project the center of the box onto the plane normal
            // This is the distance from p0 to the box center along the normal vector direction
            float centerDistance = Vector3.Dot(Center - p0, normal);

            // 3. Compute the maximum projection radius of the AABB extents onto the normal
            // This effectively finds the corner furthest "to the right" along the normal vector
            float maxRadiusAlongNormal =
                MathF.Abs(Extents.X * normal.X) +
                MathF.Abs(Extents.Y * normal.Y) +
                MathF.Abs(Extents.Z * normal.Z);

            // 4. If the furthest right point is past the plane (> 0), some part is to the right
            return (centerDistance + maxRadiusAlongNormal) > 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrustumResult TestFrustum(ReadOnlySpan<FrustumPlane> planes)
        {
            bool fullyInside = true;

            for (int i = 0; i < planes.Length; i++)
            {
                ref readonly var plane = ref planes[i];

                // 1. Distance from the box center to the plane
                float centerDistance = Vector3.Dot(Center, plane.Normal) + plane.D;

                // 2. SIMD-accelerated projection radius calculation
                // Replaces 3 MathF.Abs calls with a single accelerated Vector3.Dot
                Vector3 absNormal = Vector3.Abs(plane.Normal);
                float maxRadius = Vector3.Dot(Extents, absNormal);

                // 3. Check against the "Outside" boundary
                if (centerDistance + maxRadius < 0f)
                {
                    return FrustumResult.Outside;
                }

                // 4. Check if it's straddling the boundary
                if (centerDistance - maxRadius < 0f)
                {
                    fullyInside = false;
                }
            }

            return fullyInside ? FrustumResult.Inside : FrustumResult.Intersecting;
        }
    }
    
}
