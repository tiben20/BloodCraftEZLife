using BloodCraftUI.NewUI;
using BloodCraftUI.Services;
using HarmonyLib;
using ProjectM.UI;

namespace BloodCraftUI.Patches;

public class EscapeMenuPatch
{
    [HarmonyPatch(typeof(EscapeMenuView), "OnDestroy")]
    [HarmonyPrefix]
    private static void EscapeMenuViewOnDestroyPrefix()
    {
        if (!BCUIManager.IsInitialized) return;

        // User has left the server. Reset all ui as the next server might be a different one
        BCUIManager.Reset();
        MessageService.Destroy();
    }
}