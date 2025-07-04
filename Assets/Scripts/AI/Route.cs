using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mechadroids {
    // Route points for the AI
    [CreateAssetMenu(menuName = "Mechadroids/RouteSettings", fileName = "Route", order = 0)]
    public class Route : ScriptableObject {
        public int routeId;
        public Vector3[] routePoints;
        [HideInInspector] public bool showHandles = true;


        private void OnEnable() {

            SceneView.duringSceneGui += OnSceneUpdate;

        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnSceneUpdate;

        }

        private void OnSceneUpdate(SceneView sceneView) {
            if(!showHandles || routePoints == null || routePoints.Length == 0) return;

            Handles.Label(routePoints[0], "Start");
            for(int i = 1; i < routePoints.Length; i++) {
                Handles.DrawLine(routePoints[i - 1], routePoints[i]);
            }
            Handles.Label(routePoints[^1], "End");
        }

    }




}
