using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BloodCraftUI.Config;
using BloodCraftUI.NewUI;
using BloodCraftUI.NewUI.UICore.UI.Panel;
using BloodCraftUI.Services;
using BloodCraftUI.Utils;
using HarmonyLib;
using Il2CppSystem;
using ProjectM.Network;
using ProjectM.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace BloodCraftUI.Patches
{
    [HarmonyPatch]
    internal static class ClientChatPatch
    {
        private static string _currentBox;

        [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem._OnInputEndEdit))]
        [HarmonyPostfix]
        static void _OnInputEndEditPostfix(ClientChatSystem __instance)
        {
            LogUtils.LogInfo($"_OnInputEndEditPostfix");
        }

        [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem._OnInputSelect))]
        [HarmonyPostfix]
        static void _OnInputSelectPostfix(string arg0)
        {
            LogUtils.LogInfo($"_OnInputSelectPostfix: {arg0}");
        }

        private enum InterceptFlag
        {
            ListBoxes,
            ListBoxContent
        }

        private static Dictionary<InterceptFlag, int> _flags = new();
        const string _color_pattern = "<color=.*?>(.*?)</color>";
        const string _extract_color_pattern = "(?<=<color=)[^>]+";

        [HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem.OnUpdate))]
        [HarmonyPrefix]
        static void OnUpdatePrefix(ClientChatSystem __instance)
        {
            if (Plugin.IsClientNull())
                return;

            var entities = __instance._ReceiveChatMessagesQuery.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (var entity in entities)
                {
                    if (!entity.Has<ChatMessageServerEvent>()) continue;

                    var chatMessage = entity.Read<ChatMessageServerEvent>();
                    var message = chatMessage.MessageText.Value;

                    if (chatMessage.MessageType == ServerChatMessageType.Local)
                    {
                        if (message.StartsWith(".fam"))
                        {
                            DestroyMessage(entity);
                            continue;
                        }
                    }

                    if (!chatMessage.MessageType.Equals(ServerChatMessageType.System))
                        continue;

                    switch (message)
                    {
                        /////// FLAGS
                        case { } x when message.StartsWith("Couldn't find familiar to unbind"):
                        case { } y when message.Contains(">unbound</color>!"):
                            BloodCraftStateService.IsFamUnbound = true;
                            break;
                        case { } x when message.Contains(">bound</color>!"):
                            BloodCraftStateService.IsFamUnbound = false;
                            break;

                        /////// LOGIC
                        case { } x when message.StartsWith("Familiar Boxes"):
                            ClearFlags();
                            _flags[InterceptFlag.ListBoxes] = 1;
                            BCUIManager.GetPanel<BoxListPanel>().Reset();
                            break;
                        case { } x when message.StartsWith("<color=yellow>1</color>|"):
                            ClearFlags();
                            _flags[InterceptFlag.ListBoxContent] = 1;
                            if (_currentBox != null)
                            {
                                BCUIManager.GetBoxPanel(_currentBox).Reset();
                                ProcessBoxContentEntry(message);
                            }

                            break;
                        case { } x when message.StartsWith("Box Selected"):
                            var index = message.IndexOf('-');
                            var boxNameTemp = message.Substring(index, message.Length - index).Trim();
                            _currentBox = Regex.Matches(boxNameTemp, _color_pattern).FirstOrDefault()?.Groups[1].Value;
                            break;

                        default:
                            {
                                //list box content
                                if (_flags.HasKeyValue(InterceptFlag.ListBoxContent, 1))
                                {
                                    //stop
                                    if (message.Length >= 2 && !message.Contains("</color>|"))
                                    {
                                        _flags.SetValue(InterceptFlag.ListBoxContent, 0);
                                        continue;
                                    }

                                    ProcessBoxContentEntry(message);
                                }

                                //list boxes
                                if (_flags.HasKeyValue(InterceptFlag.ListBoxes, 1))
                                {
                                    //stop
                                    if (message.Trim().Contains(" ") || !message.StartsWith("<color"))
                                    {
                                        _flags.SetValue(InterceptFlag.ListBoxes, 0);
                                        continue;
                                    }

                                    var text = Regex.Matches(message, _color_pattern).FirstOrDefault()?.Groups[1].Value;
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        BCUIManager.GetPanel<BoxListPanel>().AddListEntry(text);
                                    }

                                    DestroyMessage(entity);
                                }
                            }
                            break;
                    }
                }
            }
            finally
            {
                entities.Dispose();
            }
        }

        private static void ProcessBoxContentEntry(string message)
        {
            try
            {
                var colorText = Regex.Match(message, _extract_color_pattern).Value;


                var text = string.Join(' ', Regex.Matches(message, _color_pattern).Select(a => a.Groups[1].Value));
                var colorName = GetColorName(colorText);
                var text2 = $"{text.Substring(2).Trim()}{(colorName.Name == "Normal" ? null : $" - {colorName.Name}")}";
                var number = Convert.ToInt32(text.Substring(0, 1));
                BCUIManager.GetBoxPanel(_currentBox).AddListEntry(number, text2, colorName);
            }
            catch
            {
                LogUtils.LogError($"{nameof(ProcessBoxContentEntry)} Parsing error");
            }
        }

        private static ColorNameData GetColorName(string colorText)
        {
            switch (colorText)
            {
                case "#008080":
                    return new ColorNameData { Name = "Illusion", Color = new Color(0f, 128f, 128f) };
                case "#00FFFF":
                    return new ColorNameData { Name = "Frost", Color = new Color(0f, 255f, 255f) };

                case "#FF0000":
                    return new ColorNameData { Name = "Blood", Color = new Color(255f, 0f, 0f) };
                case "#FFD700":
                    return new ColorNameData { Name = "Storm", Color = new Color(255f, 215f, 0f) };
                case "#A020F0":
                    return new ColorNameData { Name = "Chaos", Color = new Color(160f, 32f, 240f) };
                case "#00FF00":
                    return new ColorNameData { Name = "Unholy", Color = new Color(0f, 255f, 0f) };
                default:
                    return new ColorNameData { Name = "Normal", Color = Color.white };

            }
        }

        public class ColorNameData
        {
            public Color Color { get; set; }
            public string Name { get; set; }
        }

        private static void ClearFlags()
        {
            _flags.Clear();
        }

        private static void DestroyMessage(Entity entity)
        {
            if (Settings.ClearServerMessages)
                Plugin.EntityManager.DestroyEntity(entity);

        }
    }

}
