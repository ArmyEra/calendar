using UnityEngine;

namespace Utils
{
    public static class Vector2Extensions
    {
        public static bool IsNull(this Vector2 value)
            => value == Vector2.zero;
    }
}