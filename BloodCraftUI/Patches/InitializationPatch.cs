using System;
using BloodCraftUI.NEW;
using BloodCraftUI.Services;
using BloodCraftUI.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.UI;
using Unity.Collections;
using Unity.Entities;

namespace BloodCraftUI.Patches
{
    [HarmonyPatch]
    public static class InitializationPatch
    {
        [HarmonyPatch]
        internal class CharacterHUDEntry_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CharacterHUDEntry), nameof(CharacterHUDEntry.Awake))]
            private static void AwakePostfix()
            {
                if (!UICustomManager.Initializing) return;
                LogUtils.LogInfo("Creating UI...");
                UICustomManager.InitUI();
            }
        }

        [HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.OnUpdate))]
        [HarmonyPostfix]
        static void OnUpdatePostfix(GameDataManager __instance)
        {
            try
            {
                if (!Plugin.IsInitialized) return;

                Plugin.InitializeProperties(__instance);
            }
            catch (Exception ex)
            {
                LogUtils.LogError($"{ex}");
            }
        }

        [HarmonyPatch(typeof(CommonClientDataSystem), nameof(CommonClientDataSystem.OnUpdate))]
        [HarmonyPostfix]
        static void OnUpdatePostfix(CommonClientDataSystem __instance)
        {
            if (UICustomManager.Initializing) return;

            var entities = __instance.__query_1840110765_0.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (Entity entity in entities)
                {
                    if (entity.Has<LocalUser>()) MessageService.SetUser(entity);
                    break;
                }
            }
            finally
            {
                entities.Dispose();
            }

            entities = __instance.__query_1840110765_1.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (Entity entity in entities)
                {
                    if (entity.Has<LocalCharacter>()) MessageService.SetCharacter(entity);
                    break;
                }
            }
            finally
            {
                entities.Dispose();
            }
        }
    }
}