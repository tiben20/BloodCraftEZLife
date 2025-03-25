using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UniverseLib;
using UniverseLib.Input;
using IL2CPPUtils = Il2CppInterop.Common.Il2CppInteropUtils;

namespace NoFog._TRASH
{
    /*public static class IInputManager
    {
        // TODO: Refactor this to have the input system used on the game saved in a variable,
        // and just call the methods of this variable in each function, like a proper wrapper.
        private static InputType currentInputType;

        public static void Setup()
        {
            currentInputType = InputManager.CurrentType;

            switch (currentInputType)
            {
                case InputType.Legacy:
                    ILegacyInput.Init();
                    break;
                case InputType.InputSystem:
                case InputType.None:
                    break;
            }
        }

        public static bool GetKey(KeyCode key)
        {
            switch (currentInputType)
            {
                case InputType.Legacy:
                    return ILegacyInput.GetKey(key);
                case InputType.InputSystem:
                case InputType.None:
                default:
                    return InputManager.GetKey(key);
            }
        }

        public static bool GetKeyDown(KeyCode key)
        {
            switch (currentInputType)
            {
                case InputType.Legacy:
                    return ILegacyInput.GetKeyDown(key);
                case InputType.InputSystem:
                case InputType.None:
                default:
                    return InputManager.GetKeyDown(key);
            }
        }

        public static bool GetKeyUp(KeyCode key)
        {
            switch (currentInputType)
            {
                case InputType.Legacy:
                    return ILegacyInput.GetKeyUp(key);
                case InputType.InputSystem:
                case InputType.None:
                default:
                    return InputManager.GetKeyUp(key);
            }
        }

        public static bool GetMouseButton(int button)
        {
            switch (currentInputType)
            {
                case InputType.Legacy:
                    return ILegacyInput.GetMouseButton(button);
                case InputType.InputSystem:
                case InputType.None:
                default:
                    return InputManager.GetMouseButton(button);
            }
        }

        public static bool GetMouseButtonDown(int button)
        {
            switch (currentInputType)
            {
                case InputType.Legacy:
                    return ILegacyInput.GetMouseButtonDown(button);
                case InputType.InputSystem:
                case InputType.None:
                default:
                    return InputManager.GetMouseButtonDown(button);
            }
        }

        // We won't mock this as it made it impossible to drag the mod panels across the screen in some games for some reason.
        // It might affect some games that use custom classes to control the camera, but those would still probably need to be
        // manually disabled because of the camera position control override. Should try it out with more games.
        public static Vector3 MousePosition => (Vector3)InputManager.MousePosition;
    }

    public static class ILegacyInput
    {
        public static Dictionary<KeyCode, bool> getKeyDict = new Dictionary<KeyCode, bool>();
        public static Dictionary<KeyCode, bool> getKeyDownDict = new Dictionary<KeyCode, bool>();
        public static Dictionary<KeyCode, bool> getKeyUpDict = new Dictionary<KeyCode, bool>();

        public static Dictionary<int, bool> getMouseButton = new Dictionary<int, bool>();
        public static Dictionary<int, bool> getMouseButtonDown = new Dictionary<int, bool>();

        // Wrapped methods

        public static bool GetKey(KeyCode key)
        {
            if (key == KeyCode.None) return false;
            // Trigger the original InputManager method
            InputManager.GetKey(key);
            return getKeyDict[key];
        }

        public static bool GetKeyDown(KeyCode key)
        {
            if (key == KeyCode.None) return false;
            // Trigger the original InputManager method
            InputManager.GetKeyDown(key);
            return getKeyDownDict[key];
        }

        public static bool GetKeyUp(KeyCode key)
        {
            if (key == KeyCode.None) return false;
            // Trigger the original InputManager method
            InputManager.GetKeyUp(key);
            return getKeyUpDict[key];
        }

        public static bool GetMouseButton(int button)
        {
            // Trigger the original InputManager method
            InputManager.GetMouseButton(button);
            return getMouseButton[button];
        }

        public static bool GetMouseButtonDown(int button)
        {
            // Trigger the original InputManager method
            InputManager.GetMouseButtonDown(button);
            return getMouseButtonDown[button];
        }

        // Patch the input methods of the legacy input system
        public static void Init()
        {
            Type t_Input = ReflectionUtility.GetTypeByName("UnityEngine.Input");

            try
            {
                MethodInfo getKeyTarget = t_Input.GetMethod("GetKey", new Type[] { typeof(string) });
                //ExplorerCore.LogWarning(getKeyTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyString))));
            }
            catch { }

            try
            {
                MethodInfo getKeyTarget = t_Input.GetMethod("GetKey", new Type[] { typeof(KeyCode) });
                //ExplorerCore.LogWarning(getKeyTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyKeyCode))));
            }
            catch { }

            try
            {
                MethodInfo getKeyDownTarget = t_Input.GetMethod("GetKeyDown", new Type[] { typeof(string) });
                //ExplorerCore.LogWarning(getKeyDownTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyDownTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyDownTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyDownString))));
            }
            catch { }

            try
            {
                MethodInfo getKeyDownTarget = t_Input.GetMethod("GetKeyDown", new Type[] { typeof(KeyCode) });
                //ExplorerCore.LogWarning(getKeyDownTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyDownTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyDownTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyDownKeyCode))));
            }
            catch { }

            try
            {
                MethodInfo getKeyUpTarget = t_Input.GetMethod("GetKeyUp", new Type[] { typeof(string) });
                //ExplorerCore.LogWarning(getKeyUpTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyUpTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyUpTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyUpString))));
            }
            catch { }

            try
            {
                MethodInfo getKeyUpTarget = t_Input.GetMethod("GetKeyUp", new Type[] { typeof(KeyCode) });
                //ExplorerCore.LogWarning(getKeyUpTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getKeyUpTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getKeyUpTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideKeyUpKeyCode))));
            }
            catch { }

            try
            {
                MethodInfo getMouseButtonTarget = t_Input.GetMethod("GetMouseButton", new Type[] { typeof(int) });
                //ExplorerCore.LogWarning(getMouseButtonTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getMouseButtonTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getMouseButtonTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideMouseButton))));
            }
            catch { }

            try
            {
                MethodInfo getMouseButtonDownTarget = t_Input.GetMethod("GetMouseButtonDown", new Type[] { typeof(int) });
                //ExplorerCore.LogWarning(getMouseButtonDownTarget);
                if (IL2CPPUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(getMouseButtonDownTarget) == null)
                    throw new Exception();
                Plugin.Harmony.Patch(getMouseButtonDownTarget,
                    postfix: new(AccessTools.Method(typeof(ILegacyInput), nameof(OverrideMouseButtonDown))));
            }
            catch { }
        }

        // Postfix functions

        public static void OverrideKeyString(ref bool __result, ref string name)
        {
            KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
            getKeyDict[thisKeyCode] = __result;
        }

        public static void OverrideKeyKeyCode(ref bool __result, ref KeyCode key)
        {
            if (key == KeyCode.None) return;

            getKeyDict[key] = __result;
        }

        public static void OverrideKeyDownString(ref bool __result, ref string name)
        {
            KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
            getKeyDownDict[thisKeyCode] = __result;
        }

        public static void OverrideKeyDownKeyCode(ref bool __result, ref KeyCode key)
        {
            if (key == KeyCode.None) return;

            getKeyDownDict[key] = __result;
        }

        public static void OverrideKeyUpString(ref bool __result, ref string name)
        {
            KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
            getKeyUpDict[thisKeyCode] = __result;
        }

        public static void OverrideKeyUpKeyCode(ref bool __result, ref KeyCode key)
        {
            if (key == KeyCode.None) return;

            getKeyUpDict[key] = __result;
        }

        public static void OverrideMouseButton(ref bool __result, ref int button)
        {
            getMouseButton[button] = __result;
        }

        public static void OverrideMouseButtonDown(ref bool __result, ref int button)
        {
            getMouseButtonDown[button] = __result;
        }
    }*/
}
