using System.Collections.Generic;
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
        
        public static Entity FindNearestEnemyEntity(Transform transform, Teams team, List<Entity> entities)
        {
            var bestDistance = float.MaxValue;
            Entity nearestEntity = null;

            var position = PhysicsUtils.Vector3Y0(transform.position);

            foreach (var entity in entities)
            {
                if (entity && entity.team != team)
                {
                    var newDistance = Vector3.Distance(position, PhysicsUtils.Vector3Y0(entity.transform.position));
                    if (newDistance < bestDistance)
                    {
                        bestDistance = newDistance;
                        nearestEntity = entity;
                    }
                }
            }

            return nearestEntity;
        }
    }
}