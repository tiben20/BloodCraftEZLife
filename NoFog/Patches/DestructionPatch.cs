using HarmonyLib;
using NoFog.NEW;
using ProjectM;

namespace NoFog.Patches
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
