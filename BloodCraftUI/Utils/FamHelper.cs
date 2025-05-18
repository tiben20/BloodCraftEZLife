using ProjectM;
using Unity.Entities;

namespace BloodCraftUI.Utils
{
    internal static class FamHelper
    {
        public static Entity FindActiveFamiliar(Entity playerCharacter)
        {
            if (playerCharacter.TryGetBuffer<FollowerBuffer>(out var followers) && !followers.IsEmpty)
            {
                foreach (FollowerBuffer follower in followers)
                {
                    Entity familiar = follower.Entity._Entity;
                    if (familiar.Has<BlockFeedBuff>()) return familiar;
                }
            }

            return Entity.Null;
        }
    }
}
