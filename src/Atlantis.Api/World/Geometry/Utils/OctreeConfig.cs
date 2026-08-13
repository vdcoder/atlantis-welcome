namespace Atlantis.Api.World.Geometry.Utils
{
    public class OctreeConfig
    {
        public int MaxObjectsPerNode { get; set; } = 8;
        public float MinNodeRadius { get; set; } = 1.0f;
    }
}
