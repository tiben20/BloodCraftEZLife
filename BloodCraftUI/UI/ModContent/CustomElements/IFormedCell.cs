using System;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;

namespace BloodCraftEZLife.UI.ModContent.CustomElements;

public interface IFormedCell : ICell
{
    public int CurrentDataIndex { get; set; }
    public Action<int> OnClick { get; set; }
}