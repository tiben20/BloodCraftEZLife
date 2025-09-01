using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using ProjectM;
using ProjectM.Gameplay.Scripting;
using ProjectM.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.DebugDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using static BloodCraftEZLife.Config.Settings;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class ChatPanel : ResizeablePanelBase
    {
        public override string PanelId => "ChatPanel";
        public override int MinWidth => 180;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.ChatPanel;
        public override float Opacity => Settings.UITransparency;

        private ScrollPool<ButtonCell> _scrollPoolUsers;
        private ButtonListHandler<string, ButtonCell> __scrollDataUsersHandler;

        private ScrollPool<ButtonCell> _scrollPoolMessages;
        private MessageListHandler<ChatMessage, ButtonCell> _scrollDataMessagesHandler;
        private bool _isInitialized;

        private InputFieldRef InputBox;

        public ChatPanel(UIBase owner) : base(owner)
        {
            SetTitle("Chat messages");
        }

        

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {

            // Title bar
            GameObject userPanel = UIFactory.CreateHorizontalGroup(ContentRoot, "TitleBar", false, true, true, true, 2,
                new Vector4(2, 2, 2, 2), Theme.PanelBackground);
            UIFactory.SetLayoutElement(userPanel, minHeight: 150, flexibleHeight: 0);

            __scrollDataUsersHandler = new ButtonListHandler<string, ButtonCell>(_scrollPoolUsers, ConfigSaveManager.GetUsers, SetCell, ShouldDisplay, OnUserClicked);
            _scrollPoolUsers = UIFactory.CreateScrollPool<ButtonCell>(userPanel, "UserList", out GameObject scrollObj,out _);
            _scrollPoolUsers.Initialize(__scrollDataUsersHandler);
            

            _scrollDataMessagesHandler = new MessageListHandler<ChatMessage, ButtonCell>(_scrollPoolMessages, ConfigSaveManager.GetMessages, SetChatCell, ShouldDisplayMessage);
            _scrollPoolMessages = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "MessageList", out GameObject scrollMessageObj,out _);
            _scrollPoolMessages.Initialize(_scrollDataMessagesHandler);
            UIFactory.SetLayoutElement(scrollMessageObj, flexibleHeight: 9999);


            InputBox = UIFactory.CreateInputField(ContentRoot, "inputHolder", "");

            UIFactory.SetLayoutElement(InputBox.GameObject, minHeight: 40, flexibleHeight: 0);
            
            InputBox.PlaceholderText.text = "Whisper here";

            VerticalLayoutGroup contentGroup = ContentRoot.GetComponent<VerticalLayoutGroup>();
            contentGroup.spacing = 2f;
            contentGroup.padding.top = 2;
            contentGroup.padding.bottom = 2;
            contentGroup.padding.left = 0;
            contentGroup.padding.right = 2;

            //ProjectM.ButtonInputAction.ClanMenu
            //ClanMenu
            InputBox.Component.onSelect.AddListener(OnFocus);
            InputBox.Component.onSubmit.AddListener(OnSubmit);

            RefreshData();
        }

        void OnSubmit(string text)
        {

        }

        void OnFocus(string text)
        {
           
        }

        internal override void Reset()
        {   
            __scrollDataUsersHandler.RefreshData();
            _scrollPoolUsers.Refresh(true);
        }

        public void RefreshData()
        {
            __scrollDataUsersHandler.RefreshData();
            _scrollPoolUsers.Refresh(true);
        }

        public void RefreshMessage(string user)
        {
            _scrollDataMessagesHandler.RefreshData(user);
            _scrollPoolMessages.Refresh(true);
        }

        public override void SetActive(bool active)
        {
            var shouldUpdateData = _isInitialized && active && Enabled == false;
            _isInitialized = true;
            base.SetActive(active);
        }

        #region ScrollPool handling



        private void OnUserClicked(int dataIndex)
        {

            string user = ConfigSaveManager.ChatMessages.GetUsers[dataIndex];
            _scrollDataMessagesHandler.RefreshData(user);
            _scrollPoolMessages.Refresh(true);
            _scrollPoolUsers.Refresh(true);
        }

        private bool ShouldDisplay(string data, string filter) => true;
        private bool ShouldDisplayMessage(ChatMessage data, string filter) => true;
        

        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= ConfigSaveManager.ChatMessages.GetUsers.Count)
            {
                cell.Disable();
                return;
            }
            
            cell.Button.ButtonText.text = ConfigSaveManager.ChatMessages.GetUsers[index];
            if (ConfigSaveManager.ChatMessages.GetUsers[index] == ConfigSaveManager.ChatMessages.CurrentUser)
            {
                
                cell.DoSelect(true);
                InputBox.PlaceholderText.text = "Whisper to " + cell.Button.ButtonText.text;


            }
            else
                cell.DoSelect(false);
                cell.IconImage.gameObject.SetActive(false);
        }

        private void SetChatCell(ButtonCell cell, int index)
        {
            
            if (index < 0 || index >= ConfigSaveManager.ChatMessages.GetMessages().Count)
            {
                cell.Disable();
                return;
            }
            ChatMessage chtmsg = ConfigSaveManager.ChatMessages.GetMessages()[index];
            string msg = chtmsg.From ? "From: " : "To: ";
            msg = msg + chtmsg.TextMessage;
            cell.Button.ButtonText.text = msg;
            cell.IconImage.gameObject.SetActive(false);
        }

        #endregion
    }
}
