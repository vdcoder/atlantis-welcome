namespace Atlantis.Api.World._3D
{
    public class OctreeConfig
    {
        public int MaxObjectsPerNode { get; set; } = 8;
        public float MinNodeRadius { get; set; } = 1.0f;
    }
}
