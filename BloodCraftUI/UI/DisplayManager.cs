using System.Collections;
using BloodCraftUI.NEW;
using BloodCraftUI.Utils;
using UnityEngine;

namespace BloodCraftUI.UI
{
    public static class DisplayManager
    {
        public static int ActiveDisplayIndex { get; private set; }
        public static Display ActiveDisplay => Display.displays[ActiveDisplayIndex];

        public static int Width => ActiveDisplay.renderingWidth;
        public static int Height => ActiveDisplay.renderingHeight;

        public static Vector3 MousePosition => Application.isEditor
            ? InputManager.MousePosition
            : Display.RelativeMouseAt(InputManager.MousePosition);

        public static bool MouseInTargetDisplay => MousePosition.z == ActiveDisplayIndex;

        private static Camera canvasCamera;

        internal static void Init()
        {
            SetDisplay(0);
        }

        public static void SetDisplay(int display)
        {
            if (ActiveDisplayIndex == display)
                return;

            if (Display.displays.Length <= display)
            {
                LogUtils.LogWarning($"Cannot set display index to {display} as there are not enough monitors connected!");

                return;
            }

            ActiveDisplayIndex = display;
            ActiveDisplay.Activate();

            UICustomManager.UICanvas.targetDisplay = display;

            // ensure a camera is targeting the display
            if (!Camera.main || Camera.main.targetDisplay != display)
            {
                if (!canvasCamera)
                {
                    canvasCamera = new GameObject("BCUI_CanvasCamera").AddComponent<Camera>();
                    Object.DontDestroyOnLoad(canvasCamera.gameObject);
                    canvasCamera.hideFlags = HideFlags.HideAndDontSave;
                }
                canvasCamera.targetDisplay = display;
            }

            RuntimeHelper.StartCoroutine(FixPanels());
        }

        private static IEnumerator FixPanels()
        {
            yield return null;
            yield return null;

            foreach (UEPanel panel in UICustomManager.UIPanels.Values)
            {
                panel.EnsureValidSize();
                panel.EnsureValidPosition();
                panel.Dragger.OnEndResize();
            }

            foreach (UEPanel panel in UICustomManager.UIContentPanels.Values)
            {
                panel.EnsureValidSize();
                panel.EnsureValidPosition();
                panel.Dragger.OnEndResize();
            }
        }
    }
}
