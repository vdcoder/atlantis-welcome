using Humanizer;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Atlantis.Api.World._3D
{
    public class BoundingCube
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public BoundingCube(Vector3 a_center, float a_radius)
        {
            Center = a_center;
            Radius = a_radius;
        }

        public bool Contains(Vector3 point)
        {
            return
                Center.X - Radius <= point.X && Center.X + Radius >= point.X &&
                Center.Y - Radius <= point.Y && Center.Y + Radius >= point.Y &&
                Center.Z - Radius <= point.Z && Center.Z + Radius >= point.Z;
        }

        public bool Contains(BoundingBox box)
        {
            return Contains(box.Min) && Contains(box.Max);
        }

        public bool Intersects(BoundingBox box)
        {
            // 1. Calculate absolute distance between centers on each axis
            float distanceX = Math.Abs(Center.X - box.Center.X);
            float distanceY = Math.Abs(Center.Y - box.Center.Y);
            float distanceZ = Math.Abs(Center.Z - box.Center.Z);

            // 2. Sum the half-widths (extents) for each axis
            // Note: Since Cube's Radius is half-side, it acts as the cube's extent
            float sumExtentsX = Radius + box.Extents.X;
            float sumExtentsY = Radius + box.Extents.Y;
            float sumExtentsZ = Radius + box.Extents.Z;

            // 3. Separation Axis Theorem (SAT): Overlap must occur on all 3 axes
            return (distanceX <= sumExtentsX) &&
                   (distanceY <= sumExtentsY) &&
                   (distanceZ <= sumExtentsZ);
        }

        public bool IntersectsRay(Ray ray, out float distance)
        {
            // 1. Calculate inverse directions.
            Vector3 dirFrac = new Vector3(
                1f / ray.Direction.X,
                1f / ray.Direction.Y,
                1f / ray.Direction.Z
            );

            // 2. Compute the uniform min and max bounds for the cube dynamically
            Vector3 min = Center - new Vector3(Radius);
            Vector3 max = Center + new Vector3(Radius);

            // 3. X-Axis: Calculate intervals and explicitly sort them
            float tx1 = (min.X - ray.Origin.X) * dirFrac.X;
            float tx2 = (max.X - ray.Origin.X) * dirFrac.X;
            float tmin = Math.Min(tx1, tx2);
            float tmax = Math.Max(tx1, tx2);

            // 4. Y-Axis: Calculate, sort, and accumulate the overlapping window
            float ty1 = (min.Y - ray.Origin.Y) * dirFrac.Y;
            float ty2 = (max.Y - ray.Origin.Y) * dirFrac.Y;
            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            // 5. Z-Axis: Calculate, sort, and accumulate the overlapping window
            float tz1 = (min.Z - ray.Origin.Z) * dirFrac.Z;
            float tz2 = (max.Z - ray.Origin.Z) * dirFrac.Z;
            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            // 6. Handle the NaN corner-case (parallel ray grazing the exact surface)
            if (float.IsNaN(tmin) || float.IsNaN(tmax))
            {
                distance = 0f;
                return false;
            }

            // 7. Check if the ray misses the cube entirely (intervals don't overlap)
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
                distance = tmax; // Ray is inside, returning the exit time!
                return true;
            }

            // Both tmin and tmax are negative; the cube is completely behind the ray
            distance = 0f;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrustumResult TestFrustum(ReadOnlySpan<FrustumPlane> planes)
        {
            bool fullyInside = true;

            for (int i = 0; i < planes.Length; i++)
            {
                ref readonly var plane = ref planes[i];

                // 1. Distance from the cube center to the plane
                float centerDistance = Vector3.Dot(Center, plane.Normal) + plane.D;

                // 2. SIMD-accelerated projection radius calculation
                // Vector3.Abs utilizes hardware acceleration to strip signs instantly
                Vector3 absNormal = Vector3.Abs(plane.Normal);
                float maxRadius = Radius * (absNormal.X + absNormal.Y + absNormal.Z);

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
