using BloodCraftEZLife.Services;
using BloodCraftEZLife.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.UI;
using System;
using Unity.Collections;
using Unity.Entities;
using static ProjectM.Roofs.RoofTestSceneBootstrapNew;

namespace BloodCraftEZLife.Patches;

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
        var e = networkIdMap._NetworkIdToEntityMap[localUserNetworkId];
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
                if (!entity.Has<ChatMessageServerEvent>())
                    continue;
                MessageService.HandleMessage(entity, __instance._WhisperUserName);
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
}