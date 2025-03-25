using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using NoFog.Behaviors;
using NoFog.Config;
using NoFog.Utils;
using ProjectM;
using Unity.Entities;
using UnityEngine;
using UniverseLib;

namespace NoFog
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    //[BepInDependency("panthernet.SilkwormReborn")]
    public class Plugin : BasePlugin
    {
        Harmony _harmony;
        public static Plugin Instance { get; private set; }
        public static Harmony Harmony => Instance._harmony;
        public static ManualLogSource LogInstance => Instance.Log;
        public static Settings Settings { get; private set; }
        static World _client;
        public static EntityManager EntityManager => _client.EntityManager;
        public static bool IsInitialized { get; private set; }

        public static bool IsClientNull() => _client == null;

        // This is the method that is called when the game is started.
        // It is an automatic method that is called by BepInEx

        public static void InitializeProperties(GameDataManager gameDataManager)
        {
            _client = gameDataManager.World;
        }

        public override void Load()
        {
            LogUtils.Init(Log);

            Universe.Init(1f, null, (s, type) =>
            {
                switch (type)
                {
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                        LogUtils.LogError(s);
                        break;
                    case LogType.Warning:
                        LogUtils.LogWarning(s);
                        break;
                    case LogType.Log:
                        LogUtils.LogInfo(s);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }, new()
            {
                Disable_EventSystem_Override = true,
                Force_Unlock_Mouse = true
            });

            Instance = this;
            Settings = new Settings();
            Settings.InitConfig();
            CoreBehavior.Setup();

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} version {PluginInfo.PLUGIN_VERSION} is loaded!");

            // Harmony patching
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            IsInitialized = true;
        }

        // This is the method that is called when the game is closed.
        // It is an automatic method that is called by BepInEx
        // It is also used to unpatch the Harmony patches.
        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            return true;
        }

        public static void Reset()
        {
            _client = null;
            IsInitialized = false;
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "panthernet.NoFog";
        public const string PLUGIN_NAME = "NoFog";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
