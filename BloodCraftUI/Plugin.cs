using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BloodCraftUI.Behaviors;
using BloodCraftUI.Config;
using BloodCraftUI.NewUI;
using BloodCraftUI.NewUI.Patches;
using BloodCraftUI.NewUI.UICore.UI;
using BloodCraftUI.Patches;
using BloodCraftUI.Utils;
using Bloodstone;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using Unity.Entities;
using UnityEngine;

namespace BloodCraftUI
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Plugin Instance { get; private set; }
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


        public static void Reset()
        {
            _client = null;
            IsInitialized = false;
        }


        /// /////////////////// NEW


        private static Harmony _harmonyBootPatch;
        private static Harmony _harmonyChatPatch;
        private static Harmony _harmonyInitPatch;
        private static Harmony _harmonyCanvasPatch;
        private static Harmony _harmonyMenuPatch;
        internal static Harmony _harmonyVersionStringPatch;
        private static FrameTimer _uiInitialisedTimer;

        public override void Load()
        {
            LogUtils.Init(Log);
            Instance = this;

            if (VWorld.IsServer)
            {
                LogUtils.LogWarning($"Plugin {MyPluginInfo.PLUGIN_GUID} is a client plugin only. Not continuing to load on server.");
                return;
            }

            BCUIManager.Initialize();
            BCUIManager.SetActive(true);

            Settings = new Settings().InitConfig();
            CoreBehavior.Setup();
            IsInitialized = true;

            _harmonyBootPatch = Harmony.CreateAndPatchAll(typeof(GameManagerPatch));
            _harmonyMenuPatch = Harmony.CreateAndPatchAll(typeof(EscapeMenuPatch));
            _harmonyCanvasPatch = Harmony.CreateAndPatchAll(typeof(UICanvasSystemPatch));
            _harmonyVersionStringPatch = Harmony.CreateAndPatchAll(typeof(VersionStringPatch));
            _harmonyChatPatch = Harmony.CreateAndPatchAll(typeof(ClientChatPatch));
            _harmonyInitPatch = Harmony.CreateAndPatchAll(typeof(InitializationPatch));

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} version {PluginInfo.PLUGIN_VERSION} is loaded!");

            AddTestUI();
        }

        public override bool Unload()
        {
            _harmonyBootPatch.UnpatchSelf();
            _harmonyCanvasPatch.UnpatchSelf();
            _harmonyMenuPatch.UnpatchSelf();
            _harmonyVersionStringPatch.UnpatchSelf();
            _harmonyChatPatch.UnpatchSelf();
            _harmonyInitPatch.UnpatchSelf();
            return true;
        }

        private void AddTestUI()
        {
            BCUIManager.OnInitialized();
        }

        public static void GameDataOnInitialize(World world)
        {
            if (VWorld.IsClient)
            {
                // We only want to run this once, so unpatch the hook that initiates this callback.
                _harmonyBootPatch.UnpatchSelf();
                _uiInitialisedTimer = new FrameTimer();

                _uiInitialisedTimer.Initialise(() =>
                    {
                        BCUIManager.OnInitialized();
                        _uiInitialisedTimer.Stop();
                        LogUtils.LogInfo($"UI Manager initialized");
                    },
                    TimeSpan.FromSeconds(5),
                    true).Start();
            }
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "panthernet.BloodCraftUI";
        public const string PLUGIN_NAME = "BloodCraftUI";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
