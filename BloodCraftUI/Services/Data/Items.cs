using BloodCraftEZLife.Utils;
using Bloody.Core;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace BloodCraftEZLife.Services.Data
{
    public class Items
    {
        private Items() { }

        private static Items _instance;
        internal static Items Instance => _instance ??= new Items();

        public ItemModel GetPrefabById(PrefabGUID prefabGuid)
        {
            try
            {
                var entity = Plugin.Core.PrefabCollectionSystem._PrefabLookupMap[prefabGuid];
                return FromEntity(entity);
            }
            catch (Exception ex)
            {
                LogUtils.LogError(ex.Message);
            }

            return null;
        }

        public ItemModel FromEntity(Entity entity)
        {
            try
            {
                if (entity.Has<ItemData>())
                {
                    return new ItemModel(entity);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogError(ex.Message);
            }

            return null;
        }

        private List<ItemModel> _itemPrefabs;
        private List<ItemModelNoEntity> _itemStackablePrefabs;
        public List<ItemModel> Prefabs => _itemPrefabs ??= GetItemPrefabs();
        public List<ItemModelNoEntity> ItemPrefabs => _itemStackablePrefabs ??= GetStackablePrefabs();

        public static void ListEntityComponents(Entity entity)
        {
            if (!Plugin.EntityManager.Exists(entity))
            {
                return;
            }

            // Get all component types on this entity
            var componentTypes = Plugin.EntityManager.GetComponentTypes(entity);

            LogUtils.LogInfo($"Entity {entity.Index} has {componentTypes.Length} components:");

            foreach (var type in componentTypes)
            {
                LogUtils.LogInfo($" - {type}");
            }

            // Dispose because GetComponentTypes returns a NativeArray
            componentTypes.Dispose();
        }

        private List<ItemModelNoEntity> GetStackablePrefabs()
        {
            
            var result = new List<ItemModelNoEntity>();
            var gameData = Plugin._client.GetExistingSystemManaged<GameDataSystem>();
            var map = gameData.ItemHashLookupMap; // NativeParallelHashMap<PrefabGUID, ItemData>
            var keys = map.GetKeyArray(Allocator.Temp);
            try
            {
                foreach (var key in keys)
                {
                    ItemData value = default;
                    if (map.TryGetValue(key, out value))
                    {

                        if (value.ItemType == ItemType.Stackable)
                        {

                            LogUtils.LogInfo(key.JsonPrefabName());
                            ItemModelNoEntity itemModelNoEnt = new ItemModelNoEntity();
                            itemModelNoEnt.ItemCategory = value.ItemCategory;
                            itemModelNoEnt.ItemType = value.ItemType;
                            itemModelNoEnt.Name = key.JsonPrefabName();
                            itemModelNoEnt.PrefabGuid = key;
                            var manageddata = Plugin.Core.GameDataSystem.ManagedDataRegistry.GetOrDefault<ManagedItemData>(key);
                            if (manageddata != null)
                            {
                                itemModelNoEnt.Icon = manageddata?.Icon;
                            }
                            result.Add(itemModelNoEnt);
                        }
                    }
                }
            }
            finally
            {
                keys.Dispose();
            }
            return result;
        }

        private List<ItemModel> GetItemPrefabs()
        {
            var result = new List<ItemModel>();
            var gameData = Plugin._client.GetExistingSystemManaged<GameDataSystem>();

            foreach (var prefabEntity in gameData.ItemHashLookupMap.m_HashMapData.GetValueArray(Allocator.Temp))
            {
                var itemModel = FromEntity(prefabEntity.Entity);
                if (itemModel != null)
                {
                    result.Add(itemModel);
                }
            }
            
            return result;
        }

        private List<ItemModel> _weapons;
        public List<ItemModel> Weapons => _weapons ??= Prefabs.Where(itemModel => itemModel.EquipmentType == EquipmentType.Weapon).ToList();


        private List<ItemModelNoEntity> _alchemys;
        public List<ItemModelNoEntity> Alchemys => _alchemys ??= ItemPrefabs.Where(itemModel => itemModel.ItemCategory == ItemCategory.Alchemy).ToList();
        
        public List<ItemModelNoEntity> Stackables(ItemCategory cat) => ItemPrefabs.Where(itemModel => itemModel.ItemCategory == cat || cat == ItemCategory.ALL).ToList();
        



    }
}
