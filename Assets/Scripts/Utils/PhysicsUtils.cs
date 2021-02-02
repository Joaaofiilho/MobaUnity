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
    }
}