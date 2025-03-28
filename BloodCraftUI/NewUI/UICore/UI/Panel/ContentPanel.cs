using System.Collections.Generic;
using System.Linq;
using BloodCraftUI.NewUI.UICore.UI.Controls;
using BloodCraftUI.NewUI.UICore.UI.Panel.Base;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Panels;
using UnityEngine;
using UIBase = BloodCraftUI.NewUI.UICore.UniverseLib.UI.UIBase;

namespace BloodCraftUI.NewUI.UICore.UI.Panel
{
    public class ContentPanel : ResizeablePanelBase
    {
        public ContentPanel(UIBase owner) : base(owner)
        {
        }

        public override string Name => "CorePanel";
        public override int MinWidth => 340;
        public override int MinHeight => 25;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.95f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.95f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        protected override BCUIManager.Panels PanelType => BCUIManager.Panels.Base;

        private GameObject _uiAnchor;
        private UIScaleSettingButton _screenScale;
        private List<PanelBase> UIPanels { get; set; } = new();

        public void TEST_BOX_LIST()
        {
            if (GetPanel<BoxListPanel>() == null)
            {
                var item = new BoxListPanel(Owner);
                UIPanels.Add(item);
                item.AddList("Test 1 ");
                item.AddList("My sweet box1");
                item.AddList("My sweet box2");
                item.AddList("My sweet box3");
                item.AddList("My sweet box4");
                item.AddList("My sweet box5");
                item.AddList("My sweet box6");
            }
        }

        protected override void ConstructPanelContent()
        {
            TitleBar.SetActive(false);
            _uiAnchor = UIFactory.CreateHorizontalGroup(ContentRoot, "UIAnchor", true, true, true, true);
            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            var text = UIFactory.CreateLabel(_uiAnchor, "UIAnchorText", $"BCUI {PluginInfo.PLUGIN_VERSION}");
            UIFactory.SetLayoutElement(text.gameObject, 0, 25, 1, 1);

            var boxListButton = UIFactory.CreateButton(_uiAnchor, "ExpandActionsButton", "Box List");
            UIFactory.SetLayoutElement(boxListButton.GameObject, ignoreLayout: false, minWidth: 120, minHeight: 25);
            boxListButton.OnClick = () =>
            {
                var panel = GetPanel<BoxListPanel>();
                if (panel == null)
                    UIPanels.Add(new BoxListPanel(Owner));
                else
                {
                    panel.SetActive(true);
                    panel.RunUpdateCommand();
                }
            };

            var testButton = UIFactory.CreateButton(_uiAnchor, "TestButton", "Test Me");
            UIFactory.SetLayoutElement(testButton.GameObject, ignoreLayout: false, minWidth: 120, minHeight: 25);
            testButton.OnClick = () => { };

            //_screenScale = new UIScaleSettingButton();

        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
        }

        internal override void Reset()
        {
            foreach (var value in UIPanels)
            {
                value.Destroy();
            }

            UIPanels.Clear();
        }

        public override void Update()
        {
            base.Update();
            // Call update on the panels that need it
        }

        public T GetPanel<T>()
            where T : class
        {
            var t = typeof(T);
            return UIPanels.FirstOrDefault(a => a.GetType() == t) as T;
        }
    }
}