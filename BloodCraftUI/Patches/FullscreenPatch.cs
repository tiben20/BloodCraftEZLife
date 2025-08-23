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
using static BloodCraftEZLife.Services.FullscreenSettingService;
using BloodCraftEZLife.UI.UniverseLib.UI;
using UnityEngine.UIElements;
using UnityEngine.UI;
using ProjectM.Presentation;
using BloodCraftEZLife.Config;
using static ProjectM.VBloodSystem;
using ProjectM.Gameplay.Systems;
using BloodCraftEZLife.UI;
using Stunlock.Core;
using UnityEngine.Events;

namespace BloodCraftEZLife.Patches;

[HarmonyPatch]
internal static class FullscreenPatch
{


    [HarmonyPatch(typeof(ClanMenu), nameof(ClanMenu._StunShared_UI_IInitializeableUI_InitializeUI_b__137_0))]
    [HarmonyPostfix]
    private static void ClanMenu2(ClanMenu __instance, ClanMenu_MemberEntry entry, ClanMenu_MemberEntry.Data data)
    {
        
        if (TeleportsService._Completed || !data.IsOnline)
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
        ContentPanel panel;
        //hidding our main panel when in mainmenu
        panel = (ContentPanel)Plugin.UIManager._contentPanel;
        if (panel != null)
        {
            var group = panel.UIRoot.GetComponent<CanvasGroup>();
            if (group == null)
                group = panel.UIRoot.AddComponent<CanvasGroup>();

            // Hide
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
    [HarmonyPatch(typeof(MainMenuNewView), nameof(MainMenuNewView.Awake))]
    [HarmonyPostfix]
    private static void _FullscreenMenuOnUpdate(MainMenuNewView __instance)
    {
        //hidding our config panels when pressed escape while in the settings pannel
        var panel = Plugin.UIManager.GetPanel<SettingsPanel>();
        if (panel != null)
        {
            panel.SetActive(false);
        }
        var panel2 = Plugin.UIManager.GetPanel<HotkeysPanel>();
        if (panel2 != null)
        {
            panel2.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(FullscreenMenuMapper), nameof(FullscreenMenuMapper.OnStopRunning))]
    [HarmonyPostfix]
    private static void _FullscreenMenuOnStopRunning(FullscreenMenu __instance)
    {
        
        TeleportsService._InMenu = false;
        ContentPanel panel;
        panel = (ContentPanel)Plugin.UIManager._contentPanel;
        if (panel != null)
        {
            var group = panel.UIRoot.GetComponent<CanvasGroup>();
            if (group == null)
                group = panel.UIRoot.AddComponent<CanvasGroup>();

            // show
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        HideSettingsPanel(null);
    }

    [HarmonyPatch(typeof(OptionsMenu), nameof(OptionsMenu.SwitchView))]
    [HarmonyPostfix]
    private static void OptionsMenuSwitchView(OptionsMenu __instance, ProjectM.UI.OptionsMenu.ViewState state, bool initialSwitch)
    {
        Transform t = __instance.GeneralPanel.gameObject.transform.parent;
        RectTransform tt = t.GetComponent<RectTransform>();
        FullscreenSettingService.DeltaRect = new Vector2(tt.rect.width, tt.rect.height);
        FullscreenSettingService.AnchorRect = new Vector2(tt.anchoredPosition.x, tt.anchoredPosition.y);

        if (!Plugin.UIManager.IsInitialized)
        {
            //start the manager since we got to the settings before entering a server
            LogUtils.LogInfo("Creating UI...");
            Plugin.UIOnInitialize();
        }

        SimpleStunButton simpleButton = __instance.SoundTabButton;

        //Check if button exist if it exist quit
        //if not clone and add our own
        Transform btnSearch = simpleButton.transform.parent.Find("EZLifeButton");
        if (btnSearch == null)
        {
            var settingsButton = UnityEngine.Object.Instantiate(simpleButton.gameObject, simpleButton.transform.parent);
            settingsButton.name = "EZLifeButton";

            var newSettingsButton = settingsButton.GetComponent<SimpleStunButton>();

            var label = settingsButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
                label.text = "EZLife";
            newSettingsButton.m_OnClick.RemoveAllListeners();

            __instance.GeneralTabButton.m_OnClick.AddListener((UnityAction)(() => HideSettingsPanel(newSettingsButton)));

            __instance.ControlsTabButton.m_OnClick.AddListener((UnityAction)(() => HideSettingsPanel(newSettingsButton)));
            __instance.GraphicsTabButton.m_OnClick.AddListener((UnityAction)(() => HideSettingsPanel(newSettingsButton)));
            __instance.SoundTabButton.m_OnClick.AddListener((UnityAction)(() => HideSettingsPanel(newSettingsButton)));
            SimpleStunButton smp = __instance.GeneralTabButton;
            newSettingsButton.m_OnClick.AddListener(() =>
            {
                smp.ForceHighlight = false;
                newSettingsButton.ForceHighlight = true;
                FullscreenSettingService.RaiseHeaderButtonClicked(__instance);
            });
            
            //get the button templates
            //FullscreenSettingService.ClonePanelAndGetTemplates(__instance.GeneralPanel);
            FullscreenSettingService.ClonePanelAndGetTemplates(__instance.ControlsPanel);
            
        }
        

    }

    [HarmonyPatch(typeof(ProjectM.UI.RebindingMenu), nameof(ProjectM.UI.RebindingMenu.Awake))]
    [HarmonyPostfix]
    private static void RebindingMenuAwake(RebindingMenu __instance)
    {
        LogUtils.LogError("Starting here");
        UnityHelper.PrintParents(__instance.transform);
        FullscreenSettingService._templates.Binding = __instance.ControlsInputEntryPrefab;
        

    }
    private static void HideSettingsPanel(SimpleStunButton btn)
    {
        if (btn != null)
            btn.ForceHighlight = false;
        var panel = Plugin.UIManager.GetPanel<SettingsPanel>();
        if (panel != null)
        {
            panel.SetActive(false);
            return;
            
        }
    }


}
