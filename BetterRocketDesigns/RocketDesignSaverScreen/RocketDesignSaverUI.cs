using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverUI : MonoBehaviour
    {
        public event Action<string> OnFilterChanged;
        public event Action<string> OnSaveButtonClicked;
        public event Action OnCancelButtonClicked;

        private List<RocketDesign> filteredRocketDesigns;
        private bool saveButtonAsReplaceButton;

        private int _windowId;
        private string filterTextInputText;
        private Rect _windowPosition;
        private Vector2 filteredRocketDesignScrollPosition;

        private GUIStyle rocketDesignSaverWindowStyle;
        private GUIStyle filterTextInputStyle;
        private GUIStyle filteredRocketDesignScrollViewStyle;
        private GUIStyle filteredRocketDesignButtonStyle;
        private GUIStyle saveOrReplaceButtonStyle;
        private GUIStyle cancelButtonStyle;

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition = new Rect();
            saveButtonAsReplaceButton = false;

            InitStyle();
        }

        public void UpdateFilteredRocketDesigns(List<RocketDesign> rocketDesigns) {
            this.filteredRocketDesigns = rocketDesigns;
        }

        private void HandleFilterTextInputChange(string newFilterText)
        {
            saveButtonAsReplaceButton = false;

            foreach (RocketDesign rocketDesign in this.filteredRocketDesigns)
            {
                if(rocketDesign.Name == newFilterText)
                {
                    saveButtonAsReplaceButton = true;
                    break;
                }
            }

            OnFilterChanged.Invoke(newFilterText);
        }

        private void HandleSaveButtonClick()
        {
            OnSaveButtonClicked.Invoke(filterTextInputText);
        }

        private void HandleCancelButtonClick()
        {
            OnCancelButtonClicked.Invoke();
        }

        private void HandleExistingRocketdesignButtonClick(int index, RocketDesign rocketDesign)
        {
            filterTextInputText = rocketDesign.Name;
            saveButtonAsReplaceButton = true;

            HandleFilterTextInputChange(filterTextInputText);
        }

        private void OnGUI()
        {
            _windowPosition = GUILayout.Window(_windowId, _windowPosition, OnWindow, "Save Rocket Design as", rocketDesignSaverWindowStyle);
        }

        private void OnWindow(int windowId)
        {
            filterTextInputText = GUILayout.TextField(filterTextInputText, filterTextInputStyle);

            if(GUI.changed)
            {
                HandleFilterTextInputChange(filterTextInputText);
            }

            GUILayout.BeginVertical(GUILayout.Height(150f));

            filteredRocketDesignScrollPosition = GUILayout.BeginScrollView(filteredRocketDesignScrollPosition, filteredRocketDesignScrollViewStyle);

            for (int i = 0; i < this.filteredRocketDesigns.Count; i++)
            {
                RocketDesign rocketDesign = this.filteredRocketDesigns[i];

                GUIContent buttonContent = new GUIContent
                {
                    image = rocketDesign.ThumbnailImage,
                    text = rocketDesign.Name,
                    tooltip = $"Select: {rocketDesign.Name}",
                };

                if (GUILayout.Button(buttonContent, filteredRocketDesignButtonStyle, GUILayout.ExpandWidth(true)))
                {
                    HandleExistingRocketdesignButtonClick(i, rocketDesign);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(saveButtonAsReplaceButton ? "replace" : "save", saveOrReplaceButtonStyle))
            {
                HandleSaveButtonClick();
            }

            if (GUILayout.Button("Cancel", cancelButtonStyle))
            {
                HandleCancelButtonClick();
            }

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void InitStyle()
        {
            rocketDesignSaverWindowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                fixedWidth = 300,
                stretchHeight = true
            };

            filterTextInputStyle = new GUIStyle(HighLogic.Skin.textField);

            filteredRocketDesignScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            filteredRocketDesignButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
                fixedHeight = 100,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            saveOrReplaceButtonStyle = new GUIStyle(HighLogic.Skin.button);
            saveOrReplaceButtonStyle.fixedWidth = 100;
            cancelButtonStyle = new GUIStyle(HighLogic.Skin.button);
            cancelButtonStyle.fixedWidth = 100;
        }
    }
}
