using System;
using BloodCraftUI.UI.UniverseLib.UI.Widgets.ScrollView;

namespace BloodCraftUI.UI.CustomLib.Cells;

public interface IFormedCell : ICell
{
    public int CurrentDataIndex { get; set; }
    public Action<int> OnClick { get; set; }
}