using HarmonyLib;
using ProjectM.UI;

namespace BloodCraftUI.NewUI.Patches;

public class EscapeMenuPatch
{
    [HarmonyPatch(typeof (EscapeMenuView), "OnDestroy")]
    [HarmonyPrefix]
    private static void EscapeMenuViewOnDestroyPrefix()
    {
        if (!BCUIManager.IsInitialised) return;
        
        // User has left the server. Reset all ui as the next server might be a different one
        BCUIManager.Reset();
    }
}