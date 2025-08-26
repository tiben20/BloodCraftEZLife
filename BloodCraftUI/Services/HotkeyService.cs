using UnityEngine.UI;
using UnityEngine;
using ProjectM.UI;
using BloodCraftEZLife.Utils;
using ProjectM;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.Config;
using Unity.Entities;
using Unity.Mathematics;
using static Il2CppSystem.Data.Common.ObjectStorage;
using System.Collections.Generic;
using ProjectM.Scripting;
using static BloodCraftEZLife.Services.TeleportsService;


namespace BloodCraftEZLife.Services
{
    public class Hotkey
    {
        public KeyCode key { get; set; }
        public string action { get; set; }
        public Hotkey(KeyCode k, string a)
        {
            key = k;
            action=a;
        }
    }

    internal static class HotkeyService
    {
        private static Dictionary<KeyCode, string> bindings = new();
        private static float timer = 0f;
        public static bool WaitingForInput = false;
        public static bool NewKey = false;
        public static KeyCode WaitedForKey = KeyCode.None;
        public static KeyCode ChangingOldKey = KeyCode.None;


        public static void Initialise()
        {
            SettingsHotkeys h = ConfigSaveManager.LoadHotkeys();
            bindings = h.HotKeys;
        }
        public static List<Hotkey> GetHotkeys()
        {
            List<Hotkey> k = new List<Hotkey>();
            foreach (var key in bindings.Keys)
            {
                k.Add(new Hotkey(key, bindings[key]));
            }
            return k;
        }

        public static int HotkeyCount()
        {
            return bindings.Count;
         }

        public static int GetHotkeyIndex(KeyCode k)
        {
            int i = 0;
            foreach (var kvp in bindings)
            {
                if (kvp.Key == k)
                    return i;
                i++;
            }
            return -1;
        }

        public static Hotkey GetHotkey(int index)
        {
            int i = 0;
            foreach (var kvp in bindings)
            {
                if (index == i)
                    return new Hotkey(kvp.Key, kvp.Value);
                i++;
            }
            return new Hotkey(KeyCode.F1,""); // not found
        }

        public static void Delete(KeyCode key)
        {

            bindings.Remove(key);
        }

        public static void Register(KeyCode key, string action)
        {
            
            bindings[key] = action;
        }

        public static void SetKey(Hotkey key, KeyCode newkey)
        {
            if (bindings.ContainsKey(key.key))
            {
                string oldaction = bindings[key.key];
                bindings.Remove(key.key);
                bindings.Add(newkey, oldaction);
                
                return;
            }
            bindings[key.key] = key.action;
        }

        public static void Unregister(KeyCode key)
        {
            bindings.Remove(key);
        }

        public static void SaveHotkeys()
        {
            ConfigSaveManager.Save(bindings);
        }

        public static void Update()
        {
            if (WaitingForInput)
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {

                    if (Input.GetKeyDown(k) && !k.ToString().ToLower().Contains("mouse"))
                    {
                        if (bindings.ContainsKey(k))
                        {
                            if (ChangingOldKey != k)
                                continue;
                        }

                        WaitedForKey = k;
                        WaitingForInput = false;
                        return;
                    }
                }
                return;
            }
            timer += Time.deltaTime;
            if (timer < 0.3f)
                return;
            foreach (var kvp in bindings)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    var panel = Plugin.UIManager.GetPanel<PopupPanel>();
                    if (panel != null)
                    {
                        panel.ShowMessage(kvp.Value, 3f, PopupPanel.MessageType.Small);
                    }
                    if (kvp.Value.Contains("|"))
                    {
                        foreach (var valuesplit in kvp.Value.Split("|"))
                            MessageService.EnqueueMessage(valuesplit);
                    }
                    else
                    {
                        MessageService.EnqueueMessage(kvp.Value);
                    }
                    timer = 0f;
                }
            }
        }
    }
}
