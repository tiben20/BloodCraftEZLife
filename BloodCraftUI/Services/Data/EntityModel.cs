using ProjectM;
using Stunlock.Core;
using Unity.Entities;


namespace BloodCraftEZLife.Services.Data
{
    public abstract class EntityModel
    {
        protected EntityModel(Entity entity)
        {
            Entity = entity;
            Internals = new BaseEntityModel(Plugin._client, entity);
        }

        public Entity Entity { get; }

        public PrefabGUID PrefabGUID => Internals.PrefabGUID ?? new PrefabGUID();

        public BaseEntityModel Internals { get; }
    }
}