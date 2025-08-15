using BloodCraftEZLife.Services;
using BloodCraftEZLife.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.UI;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;
using static ProjectM.UI.SocialMenu;
using System;
using System.Collections.Generic;
using Unity.Entities.CodeGeneratedJobForEach;
using BloodCraftEZLife.UI.ModContent;

namespace BloodCraftEZLife.Patches;

[HarmonyPatch]
internal static class PlayerListPatch
{


    [HarmonyPatch(typeof(ClanMenu), nameof(ClanMenu._StunShared_UI_IInitializeableUI_InitializeUI_b__137_0))]
    [HarmonyPostfix]
    private static void ClanMenu2(ClanMenu __instance, ClanMenu_MemberEntry entry, ClanMenu_MemberEntry.Data data)
    {
        if (TeleportsService._Completed)
            return;
        //clan member name
        TeleportsService.AddClanMember(data.Name);
    }

    [HarmonyPatch(typeof(ClanMenu), nameof(ClanMenu._StunShared_UI_IInitializeableUI_InitializeUI_b__137_3))]
    [HarmonyPostfix]
    private static void ClanMenu3(ClanMenu __instance, ClanMenu_MemberEntry entry, ClanMenu_MemberEntry.Data data)
    {
        if (TeleportsService._Completed)
            return;
        TeleportsService.AddSocialMember(data.Name);

        
    }

    [HarmonyPatch(typeof(FullscreenMenuMapper), nameof(FullscreenMenuMapper.OnStartRunning))]
    [HarmonyPostfix]
    private static void _FullscreenMenuOnStartRunning(FullscreenMenu __instance)
    {
        TeleportsService._InMenu = true;
        TeleportsService.ClearList();
        LogUtils.LogInfo($"OnStartRunning");
    }

    [HarmonyPatch(typeof(FullscreenMenuMapper), nameof(FullscreenMenuMapper.OnStopRunning))]
    [HarmonyPostfix]
    private static void _FullscreenMenuOnStopRunning(FullscreenMenu __instance)
    {
        TeleportsService._InMenu = false;
        LogUtils.LogInfo($"OnStopRunning");

      
    }

}
