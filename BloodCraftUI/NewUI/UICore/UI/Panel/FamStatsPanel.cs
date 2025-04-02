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
using ProgressBar = BloodCraftUI.NewUI.UICore.UI.Panel.Base.ProgressBar;

namespace BloodCraftUI.NewUI.UICore.UI.Panel
{
    internal class FamStatsPanel: ResizeablePanelBase
    {
        public override string PanelId => "FamStatsPanel";
        public override int MinWidth => 340;
        public override int MinHeight => 400;
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

        //controls for an update
        TextMeshProUGUI _nameLabel;
        TextMeshProUGUI _levelLabel;
        TextMeshProUGUI _healthLabel;
        TextMeshProUGUI _statsLabel;
        private ProgressBar _progressBar;

        public FamStatsPanel(UIBase owner) : base(owner)
        {
        }

        public void UpdateData(FamStats data)
        {
            var doFlash = _data != null && _data.ExperiencePercent != data.ExperiencePercent;
            _data = data;
            _nameLabel.text = $"{data.Name}{(string.IsNullOrEmpty(data.School) ? null : $" - {data.School}")}";
            _levelLabel.text = $"Level: {data.Level} Prestige: {data.PrestigeLevel}";
            _healthLabel.text = $"Health: {data.MaxHealth}";
            _statsLabel.text = $"PP: {data.PhysicalPower} SP: {data.SpellPower}";
            _progressBar.SetProgress(data.ExperiencePercent, "", "", ActiveState.Active, Color.green, data.ExperienceValue.ToString(), doFlash);
        }

        protected override void ConstructPanelContent()
        {
            TitleBar.SetActive(false);
            _uiAnchor = UIFactory.CreateVerticalGroup(ContentRoot, "UIAnchor", true, false, true, true);
            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            var hor1 = UIFactory.CreateHorizontalGroup(_uiAnchor, "HorizontalNameGroup", true, true, true, true);
            _nameLabel = UIFactory.CreateLabel(hor1, "FamNameText", BloodCraftStateService.CurrentFamName ?? "Unknown");
            UIFactory.SetLayoutElement(_nameLabel.gameObject, 0, 25, 1, 1);

            _levelLabel = UIFactory.CreateLabel(_uiAnchor, "FamLevelText", "Unknown");
            UIFactory.SetLayoutElement(_levelLabel.gameObject, 0, 25, 1, 1);
            _healthLabel = UIFactory.CreateLabel(_uiAnchor, "FamHealthText", "Unknown");
            UIFactory.SetLayoutElement(_healthLabel.gameObject, 0, 25, 1, 1);
            _statsLabel = UIFactory.CreateLabel(_uiAnchor, "FamStatsText", "Unknown");
            UIFactory.SetLayoutElement(_statsLabel.gameObject, 0, 25, 1, 1);

            var progressBarHolder = UIFactory.CreateUIObject("ProgressBarContent", _uiAnchor);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(progressBarHolder, false, false, true, true);
            UIFactory.SetLayoutElement(progressBarHolder, ignoreLayout: true);
            var progressRect = progressBarHolder.GetComponent<RectTransform>();
            progressRect.anchorMin = Vector2.zero;
            progressRect.anchorMax = Vector2.right;
            progressRect.pivot = new Vector2(0.5f, 1);
            _progressBar = new ProgressBar(progressBarHolder, Colour.DefaultBar);

            SetDefaultSizeAndPosition();
        }

        internal override void Reset()
        {
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            SendUpdateStatsCommand();
            _queryTimer = new Timer(Settings.FamStatsQueryIntervalInSeconds);
            _queryTimer.AutoReset = true;
            _queryTimer.Elapsed += (_, _) => SendUpdateStatsCommand();
            _queryTimer.Start();
        }

        private void SendUpdateStatsCommand()
        {
            MessageService.EnqueueMessage(MessageService.BCCOM_FAMSTATS);
        }
    }
}
