using System.Collections;
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
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPosition => new Vector2(Owner.Scaler.m_ReferenceResolution.x - 150,
            Owner.Scaler.m_ReferenceResolution.y * 0.5f);
        public override bool CanDrag => true;
        private readonly Color _pbColor = new Color(1f, 50f, 32f, 255f);

        // Allow vertical resizing only
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
        private TextMeshProUGUI _drValue;

        // Content size fitter reference
        private ContentSizeFitter _contentSizeFitter;

        public FamStatsPanel(UIBase owner) : base(owner)
        {
        }

        private void RecalculateHeight()
        {
            if (_uiAnchor == null) return;

            // Force layout rebuild
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_uiAnchor.GetComponent<RectTransform>());

            // Get the content size with padding
            float contentHeight = _uiAnchor.GetComponent<RectTransform>().rect.height;
            float paddingTotal = 0; // Account for padding (12px top + 12px bottom)

            // Set the minimum height based on content or MinHeight, whichever is larger
            float newHeight = Mathf.Max(contentHeight + paddingTotal, MinHeight);

            // Only update if different to avoid layout thrashing
            if (Mathf.Abs(Rect.sizeDelta.y - newHeight) > 1f)
            {
                Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, newHeight);
            }
        }

        public void UpdateData(FamStats data)
        {
            if (data == null) return;

            var doFlash = _data != null && _data.ExperiencePercent != data.ExperiencePercent;
            _data = data;

            // Ensure we have a name to display
            string nameToShow = !string.IsNullOrEmpty(data.Name) ? data.Name : "Unknown Familiar";

            // Update name with school if available
            var schoolText = string.IsNullOrEmpty(data.School) ? "" : $" - {data.School}";
            if (_nameLabel != null)
                _nameLabel.text = $"{nameToShow}{schoolText}";

            // Update level info
            if (_levelLabel != null)
                _levelLabel.text =
                    $"Level: {data.Level}{(string.IsNullOrEmpty(schoolText) ? null : $"   Prestige: {data.PrestigeLevel}")}";

            // Update stat values
            if (_healthValue != null)
                _healthValue.text = data.MaxHealth.ToString();

            if (_ppValue != null)
                _ppValue.text = data.PhysicalPower.ToString();

            if (_spValue != null)
                _spValue.text = data.SpellPower.ToString();

            if (_drValue != null)
                _drValue.text = data.DamageReduction;

            // Update progress bar
            if (_progressBar != null)
            {
                _progressBar.SetProgress(
                    data.ExperiencePercent / 100f,
                    "",
                    $"XP: {data.ExperienceValue} ({data.ExperiencePercent}%)",
                    ActiveState.Active,
                    _pbColor,
                    data.ExperienceValue.ToString(),
                    doFlash
                );
            }

            // Force layout rebuild and recalculate height
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRoot.GetComponent<RectTransform>());
            RecalculateHeight();
        }

        protected override void ConstructPanelContent()
        {
            // Hide the title bar and set up the panel
            TitleBar.SetActive(false);
            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            var color = Colour.PanelBackground;
            color.a = 200f;

            // Create main container
            _uiAnchor = UIFactory.CreateVerticalGroup(ContentRoot, "UIAnchor", true, false, true, true, 4,
                new Vector4(12, 2, 12, 2), color);

            // Make the content control the height
            UIFactory.SetLayoutElement(_uiAnchor, flexibleWidth: 9999, minHeight: 0, flexibleHeight: 0);

            // Add content size fitter to allow content to determine its size
            _contentSizeFitter = _uiAnchor.AddComponent<ContentSizeFitter>();
            _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Create header section
            CreateHeaderSection();

            // Create stats container
            CreateStatsSection();

            // Create XP progress bar at the bottom
            CreateProgressBarSection();

            // Set default position
            SetDefaultSizeAndPosition();
        }

        private void CreateHeaderSection()
        {
            // Create container for the header section with preferred height
            var headerContainer = UIFactory.CreateVerticalGroup(_uiAnchor, "HeaderContainer", true, false, true, true, 4, default, new Color(0.15f, 0.15f, 0.15f));
            UIFactory.SetLayoutElement(headerContainer, minHeight: 80, preferredHeight: 80, flexibleHeight: 0, flexibleWidth: 9999);

            // Familiar name with larger font
            _nameLabel = UIFactory.CreateLabel(headerContainer, "FamNameText", BloodCraftStateService.CurrentFamName ?? "Unknown", TextAlignmentOptions.Center, null, 18);
            UIFactory.SetLayoutElement(_nameLabel.gameObject, minHeight: 30, preferredHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);
            _nameLabel.fontStyle = FontStyles.Bold;

            // Level info
            _levelLabel = UIFactory.CreateLabel(headerContainer, "FamLevelText", "Level: Unknown   Prestige: Unknown", TextAlignmentOptions.Center, null, 16);
            UIFactory.SetLayoutElement(_levelLabel.gameObject, minHeight: 30, preferredHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);
        }

        private void CreateStatsSection()
        {
            // Stats container with preferred height
            _statsContainer = UIFactory.CreateVerticalGroup(_uiAnchor, "StatsContainer", true, false, true, true, 6, default, new Color(0.12f, 0.12f, 0.12f));
            UIFactory.SetLayoutElement(_statsContainer, minHeight: 130, preferredHeight: 130, flexibleHeight: 0, flexibleWidth: 9999);

            // Create stat rows with consistent styling
            CreateStatRow(_statsContainer, "Health", out _healthRow, out _healthValue);
            CreateStatRow(_statsContainer, "Physical Power", out _ppRow, out _ppValue);
            CreateStatRow(_statsContainer, "Spell Power", out _spRow, out _spValue);
            CreateStatRow(_statsContainer, "Damage Reduction", out _, out _drValue);
        }

        private void CreateStatRow(GameObject parent, string label, out GameObject rowObj, out TextMeshProUGUI valueText)
        {
            // Create a horizontal row for each stat with preferred height
            rowObj = UIFactory.CreateHorizontalGroup(parent, $"{label}Row", false, false, true, true, 5, default, new Color(0.18f, 0.18f, 0.18f));
            UIFactory.SetLayoutElement(rowObj, minHeight: 36, preferredHeight: 36, flexibleHeight: 0, flexibleWidth: 9999);

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
            // Create a container with preferred height
            var progressBarHolder = UIFactory.CreateUIObject("ProgressBarContent", _uiAnchor);
            UIFactory.SetLayoutElement(progressBarHolder, minHeight: 30, preferredHeight: 30, flexibleHeight: 0, flexibleWidth: 9999);

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
            // Clean up timer if needed
            if (_queryTimer != null)
            {
                _queryTimer.Stop();
                _queryTimer.Dispose();
                _queryTimer = null;
            }

            // Reset progress bar if needed
            _progressBar?.Reset();
        }

        public override void OnFinishResize()
        {
            base.OnFinishResize();

            // After manual resize, make sure content still fits
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRoot.GetComponent<RectTransform>());
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
                    School = "Unholy",
                    DamageReduction = "8.1%"
                });
            }

            // Delay the initial height calculation
            CoroutineUtility.StartCoroutine(DelayedHeightCalculation());

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
                    if (_data.ExperiencePercent > 100)
                        _data.ExperiencePercent = 0;
                    _data.PhysicalPower++;
                    UpdateData(_data);
                }
            };
            _queryTimer.Start();
        }

        private IEnumerator DelayedHeightCalculation()
        {
            // Wait a few frames for layout to stabilize
            yield return null;
            yield return null;

            // Calculate the height based on content
            RecalculateHeight();
        }

        private void SendUpdateStatsCommand()
        {
            MessageService.EnqueueMessage(MessageService.BCCOM_FAMSTATS);
        }
    }
}