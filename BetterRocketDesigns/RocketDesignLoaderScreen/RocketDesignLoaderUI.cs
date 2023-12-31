﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignLoaderScreen
{
    internal class RocketDesignLoaderUI: MonoBehaviour
    {
        private RocketDesign _selectedRocketDesign;

        public event Action<string> OnFilterChanged;
        public event Action OnCancelButtonClicked;
        public event Action<RocketDesign> OnLoadButtonClicked;

        private List<RocketDesign> filteredRocketDesigns;

        private int _windowId;
        private string filterTextInputText;
        private Rect _windowPosition;
        private Vector2 filteredRocketDesignScrollPosition;

        private GUIStyle rocketDesignSaverWindowStyle;
        private GUIStyle filterTextInputStyle;
        private GUIStyle filteredRocketDesignScrollViewStyle;
        private GUIStyle filteredRocketDesignButtonStyle;
        private GUIStyle loadButtonStyle;
        private GUIStyle cancelButtonStyle;

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition = new Rect();

            InitStyle();
        }

        public void UpdateFilteredRocketDesigns(List<RocketDesign> rocketDesigns)
        {
            this.filteredRocketDesigns = rocketDesigns;
        }

        private void HandleFilterTextInputChange(string newFilterText)
        {
            _selectedRocketDesign = null;

            OnFilterChanged.Invoke(newFilterText);
        }

        private void HandleLoadButtonClick()
        {
            if (_selectedRocketDesign == null) return;

            OnLoadButtonClicked.Invoke(_selectedRocketDesign);
        }

        private void HandleCancelButtonClick()
        {
            OnCancelButtonClicked.Invoke();
        }

        private void HandleExistingRocketdesignButtonClick(int index, RocketDesign rocketDesign)
        {
            _selectedRocketDesign = rocketDesign;
        }

        private void OnGUI()
        {
            _windowPosition = GUILayout.Window(_windowId, _windowPosition, OnWindow, "Load Rocket Design as", rocketDesignSaverWindowStyle);
        }

        private void OnWindow(int windowId)
        {
            filterTextInputText = GUILayout.TextField(filterTextInputText, filterTextInputStyle);

            if (GUI.changed)
            {
                HandleFilterTextInputChange(filterTextInputText);
            }

            GUILayout.BeginVertical(GUILayout.Height(150f));

            filteredRocketDesignScrollPosition = GUILayout.BeginScrollView(filteredRocketDesignScrollPosition, filteredRocketDesignScrollViewStyle);

            for (int i = 0; i < this.filteredRocketDesigns.Count; i++)
            {
                RocketDesign rocketDesign = this.filteredRocketDesigns[i];

                string buttonText = $"<b>{rocketDesign.Name}</b>";

                string labelsButtonText = "";

                foreach(var label in rocketDesign.Labels)
                {
                    labelsButtonText += $"{label} - ";
                }

                if (rocketDesign.Labels.Count > 0)
                {
                    buttonText += $"\n{labelsButtonText}";
                }

                string capabilitiesButtonText = "";

                foreach (var kvp in rocketDesign.Capabilities)
                {
                    capabilitiesButtonText += $"{kvp.Key}: {kvp.Value}; ";
                }

                if (rocketDesign.Capabilities.Count > 0)
                {
                    buttonText += $"\nCap: {capabilitiesButtonText}";
                }

                GUIContent buttonContent = new GUIContent
                {
                    image = rocketDesign.ThumbnailImage,
                    text = buttonText,
                    tooltip = $"Load: {rocketDesign.Name}",
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

            if (GUILayout.Button("load", loadButtonStyle))
            {
                HandleLoadButtonClick();
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

            loadButtonStyle = new GUIStyle(HighLogic.Skin.button);
            loadButtonStyle.fixedWidth = 100;
            cancelButtonStyle = new GUIStyle(HighLogic.Skin.button);
            cancelButtonStyle.fixedWidth = 100;
        }
    }
}
