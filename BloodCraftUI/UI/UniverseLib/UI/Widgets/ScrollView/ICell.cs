using BloodmoonPluginsUI.UI.UniverseLib.UI.ObjectPool;
using UnityEngine;

namespace BloodmoonPluginsUI.UI.UniverseLib.UI.Widgets.ScrollView;

public interface ICell : IPooledObject
{
    bool Enabled { get; }

    RectTransform Rect { get; set; }

    void Enable();
    void Disable();
}