namespace Atlantis.Api.World._3D
{
    using Humanizer;
    using Microsoft.Build.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Numerics;
    using System.Xml.Linq;

    public class OctreeNode
    {
        public BoundingCube Cube { get; private set; }

        public List<OctreeObject> Objects = new();

        public OctreeNode[]? Children = null;

        public OctreeConfig Config { get; }

        public OctreeNode(BoundingCube a_cube, OctreeConfig config)
        {
            Cube = a_cube;
            Config = config;
        }

        // #### PUBLIC METHODS ####

        public void GetDescendantCubes(List<BoundingCube> a_cubes)
        {
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.GetDescendantCubes(a_cubes);
                }
                return;
            }
        }

        public bool CanFit(OctreeObject a_obj)
        {
            return Cube.Contains(a_obj.Bounds);
        }

        public bool Add(OctreeObject a_obj)
        {
            if (Cube.Contains(a_obj.Bounds))
            {
                // If we don't have children yet, check if we should make them (have too many objects etc)
                if (Children == null && Objects.Count >= Config.MaxObjectsPerNode && (Cube.Radius / 2) >= Config.MinNodeRadius)
                {
                    Split();
                }

                // Try to push the object down into children (unless it intersects with too many of them, in which case it stays here)
                if (!TryPushToChildren(a_obj))
                {
                    Objects.Add(a_obj);
                }

                return true;
            }
            return false;
        }

        public bool Remove(OctreeObject a_obj)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (ReferenceEquals(Objects[i], a_obj))
                {
                    Objects.Remove(Objects[i]);
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                    return true;
                }
            }

            if (Children != null)
            {
                bool bRemoved = false;
                for (int i = 0; i < 8; i++)
                {
                    bRemoved = bRemoved || Children[i].Remove(a_obj);
                }
                if (bRemoved && ShouldMerge())
                {
                    Merge();
                }
                return bRemoved;
            }
            return false;
        }
         
        public void GetAllDescendants(HashSet<OctreeObject> a_objects) {
            // Add all objects in this node
            foreach (var obj in Objects)
            {
                a_objects.Add(obj);
            }
            // Recursively add all objects in children
            if (Children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    Children[i].GetAllDescendants(a_objects);
                }
            }
        }

        public void GetObjectsInsideFrustum(HashSet<OctreeObject> a_objects, ReadOnlySpan<FrustumPlane> a_frustumPlanes)
        {
            FrustumResult frustumResult = Cube.TestFrustum(a_frustumPlanes);

            if (frustumResult == FrustumResult.Outside)
            {
                return; // Early exit: Skip this node and all its children
            }

            if (frustumResult == FrustumResult.Inside)
            {
                // Optimization: Fully inside! Unconditionally add all objects in this 
                // branch without testing any more bounding boxes down the line.
                GetAllDescendants(a_objects);
                return;
            }

            // If Intersecting: Add this node's immediate objects (with individual tests if necessary)
            // and continue the walk down the tree
            foreach (var obj in Objects)
            {
                if (obj.Bounds.TestFrustum(a_frustumPlanes) != FrustumResult.Outside)
                {
                    a_objects.Add(obj);
                }
            }

            if (Children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    Children[i].GetObjectsInsideFrustum(a_objects, a_frustumPlanes);
                }
            }
        }

        public void FindClosestObject(Ray ray, ref OctreeObject? closestObj, ref float closestDist)
        {
            if (!Cube.IntersectsRay(ray, out float tMin) || tMin > closestDist)
                return;

            foreach (var obj in Objects)
            {
                if (!obj.Bounds.IntersectsRay(ray, out float objTMin) || objTMin > closestDist)
                    continue;

                // Exhaustive triangle check
                for (int i = 0; i < obj.Triangles.Length; i += 3)
                {
                    if (ray.IntersectsTriangle(obj.Triangles[i], obj.Triangles[i + 1], obj.Triangles[i + 2], out float t))
                    {
                        if (t < closestDist)
                        {
                            closestDist = t;
                            closestObj = obj;
                        }
                    }
                }
            }

            if (Children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    Children[i].FindClosestObject(ray, ref closestObj, ref closestDist);
                }
            }
        }

        public bool HasAnyObjects()
        {
            if (Objects.Count > 0) return true;

            if (Children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (Children[i].HasAnyObjects()) return true;
                }
            }

            return false;
        }

        // #### PRIVATE METHODS ####

        private void Split()
        {
            float newRadius = Cube.Radius / 2f;
            Children = new OctreeNode[8];
            Children[0] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(-newRadius, newRadius, -newRadius), newRadius), Config);
            Children[1] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(newRadius, newRadius, -newRadius), newRadius), Config);
            Children[2] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(-newRadius, newRadius, newRadius), newRadius), Config);
            Children[3] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(newRadius, newRadius, newRadius), newRadius), Config);
            Children[4] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(-newRadius, -newRadius, -newRadius), newRadius), Config);
            Children[5] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(newRadius, -newRadius, -newRadius), newRadius), Config);
            Children[6] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(-newRadius, -newRadius, newRadius), newRadius), Config);
            Children[7] = new OctreeNode(new BoundingCube(Cube.Center + new Vector3(newRadius, -newRadius, newRadius), newRadius), Config);

            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                OctreeObject existingObj = Objects[i];
                if (TryPushToChildren(existingObj))
                {
                    Objects.Remove(existingObj);
                }
            }
        }

        private void Merge()
        {
            if (Children != null)
            {
                // Collect unique objects
                HashSet<OctreeObject> uniqueChildObjects = new HashSet<OctreeObject>();
                foreach (OctreeNode child in Children)
                {
                    foreach (OctreeObject obj in child.Objects)
                    {
                        uniqueChildObjects.Add(obj);
                    }
                }
                // Add unique objects back to this node
                foreach (OctreeObject obj in uniqueChildObjects)
                {
                    Objects.Add(obj);
                }
            }
            Children = null;
        }

        private bool ShouldMerge()
        {
            if (Children != null)
            {
                HashSet<OctreeObject> uniqueChildObjects = new HashSet<OctreeObject>();
                foreach (OctreeNode child in Children)
                {
                    if (child.Children != null)
                    {
                        return false; // Cannot merge if any child has children of its own
                    }
                    foreach (OctreeObject obj in child.Objects)
                    {
                        uniqueChildObjects.Add(obj);
                    }
                }
                return Objects.Count + uniqueChildObjects.Count <= Config.MaxObjectsPerNode;
            }
            return false;
        }

        private bool TryPushToChildren(OctreeObject a_obj)
        {
            if (Children != null)
            {
                int nIntersectsCount = 0;
                for (int i = Children.Length - 1; i >= 0; i--)
                {
                    if (Children[i].Cube.Intersects(a_obj.Bounds))
                    {
                        nIntersectsCount++;
                    }
                }
                if (nIntersectsCount >= 4)
                {
                    for (int i = Children.Length - 1; i >= 0; i--)
                    {
                        if (Children[i].Cube.Intersects(a_obj.Bounds))
                        {
                            Children[i].Add(a_obj);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
    
}
