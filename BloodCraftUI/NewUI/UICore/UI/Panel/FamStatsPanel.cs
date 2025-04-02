using BloodCraftUI.NewUI.UICore.UI.Panel.Base;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Panels;
using BloodCraftUI.Services;
using System.Timers;
using BloodCraftUI.Config;
using BloodCraftUI.NewUI.UICore.UI.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BloodCraftUI.NewUI.UICore.UI.Util;

namespace BloodCraftUI.NewUI.UICore.UI.Panel
{
    internal class FamStatsPanel : ResizeablePanelBase
    {
        public override string PanelId => "FamStatsPanel";
        public override int MinWidth => 340;
        public override int MinHeight => 30;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPosition => new Vector2(Owner.Scaler.m_ReferenceResolution.x - 150,
            Owner.Scaler.m_ReferenceResolution.y * 0.5f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override BCUIManager.Panels PanelType => BCUIManager.Panels.FamStats;
        private GameObject _uiAnchor;
        private Timer _queryTimer;
        private FamStats _data = new();

        // Controls for an update
        private TextMeshProUGUI _nameLabel;
        private TextMeshProUGUI _levelLabel;
        private GameObject _statsContainer;
        private ProgressBar _progressBar;

        // Stat entry layout elements
        private GameObject _healthRow;
        private GameObject _ppRow;
        private GameObject _spRow;
        private TextMeshProUGUI _healthValue;
        private TextMeshProUGUI _ppValue;
        private TextMeshProUGUI _spValue;

        public FamStatsPanel(UIBase owner) : base(owner)
        {
        }

        public void UpdateData(FamStats data)
        {
            var doFlash = _data != null && _data.ExperiencePercent != data.ExperiencePercent;
            _data = data;

            // Ensure we have a name to display
            string nameToShow = !string.IsNullOrEmpty(data.Name) ? data.Name : "Unknown Familiar";

            // Update name with school if available
            string schoolText = string.IsNullOrEmpty(data.School) ? "" : $" - {data.School}";
            _nameLabel.text = $"{nameToShow}{schoolText}";

            // Update level info
            _levelLabel.text = $"Level: {data.Level}   Prestige: {data.PrestigeLevel}";

            // Update stat values
            _healthValue.text = data.MaxHealth.ToString();
            _ppValue.text = data.PhysicalPower.ToString();
            _spValue.text = data.SpellPower.ToString();

            // Update progress bar
            _progressBar.SetProgress(
                data.ExperiencePercent / 100f,
                "",
                $"XP: {data.ExperienceValue} ({data.ExperiencePercent}%)",
                ActiveState.Active,
                Color.green,
                data.ExperienceValue.ToString(),
                doFlash
            );
        }

        protected override void ConstructPanelContent()
        {
            // Hide the title bar and set up the panel
            TitleBar.SetActive(false);
            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            // Create main container
            _uiAnchor = UIFactory.CreateVerticalGroup(ContentRoot, "UIAnchor", true, false, true, true, 8,
                new Vector4(12, 12, 12, 12), Colour.PanelBackground);
            UIFactory.SetLayoutElement(_uiAnchor, flexibleWidth: 9999, flexibleHeight: 9999);

            // Create header section
            CreateHeaderSection();

            // Create stats container
            CreateStatsSection();

            // Create XP progress bar at the bottom
            CreateProgressBarSection();

            SetDefaultSizeAndPosition();
        }

        private void CreateHeaderSection()
        {
            // Create container for the header section
            var headerContainer = UIFactory.CreateVerticalGroup(_uiAnchor, "HeaderContainer", true, false, true, true, 4, default, new Color(0.15f, 0.15f, 0.15f));
            UIFactory.SetLayoutElement(headerContainer, minHeight: 80, flexibleHeight: 0, flexibleWidth: 9999);

            // Familiar name with larger font
            _nameLabel = UIFactory.CreateLabel(headerContainer, "FamNameText", BloodCraftStateService.CurrentFamName ?? "Unknown", TextAlignmentOptions.Center, null, 18);
            UIFactory.SetLayoutElement(_nameLabel.gameObject, minHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);
            _nameLabel.fontStyle = FontStyles.Bold;

            // Level info
            _levelLabel = UIFactory.CreateLabel(headerContainer, "FamLevelText", "Level: Unknown   Prestige: Unknown", TextAlignmentOptions.Center, null, 16);
            UIFactory.SetLayoutElement(_levelLabel.gameObject, minHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);
        }

        private void CreateStatsSection()
        {
            // Stats container with fixed height
            _statsContainer = UIFactory.CreateVerticalGroup(_uiAnchor, "StatsContainer", true, false, true, true, 6, default, new Color(0.12f, 0.12f, 0.12f));
            UIFactory.SetLayoutElement(_statsContainer, minHeight: 130, flexibleHeight: 0, flexibleWidth: 9999);

            // Create stat rows with consistent styling
            CreateStatRow(_statsContainer, "Health", out _healthRow, out _healthValue);
            CreateStatRow(_statsContainer, "Physical Power", out _ppRow, out _ppValue);
            CreateStatRow(_statsContainer, "Spell Power", out _spRow, out _spValue);
        }

        private void CreateStatRow(GameObject parent, string label, out GameObject rowObj, out TextMeshProUGUI valueText)
        {
            // Create a horizontal row for each stat
            rowObj = UIFactory.CreateHorizontalGroup(parent, $"{label}Row", false, false, true, true, 5, default, new Color(0.18f, 0.18f, 0.18f));
            UIFactory.SetLayoutElement(rowObj, minHeight: 36, flexibleHeight: 0, flexibleWidth: 9999);

            // Stat label
            var statLabel = UIFactory.CreateLabel(rowObj, $"{label}Label", label, TextAlignmentOptions.Left, null, 15);
            UIFactory.SetLayoutElement(statLabel.gameObject, minWidth: 150, flexibleWidth: 0, minHeight: 36, flexibleHeight: 0);

            // Value display
            valueText = UIFactory.CreateLabel(rowObj, $"{label}Value", "0", TextAlignmentOptions.Right, Color.white, 15);
            UIFactory.SetLayoutElement(valueText.gameObject, minWidth: 100, flexibleWidth: 9999, minHeight: 36, flexibleHeight: 0);
            valueText.fontStyle = FontStyles.Bold;
        }

        private void CreateProgressBarSection()
        {
            // Create a simple container with absolute positioning
            var progressBarHolder = UIFactory.CreateUIObject("ProgressBarContent", _uiAnchor);

            // Set layout element to control height only
            UIFactory.SetLayoutElement(progressBarHolder, minHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);

            // Set rect transform to stretch horizontally
            var rect = progressBarHolder.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 30);

            // Create the progress bar
            _progressBar = new ProgressBar(progressBarHolder, Colour.DefaultBar);

            // Force layout rebuild to ensure everything is sized correctly
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            // Set initial progress (important for mask approach)
            _progressBar.SetProgress(0f, "", "XP: 0 (0%)", ActiveState.Active, Colour.DefaultBar, "", false);
        }

        internal override void Reset()
        {
            // Nothing to reset specifically
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();

            if (Plugin.IS_TESTING)
            {
                UpdateData(new FamStats
                {
                    Name = "TestFamiliar",
                    Level = 99,
                    PrestigeLevel = 5,
                    ExperienceValue = 6500,
                    ExperiencePercent = 65,
                    MaxHealth = 5000,
                    PhysicalPower = 450,
                    SpellPower = 575,
                    School = "Unholy"
                });
            }

            // Start querying for updates
            SendUpdateStatsCommand();
            _queryTimer = new Timer(Settings.FamStatsQueryIntervalInSeconds * 1000);
            _queryTimer.AutoReset = true;
            _queryTimer.Elapsed += (_, _) =>
            {
                SendUpdateStatsCommand();
                if (Plugin.IS_TESTING)
                {
                    _data.ExperiencePercent += 10;
                    if(_data.ExperiencePercent > 100)
                        _data.ExperiencePercent = 0;
                    UpdateData(_data);
                }
            };
            _queryTimer.Start();
        }

        private void SendUpdateStatsCommand()
        {
            MessageService.EnqueueMessage(MessageService.BCCOM_FAMSTATS);
        }
    }
}