namespace Atlantis.Api.World._3D
{
    using global::Octree;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Numerics;
    using System.Xml.Linq;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public partial class Octree
    {
        private OctreeNode _rootNode;

        private OctreeConfig _config;

        public int Count { get; private set; }

        public BoundingCube Cube
        {
            get { return _rootNode.Cube; }
        }

        public BoundingCube[] GetAllCubes()
        {
            var bounds = new List<BoundingCube>();
            _rootNode.GetDescendantCubes(bounds);
            bounds.Add(_rootNode.Cube);
            return bounds.ToArray();
        }

        public Octree(BoundingBox a_initialBoundingBox, float a_nMinNodeRadius, int a_nMaxObjectsPerNode)
        {
            Count = 0;
            _config = new OctreeConfig
            {
                MinNodeRadius = a_nMinNodeRadius,
                MaxObjectsPerNode = a_nMaxObjectsPerNode
            };

            // Calculate the maximum extent of the initial bounding box and round it up to the nearest power of two
            float maxExtent = Math.Max(a_initialBoundingBox.Extents.X, Math.Max(a_initialBoundingBox.Extents.Y, a_initialBoundingBox.Extents.Z));
            if (maxExtent <= 1f) {
                maxExtent = 1f;
            }
            else
            {
                float exponent = MathF.Ceiling(MathF.Log2(maxExtent));
                maxExtent = MathF.Pow(2f, exponent);
            }

            _rootNode = new OctreeNode(
                new BoundingCube(
                    a_initialBoundingBox.Center,
                    maxExtent
                ), _config);
        }

        // #### PUBLIC METHODS ####

        public void Add(OctreeObject a_obj)
        {
            // Add object or expand the octree until it can be added
            if (!_rootNode.CanFit(a_obj))
            {
                Recreate(a_obj);
            }
            else
            {
                _rootNode.Add(a_obj);
                Count++;
            }
        }

        public bool Remove(OctreeObject a_obj)
        {
            bool bRemoved = _rootNode.Remove(a_obj);
            if (bRemoved)
            {
                Count--;
            }
            return bRemoved;
        }

        public bool GetObjectsInsideFrustum(HashSet<OctreeObject> a_objects, ReadOnlySpan<FrustumPlane> a_frustumPlanes)
        {
            a_objects.Clear();
            _rootNode.GetObjectsInsideFrustum(a_objects, a_frustumPlanes);
            return a_objects.Count > 0;
        }

        public void FindClosestObject(Ray ray, ref OctreeObject? closestObj, ref float closestDist)
        {
            _rootNode.FindClosestObject(ray, ref closestObj, ref closestDist);
        }

        public bool GetVisibleObjectsInsideFrustum(List<OctreeObject> a_objects, Vector3 a_position, ReadOnlySpan<FrustumPlane> a_frustumPlanes)
        {
            a_objects.Clear();

            HashSet<OctreeObject> frustrumObjects = new HashSet<OctreeObject>();
            GetObjectsInsideFrustum(frustrumObjects, a_frustumPlanes);

            foreach (var obj in frustrumObjects)
            {
                OctreeObject? closestObj = null;
                float closestDist = float.MaxValue;
                FindClosestObject(new Ray(a_position, obj.Bounds.Center - a_position), ref closestObj, ref closestDist);
                if (closestObj == obj)
                {
                    a_objects.Add(obj);
                }
            }

            return a_objects.Count > 0;
        }

        public bool GetVisibleObjectsInsideFrustum_Parallel(List<OctreeObject> a_objects, Vector3 a_position, ReadOnlySpan<FrustumPlane> a_frustumPlanes)
        {
            a_objects.Clear();

            HashSet<OctreeObject> frustrumObjects = new HashSet<OctreeObject>();
            GetObjectsInsideFrustum(frustrumObjects, a_frustumPlanes);

            Parallel.ForEach(frustrumObjects, obj =>
            {
                OctreeObject? closestObj = null;
                float closestDist = float.MaxValue;
                FindClosestObject(new Ray(a_position, obj.Bounds.Center - a_position), ref closestObj, ref closestDist);
                if (closestObj == obj)
                {
                    lock (a_objects)
                    {
                        a_objects.Add(obj);
                    }
                }
            });

            return a_objects.Count > 0;
        }

        // #### PRIVATE METHODS ####

        private void Recreate(OctreeObject a_newObj)
        {
            // Collect all existing objects in the octree, including the new object
            HashSet<OctreeObject> allObjects = new HashSet<OctreeObject>();
            _rootNode.GetAllDescendants(allObjects);
            allObjects.Add(a_newObj);

            // Create a new bounding cube that encompasses both the old root node and the new object's position, power-of-two sized
            Vector3 newAverageCenter = (a_newObj.Bounds.Center + _rootNode.Cube.Center) / 2f;
            float nOldCenterMoveByDistance = Vector3.Distance(_rootNode.Cube.Center, newAverageCenter); // Calculate the distance the old center has moved to the new average center
            float nNewStartingRadius = _rootNode.Cube.Radius + nOldCenterMoveByDistance; // Start with the old radius and add the distance moved to ensure the new cube contains the old cube
            BoundingCube newCube = new BoundingCube(
                newAverageCenter,
                nNewStartingRadius
            );
            while (newCube.Contains(a_newObj.Bounds) == false)
            {
                newCube.Radius *= 2f;
            }

            // Create a new root node with the new bounding cube
            _rootNode = new OctreeNode(newCube, _config);

            // Add all existing objects back into the new octree
            Count = 0;
            foreach (var obj in allObjects)
            {
                _rootNode.Add(obj);
                Count++;
            }
        }
    }
}
