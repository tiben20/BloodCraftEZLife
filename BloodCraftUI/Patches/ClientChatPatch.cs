using BloodmoonPluginsUI.Services;
using BloodmoonPluginsUI.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.UI;
using System;
using Unity.Collections;
using Unity.Entities;
using static ProjectM.Roofs.RoofTestSceneBootstrapNew;

namespace BloodmoonPluginsUI.Patches;

[HarmonyPatch]
internal static class ClientChatPatch
{
    [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem.ReceiveChatMessages))]
    [HarmonyPostfix]
    private static void _OnReceiveChatMessagesPostfix(NetworkId localUserNetworkId,
        ChatColorsAsset colors,
        bool showTimeStamp,
        string timeStamp,
        NetworkIdLookupMap networkIdMap)
    {
        LogUtils.LogInfo($"_OnInputEndEditPostfix");
        var e = networkIdMap._NetworkIdToEntityMap[localUserNetworkId];
    }

    [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem._OnInputEndEdit))]
    [HarmonyPostfix]
    private static void _OnInputEndEditPostfix(ClientChatSystem __instance)
    {
        LogUtils.LogInfo($"_OnInputEndEditPostfix");
    }

    [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem._OnInputSelect))]
    [HarmonyPostfix]
    private static void _OnInputSelectPostfix(string arg0)
    {
        LogUtils.LogInfo($"_OnInputSelectPostfix: {arg0}");
    }

    [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem.OnUpdate))]
    [HarmonyPrefix]
    private static void OnUpdatePrefix(ClientChatSystem __instance)
    {
        if (Plugin.IsClientNull())
            return;

        if (__instance == null)
            return;

        var entities = __instance._ReceiveChatMessagesQuery.ToEntityArray(Allocator.Temp);
        
        try
        {
            foreach (var entity in entities)
            {
                if (!entity.Has<ChatMessageServerEvent>()) continue;

                MessageService.HandleMessage(entity);
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
}