using BloodCraftUI.NEW;
using HarmonyLib;
using ProjectM;

namespace BloodCraftUI.Patches
{
    [HarmonyPatch]
    internal static class DestructionPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnDestroy))]
        private static void OnDestroyPrefix(ClientBootstrapSystem __instance)
        {
            UICustomManager.Destroy();
           // Plugin.Reset();
        }
    }
}
