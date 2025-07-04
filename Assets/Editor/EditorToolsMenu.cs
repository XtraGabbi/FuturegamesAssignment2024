using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Mechadroids.Editor {
    public static class EditorToolsMenu {
        static readonly string[] scenesToLoadInOrder = new[] {
            "Boot",
            "Entities",
            "Level"
        };

        private static bool showRouteHandles = true;

        [MenuItem("Mechadroids/Load Scenes")]
        private static void LoadScenes() {
            string path = "Assets/Scenes";
            foreach(string scene in scenesToLoadInOrder) {
                EditorSceneManager.OpenScene($"{path}/{scene}.unity", OpenSceneMode.Additive);
            }
        }

        [MenuItem("Mechadroids/Toggle Route Handles %h")] // Ctrl+H on Windows
        private static void ToggleRouteHandles() {
            showRouteHandles = !showRouteHandles;
            Debug.Log($"Route Handles {(showRouteHandles ? "Enabled" : "Disabled")}");

            // Find and update all Route assets in the project
            string[] guids = AssetDatabase.FindAssets("t:Route");
            foreach(string guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Route route = AssetDatabase.LoadAssetAtPath<Route>(path);
                if(route != null) {
                    route.showHandles = showRouteHandles;
                    EditorUtility.SetDirty(route);
                }
            }

            SceneView.RepaintAll();
        }
    }
}
