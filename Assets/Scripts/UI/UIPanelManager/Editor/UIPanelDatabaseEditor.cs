#if UNITY_EDITOR
using UnityEditor;

namespace Core.UI.Data
{
    [CustomEditor(typeof(UIPanelDatabase))]
    public class UIPanelDatabaseEditor : Editor
    {
        private UIPanelDatabase _db;

        private void OnEnable()
        {
            _db = (UIPanelDatabase)target;
        }

        public override void OnInspectorGUI()
        {
            if (_db.HasDuplicateIds())
            {
                EditorGUILayout.HelpBox(
                    "Invalid UIPanel Id detected (duplicate or null/empty).\n" +
                    "Each UIPanel Id must be unique and non-empty.",
                    MessageType.Warning
                );
            }

            DrawDefaultInspector();
        }
    }
}
#endif
