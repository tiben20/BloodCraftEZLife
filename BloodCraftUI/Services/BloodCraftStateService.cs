using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using BloodCraftEZLife.Behaviors;
using BloodCraftEZLife.UI;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.Utils;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;

namespace BloodCraftEZLife.Services
{
    internal static class BloodCraftStateService
    {
        private static Entity _familiar;

        //// FLAG PROPERTIES
        public static bool IsFamUnbound { get; private set; }
        public static bool IsFamBound => !IsFamUnbound;

        public static void Initialize()
        {
            CoreUpdateBehavior.Actions.Add(OnUpdate);
        }

        private static volatile int _skipCounter;

        private static void OnUpdate()
        {
            if(Plugin.IsClientNull() || Plugin.LocalCharacter == Entity.Null)
                return;

            if(_skipCounter < 3)
            {
                Interlocked.Increment(ref _skipCounter);
                return;
            }
            _skipCounter = 0;
        }
    }
}
