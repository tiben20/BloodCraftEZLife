using ProjectM.Network;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.ModContent;
using Unity.Entities;
using BloodCraftEZLife.Utils;
using ProjectM;
using System.Timers;
using BepInEx.Logging;
using Unity.Collections;
using static ProjectM.Metrics;
using Unity.Mathematics;
using Unity.Transforms;

namespace BloodCraftEZLife.Services
{
    internal static partial class MessageService
    {
        private enum InterceptFlag
        {
            ListBoxes,
            ListBoxContent
        }

        public struct TeleportRequest
        {
            public string Username;
            public int Acceptcount;
            public TeleportRequest(string username)
            {
                Username = username;
                Acceptcount = 0;
            }
        }

        
        
        private static readonly Dictionary<InterceptFlag, int> Flags = new();
        const string COLOR_PATTERN = "<color=.*?>(.*?)</color>";
        const string EXTRACT_BOX_NAME_PATTERN = "<color=[^>]+>(?<box>.*?)</color>";
        const string EXTRACT_COLOR_PATTERN = "(?<=<color=)[^>]+";
        const string EXTRACT_FAM_LVL_PATTERN = @"\[<color=[^>]+>(\d+)</color>\]\[<color=[^>]+>(\d+)</color>\].*?<color=yellow>(\d+)</color>.*?<color=white>(\d+)%</color>";
        const string EXTRACT_FAM_STATS_PATTERN = @"<color=[^>]+>([^<]+)</color>:\s*<color=[^>]+>([^<]+)</color>(?:,\s*)?";
        const string EXTRACT_FAM_NAME_PATTERN = @"<color=[^>]+>(?<name>[^<]+)</color>";
        const string EXTRACT_FAM_SCHOOL_PATTERN = @"-\s*<color=[^>]+>(?<school>[^<]+)</color>";

        public static bool BoxContentFlag { get; set; }


        internal static void HandleMessage(Entity entity, string whispername)
        {
            var chatMessage = entity.Read<ChatMessageServerEvent>();
            var message = chatMessage.MessageText.Value;
            
            if (whispername != null)
            { 
                if (chatMessage.MessageType.Equals(ServerChatMessageType.WhisperFrom))
                {
                    ConfigSaveManager.ChatMessages.AddMessage(whispername, message, chatMessage.TimeUTC, true);

                    ConfigSaveManager.SavePerServerChat();

                }
                if (chatMessage.MessageType.Equals(ServerChatMessageType.WhisperTo))
                {
                    ConfigSaveManager.ChatMessages.AddMessage(whispername, message, chatMessage.TimeUTC, false);
                    ConfigSaveManager.SavePerServerChat();

                }
        }
            if (chatMessage.MessageType.Equals(ServerChatMessageType.System))
            {
                if (message.Contains("has requested to teleport") && Settings.IsAutoTeleportEnabled)
                {
                    //Spectarz has requested to teleport to you
                    //use ... to accept
                    //Player Spectarz has teleported to you
                    //old version failsafe
                    var regex =
                        new Regex(">([^<]+)<");

                    Match match = regex.Match(message);
                    if (match.Success)
                    {
                        //MessageService.EnqueueMessage(".stp tpa " + match.Groups[1].Value);
                        ContentPanel pan = (ContentPanel)Plugin.UIManager._contentPanel;
                        pan.AddTeleportRequest(match.Groups[1].Value);
                    }


                    return;
                }
                if (message.Contains("has teleported to you") && false)
                {
                    LogUtils.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} " + message);
                    var regex = new Regex(">[^<]+</color>[^>]+>([^<]+)<");
                    Match match = regex.Match(message);
                    if (match.Success)
                    {
                        ContentPanel pan = (ContentPanel)Plugin.UIManager._contentPanel;
                        pan.RemoveTeleportRequest(match.Groups[1].Value);
                    }
                    return;

                }
            }
        }

        

        

    }
}
