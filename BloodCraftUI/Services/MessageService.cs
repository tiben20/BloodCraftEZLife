using System.Collections.Generic;
using System.Linq;
using BloodCraftUI.Utils;
using Il2CppInterop.Runtime;
using ProjectM.Network;
using Unity.Entities;

namespace BloodCraftUI.Services
{
    internal static class MessageService
    {
        static EntityManager EntityManager => Plugin.EntityManager;
        static readonly ComponentType[] _networkEventComponents =
        [
            ComponentType.ReadOnly(Il2CppType.Of<FromCharacter>()),
            ComponentType.ReadOnly(Il2CppType.Of<NetworkEventType>()),
            ComponentType.ReadOnly(Il2CppType.Of<SendNetworkEventTag>()),
            ComponentType.ReadOnly(Il2CppType.Of<ChatMessageEvent>())
        ];

        private static readonly Queue<string> OutputMessages  = new();
        private static Entity _localCharacter = Entity.Null;
        private static Entity _localUser = Entity.Null;
        private static bool _isInitialized;

        static readonly NetworkEventType _networkEventType = new()
        {
            IsAdminEvent = false,
            EventId = NetworkEvents.EventId_ChatMessageEvent,
            IsDebugEvent = false,
        };

        public static void QueueMessage(string text)
        {
            OutputMessages.Enqueue(text);
        }

        public static string DequeueMessage()
        {
            return OutputMessages.Any() ? OutputMessages.Dequeue() : null;
        }

        public static void SendMessage(string text)
        {
            ChatMessageEvent chatMessageEvent = new()
            {
                MessageText = text,
                MessageType = ChatMessageType.Local,
                ReceiverEntity = _localUser.Read<NetworkId>()
            };

            var networkEntity = EntityManager.CreateEntity(_networkEventComponents);
            networkEntity.Write(new FromCharacter { Character = _localCharacter, User = _localUser });
            networkEntity.Write(_networkEventType);
            networkEntity.Write(chatMessageEvent);
        }

        public static void ProcessAllMessages()
        {
            if(!_isInitialized) return;

            var msg = DequeueMessage();
            while (!string.IsNullOrEmpty(msg))
            {
                SendMessage(msg);
                msg = DequeueMessage();
            }
        }

        public static void Destroy()
        {
            _localCharacter = Entity.Null;
            _localUser = Entity.Null;
            OutputMessages.Clear();
            _isInitialized = false;
        }

        public static void SetCharacter(Entity entity)
        {
            _localCharacter = entity;
            if (_localCharacter != Entity.Null && _localUser != Entity.Null)
                _isInitialized = true;
        }

        public static void SetUser(Entity entity)
        {
            _localUser = entity;
            if (_localCharacter != Entity.Null && _localUser != Entity.Null)
                _isInitialized = true;
        }
    }
}
