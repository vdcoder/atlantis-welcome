using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Atlantis.Api.World._3D
{
    public class TextSpacialObject
    {
        public string FuzzyDescription { get; set; } = string.Empty;
        public string FuzzyDistance { get; set; } = string.Empty;
        public string FuzzyDirection { get; set; } = string.Empty; // Added for LLM navigation
    }

    public class TextSpacial
    {
        public List<string> GetVisibleAsText(BasicCamera a_camera, Octree a_octree)
        {
            List<string> visibleTextObjects = new List<string>();
            List<TextSpacialObject> visibleTextSpacialObjects = GetVisible(a_camera, a_octree);

            foreach (var textSpacialObject in visibleTextSpacialObjects)
            {
                // Formats as: "A red building - 45.20m to your front-left and slightly above"
                visibleTextObjects.Add($"{textSpacialObject.FuzzyDescription} - {textSpacialObject.FuzzyDistance} {textSpacialObject.FuzzyDirection}");
            }
            return visibleTextObjects;
        }

        public List<TextSpacialObject> GetVisible(BasicCamera a_camera, Octree a_octree)
        {
            List<TextSpacialObject> visibleTextObjects = new List<TextSpacialObject>();

            float nFOVRadians = a_camera.FOV * (MathF.PI / 180f);
            float nAspectRatio = a_camera.AspectRatio;
            float nNearPlane = a_camera.NearPlane;
            float nFarPlane = a_camera.FarPlane;

            Frustrum frustrum = new Frustrum(a_camera, nFOVRadians, nAspectRatio, nNearPlane, nFarPlane);

            List<OctreeObject> visibleOctreeObjects = new List<OctreeObject>();
            a_octree.GetVisibleObjectsInsideFrustum(visibleOctreeObjects, a_camera.Position, frustrum.Planes.AsSpan());

            Vector3 forward = Vector3.Normalize(a_camera.Forward);
            Vector3 worldUp = MathF.Abs(forward.Y) > 0.99f ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
            Vector3 up = Vector3.Cross(right, forward);

            foreach (var octreeObject in visibleOctreeObjects)
            {
                float nMeters = octreeObject.GetVisualDistanceToPoint(a_camera.Position);

                // Avoid division by zero if camera is sitting exactly inside the object center
                float safeDistance = MathF.Max(nMeters, 0.1f);

                // 1. Calculate how much screen real estate the object occupies.
                // This is a fast approximation of its solid angle/angular size.
                // (Object Radius / Distance) acts as a screen percentage scale factor.
                float visualScale = octreeObject.ApproximateBoundingRadius / safeDistance;

                // 2. Select the descriptive tier based on screen presence, not flat distance.
                string angularDescription = GetAngularLodDescription(octreeObject, visualScale);

                Vector3 toObject = octreeObject.Bounds.Center - a_camera.Position;
                float localForward = Vector3.Dot(toObject, forward);
                float localRight = Vector3.Dot(toObject, right);
                float localUp = Vector3.Dot(toObject, up);

                string directionText = GetRelativeDirectionText(localForward, localRight, localUp);

                visibleTextObjects.Add(new TextSpacialObject
                {
                    FuzzyDescription = angularDescription,
                    FuzzyDistance = nMeters.ToString("F2") + "m",
                    FuzzyDirection = directionText
                });
            }
            return visibleTextObjects;
        }

        private static string GetAngularLodDescription(OctreeObject obj, float visualScale)
        {
            // Tier 1 (LOD 0): Takes up massive screen space. Crisp, clear, highly detailed.
            // Example: A crate right in front of you (Radius 1m / Dist 2m = 0.5)
            // Example: A giant mountain far away (Radius 1000m / Dist 2000m = 0.5)
            if (visualScale >= 0.15f)
            {
                return obj.Lod0_Detailed;
            }

            // Tier 2 (LOD 1): Moderate screen footprint. Clear enough to categorize perfectly.
            // Example: A crate across the street (Radius 1m / Dist 10m = 0.1)
            // Example: A mountain far down the horizon (Radius 1000m / Dist 8000m = 0.125)
            if (visualScale >= 0.04f)
            {
                return !string.IsNullOrEmpty(obj.Lod1_Generic) ? obj.Lod1_Generic : $"a generic {obj.BroadCategory.ToLower()}";
            }

            // Tier 3 (LOD 2): Tiny speck on screen. Only raw geometric mass or silhouettes.
            // Example: A crate 40 meters away (Radius 1m / Dist 40m = 0.025)
            if (visualScale >= 0.01f)
            {
                return !string.IsNullOrEmpty(obj.Lod2_Minimal) ? obj.Lod2_Minimal : $"a distant outline of a {obj.BroadCategory.ToLower()}";
            }

            // Tier 4 (LOD 3): Absolute spatial ambiguity. Barely a single pixel.
            // A tiny pebble far away, or a crate 200 meters out.
            return "something small and indistinct in the distance";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRelativeDirectionText(float forward, float right, float up)
        {
            // Set dead-zone thresholds (in meters) to classify things as "dead center" vs "to the side"
            float horizontalThreshold = 1.5f;
            float verticalThreshold = 1.0f;

            string horizontalStr = "";
            string verticalStr = "";

            // Determine Horizontal Clock-face / Grid positioning
            if (forward > horizontalThreshold)
            {
                if (right > horizontalThreshold) horizontalStr = "to your front-right";
                else if (right < -horizontalThreshold) horizontalStr = "to your front-left";
                else horizontalStr = "directly ahead";
            }
            else if (forward < -horizontalThreshold)
            {
                if (right > horizontalThreshold) horizontalStr = "to your back-right";
                else if (right < -horizontalThreshold) horizontalStr = "to your back-left";
                else horizontalStr = "directly behind you";
            }
            else // Level with the camera on the depth axis
            {
                if (right > horizontalThreshold) horizontalStr = "to your right";
                else if (right < -horizontalThreshold) horizontalStr = "to your left";
                else horizontalStr = "exactly where you are standing";
            }

            // Determine Vertical positioning
            if (up > verticalThreshold) verticalStr = " and above you";
            else if (up > 0.2f) verticalStr = " and slightly above";
            else if (up < -verticalThreshold) verticalStr = " and below you";
            else if (up < -0.2f) verticalStr = " and slightly below";

            return horizontalStr + verticalStr;
        }
    }
}