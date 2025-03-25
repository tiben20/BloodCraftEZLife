using BloodCraftUI.NewUI.UICore.UniverseLib.UI;
using HarmonyLib;
using TMPro;

namespace BloodCraftUI.NewUI.Patches;

public class VersionStringPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VersionString), nameof(VersionString.Start))]
    public static void VersionString_Start(VersionString __instance)
    {
        var tmp = __instance.gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
        
        if (tmp != null)
        {
            UIFactory.Font = tmp.font;
            UIFactory.FontMaterial = tmp.fontMaterial;
            Plugin._harmonyVersionStringPatch.UnpatchSelf();
        }
    }
}