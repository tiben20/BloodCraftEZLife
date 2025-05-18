using System;
using BloodCraftUI.Behaviors;
using BloodCraftUI.Utils;
using ProjectM;
using Unity.Entities;

namespace BloodCraftUI.Services
{
    internal static class BloodCraftStateService
    {
        private static Entity _familiar;

        //// FLAG PROPERTIES
        public static bool IsFamUnbound { get; private set; }
        public static bool IsFamBound => !IsFamUnbound;
        public static string CurrentFamName { get; private set; }
        public static float CurrentFamiliarHP { get; private set; }


        public static void Initialize()
        {
            CoreUpdateBehavior.Actions.Add(OnUpdate);
        }

        private static void OnUpdate()
        {
            return;
            if(Plugin.IsClientNull() || Plugin.LocalCharacter == Entity.Null)
                return;

            if(_familiar == Entity.Null)
                _familiar = FamHelper.FindActiveFamiliar(Plugin.LocalCharacter);

            if(_familiar == Entity.Null)
            {
                IsFamUnbound = true;
                CurrentFamName = string.Empty;
            }
            else
            {
                var hp = _familiar.Read<Health>();
                if (hp.IsDead)
                {
                    _familiar = Entity.Null;
                    return;
                }
                IsFamUnbound = false;
                if(CurrentFamiliarHP != hp.Value)
                    LogUtils.LogWarning($" FAMILIAR HP DEBUG: {hp.Value}");
                CurrentFamiliarHP = hp.Value;
            }
        }
    }
}
