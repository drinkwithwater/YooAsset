#if UNITY_2020_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YooAsset.Editor
{
    /// <summary>
    /// 分屏控件
    /// </summary>
    public class SplitView : TwoPaneSplitView
    {
#if UNITY_6000_0_OR_NEWER 
        public new class UxmlFactory : UxmlElementAttribute
        {
        }
#else
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits>
        {
        } 
#endif

        /// <summary>
        /// 窗口分屏适配
        /// </summary>
        public static void Adjuster(VisualElement root)
        {
            var topGroup = root.Q<VisualElement>("TopGroup");
            var bottomGroup = root.Q<VisualElement>("BottomGroup");
            topGroup.style.minHeight = 100f;
            bottomGroup.style.minHeight = 100f;
            root.Remove(topGroup);
            root.Remove(bottomGroup);
            var spliteView = new SplitView();
            spliteView.fixedPaneInitialDimension = 300;
            spliteView.orientation = TwoPaneSplitViewOrientation.Vertical;
            spliteView.contentContainer.Add(topGroup);
            spliteView.contentContainer.Add(bottomGroup);
            root.Add(spliteView);
        }
    }

    /// <summary>
    /// 显示开关（眼睛图标）
    /// </summary>
    public class DisplayToggle : Toggle
    {
        private readonly VisualElement _checkbox;

        public DisplayToggle()
        {
            _checkbox = this.Q<VisualElement>("unity-checkmark");
            RefreshIcon();
        }

        /// <summary>
        /// 刷新图标
        /// </summary>
        public void RefreshIcon()
        {
            if (this.value)
            {
                var icon = EditorGUIUtility.IconContent("animationvisibilitytoggleoff@2x").image as Texture2D;
                _checkbox.style.backgroundImage = icon;
            }
            else
            {
                var icon = EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x").image as Texture2D;
                _checkbox.style.backgroundImage = icon;
            }
        }
    }
}
#endif