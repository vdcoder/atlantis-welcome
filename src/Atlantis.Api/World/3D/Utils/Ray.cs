namespace Atlantis.Api.World._3D
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of rays.
    /// </summary>
    /// <remarks>
    /// A ray is an infinite line starting at <see cref="Origin"/> and going in some <see cref="Direction"/>.
    /// 
    /// This class was inspired by the Ray type of the Unity Engine and 
    /// designed with the exact same interface to provide maximum compatibility.
    /// </remarks>
    [DataContract]
    public struct Ray
    {
        /// <summary>
        /// Gets or sets the origin of the ray.
        /// </summary>
        [DataMember]
        public Vector3 Origin { get; set; }

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        [DataMember]
        private Vector3 _direction;
        [DataMember]
        private Vector3 _inverseDirection;
        /// <summary>
        /// Gets or sets the direction of the ray.
        /// </summary>
        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = Vector3.Normalize(value); _inverseDirection = new Vector3(1.0f / _direction.X, 1.0f / _direction.Y, 1.0f / _direction.Z); }
        }

        public Vector3 InverseDirection { get { return _inverseDirection; } } // Cached for ultra-fast AABB checks

        /// <summary>
        /// Creates a ray starting at origin along direction.
        /// </summary>
        /// <param name="origin">The origin of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            _direction = Vector3.Normalize(direction);
        }

        /// <summary>
        /// Returns a point at the given distance along the ray.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns>The point on the ray.</returns>
        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }

        /// <summary>
        /// Returns a nicely formatted string for this ray.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Origin: {0}, Dir: {1}",
                Origin,
                Direction
            );
        }

        /// <summary>
        /// Returns a nicely formatted string for this ray.
        /// </summary>
        /// <param name="format">The format for the origin and direction.</param>
        public string ToString(string format)
        {
            return String.Format("Origin: {0}, Dir: {1}",
                Origin.ToString(format),
                Direction.ToString(format)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsTriangle(Vector3 v0, Vector3 v1, Vector3 v2, out float t)
        {
            t = 0;
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 h = Vector3.Cross(Direction, edge2);
            float a = Vector3.Dot(edge1, h);

            if (a > -1e-6f && a < 1e-6f) return false; // Ray is parallel to triangle

            float f = 1.0f / a;
            Vector3 s = Origin - v0;
            float u = f * Vector3.Dot(s, h);

            if (u < 0.0f || u > 1.0f) return false;

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(Direction, q);

            if (v < 0.0f || u + v > 1.0f) return false;

            t = f * Vector3.Dot(edge2, q);
            return t > 1e-6f; // True if hit is in front of the ray
        }
    }
}