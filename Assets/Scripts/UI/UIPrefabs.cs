 using UnityEngine;

namespace Mechadroids.UI {

    public enum UIElementType {
        Single = 0,
        Multiple = 1
    }

    [CreateAssetMenu(menuName = "Mechadroids/UIPrefabs", fileName = "UIPrefabs", order = 0)]
    public class UIPrefabs : ScriptableObject {
        public DebugMenuReference debugMenuReferencePrefab;

        [Tooltip("UIElementReference prefabs for each UIElementType")]
        [SerializeField] private UIElementReference [] uiElementReferencePrefabs;

        // add menu ui prefabs here as well

        public UIElementReference GetUIElementReference(UIElementType type) {
            return uiElementReferencePrefabs[(int)type];
        }
    }
}
