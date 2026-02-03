using System.Collections.Generic;

using UnityEngine;

namespace Core.UI.Data
{
    [System.Serializable]
    public class UIPanelData
    {
        public string Id;
        public BaseUIPanel Prefab;
    }

    [CreateAssetMenu(fileName = "UIPanelDatabase", menuName = "UI/Database", order = 1)]
    public class UIPanelDatabase : ScriptableObject
    {
        [SerializeField] private List<UIPanelData> _allPanels = new List<UIPanelData>();
        private Dictionary<string, BaseUIPanel> _dict;

        public void Build()
        {
            _dict = new Dictionary<string, BaseUIPanel>();
            foreach (var item in _allPanels)
            {
                if (item == null || string.IsNullOrEmpty(item.Id) || item.Prefab == null)
                    continue;

                if (_dict.ContainsKey(item.Id))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[UIPanelDatabase] Duplicate Id: {item.Id}");
#endif
                    continue;
                }
                _dict[item.Id] = item.Prefab;
            }
        }

        public bool TryGetPrefab(string id, out BaseUIPanel prefab)
        {
            if (_dict == null || _dict.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[UIPanelDatabase] Prefab not found for id='{id}'.");
#endif
                prefab = null;
                return false;
            }

            return _dict.TryGetValue(id, out prefab);
        }

        public bool GetUIPanelName(string id)
        {
            return _dict.ContainsKey(id);
        }

        /// <summary>
        /// Check if there are invalid or duplicate UIPanel Ids
        /// </summary>
        /// <returns>True if invalid state exists</returns>
        public bool HasDuplicateIds()
        {
            if (_allPanels == null || _allPanels.Count == 0)
            {
                return false;
            }

            HashSet<string> seen = new HashSet<string>();

            foreach (var item in _allPanels)
            {
                if (item == null)
                    continue;

                if (string.IsNullOrEmpty(item.Id) || !seen.Add(item.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
