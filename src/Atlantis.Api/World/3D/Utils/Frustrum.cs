using System;
using System.Numerics;

namespace Atlantis.Api.World._3D
{
    public enum FrustumResult
    {
        Outside,       // Fully outside at least one plane (Cull immediately)
        Intersecting,  // Straddling a boundary (Must test child nodes)
        Inside         // Fully inside all 6 planes (Collect everything, skip child tests)
    }

    public struct FrustumPlane
    {
        public Vector3 Normal;
        public float D; // Distance from origin: Ax + By + Cz + D = 0

        public FrustumPlane(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
            float length = normal.Length();
            if (length > 1e-6f) normal /= length;
            Normal = normal;
            D = -Vector3.Dot(normal, p0); // D = -dot(normal, point_on_plane)
        }
    }

    public class Frustrum
    {
        // Order convention: 0=Near, 1=Far, 2=Left, 3=Right, 4=Top, 5=Bottom
        public FrustumPlane[] Planes { get; private set; } = new FrustumPlane[6];

        public Frustrum(BasicCamera camera, float fovRadians, float aspectRatio, float nearPlane, float farPlane)
        {
            // 1. Calculate the base camera axes
            Vector3 forward = Vector3.Normalize(camera.Forward);

            // Derive Right and Up assuming world-up is (0, 1, 0)
            // If the camera points straight up/down, you may need a fallback mechanism
            Vector3 worldUp = MathF.Abs(forward.Y) > 0.99f ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
            Vector3 up = Vector3.Cross(right, forward);

            // 2. Compute the half-heights and half-widths at both Near and Far distances
            float tanFOV = MathF.Tan(fovRadians * 0.5f);

            float nearHeight = 2.0f * nearPlane * tanFOV;
            float nearWidth = nearHeight * aspectRatio;
            float farHeight = 2.0f * farPlane * tanFOV;
            float farWidth = farHeight * aspectRatio;

            float hNearH = nearHeight * 0.5f;
            float hNearW = nearWidth * 0.5f;
            float hFarH = farHeight * 0.5f;
            float hFarW = farWidth * 0.5f;

            // 3. Compute Near and Far center positions
            Vector3 nearCenter = camera.Position + (forward * nearPlane);
            Vector3 farCenter = camera.Position + (forward * farPlane);

            // 4. Calculate the 8 frustum corners in World Space
            Vector3 ntl = nearCenter + (up * hNearH) - (right * hNearW); // Near Top Left
            Vector3 ntr = nearCenter + (up * hNearH) + (right * hNearW); // Near Top Right
            Vector3 nbl = nearCenter - (up * hNearH) - (right * hNearW); // Near Bottom Left
            Vector3 nbr = nearCenter - (up * hNearH) + (right * hNearW); // Near Bottom Right

            Vector3 ftl = farCenter + (up * hFarH) - (right * hFarW);   // Far Top Left
            Vector3 ftr = farCenter + (up * hFarH) + (right * hFarW);   // Far Top Right
            Vector3 fbl = farCenter - (up * hFarH) - (right * hFarW);   // Far Bottom Left
            Vector3 fbr = farCenter - (up * hFarH) + (right * hFarW);   // Far Bottom Right

            // 5. Construct the 6 planes using winding orders that point Normals INWARD
            Planes[0] = new FrustumPlane(nbl, ntl, ntr); // Near Plane
            Planes[1] = new FrustumPlane(ftr, ftl, fbl); // Far Plane
            Planes[2] = new FrustumPlane(nbl, fbl, ftl); // Left Plane
            Planes[3] = new FrustumPlane(ntr, ftr, fbr); // Right Plane
            Planes[4] = new FrustumPlane(ntl, ftl, ftr); // Top Plane
            Planes[5] = new FrustumPlane(fbl, nbl, nbr); // Bottom Plane
        }
    }
}