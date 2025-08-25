using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.Services.Data;
using Bloody.Core;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using ProjectM.Scripting;
using ProjectM.Shared;
using ProjectM.UI;
using Stunlock.Core;
using Stunlock.Localization;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BloodCraftEZLife.Utils;
internal static class Extensions
{
    static EntityManager EntityManager => Plugin.EntityManager;
    //static ServerGameManager ServerGameManager => ;

    // static ClientGameManager ClientGameManager => Core.ClientGameManager;
    //static SystemService SystemService => Core.SystemService;
    // static PrefabCollectionSystem PrefabCollectionSystem => SystemService.PrefabCollectionSystem;

    const string EMPTY_KEY = "LocalizationKey.Empty";

    public static string GetText(this SimpleStunButton helper)
    {
        var label = helper.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label != null)
            return label.text;
        return "";
    }

    public static void SetText(this SimpleStunButton helper, string text)
    {
        var label = helper.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label != null)
            label.text = text;
    }

    public static bool IsEnabled(this TextMeshProUGUI entity)
    {
        return entity?.gameObject?.activeSelf ?? false;
    }

    public static Color GetTransparent(this Color baseColor, float alpha = 0.7f)
    {
        return new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }

    public delegate void WithRefHandler<T>(ref T item);
    public static void With<T>(this Entity entity, WithRefHandler<T> action) where T : struct
    {
        T item = entity.ReadRW<T>();
        action(ref item);

        EntityManager.SetComponentData(entity, item);
    }
    public static void AddWith<T>(this Entity entity, WithRefHandler<T> action) where T : struct
    {
        if (!entity.Has<T>())
        {
            entity.Add<T>();
        }

        entity.With(action);
    }
    public static unsafe void Write<T>(this Entity entity, T componentData) where T : struct
    {
        ComponentType componentType = new(Il2CppType.Of<T>());
        TypeIndex typeIndex = componentType.TypeIndex;

        byte[] byteArray = StructureToByteArray(componentData);
        int size = Marshal.SizeOf<T>();

        fixed (byte* byteData = byteArray)
        {
            EntityManager.SetComponentDataRaw(entity, typeIndex, byteData, size);
        }
    }
    static byte[] StructureToByteArray<T>(T structure) where T : struct
    {
        int size = Marshal.SizeOf(structure);
        byte[] byteArray = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(structure, ptr, true);

        Marshal.Copy(ptr, byteArray, 0, size);
        Marshal.FreeHGlobal(ptr);

        return byteArray;
    }
    unsafe static T ReadRW<T>(this Entity entity) where T : struct
    {
        ComponentType componentType = new(Il2CppType.Of<T>());
        TypeIndex typeIndex = componentType.TypeIndex;

        void* componentData = EntityManager.GetComponentDataRawRW(entity, typeIndex);
        return Marshal.PtrToStructure<T>(new IntPtr(componentData));
    }
    public unsafe static T Read<T>(this Entity entity) where T : struct
    {
        ComponentType componentType = new(Il2CppType.Of<T>());
        TypeIndex typeIndex = componentType.TypeIndex;

        void* componentData = EntityManager.GetComponentDataRawRO(entity, typeIndex);
        return Marshal.PtrToStructure<T>(new IntPtr(componentData));
    }
    public static DynamicBuffer<T> ReadBuffer<T>(this Entity entity) where T : struct
    {
        return EntityManager.GetBuffer<T>(entity);
    }
    public static DynamicBuffer<T> AddBuffer<T>(this Entity entity) where T : struct
    {
        return EntityManager.AddBuffer<T>(entity);
    }
    public unsafe static void* GetComponentData(this Entity entity, TypeIndex typeIndex)
    {
        return EntityManager.GetComponentDataRawRO(entity, typeIndex);
    }
    public unsafe static void SetComponentData(this Entity entity, TypeIndex typeIndex, void* byteData, int size)
    {
        EntityManager.SetComponentDataRaw(entity, typeIndex, byteData, size);
    }
    public unsafe static void* GetBufferData(this Entity entity, TypeIndex typeIndex)
    {
        return EntityManager.GetBufferRawRO(entity, typeIndex);
    }
    public static int GetBufferLength(this Entity entity, TypeIndex typeIndex)
    {
        return EntityManager.GetBufferLength(entity, typeIndex);
    }
    public static void SetBufferData<T>(Entity prefabSource, T[] bufferArray) where T : struct
    {
        DynamicBuffer<T> buffer = prefabSource.Has<T>() ? prefabSource.ReadBuffer<T>() : prefabSource.AddBuffer<T>();
        buffer.Clear();

        foreach (T element in bufferArray)
        {
            buffer.Add(element);
        }
    }
    public static bool TryGetComponent<T>(this Entity entity, out T componentData) where T : struct
    {
        componentData = default;

        if (entity.Has<T>())
        {
            componentData = entity.Read<T>();

            return true;
        }

        return false;
    }
    public static bool TryGetComponentObject<T>(this Entity entity, EntityManager entityManager, out T componentObject) where T : class
    {
        componentObject = default;

        if (entityManager.HasComponent<T>(entity))
        {
            componentObject = entityManager.GetComponentObject<T>(entity);
            return componentObject != null;
        }

        return false;
    }
    public static bool TryRemoveComponent<T>(this Entity entity) where T : struct
    {
        if (entity.Has<T>())
        {
            entity.Remove<T>();

            return true;
        }

        return false;
    }

    public static bool HasComponentInternal<T>(this EntityManager entityManager, Entity entity)
    {
        try
        {
            return entityManager.HasComponent<T>(entity);
        }
        catch
        {
            return false;
        }
    }

    public static bool TryGetComponentDataInternal<T>(this EntityManager entityManager, Entity entity, out T value)
    where T : new()
    {
        try
        {
            value = entityManager.GetComponentData<T>(entity);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static List<T> ToList<T>(this DynamicBuffer<T> bufferList) where T : new()
    {
        if (bufferList.Length == 0)
        {
            return null;
        }

        var result = new List<T>();
        foreach (var item in bufferList)
        {
            result.Add(item);
        }

        return result;
    }

    public static List<T> GetBufferInternal<T>(this EntityManager entityManager, Entity entity) where T : struct
    {
        try
        {
            return entityManager.GetBuffer<T>(entity).ToList();
        }
        catch (Exception ex)
        {
            //LogUtils.LogDebug(ex.Message);
            return null;
        }
    }

    public static T GetManagedComponentDataInternal<T>(this World world, BaseEntityModel entity)
    where T : class
    {
        var prefabGuid = entity.PrefabGUID;
        if (prefabGuid == null)
        {
            return null;
            ;
        }

        var managedDataRegistry = world.GetExistingSystemManaged<GameDataSystem>().ManagedDataRegistry;
        return managedDataRegistry.GetOrDefault<T>(prefabGuid.Value);
    }

    public static bool Has<T>(this Entity entity)
    {
        return EntityManager.HasComponent(entity, new(Il2CppType.Of<T>()));
    }
    public static bool Has(this Entity entity, ComponentType componentType)
    {
        return EntityManager.HasComponent(entity, componentType);
    }

    public static void Add<T>(this Entity entity)
    {
        EntityManager.AddComponent(entity, new(Il2CppType.Of<T>()));
    }
    public static void Add(this Entity entity, ComponentType componentType)
    {
        EntityManager.AddComponent(entity, componentType);
    }
    public static void Remove<T>(this Entity entity)
    {
        EntityManager.RemoveComponent(entity, new(Il2CppType.Of<T>()));
    }
    public static bool TryGetFollowedPlayer(this Entity entity, out Entity player)
    {
        player = Entity.Null;

        if (entity.TryGetComponent(out Follower follower))
        {
            if (follower.Followed._Value.TryGetPlayer(out player))
            {
                return true;
            }
        }

        return false;
    }
    public static bool TryGetPlayer(this Entity entity, out Entity player)
    {
        player = Entity.Null;

        if (entity.Has<PlayerCharacter>())
        {
            player = entity;

            return true;
        }

        return false;
    }
    public static bool IsPlayer(this Entity entity)
    {
        if (entity.Has<VampireTag>())
        {
            return true;
        }

        return false;
    }
    public static bool IsDifferentPlayer(this Entity entity, Entity target)
    {
        if (entity.IsPlayer() && target.IsPlayer() && !entity.Equals(target))
        {
            return true;
        }

        return false;
    }
    public static bool IsFollowingPlayer(this Entity entity)
    {
        if (entity.TryGetComponent(out Follower follower))
        {
            if (follower.Followed._Value.IsPlayer())
            {
                return true;
            }
        }

        return false;
    }

    public static string GetLocalizedName(this PrefabGUID prefabGuid)
    {
        if (PrefabNames.LocalizedNameKeys.TryGetValue(prefabGuid, out var guid))
        {
            if (Localization.LocalizedStrings.TryGetValue(AssetGuid.FromString(guid), out var localizedName))
            {
                if (!string.IsNullOrEmpty(localizedName))
                    return localizedName;
            }
        }

        return EMPTY_KEY;
    }


    public static int GetUnitLevel(this Entity entity)
    {
        if (entity.TryGetComponent(out UnitLevel unitLevel))
        {
            return unitLevel.Level._Value;
        }

        return 0;
    }

    public static Entity GetBuffTarget(this Entity entity)
    {
        return CreateGameplayEventServerUtility.GetBuffTarget(EntityManager, entity);
    }

    public static Entity GetSpellTarget(this Entity entity)
    {
        return CreateGameplayEventServerUtility.GetSpellTarget(EntityManager, entity);
    }
    public static bool TryGetTeamEntity(this Entity entity, out Entity teamEntity)
    {
        teamEntity = Entity.Null;

        if (entity.TryGetComponent(out TeamReference teamReference))
        {
            Entity teamReferenceEntity = teamReference.Value._Value;

            if (teamReferenceEntity.Exists())
            {
                teamEntity = teamReferenceEntity;

                return true;
            }
        }

        return false;
    }
    public static bool Exists(this Entity entity)
    {
        return entity.HasValue() && entity.IndexWithinCapacity() && EntityManager.Exists(entity);
    }
    public static bool IsDisabled(this Entity entity)
    {
        return entity.Has<Disabled>();
    }
    public static bool IsVBlood(this Entity entity)
    {
        return entity.Has<VBloodUnit>();
    }
    public static ulong GetSteamId(this Entity entity)
    {
        if (entity.TryGetComponent(out PlayerCharacter playerCharacter))
        {
            return playerCharacter.UserEntity.Read<User>().PlatformId;
        }
        else if (entity.TryGetComponent(out User user))
        {
            return user.PlatformId;
        }

        return 0;
    }
    public static NetworkId GetNetworkId(this Entity entity)
    {
        if (entity.TryGetComponent(out NetworkId networkId))
        {
            return networkId;
        }

        return NetworkId.Empty;
    }
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
    public static PrefabGUID GetPrefabGUID(this Entity entity)
    {
        if (entity.TryGetComponent(out PrefabGUID prefabGUID)) return prefabGUID;

        return PrefabGUID.Empty;
    }
    public static Entity GetUserEntity(this Entity character)
    {
        if (character.TryGetComponent(out PlayerCharacter playerCharacter)) return playerCharacter.UserEntity;

        return Entity.Null;
    }
    public static User GetUser(this Entity entity)
    {
        if (entity.TryGetComponent(out PlayerCharacter playerCharacter) && playerCharacter.UserEntity.TryGetComponent(out User user)) return user;
        else if (entity.TryGetComponent(out user)) return user;

        return User.Empty;
    }

    public static float3 GetAimPosition(this Entity entity)
    {
        if (entity.TryGetComponent(out EntityInput entityInput))
        {
            return entityInput.AimPosition;
        }

        return float3.zero;
    }
    public static bool TryGetPosition(this Entity entity, out float3 position)
    {
        position = float3.zero;

        if (entity.TryGetComponent(out Translation translation))
        {
            position = translation.Value;

            return true;
        }

        return false;
    }
    public static float3 GetPosition(this Entity entity)
    {
        if (entity.TryGetComponent(out Translation translation))
        {
            return translation.Value;
        }

        return float3.zero;
    }
    public static bool TryGetMatch(this HashSet<(ulong, ulong)> hashSet, ulong value, out (ulong, ulong) matchingPair)
    {
        matchingPair = default;

        foreach (var pair in hashSet)
        {
            if (pair.Item1 == value || pair.Item2 == value)
            {
                matchingPair = pair;

                return true;
            }
        }

        return false;
    }
    public static bool IsCustomSpawned(this Entity entity)
    {
        if (entity.TryGetComponent(out IsMinion isMinion) && isMinion.Value)
        {
            return true;
        }

        return false;
    }
    public static void Destroy(this Entity entity)
    {
        if (entity.Exists()) DestroyUtility.Destroy(EntityManager, entity);
    }
    public static void SetTeam(this Entity entity, Entity teamSource)
    {
        if (entity.Has<Team>() && entity.Has<TeamReference>() && teamSource.TryGetComponent(out Team sourceTeam) && teamSource.TryGetComponent(out TeamReference sourceTeamReference))
        {
            Entity teamRefEntity = sourceTeamReference.Value._Value;
            int teamId = sourceTeam.Value;

            entity.With((ref TeamReference teamReference) =>
            {
                teamReference.Value._Value = teamRefEntity;
            });

            entity.With((ref Team team) =>
            {
                team.Value = teamId;
            });
        }
    }
    public static void SetFaction(this Entity entity, PrefabGUID factionPrefabGUID)
    {
        if (entity.Has<FactionReference>())
        {
            entity.With((ref FactionReference factionReference) =>
            {
                factionReference.FactionGuid._Value = factionPrefabGUID;
            });
        }
    }

    public static bool HasKeyValue<T>(this Dictionary<T, int> dic, T key, int value)
    {
        return dic.ContainsKey(key) && dic[key] == value;
    }

    public static void SetValue<T>(this Dictionary<T, int> dic, T key, int value)
    {
        dic[key] = value;
    }

    public static unsafe bool TryGetBuffer<T>(this Entity entity, out DynamicBuffer<T> dynamicBuffer) where T : struct
    {
        if (Plugin.EntityManager.TryGetBuffer(entity, out dynamicBuffer))
        {
            return true;
        }

        dynamicBuffer = default;
        return false;
    }

    public static bool HasValue(this Entity entity)
    {
        return entity != Entity.Null;
    }

    const string PREFIX = "Entity(";
    const int LENGTH = 7;
    public static bool IndexWithinCapacity(this Entity entity)
    {
        string entityStr = entity.ToString();
        ReadOnlySpan<char> span = entityStr.AsSpan();

        if (!span.StartsWith(PREFIX)) return false;
        span = span[LENGTH..];

        int colon = span.IndexOf(':');
        if (colon <= 0) return false;

        ReadOnlySpan<char> tail = span[(colon + 1)..];

        int closeRel = tail.IndexOf(')');
        if (closeRel <= 0) return false;

        // Parse numbers
        if (!int.TryParse(span[..colon], out int index)) return false;
        if (!int.TryParse(tail[..closeRel], out _)) return false;

        // Single unsigned capacity check
        int capacity = EntityManager.EntityCapacity;
        bool isValid = (uint)index < (uint)capacity;

        if (!isValid)
        {
            // Core.Log.LogWarning($"Entity index out of range! ({index}>{capacity})");
        }

        return isValid;
    }

    
 }