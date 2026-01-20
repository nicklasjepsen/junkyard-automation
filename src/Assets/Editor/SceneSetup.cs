using UnityEngine;
using UnityEditor;
using JunkyardAutomation.Core;
using JunkyardAutomation.UI;

namespace JunkyardAutomation.Editor
{
    /// <summary>
    /// Editor utility to set up the game scene with all required GameObjects.
    /// </summary>
    public static class SceneSetup
    {
        [MenuItem("Junkyard/Setup Game Scene")]
        public static void SetupGameScene()
        {
            // 1. Set up Main Camera
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
                camObj.tag = "MainCamera";
            }

            mainCam.orthographic = true;
            mainCam.orthographicSize = 8f;
            mainCam.transform.position = new Vector3(0, 0, -10);
            mainCam.backgroundColor = new Color(0.2f, 0.2f, 0.25f);

            // Add CameraController if not present
            if (mainCam.GetComponent<CameraController>() == null)
            {
                mainCam.gameObject.AddComponent<CameraController>();
            }

            Debug.Log("[SceneSetup] Main Camera configured");

            // 2. Create GridSystem
            GameObject gridSystemObj = GameObject.Find("GridSystem");
            if (gridSystemObj == null)
            {
                gridSystemObj = new GameObject("GridSystem");
            }

            GridSystem gridSystem = gridSystemObj.GetComponent<GridSystem>();
            if (gridSystem == null)
            {
                gridSystem = gridSystemObj.AddComponent<GridSystem>();
            }

            Debug.Log("[SceneSetup] GridSystem created");

            // 3. Create Grid Renderer
            GameObject gridRendererObj = GameObject.Find("GridRenderer");
            if (gridRendererObj == null)
            {
                gridRendererObj = new GameObject("GridRenderer");
            }

            if (gridRendererObj.GetComponent<MeshFilter>() == null)
            {
                gridRendererObj.AddComponent<MeshFilter>();
            }
            if (gridRendererObj.GetComponent<MeshRenderer>() == null)
            {
                gridRendererObj.AddComponent<MeshRenderer>();
            }
            if (gridRendererObj.GetComponent<GridRenderer>() == null)
            {
                gridRendererObj.AddComponent<GridRenderer>();
            }

            Debug.Log("[SceneSetup] GridRenderer created");

            // 4. Create Tile Highlighter
            GameObject highlighterObj = GameObject.Find("TileHighlighter");
            if (highlighterObj == null)
            {
                highlighterObj = new GameObject("TileHighlighter");
            }

            if (highlighterObj.GetComponent<MeshFilter>() == null)
            {
                highlighterObj.AddComponent<MeshFilter>();
            }
            if (highlighterObj.GetComponent<MeshRenderer>() == null)
            {
                highlighterObj.AddComponent<MeshRenderer>();
            }
            if (highlighterObj.GetComponent<TileHighlighter>() == null)
            {
                highlighterObj.AddComponent<TileHighlighter>();
            }

            Debug.Log("[SceneSetup] TileHighlighter created");

            // 5. Create GameManager
            GameObject gameManagerObj = GameObject.Find("GameManager");
            if (gameManagerObj == null)
            {
                gameManagerObj = new GameObject("GameManager");
            }

            GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
            if (gameManager == null)
            {
                gameManager = gameManagerObj.AddComponent<GameManager>();
            }

            Debug.Log("[SceneSetup] GameManager created");

            // 6. Create Debug UI
            GameObject debugUIObj = GameObject.Find("DebugUI");
            if (debugUIObj == null)
            {
                debugUIObj = new GameObject("DebugUI");
            }

            if (debugUIObj.GetComponent<DebugOverlayUI>() == null)
            {
                debugUIObj.AddComponent<DebugOverlayUI>();
            }

            Debug.Log("[SceneSetup] DebugUI created");

            // Mark scene dirty so it can be saved
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );

            Debug.Log("[SceneSetup] Scene setup complete! Press Play to test.");
            Debug.Log("[SceneSetup] Controls: WASD/Arrows to pan, Mouse wheel to zoom, F3 for debug overlay");
        }
    }
}
