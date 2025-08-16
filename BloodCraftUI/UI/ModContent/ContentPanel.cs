using System.Collections.Generic;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib.Controls;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.Utils;
using ProjectM;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static BloodCraftEZLife.Services.MessageService;
using UIBase = BloodCraftEZLife.UI.UniverseLib.UI.UIBase;

namespace BloodCraftEZLife.UI.ModContent
{
    public class ContentPanel : ResizeablePanelBase
    {
        public override string PanelId => "CorePanel";

        public override int MinWidth => Settings.UseHorizontalContentLayout ? 340 : 100;
        //public override int MaxWidth => 150;

        public override int MinHeight => 25;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        
        public override Vector2 DefaultPosition => new Vector2(0f, Owner.Scaler.m_ReferenceResolution.y);
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.Base;

        private GameObject _uiAnchor;
        private UIScaleSettingButton _scaleButtonData;
        private List<GameObject> _objectsList;
        private UniverseLib.UI.Models.LabelRef _anchorLabel;
        private UnityEngine.UI.Toggle _pinToggle;

        private float timer = 0f;
        public float interval = 5f; // seconds
        private List<TeleportRequest> _teleportRequest = new();

        public ContentPanel(UIBase owner) : base(owner)
        {
        }

        public void ToggleGameObject(bool newValue, string objectName)
        {
            if (objectName == "header")
            {
                _anchorLabel.GameObject.SetActive(newValue);
                return;
            }
            GameObject btn = _objectsList.Find(n => n.name == objectName);
            if (btn != null)
            {
                btn.SetActive(newValue);
            }
        }
        protected override void ConstructPanelContent()
        {
            
            TitleBar.SetActive(false);
            _uiAnchor = Settings.UseHorizontalContentLayout
                ? UIFactory.CreateHorizontalGroup(ContentRoot, "UIAnchor", true, true, true, true)
                : UIFactory.CreateVerticalGroup(ContentRoot, "UIAnchor", false, true, true, true, padding: new Vector4(5,5,5,5));

            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            _objectsList = new List<GameObject>();


            if (CanDrag)
            {
                // Create pin button as a child of ContentRoot (panel root) instead of _uiAnchor
                var pinButton = UIFactory.CreateToggle(_uiAnchor, "PinButton");
                // Set layout element to position it correctly
                UIFactory.SetLayoutElement(pinButton.GameObject, minHeight: 15, preferredHeight: 15, flexibleHeight: 0,
                    minWidth: 15, preferredWidth: 15, flexibleWidth: 0, ignoreLayout: false);
                
                // Set toggle properties
                pinButton.Toggle.isOn = false;
                //pinButton.OnValueChanged += (value) => IsPinned = value; cétait important?
                _pinToggle = pinButton.Toggle;

                // Make the label text empty or minimal
                pinButton.Text.text = " ";
            }


            _anchorLabel = UIFactory.CreateLabel(_uiAnchor, "UIAnchorText", $"BMOON {PluginInfo.PLUGIN_VERSION}");
            UIFactory.SetLayoutElement(_anchorLabel.GameObject, 80, 25, 1, 1);
            _objectsList.Add(_anchorLabel.GameObject);
            _anchorLabel.GameObject.SetActive(Settings.IsHeaderVisible);

            if (Settings.IsTeleportPanelEnabled)
            {
                var boxListButton = UIFactory.CreateButton(_uiAnchor, "TeleportListButton", "Teleport List");
                UIFactory.SetLayoutElement(boxListButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
                _objectsList.Add(boxListButton.GameObject);
                boxListButton.OnClick = () => 
                {
                    var panel = Plugin.UIManager.GetPanel<TeleportListPanel>();
                    if (panel != null)
                    {
                        panel.Toggle();
                        return;

                    }
                    Plugin.UIManager.AddPanel(PanelType.TeleportList); 
                };
            }

            var pullButton = UIFactory.CreateButton(_uiAnchor, "PullButton", "Pull");
            UIFactory.SetLayoutElement(pullButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
            _objectsList.Add(pullButton.GameObject);
            pullButton.OnClick = () =>
            {
                PullItemsPanel panel;
                panel = Plugin.UIManager.GetPanel<PullItemsPanel>();
                if (panel != null)
                {
                    panel.Toggle();
                    return;

                }
                Plugin.UIManager.AddPanel(PanelType.PullPanel);


                panel = Plugin.UIManager.GetPanel<PullItemsPanel>();
                if (panel != null)
                {
                    panel.RefreshData();
                }

            };

            var settingsButton = UIFactory.CreateButton(_uiAnchor, "SettingsButton", "S");
            UIFactory.SetLayoutElement(settingsButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
            _objectsList.Add(settingsButton.GameObject);
            settingsButton.OnClick = () => 
            {
                SettingsPanel panel;
                panel = Plugin.UIManager.GetPanel<SettingsPanel>();
                if (panel != null)
                {
                    panel.Toggle();
                    return;

                }
                Plugin.UIManager.AddPanel(PanelType.SettingsPanel);

 
                panel = Plugin.UIManager.GetPanel<SettingsPanel>();
                if (panel != null)
                {
                    panel.RefreshData();
                }

            };


            var scaleButton = UIFactory.CreateButton(_uiAnchor, "ScaleButton", "*");
            UIFactory.SetLayoutElement(scaleButton.GameObject, ignoreLayout: false, minWidth: 25, minHeight: 25);
            _objectsList.Add(scaleButton.GameObject);
            _scaleButtonData = new UIScaleSettingButton();
            scaleButton.OnClick = () =>
            {
                _scaleButtonData.PerformAction();
            };
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();

            if (!Settings.UseHorizontalContentLayout)
                ForceRecalculateBasePanelWidth(_objectsList);
        }

        internal override void Reset()
        {
        }

        public override void Update()
        {
            
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                TimerTick();
            }
            base.Update();
            // Call update on the panels that need it
        }
        private void TimerTick()
        {
            if (_teleportRequest.Count == 0)
                return;


            List<string> playersAtProx = PlayerAtProximity();
            for (int x = _teleportRequest.Count - 1; x >= 0; x--)
            {
                var tp = _teleportRequest[x];

                tp.Acceptcount += 1;
                if (tp.Acceptcount == 5 || playersAtProx.Contains(_teleportRequest[x].Username))
                {
                    _teleportRequest.RemoveAt(x);
                }
                else
                {
                    MessageService.EnqueueMessage(".stp tpa " + _teleportRequest[x].Username);
                    _teleportRequest[x] = tp;
                }
            }
        }

        public void AddTeleportRequest(string teleportname)
        {
            var result = _teleportRequest.Find(u => u.Username == teleportname);
            if (string.IsNullOrEmpty(result.Username))
            {
                _teleportRequest.Add(new TeleportRequest(teleportname));
                MessageService.EnqueueMessage(".stp tpa " + teleportname);
            }
        }

        public void RemoveTeleportRequest(string teleportname)
        {
            var result = _teleportRequest.Find(u => u.Username == teleportname);
            if (string.IsNullOrEmpty(result.Username))
            {
                _teleportRequest.RemoveAll(u => u.Username == teleportname);
                LogUtils.LogInfo("Removing request: "+ teleportname);
            }
        }

        public List<string> PlayerAtProximity()
        {
            var playerQuery = Plugin.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCharacter>());
            List<string> results = new List<string>();
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var p in players)
            {
                if (p == Plugin.LocalCharacter) continue;

                PlayerCharacter pp;
                if (p.Has<PlayerCharacter>())
                {
                    pp = p.Read<PlayerCharacter>();
                    float3 pos1 = p.GetPosition();

                    float3 pos2 = Plugin.LocalCharacter.GetPosition();

                    // Ignore height by forcing Y to be the same
                    pos1.y = 0;
                    pos2.y = 0;

                    float dist = math.distance(pos1, pos2);
                    if (dist < 30.0f)
                    {
                        results.Add(pp.Name.Value);
                        LogUtils.LogInfo($"{pp.Name.Value} is close to you at {dist.ToString()}");
                    }

                }
            }
            playerQuery.Dispose();
            return results;
        }

    }
}