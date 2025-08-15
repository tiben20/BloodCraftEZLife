using System;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.Utils;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace BloodCraftEZLife.UI.ModernLib
{
    public class CoreUpdateBehavior: MonoBehaviour, IDisposable
    {
        private GameObject _obj;

        public void Setup()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CoreUpdateBehavior>();
            _obj = new GameObject("ModernUICoreUpdateBehavior");
            DontDestroyOnLoad(_obj);
            _obj.hideFlags = HideFlags.HideAndDontSave;
            _obj.AddComponent<CoreUpdateBehavior>();
        }
        /// <summary>
        /// This Action is executed each tick in the Update method
        /// </summary>
        public Action ExecuteOnUpdate;

        protected void Update()
        {
            ExecuteOnUpdate?.Invoke(); //todo why it is null?
            MessageService.ProcessAllMessages(); 
        }

        public void Unload()
        {
            LogUtils.LogError("Unloading CoreUpdateBehavior");
            if (_obj)
                Destroy(_obj);
        }

        public void Dispose()
        {
            LogUtils.LogError("Disposing CoreUpdateBehavior");
            if(_obj)
                Destroy(_obj);
        }
    }
}
