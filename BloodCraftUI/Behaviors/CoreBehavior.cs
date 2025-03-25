using BloodCraftUI.Services;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace BloodCraftUI.Behaviors
{
    internal class CoreBehavior: MonoBehaviour
    {
        internal static CoreBehavior Instance { get; private set; }
        internal static void Setup()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CoreBehavior>();

            GameObject obj = new("CoreBehavior");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<CoreBehavior>();
        }

        internal void Update()
        {
            /*if (InputManager.GetKeyDown(KeyCode.F7) && !UICustomManager.Initializing)
                UICustomManager.ShowMenu = !UICustomManager.ShowMenu;
            */ //TODO
            MessageService.ProcessAllMessages();
        }
    }
}
