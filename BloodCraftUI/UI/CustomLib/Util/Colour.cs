using System;
using BloodCraftUI.Utils;
using UnityEngine;

namespace BloodCraftUI.UI.CustomLib.Util;

public static class Colour
{
    // Base colour palette
    public static Color Level1 = new(0.64f, 0, 0);
    public static Color Level2 = new(0.72f, 0.43f, 0);
    public static Color Level3 = new(1, 0.83f, 0.45f);
    public static Color Level4 = new(0.47f, 0.74f, 0.38f);
    public static Color Level5 = new(0.18f, 0.53f, 0.67f);

    // Colour constants
    public static Color DefaultBar = Level4;
    public static Color Highlight = Color.yellow;
    public static Color PositiveChange = Color.yellow;
    public static Color NegativeChange = Color.red;

    public static Color DarkBackground = new(0.07f, 0.07f, 0.07f);
    public static Color PanelBackground = new(0.17f, 0.17f, 0.17f);
    public static Color SliderFill = new(0.3f, 0.3f, 0.3f);
    public static Color SliderHandle = new(0.5f, 0.5f, 0.5f);
    public static Color DefaultText = Color.white;
    public static Color PlaceHolderText = SliderHandle;

    public static Color SelectableNormal = new(0.2f, 0.2f, 0.2f);
    public static Color SelectableHighlighted = new(0.3f, 0.3f, 0.3f);
    public static Color SelectablePressed = new(0.15f, 0.15f, 0.15f);

    public static Color SliderNormal = new(0.4f, 0.4f, 0.4f);
    public static Color SliderHighlighted = new(0.55f, 0.55f, 0.55f);
    public static Color SliderPressed = new(0.3f, 0.3f, 0.3f);

    public static Color ScrollbarNormal = new(0.4f, 0.4f, 0.4f);
    public static Color ScrollbarHighlighted = new(0.5f, 0.5f, 0.5f);
    public static Color ScrollbarPressed = new(0.3f, 0.3f, 0.3f);
    public static Color ScrollbarDisabled = new(0.5f, 0.5f, 0.5f);

    public static Color DropDownScrollBarNormal = new(0.45f, 0.45f, 0.45f);
    public static Color DropDownScrollbarHighlighted = new(0.6f, 0.6f, 0.6f);
    public static Color DropDownScrollbarPressed = new(0.4f, 0.4f, 0.4f);
    public static Color DropDownToggleNormal = new(0.35f, 0.35f, 0.35f);
    public static Color DropDownToggleHighlighted = new(0.25f, 0.25f, 0.25f);

    public static Color InputFieldNormal = new(1f, 1f, 1f);
    public static Color InputFieldHighlighted = new(0.95f, 0.95f, 0.95f);
    public static Color InputFieldPressed = new(0.78f, 0.78f, 0.78f);

    public static Color ToggleNormal = Color.black;
    public static Color ToggleCheckMark = new(0.6f, 0.7f, 0.6f);

    // TODO check if the viewport objects even need a colour or image
    public static Color ViewportBackground = new(0.07f, 0.07f, 0.07f);
    public static Color White  = Color.white;

