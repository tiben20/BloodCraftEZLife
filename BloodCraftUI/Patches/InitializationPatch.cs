using System;
using BloodCraftUI.NewUI;
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
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterHUDEntry), nameof(CharacterHUDEntry.Awake))]
        private static void AwakePostfix()
        {
            try
            {
                if (BCUIManager.IsInitialized) return;
                LogUtils.LogInfo("Creating UI...");
                Plugin.UIOnInitialize();
            }
            catch (Exception ex)
            {
                LogUtils.LogError(ex.ToString());
            }
        }
        
        [HarmonyPatch(typeof(CommonClientDataSystem), nameof(CommonClientDataSystem.OnUpdate))]
        [HarmonyPostfix]
        static void OnUpdatePostfix(CommonClientDataSystem __instance)
        {
            if (!BCUIManager.IsInitialized) return;

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