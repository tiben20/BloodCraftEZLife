using System;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Widgets.ScrollView;

namespace BloodmoonPluginsUI.UI.CustomLib.Cells;

public interface IFormedCell : ICell
{
    public int CurrentDataIndex { get; set; }
    public Action<int> OnClick { get; set; }
}