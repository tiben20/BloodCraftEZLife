using UnityEngine;

namespace BloodCraftUI.UI.UniverseLib.UI.Panels;

public interface IPanelBase
{
    BCUIManager.Panels PanelType { get; }
    string PanelId { get; }
    bool Enabled { get; set; }
    RectTransform Rect { get; }
    GameObject UIRoot { get; }
    PanelDragger Dragger { get; }

    void Destroy();
    void EnsureValidSize();
    void EnsureValidPosition();
}