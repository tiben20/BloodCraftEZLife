using System;
using System.Collections.Generic;
using System.Linq;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.Utils;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.PlayerLoop;
using static ProjectM.Roofs.RoofTestSceneBootstrapNew;
using DateTime = System.DateTime;

namespace BloodCraftEZLife.Services
{
    internal static class TeleportsService
    {
        public static bool _Completed { get; set; }
        public static bool _InMenu { get; set; }
        private static List<string> _ClanList = new();
        private static List<string> _SocialList = new();
        public class TeleportBoxData
        {
            public string Name { get; set; }
        }
        public static readonly List<TeleportBoxData> _dataList = new();

        public static List<TeleportBoxData> GetTeleportEntries()
        {
            if (_dataList.Count == 0)
                _dataList.Add(new TeleportBoxData { Name = "Open social menu" });
            return _dataList;
        }

        public static void AddListEntry(string name)
        {
            LogUtils.LogInfo("Adding " + name + "to the list");
            if (_dataList.Any(a => a.Name.Equals(name)))
                return;
            _dataList.Add(new TeleportBoxData { Name = name });

        }

        public static void UpdateList()
        {

            
            var panel = Plugin.UIManager.GetPanel<TeleportListPanel>();
            if (panel != null)
                panel.Reset();

            foreach (string cc in _ClanList)
            {

                AddListEntry(cc);
            }
            foreach (string cc in _SocialList)
            {
                
                AddListEntry(cc);
            }
            if (panel != null)
                panel.RefreshData();

        }

        public static void AddClanMember(string inputName)
        {
            if (_ClanList.Count > 0)
            {
                if (_ClanList[0] == inputName)
                {
                    _Completed = true;
                    UpdateList();
                    return;
                }
            }
            else
            {
                var panel = Plugin.UIManager.GetPanel<TeleportListPanel>();
                if (panel != null)
                    panel.Reset();
            }
            _ClanList.Add(inputName);
        }
        public static void AddSocialMember(string inputName)
        {
            if (_SocialList.Count > 0)
            {
                if (_SocialList[0] == inputName)
                {
                    _Completed = true;
                    UpdateList();
                    return;
                }
            }
            else
            {
                var panel = Plugin.UIManager.GetPanel<TeleportListPanel>();
                if (panel != null) 
                    panel.Reset();
            }
            _SocialList.Add(inputName);
        }

        public static void SetPlayerCache(Entity userEntity, bool isOffline = false)
        {
            var networkId = userEntity.Read<NetworkId>();
            var userData = userEntity.Read<User>();
            var name = userData.CharacterName.Value;
            LogUtils.LogError(name);
        }

        public static void ClearList()
        {
            _ClanList.Clear();
            _SocialList.Clear();
            _Completed = false;
            
        }
    }
}
