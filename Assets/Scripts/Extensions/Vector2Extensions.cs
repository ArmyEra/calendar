using UnityEngine;

namespace Extensions
{
    public static class Vector2Extensions
    {
        public static bool IsNull(this Vector2 value)
            => value == Vector2.zero;
    }
}