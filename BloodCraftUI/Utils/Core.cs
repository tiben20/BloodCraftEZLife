using ProjectM.Scripting;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectM.UI;
using UnityEngine.InputSystem;

namespace BloodCraftEZLife.Utils
{
    public class Core
    {
        public Core() { }

        private static Core _instance;
        internal static Core Instance => _instance ??= new Core();
        public PrefabCollectionSystem PrefabCollectionSystem => Plugin._client.GetExistingSystemManaged<PrefabCollectionSystem>();
        public GameDataSystem GameDataSystem => Plugin._client.GetExistingSystemManaged<GameDataSystem>();
        public ManagedDataRegistry ManagedDataRegistry => GameDataSystem.ManagedDataRegistry;
        public DebugEventsSystem DebugEventsSystem => Plugin._client.GetExistingSystemManaged<DebugEventsSystem>();
        public UnitSpawnerUpdateSystem UnitSpawnerUpdateSystem => Plugin._client.GetExistingSystemManaged<UnitSpawnerUpdateSystem>();
        public ClientScriptMapper ClientScriptMapper => Plugin._client.GetExistingSystemManaged<ClientScriptMapper>();
    }
}
