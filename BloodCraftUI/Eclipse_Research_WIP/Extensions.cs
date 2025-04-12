using BepInEx.Unity.IL2CPP.Utils.Collections;
using ProjectM.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloodCraftUI.Eclipse
{
    internal static class Extensions
    {
        private static IgnorePhysicsDebugSystem _monoBehaviour;

        public static Coroutine Start(this IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public static void Stop(this Coroutine routine)
        {
            if (routine != null) StopCoroutine(routine);
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (_monoBehaviour == null)
            {
                var go = new GameObject("Eclipse");
                _monoBehaviour = go.AddComponent<IgnorePhysicsDebugSystem>();
                UnityEngine.Object.DontDestroyOnLoad(go);
            }

            return _monoBehaviour.StartCoroutine(routine.WrapToIl2Cpp());
        }
        public static void StopCoroutine(Coroutine routine)
        {
            if (_monoBehaviour == null) return;
            _monoBehaviour.StopCoroutine(routine);
        }
    }
}
