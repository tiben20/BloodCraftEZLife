using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Patches;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModernLib;
using BloodCraftEZLife.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using ProjectM.UI;
using Stunlock.Core;
using Unity.Entities;
using UnityEngine;

namespace BloodCraftEZLife
{
    [BepInProcess("VRising.exe")]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Plugin Instance { get; private set; }
        public static ManualLogSource LogInstance => Instance.Log;
        public static Settings Settings { get; private set; }
        public static World _client;
        public static EntityManager EntityManager => _client.EntityManager;
        public static LocalizationService Localization { get; private set; } = new();

        public static bool IsInitialized { get; private set; }

        public static Core Core => IsGameDataInitialized ? Core.Instance : throw new InvalidOperationException("Core is not initialized");

        public static bool IsGameDataInitialized { get; set; }
        public static BCUIManager UIManager { get; set; }
        public static CoreUpdateBehavior CoreUpdateBehavior { get; set; }
        public static bool IsClient { get; private set; }
        public static Entity LocalCharacter { get; set; }

        

        public static bool IsClientNull() => _client == null;

        public const bool IS_TESTING = false;


        public static void Reset()
        {
            _client = null;
            IsInitialized = false;
            IsGameDataInitialized = false;
        }

        
        private static Harmony _harmonyChatPatch;
        private static Harmony _harmonyPlayerlistPatch;
        private static Harmony _harmonyInitPatch;
        
        internal static Harmony HarmonyVersionStringPatch;

        public override void Load()
        {
            IsClient = Application.productName != "VRisingServer";
            LogUtils.Init(Log);
            Instance = this;


            if (!IsClient)
            {
                LogUtils.LogInfo($"{PluginInfo.PLUGIN_NAME}[{PluginInfo.PLUGIN_VERSION}] is a client mod! ({Application.productName})");
                return;
            }

            Settings = new Settings().InitConfig();
            Theme.Opacity = Settings.UITransparency;
  
            UIManager = new BCUIManager();
            CoreUpdateBehavior = new CoreUpdateBehavior();
      
            CoreUpdateBehavior.Setup();
            

            IsInitialized = true;
            HarmonyVersionStringPatch = Harmony.CreateAndPatchAll(typeof(VersionStringPatch));
            _harmonyChatPatch = Harmony.CreateAndPatchAll(typeof(ClientChatPatch));
            
            _harmonyPlayerlistPatch = Harmony.CreateAndPatchAll(typeof(FullscreenPatch));
            _harmonyInitPatch = Harmony.CreateAndPatchAll(typeof(InitializationPatch));
            HotkeyService.Initialise();
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} version {PluginInfo.PLUGIN_VERSION} is loaded!");
            Localization = new LocalizationService();
        }

        
        public override bool Unload()
        {

            HarmonyVersionStringPatch.UnpatchSelf();
            _harmonyChatPatch.UnpatchSelf();
            _harmonyPlayerlistPatch.UnpatchSelf();
            _harmonyInitPatch.UnpatchSelf();

            return true;
        }

        //run on game start
        public static void GameDataOnInitialize(World world)
        {
            if (!IsGameDataInitialized && IsClient)
            {
                _client = world;
                //we have the connection string so we can load the server specific json
                FullscreenSettingService.InitialiseVbloodData(ProjectM.StunAnalytics.Client.GetServerId(EntityManager));;
                IsGameDataInitialized = true;
                
            }
        }
        public static void UIOnInitialize()
        {
            UIManager.SetupAndShowUI();
            BloodCraftStateService.Initialize();
            LogUtils.LogInfo($"UI Manager initialized");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "BloodCraftEZLife.PluginsUI";
        public const string PLUGIN_NAME = "BloodCraftEZLife";
        public const string PLUGIN_VERSION = "0.5.0";
    }
}
