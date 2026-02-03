using System;
using System.Collections.Generic;

using Core.UI.Data;

using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace Core.UI
{
    /// <summary>
    /// Manager that handles all UI panels (push, pop, create, destroy)
    /// </summary>
    public static class UIPanelManager
    {
        private static UIPanelDatabase s_uIPanelDatabase;

        /// <summary>
        /// Default panel size (reference resolution for UI scaling).
        /// </summary>
        private static Vector2 s_panelSize = new Vector2(1920, 1080);

        private static RectTransform s_panelsRoot;
        private static Camera s_uICamera;

        private static Stack<BaseUIPanel> s_panelStack = new Stack<BaseUIPanel>();
        public static int PanelCount => s_panelStack.Count;

        public static void InitEntryPoint(UIPanelDatabase UIPanelDatabase, Camera uIcamera = null)
        {
            s_uIPanelDatabase = UIPanelDatabase;
            s_uICamera = uIcamera;

            s_uIPanelDatabase.Build();
            GetUIPanelsRoot();
        }

        /// <summary>
        /// Push a panel by type
        /// </summary>
        public static IUIPanel PushUIPanel<TUIPanel>(object param = null) where TUIPanel : class, IUIPanel
        {
            if (!TryGetUIPanelNameFromType(typeof(TUIPanel), out string screenName))
            {
#if UNITY_EDITOR
                Debug.LogError($"Cannot find UIPanel for type '{typeof(TUIPanel).Name}'.");
#endif
                return null;
            }

            return PushUIPanel(screenName, param) as TUIPanel;
        }

        private static IUIPanel PushUIPanel(string screenName, object param = null)
        {
            var panel = GetPanel(screenName);
            if (panel == null) return null;

            // Optionally keep the previous panel visible
            bool dontHidePrevious = param is NavigationParameters p && p.DontHidePreviousScreen;
            if (!dontHidePrevious && PanelCount > 0)
            {
                var top = s_panelStack.Peek();
                top.OnHide();
            }

            // Display new panel and add to stack
            panel.OnDisplay(param);
            s_panelStack.Push(panel as BaseUIPanel);
            return panel;
        }

        /// <summary>
        /// Pop (remove) the top panel, or a specific one if provided.
        /// </summary>
        public static bool PopPanel(BaseUIPanel screen = null)
        {
            if (PanelCount <= 0) return false;

            // If a panel is specified, only pop if it matches the top
            var top = s_panelStack.Peek();
            if (screen != null && screen != top)
            {
                return false;
            }

            // Destroy Top Panel
            var current = s_panelStack.Pop();
            current.OnDelete();

            // Resume previous panel if exists
            if (PanelCount > 0)
            {
                var prev = s_panelStack.Peek();
                prev.OnResume();
            }

            return true;
        }

        /// <summary>
        /// Create a new panel instance from prefab
        /// </summary>
        private static BaseUIPanel GetPanel(string panelID)
        {
            if (string.IsNullOrEmpty(panelID)) return null;

            if (!s_uIPanelDatabase.TryGetPrefab(panelID, out BaseUIPanel prefab) || prefab == null)
            {
                return null;
            }

            // Instantiate under cache root
            GameObject screenGameObject = Object.Instantiate(prefab.gameObject, GetUIPanelsRoot());
            screenGameObject.name = panelID;

            BaseUIPanel panel = screenGameObject.GetComponent<BaseUIPanel>();
            panel.OnCreated();
            return panel;
        }

        private static RectTransform GetUIPanelsRoot()
        {
            if (s_panelsRoot)
            {
                return s_panelsRoot;
            }

            // Create root GameObject
            GameObject rootObject = new GameObject("UIPanels",
                                typeof(RectTransform),
                                typeof(Canvas),
                                typeof(CanvasScaler),
                                typeof(GraphicRaycaster));
            rootObject.layer = LayerMask.NameToLayer("UI");

            // Setup Canvas Scaler
            CanvasScaler scaler = rootObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = s_panelSize;

            // Setup RectTransform
            if (!rootObject.TryGetComponent(out RectTransform transform))
            {
                transform = rootObject.AddComponent<RectTransform>();
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;
                transform.pivot = new Vector2(0.5f, 0.5f);
            }

            // Setup Canvas
            if (!rootObject.TryGetComponent(out Canvas canvas))
            {
                canvas = rootObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Assign camera
            if (s_uICamera != null)
            {
                canvas.worldCamera = s_uICamera;
            }
            else
            {
                canvas.worldCamera = Camera.main;
            }

            Object.DontDestroyOnLoad(rootObject);

            s_panelsRoot = transform;
            return s_panelsRoot;
        }

        private static bool TryGetUIPanelNameFromType(Type type, out string screenName)
        {
            string name = type.Name;
            if (s_uIPanelDatabase.GetUIPanelName(name))
            {
                screenName = name;
                return true;
            }

            // Support removing "Panel"
            if (name.EndsWith("Panel"))
            {
                name = name.Substring(0, name.Length - 5);
                if (s_uIPanelDatabase.GetUIPanelName(name))
                {
                    screenName = name;
                    return true;
                }
            }

            screenName = null;
            return false;
        }
    }
}