using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BloodmoonPluginsUI.Utils;
using HarmonyLib;
using ProjectM.Network;
using ProjectM.UI;
using Unity.Collections;
using Unity.Entities;

namespace BloodmoonPluginsUI.Eclipse_Research_WIP
{
    [HarmonyPatch]
    internal class EclipseClientChatSystemPatch
    {
        static readonly Regex _regexExtract = new(@"^\[(\d+)\]:");
        static readonly Regex _regexMAC = new(@";mac([^;]+)$");

        public enum NetworkEventSubType
        {
            RegisterUser,
            ProgressToClient,
            ConfigsToClient
        }

        [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem.OnUpdate))]
        [HarmonyPrefix]
        static void OnUpdatePrefix(ClientChatSystem __instance)
        {
            if (!Plugin.IsGameDataInitialized) return;

            var entities = __instance._ReceiveChatMessagesQuery.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (Entity entity in entities)
                {
                    if (entity.Has<ChatMessageServerEvent>())
                    {
                        var chatMessage = entity.Read<ChatMessageServerEvent>();
                        var message = chatMessage.MessageText.Value;

                        if (chatMessage.MessageType.Equals(ServerChatMessageType.System) && CheckMAC(message, out string originalMessage))
                        {
                            HandleServerMessage(originalMessage);
                        }
                    }
                }
            }
            finally
            {
                entities.Dispose();
            }
        }

        static void HandleServerMessage(string message)
        {
            if (int.TryParse(_regexExtract.Match(message).Groups[1].Value, out int result))
            {
                try
                {
                    switch (result)
                    {
                        case (int)NetworkEventSubType.ProgressToClient:
                            List<string> playerData = DataService.ParseMessageString(_regexExtract.Replace(message, ""));
                            DataService.ParsePlayerData(playerData);
                            var data = DataService.ParseFamiliarData(playerData);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError($"{PluginInfo.PLUGIN_NAME}[{PluginInfo.PLUGIN_VERSION}] failed to handle message after parsing event type - {ex}");
                }
            }
            else
            {
                LogUtils.LogWarning($"{PluginInfo.PLUGIN_NAME}[{PluginInfo.PLUGIN_VERSION}] failed to parse event type after MAC verification - {message}");
            }
        }

        private static bool CheckMAC(string receivedMessage, out string originalMessage)
        {
            Match match = _regexMAC.Match(receivedMessage);
            originalMessage = string.Empty;

            if (match.Success)
            {
                originalMessage = _regexMAC.Replace(receivedMessage, "");
                return true;
            }

            return false;
        }
    }
}
