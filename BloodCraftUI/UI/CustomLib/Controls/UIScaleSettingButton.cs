using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.CustomLib.Controls;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.UI.UniverseLib.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BloodCraftEZLife.UI.CustomLib.Controls;

public class UIScaleSettingButton : SettingsButtonBase
{
    private readonly List<(string, float)> _scales =
    [
        ("small", 1f),
        ("normal", 1.5f),
        ("medium", 2f)
    ];

    private int _scaleIndex;

    public UIScaleSettingButton() : base("UIScale", "normal")
    {
        _scaleIndex = State switch
        {
            "small" => 0,
            "normal" => 1,
            "medium" => 2,
            _ => 1
        };

        ApplyScale();
    }

    public override string PerformAction()
    {
        _scaleIndex = (_scaleIndex + 1) % _scales.Count;
        ApplyScale();
        Setting.Value = _scales[_scaleIndex].Item1;
        return _scales[_scaleIndex].Item1;
    }

    protected override string Label()
    {
        return $"Toggle screen size [{_scales[_scaleIndex].Item1}]";
    }

    private void ApplyScale()
    {

        var panel = Plugin.UIManager.GetPanel<PullItemsPanel>();
        if (panel != null)
        {
            panel.ContentRoot.GetComponent<RectTransform>().localScale = new Vector3(_scales[_scaleIndex].Item2, _scales[_scaleIndex].Item2, 1f);
            panel.EnsureValidPosition();
        }

        var panel2 = Plugin.UIManager.GetPanel<ChatPanel>();
        if (panel2 != null)
        {
            panel2.ContentRoot.GetComponent<RectTransform>().localScale = new Vector3(_scales[_scaleIndex].Item2, _scales[_scaleIndex].Item2, 1f);
            panel2.EnsureValidPosition();
        }

        var panel3 = Plugin.UIManager.GetPanel<ContentPanel>();
        if (panel3 != null)
            panel3.ContentRoot.GetComponent<RectTransform>().localScale = new Vector3(_scales[_scaleIndex].Item2, _scales[_scaleIndex].Item2, 1f);

        var panel4 = Plugin.UIManager.GetPanel<TeleportListPanel>();
        if (panel4 != null)
        { 
            panel4.ContentRoot.GetComponent<RectTransform>().localScale = new Vector3(_scales[_scaleIndex].Item2, _scales[_scaleIndex].Item2, 1f);
            panel4.EnsureValidPosition();
        }
    }
}