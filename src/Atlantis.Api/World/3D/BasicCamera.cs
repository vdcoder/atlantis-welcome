using System.Numerics;

namespace Atlantis.Api.World._3D
{
    public class BasicCamera
    {
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }

        // Unity Default: 60 degrees Vertical FOV
        public float FOV { get; set; } = 60f;

        // Standard modern display aspect ratio (16:9)
        public float AspectRatio { get; set; } = 16f / 9f;

        // Unity Default: 0.3 meters. Prevents seeing inside the citizen's own geometry.
        public float NearPlane { get; set; } = 0.3f;

        // Unity Default: 1000 meters. Good balance for performance and long-range culling.
        public float FarPlane { get; set; } = 1000f;
    }
}
