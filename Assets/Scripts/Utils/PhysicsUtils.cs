using JetBrains.Annotations;
using UnityEngine;

namespace Utils
{
    public static class PhysicsUtils
    {
        public static Vector3 Vector3Y0(Vector3 vector3)
        {
            vector3.y = 0;
            return vector3;
        }

        public static Transform FindNearestTransform([NotNull] Transform position, [NotNull] Transform[] transforms)
        {
            Transform nearestTransform = transforms[0];
            var bestDistance = float.PositiveInfinity;

            foreach (var transform in transforms)
            {
                var newDistance = Vector3.Distance(position.position, transform.position);

                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    nearestTransform = transform;
                }
            }

            return nearestTransform;
        }
    }
}