    public static void SetOpacity(float opacity)
    {
        ViewportBackground = ViewportBackground.GetTransparent(opacity);

        Level1 = Level1.GetTransparent(opacity);
        Level2 = Level2.GetTransparent(opacity);
        Level3 = Level3.GetTransparent(opacity);
        Level4 = Level4.GetTransparent(opacity);
        Level5 = Level5.GetTransparent(opacity);

        DefaultBar = DefaultBar.GetTransparent(opacity);
        Highlight = Highlight.GetTransparent(opacity);
        PositiveChange = PositiveChange.GetTransparent(opacity);
        NegativeChange = NegativeChange.GetTransparent(opacity);

        DarkBackground = DarkBackground.GetTransparent(opacity);
        PanelBackground = PanelBackground.GetTransparent(opacity);
        SliderFill = SliderFill.GetTransparent(opacity);
        SliderHandle = SliderHandle.GetTransparent(opacity);
        ToggleCheckMark = ToggleCheckMark.GetTransparent(opacity);
        DefaultText = DefaultText.GetTransparent(opacity);
        PlaceHolderText = PlaceHolderText.GetTransparent(opacity);

        SelectableNormal = SelectableNormal.GetTransparent(opacity);
        SelectableHighlighted = SelectableHighlighted.GetTransparent(opacity);
        SelectablePressed = SelectablePressed.GetTransparent(opacity);

        SliderNormal = SliderNormal.GetTransparent(opacity);
        SliderHighlighted = SliderHighlighted.GetTransparent(opacity);
        SliderPressed = SliderPressed.GetTransparent(opacity);

        ScrollbarNormal = ScrollbarNormal.GetTransparent(opacity);
        ScrollbarHighlighted = ScrollbarHighlighted.GetTransparent(opacity);
        ScrollbarPressed = ScrollbarPressed.GetTransparent(opacity);
        ScrollbarDisabled = ScrollbarDisabled.GetTransparent(opacity);

        DropDownScrollBarNormal = DropDownScrollBarNormal.GetTransparent(opacity);
        DropDownScrollbarHighlighted = DropDownScrollbarHighlighted.GetTransparent(opacity);
        DropDownScrollbarPressed = DropDownScrollbarPressed.GetTransparent(opacity);
        DropDownToggleNormal = DropDownToggleNormal.GetTransparent(opacity);
        DropDownToggleHighlighted = DropDownToggleHighlighted.GetTransparent(opacity);

        InputFieldNormal = InputFieldNormal.GetTransparent(opacity);
        InputFieldHighlighted = InputFieldHighlighted.GetTransparent(opacity);
        InputFieldPressed = InputFieldPressed.GetTransparent(opacity);

        ToggleNormal = ToggleNormal.GetTransparent(opacity);
    }

    /// <summary>
    /// Parses a colour string to extract the colour or colour map.
    /// For colour maps, the appropriate colour is calculated based on the percentage provided.
    /// </summary>
    /// <param name="colourString"></param>
    /// <param name="percentage"></param>
    /// <returns></returns>
    public static Color ParseColour(string colourString, float percentage = 0)
    {
        if (string.IsNullOrEmpty(colourString)) return DefaultBar;
        if (colourString.StartsWith("@"))
        {
            var colourStrings = colourString.Split("@", StringSplitOptions.RemoveEmptyEntries);
            if (colourStrings.Length == 0) return DefaultBar;
            if (colourStrings.Length == 1)
            {
                if (!ColorUtility.TryParseHtmlString(colourStrings[0], out var onlyColour)) onlyColour = DefaultBar;
                return onlyColour;
            }

            var internalRange = percentage * (colourStrings.Length - 1);
            var index = (int)Math.Floor(internalRange);
            internalRange -= index;
            if (!ColorUtility.TryParseHtmlString(colourStrings[index], out var colour1)) colour1 = DefaultBar;
            if (!ColorUtility.TryParseHtmlString(colourStrings[index + 1], out var colour2)) colour2 = DefaultBar;
            return Color.Lerp(colour1, colour2, internalRange);
        }

        return !ColorUtility.TryParseHtmlString(colourString, out var parsedColour) ? DefaultBar : parsedColour;
    }

    private static float Contrast(float l1, float l2)
    {
        return l1 < l2
            ? (l2 + 0.05f) / (l1 + 0.05f)
            : (l1 + 0.05f) / (l2 + 0.05f);
    }

    private static float CalculateRelativeLuminance(float r, float g, float b)
    {
        float rv = r <= 0.04045f ? r / 12.92f : MathF.Pow((r + 0.055f) / 1.055f, 2.4f);
        float gv = g <= 0.04045f ? g / 12.92f : MathF.Pow((g + 0.055f) / 1.055f, 2.4f);
        float bv = b <= 0.04045f ? b / 12.92f : MathF.Pow((b + 0.055f) / 1.055f, 2.4f);

        return 0.2126f * rv + 0.7152f * gv + 0.0722f * bv;
    }

    public static Color TextColourForBackground(Color background)
    {
        float backgroundLuminance = CalculateRelativeLuminance(background.r, background.g, background.b);
        float whiteContrastRatio = Contrast(backgroundLuminance, 1f);
        float blackContrastRatio = Contrast(backgroundLuminance, 0f);
        return whiteContrastRatio > blackContrastRatio ? Color.white : Color.black;
    }
}