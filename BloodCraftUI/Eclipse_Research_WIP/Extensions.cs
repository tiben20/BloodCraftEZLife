﻿using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using ProjectM.Physics;
using UnityEngine;

namespace BloodmoonPluginsUI.Eclipse_Research_WIP
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
