using ProjectM;
using Unity.Entities;
using BloodCraftEZLife.Utils;
using Stunlock.Core;

namespace BloodCraftEZLife.Services.Data
{
    public class ItemModelNoEntity
    {
        public ItemModelNoEntity()
        {
        }
        public PrefabGUID PrefabGuid { get; set; }
        public string Name { get; set; }

        public ItemCategory ItemCategory { get; set; }
        public ItemType ItemType { get; set; }
        public EquipmentType EquipmentType { get; set; }
    }

    public partial class ItemModel : EntityModel
    {
        internal ItemModel(Entity entity) : base(entity)
        {
            PrefabName = Internals.PrefabGUID?.GetPrefabName();
            ManagedCore = new BaseManagedDataModel(Plugin._client, Internals);
        }

        public BaseManagedDataModel ManagedCore { get; }

        public string PrefabName { get; }
        public string Name => ManagedCore?.ManagedItemData?.Name.ToString();

        public ItemCategory ItemCategory => Internals.ItemData?.ItemCategory ?? ItemCategory.NONE;
        public ItemType ItemType => Internals.ItemData?.ItemType ?? 0;
        public EquipmentType EquipmentType => Internals.EquippableData?.EquipmentType ?? EquipmentType.None;
    }
}