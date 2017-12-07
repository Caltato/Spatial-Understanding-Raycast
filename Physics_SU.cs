using HoloToolkit.Unity;
using System;

namespace UnityEngine
{
    public struct RaycastHit_SU
    {
        public float distance;
        public Vector3 point;
        public SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes surfaceType;

        public RaycastHit_SU(float _distance, Vector3 _point, SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes _surfaceType)
        {
            distance = _distance;
            point = _point;
            surfaceType = _surfaceType;
        }
    }

    [Serializable]
    public class SurfaceMask
    {
        public bool Other = true, Floor = true, FloorLike = true, Platform = true, Ceiling = true, WallExternal = true, WallLike = true;
        public static SurfaceMask Default { get { return new SurfaceMask(); } }
    }

    public class Physics_SU
    {
        #region Raycast
        public static bool Raycast(Ray ray)
        {
            return Raycast(ray.origin, ray.direction);
        }
        public static bool Raycast(Ray ray, SurfaceMask surfaceMask)
        {
            return Raycast(ray.origin, ray.direction, surfaceMask);
        }
        public static bool Raycast(Ray ray, float maxDistance)
        {
            return Raycast(ray.origin, ray.direction, maxDistance);
        }
        public static bool Raycast(Ray ray, SurfaceMask surfaceMask, float maxDistance)
        {
            return Raycast(ray.origin, ray.direction, surfaceMask, maxDistance);
        }
        public static bool Raycast(Ray ray, out RaycastHit_SU hitInfo)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo);
        }
        public static bool Raycast(Ray ray, out RaycastHit_SU hitInfo, SurfaceMask surfaceMask)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, surfaceMask);
        }
        public static bool Raycast(Ray ray, out RaycastHit_SU hitInfo, float maxDistance)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, maxDistance);
        }
        public static bool Raycast(Ray ray, out RaycastHit_SU hitInfo, SurfaceMask surfaceMask, float maxDistance)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, surfaceMask, maxDistance);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction)
        {
            return Raycast(origin, direction, Mathf.Infinity);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, SurfaceMask surfaceMask)
        {
            return Raycast(origin, direction, surfaceMask, Mathf.Infinity);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
        {
            return Raycast(origin, direction, SurfaceMask.Default, maxDistance);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, SurfaceMask surfaceMask, float maxDistance)
        {
            SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding && SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            {
                // Set result pointer
                IntPtr raycastResultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();

                // Shoot Raycast
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(origin.x, origin.y, origin.z, direction.x, direction.y, direction.z, raycastResultPtr);

                // Assign temp result
                rayCastResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();

                // Check Surface Mask
                return CheckSurfaceAgainstMask(rayCastResult.SurfaceType, surfaceMask);
            }

            return false;
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit_SU hitInfo)
        {
            return Raycast(origin, direction, out hitInfo, SurfaceMask.Default);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit_SU hitInfo, SurfaceMask surfaceMask)
        {
            return Raycast(origin, direction, out hitInfo, surfaceMask, Mathf.Infinity);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit_SU hitInfo, float maxDistance)
        {
            return Raycast(origin, direction, out hitInfo, SurfaceMask.Default, maxDistance);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit_SU hitInfo, SurfaceMask surfaceMask, float maxDistance)
        {
            SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding && SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            {                // Set result pointer
                IntPtr raycastResultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();

                // Shoot Raycast
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(origin.x, origin.y, origin.z, direction.x, direction.y, direction.z, raycastResultPtr);

                // Assign temp result
                rayCastResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();

                hitInfo = new RaycastHit_SU(Vector3.Distance(origin, rayCastResult.IntersectPoint), rayCastResult.IntersectPoint, rayCastResult.SurfaceType);


                // Check Surface Mask
                return CheckSurfaceAgainstMask(rayCastResult.SurfaceType, surfaceMask);
            }
            hitInfo = new RaycastHit_SU(0, Vector3.zero, SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid);
            return false;
        }
        #endregion // Raycast
        
        private static bool CheckSurfaceAgainstMask(SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes surface, SurfaceMask mask)
        {
            if (surface != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid)
            {
                switch (surface)
                {
                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Other:
                        return mask.Other;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor:
                        return mask.Floor;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.FloorLike:
                        return mask.FloorLike;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Platform:
                        return mask.Platform;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Ceiling:
                        return mask.Ceiling;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallExternal:
                        return mask.WallExternal;

                    case SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallLike:
                        return mask.WallLike;

                    default:
                        return false;
                }
            }
            else
                return false;
        }
    }
}

