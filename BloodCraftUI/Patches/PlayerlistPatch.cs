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
using UnityEngine;
using TMPro;

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
    [HarmonyPatch(typeof(FullscreenMenu_OuterTabEntry), nameof(FullscreenMenu_OuterTabEntry.Refresh))]
    [HarmonyPostfix]
    private static void OnStartRunning_53CF59E2_b__1(FullscreenMenu_OuterTabEntry __instance,bool isSelected, bool requirementMet, bool highlight)
    {
        if (__instance.name == "TabEntry_System")
        {
            LogUtils.LogInfo($"___OnStartRunning_53CF59E2_b__38_0");
        }
        


    }

    [HarmonyPatch(typeof(OptionsMenu), nameof(OptionsMenu.Update))]
    [HarmonyPostfix]
    private static void OptionsMenuUpdate(OptionsMenu __instance)
    {
        
        //SoundTabButton
        //SimpleStunButton
        //Verify we didnt add it already
        /* HEADER BUTTON IN OPTIONS MENU EZLife */
        SimpleStunButton simpleButton = __instance.SoundTabButton;
        //Check if button exist if it exist quit
        Transform btnSearch = simpleButton.transform.parent.Find("EZLifeButton");
        if (btnSearch != null)
            return;

        var settingsButton = UnityEngine.Object.Instantiate(simpleButton.gameObject,simpleButton.transform.parent);
        settingsButton.name = "EZLifeButton";

        var newSettingsButton = settingsButton.GetComponent<SimpleStunButton>();

        var label = settingsButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label != null)
            label.text = "EZLife";
        //TODO
        newSettingsButton.m_OnClick = null;


        TextMeshProUGUI[] texts = __instance.GetComponentsInChildren<TextMeshProUGUI>(true);
        // (true) means include inactive objects

        foreach (TextMeshProUGUI tmp in texts)
        {
            if (tmp.text == "Language")
            {
                Debug.Log("Found Language");
            }
            if (tmp.text == "General")
            {
                Debug.Log("General");
            }
            if (tmp.text == "Text Scaling")
            {
                Debug.Log("Text Scaling");
            }
            Debug.Log("Found TMP: " + tmp.text);
        }
        foreach (var cmp in __instance.SoundPanel.GetComponents<Component>())
        {

            LogUtils.LogInfo(cmp.transform.ToString());
            foreach (var childCmp in cmp.GetComponentsInChildren<Component>())
            {
                ;
                LogUtils.LogInfo("Child :" + childCmp.transform.ToString());
            }

        }



    }

}
