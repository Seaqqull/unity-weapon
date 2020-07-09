using System.Collections;
using UnityEngine;

namespace Utility.Data
{
    public static class Vector2Helper
    {
        public static Vector2 MultiplyByNumber(this Vector2 vector, float number)
        {
            float x = vector.x * number;
            float y = vector.y * number;

            return new Vector2(x, y);
        }
    }

    public interface IRunLater
    {        
        void RunLater(System.Action method, float waitSeconds);
        Coroutine RunLaterValued(System.Action method, float waitSeconds);
        IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds);
    }
}
