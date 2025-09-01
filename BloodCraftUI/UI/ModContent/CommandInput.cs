
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using static UnityEngine.Rendering.DebugUI;


internal class CommandInput : PanelBase
{
    public override string PanelId => "SettingsList";
    public override int MinWidth => (int)480;
    public override int MinHeight => (int)240;
    public override int MaxWidth => (int)3840;

    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);

    public override bool CanDrag => false;
    public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
    public override PanelType PanelType => PanelType.SettingsPanel;

    private bool _isInitialized;



    public GameObject Root;
    public InputFieldRef Input;
    public System.Action<Hotkey> _onCommand; // callback to parent
    private Hotkey _currentHotkey;
    public CommandInput(UIBase owner) : base(owner)
    {
        SetTitle("InputBox");
    }
    //public CommandInput(Transform parent, System.Action<Hotkey> onCommand)
        protected override void ConstructPanelContent()
    {
        //_onCommand = onCommand;
        
        // --- Background panel with layout inside ---
        GameObject panel = UIFactory.CreateVerticalGroup(ContentRoot, "CommandPanel", true, true, true, true);
        UIFactory.SetLayoutElement(panel, minHeight: 100, minWidth: 250);

        // --- Title ---
        LabelRef titleGO = UIFactory.CreateLabel(panel, "CommandTitle", "Enter Command you can make 2 command in one if you separate with |");
        var titleText = titleGO.GameObject.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 22;
        titleText.alignment = TextAlignmentOptions.Midline;

        // --- InputField ---
        Input = UIFactory.CreateInputField(panel, "CommandInput", "");
        UIFactory.SetLayoutElement(Input.GameObject, minHeight: 30, flexibleWidth: 9999);
        Input.Component.textComponent.fontSize = 22;

        // Optional: callback
        Input.Component.onSubmit.AddListener(cmd =>
        {
            Debug.Log("Command submitted: " + cmd);
            _currentHotkey.action = cmd;
            _onCommand?.Invoke(_currentHotkey); // notify parent

        });

    }

    protected override void OnClosePanelClicked()
    {
        SetActive(false);
    }

    protected override void LateConstructUI()
    {
        base.LateConstructUI();
        float xfact = Owner.Scaler.referenceResolution.x / 1920;
        float yfact = Owner.Scaler.referenceResolution.y / 1080;
        //put sizing here
        FullscreenSettingService.ResFactor = new Vector2(xfact, yfact);
        Vector2 newRect = new Vector2(FullscreenSettingService.DeltaRect.x * xfact * 0.5f, FullscreenSettingService.DeltaRect.y * yfact * 0.5f);
        Rect.sizeDelta = newRect;

        Rect.anchoredPosition = FullscreenSettingService.AnchorRect;
        EnsureValidSize();
        EnsureValidPosition();
    }

    public void Show(BloodCraftEZLife.Services.Hotkey oldcommand)
    {
        _currentHotkey = oldcommand;
        Input.Text = oldcommand.action;
        Input.Component.ActivateInputField(); // focus automatically
        Input.Component.selectionAnchorPosition = Input.Text.Length;
        Input.Component.selectionFocusPosition = Input.Text.Length;
    }

}