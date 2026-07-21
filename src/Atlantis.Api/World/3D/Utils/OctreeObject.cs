using System.Numerics;

namespace Atlantis.Api.World._3D
{
    public class OctreeObject
    {
        public string Lod0_Detailed { get; set; } = string.Empty; // Close: "A crimson sports car with broken windows"
        public string Lod1_Generic { get; set; } = string.Empty;  // Mid: "A vehicle" or "A car"
        public string Lod2_Minimal { get; set; } = string.Empty;  // Far: "A small metallic object" or "A structural outline"

        public string BroadCategory { get; set; } = string.Empty; // Grouping category
        public Vector3[] Triangles { get; set; }    // World space mesh coordinates
        public BoundingBox Bounds { get; set; }
        public float ApproximateBoundingRadius { get { return 0.5f * Bounds.Extents.Length(); } }

        // Returns the distance from a point to the closes trangle edge
        public float GetVisualDistanceToPoint(Vector3 point)  
        {
            return Vector3.Distance(Bounds.Center, point);
            //float nMinSqrDistance = float.MaxValue;
            //foreach (var triangle in Triangles)
            //{
            //    float nSqrDistance = Vector3.DistanceSquared(point, triangle);
            //    if (nSqrDistance < nMinSqrDistance)
            //    {
            //        nMinSqrDistance = nSqrDistance;
            //    }
            //}
            //return MathF.Sqrt(nMinSqrDistance);
        }
        
    }
}
