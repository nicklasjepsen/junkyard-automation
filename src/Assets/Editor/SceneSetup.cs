using UnityEngine;
using UnityEditor;
using JunkyardAutomation.Core;
using JunkyardAutomation.UI;
using JunkyardAutomation.Placement;
using JunkyardAutomation.Simulation;

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

            // 7. Create PlacementGhost
            GameObject ghostObj = GameObject.Find("PlacementGhost");
            if (ghostObj == null)
            {
                ghostObj = new GameObject("PlacementGhost");
            }

            if (ghostObj.GetComponent<MeshFilter>() == null)
            {
                ghostObj.AddComponent<MeshFilter>();
            }
            if (ghostObj.GetComponent<MeshRenderer>() == null)
            {
                ghostObj.AddComponent<MeshRenderer>();
            }
            PlacementGhost ghost = ghostObj.GetComponent<PlacementGhost>();
            if (ghost == null)
            {
                ghost = ghostObj.AddComponent<PlacementGhost>();
            }

            Debug.Log("[SceneSetup] PlacementGhost created");

            // 8. Create PlacementManager
            GameObject placementManagerObj = GameObject.Find("PlacementManager");
            if (placementManagerObj == null)
            {
                placementManagerObj = new GameObject("PlacementManager");
            }

            PlacementManager placementManager = placementManagerObj.GetComponent<PlacementManager>();
            if (placementManager == null)
            {
                placementManager = placementManagerObj.AddComponent<PlacementManager>();
            }

            // Wire up ghost reference via SerializedObject
            SerializedObject pmSO = new SerializedObject(placementManager);
            pmSO.FindProperty("ghost").objectReferenceValue = ghost;
            pmSO.ApplyModifiedProperties();

            Debug.Log("[SceneSetup] PlacementManager created");

            // 9. Create MachineVisualManager
            GameObject visualManagerObj = GameObject.Find("MachineVisualManager");
            if (visualManagerObj == null)
            {
                visualManagerObj = new GameObject("MachineVisualManager");
            }

            if (visualManagerObj.GetComponent<MachineVisualManager>() == null)
            {
                visualManagerObj.AddComponent<MachineVisualManager>();
            }

            Debug.Log("[SceneSetup] MachineVisualManager created");

            // 10. Create BuildMenuUI
            GameObject buildMenuObj = GameObject.Find("BuildMenuUI");
            if (buildMenuObj == null)
            {
                buildMenuObj = new GameObject("BuildMenuUI");
            }

            if (buildMenuObj.GetComponent<BuildMenuUI>() == null)
            {
                buildMenuObj.AddComponent<BuildMenuUI>();
            }

            Debug.Log("[SceneSetup] BuildMenuUI created");

            // 11. Create SimulationManager
            GameObject simManagerObj = GameObject.Find("SimulationManager");
            if (simManagerObj == null)
            {
                simManagerObj = new GameObject("SimulationManager");
            }

            if (simManagerObj.GetComponent<SimulationManager>() == null)
            {
                simManagerObj.AddComponent<SimulationManager>();
            }

            Debug.Log("[SceneSetup] SimulationManager created");

            // 12. Create ItemVisualManager
            GameObject itemVisualObj = GameObject.Find("ItemVisualManager");
            if (itemVisualObj == null)
            {
                itemVisualObj = new GameObject("ItemVisualManager");
            }

            if (itemVisualObj.GetComponent<ItemVisualManager>() == null)
            {
                itemVisualObj.AddComponent<ItemVisualManager>();
            }

            Debug.Log("[SceneSetup] ItemVisualManager created");

            // Mark scene dirty so it can be saved
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );

            Debug.Log("[SceneSetup] Scene setup complete! Press Play to test.");
            Debug.Log("[SceneSetup] Controls: WASD/Arrows to pan, Mouse wheel to zoom, F3 for debug overlay");
            Debug.Log("[SceneSetup] Build: Click Conveyor, R to rotate, Left-click to place, ESC to cancel");
            Debug.Log("[SceneSetup] Simulation: SPACE to spawn item, P to pause/resume");
        }
    }
}
