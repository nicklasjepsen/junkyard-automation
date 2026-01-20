using UnityEngine;
using JunkyardAutomation.Placement;

namespace JunkyardAutomation.UI
{
    /// <summary>
    /// Simple build menu using IMGUI.
    /// Shows buttons for building conveyors and demolishing.
    /// </summary>
    public class BuildMenuUI : MonoBehaviour
    {
        [Header("Style")]
        [SerializeField] private int fontSize = 16;
        [SerializeField] private float buttonWidth = 120f;
        [SerializeField] private float buttonHeight = 40f;
        [SerializeField] private float padding = 10f;

        private GUIStyle buttonStyle;
        private GUIStyle selectedButtonStyle;
        private GUIStyle labelStyle;

        private void Start()
        {
            CreateStyles();
        }

        private void CreateStyles()
        {
            // Normal button
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = fontSize;
            buttonStyle.normal.textColor = Color.white;

            // Selected button
            selectedButtonStyle = new GUIStyle(buttonStyle);
            selectedButtonStyle.normal.background = MakeTexture(2, 2, new Color(0.3f, 0.5f, 0.8f, 1f));
            selectedButtonStyle.normal.textColor = Color.white;

            // Label
            labelStyle = new GUIStyle();
            labelStyle.fontSize = fontSize - 2;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private void OnGUI()
        {
            if (buttonStyle == null) CreateStyles();

            PlacementManager pm = PlacementManager.Instance;
            if (pm == null) return;

            // Position at bottom center
            float menuWidth = buttonWidth * 3 + padding * 4;
            float menuHeight = buttonHeight + padding * 2 + 25f;
            float x = (Screen.width - menuWidth) / 2f;
            float y = Screen.height - menuHeight - padding;

            // Background
            GUI.Box(new Rect(x, y, menuWidth, menuHeight), "");

            // Title
            GUI.Label(
                new Rect(x, y + 5f, menuWidth, 20f),
                "Build Menu",
                labelStyle
            );

            float btnY = y + 25f;
            float btnX = x + padding;

            // Conveyor button
            bool isConveyorSelected = pm.CurrentMode == PlacementMode.Build && pm.SelectedMachineType == "Conveyor";
            GUIStyle conveyorStyle = isConveyorSelected ? selectedButtonStyle : buttonStyle;

            if (GUI.Button(new Rect(btnX, btnY, buttonWidth, buttonHeight), "Conveyor", conveyorStyle))
            {
                if (isConveyorSelected)
                    pm.CancelPlacement();
                else
                    pm.EnterBuildMode("Conveyor");
            }

            btnX += buttonWidth + padding;

            // Demolish button
            bool isDemolishSelected = pm.CurrentMode == PlacementMode.Demolish;
            GUIStyle demolishStyle = isDemolishSelected ? selectedButtonStyle : buttonStyle;

            if (GUI.Button(new Rect(btnX, btnY, buttonWidth, buttonHeight), "Demolish", demolishStyle))
            {
                if (isDemolishSelected)
                    pm.CancelPlacement();
                else
                    pm.EnterDemolishMode();
            }

            btnX += buttonWidth + padding;

            // Cancel button (only show when in a mode)
            if (pm.CurrentMode != PlacementMode.None)
            {
                if (GUI.Button(new Rect(btnX, btnY, buttonWidth, buttonHeight), "Cancel (ESC)", buttonStyle))
                {
                    pm.CancelPlacement();
                }
            }

            // Show current mode and rotation info
            string modeText = $"Mode: {pm.CurrentMode}";
            if (pm.CurrentMode == PlacementMode.Build)
            {
                modeText += $" | Type: {pm.SelectedMachineType} | Rotation: {pm.CurrentRotation}° (R to rotate)";
            }

            GUI.Label(
                new Rect(x, y - 25f, menuWidth, 20f),
                modeText,
                labelStyle
            );
        }
    }
}
