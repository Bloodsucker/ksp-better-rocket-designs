using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverUI : MonoBehaviour
    {
        public event Action<string> OnFilterChanged;
        public event Action<string> OnSaveButtonClicked;
        public event Action OnCancelButtonClicked;

        private RocketDesign _newRocketDesign;
        private List<RocketDesign> filteredRocketDesigns;
        private bool saveButtonAsReplaceButton;

        private int _windowId;
        private string filterTextInputText;
        private Rect _windowPosition;
        private Vector2 filteredRocketDesignScrollPosition;
        private Vector2 newRocketDesignNewLabelsScrollPosition;
        private string newRocketDesignNewLabelInputText;

        private GUIStyle rocketDesignSaverWindowStyle;
        private GUIStyle filterTextInputStyle;
        private GUIStyle filteredRocketDesignScrollViewStyle;
        private GUIStyle filteredRocketDesignButtonStyle;
        private GUIStyle saveOrReplaceButtonStyle;
        private GUIStyle cancelButtonStyle;
        private GUIStyle newRocketDesignNewLabelsScrollViewStyle;
        private GUIStyle newRocketDesignNewLabelRemoveButtonStyle;
        private GUIStyle newRocketDesignNewLabelTextFieldStyle;
        private GUIStyle newRocketDesignNewLabelAddButtonStyle;

        public void Init(RocketDesign newRocketDesign)
        {
            _newRocketDesign = newRocketDesign;
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition = new Rect();
            saveButtonAsReplaceButton = false;

            InitStyle();
        }

        public void UpdateFilteredRocketDesigns(List<RocketDesign> rocketDesigns)
        {
            this.filteredRocketDesigns = rocketDesigns;
        }

        private void HandleFilterTextInputChange(string newFilterText)
        {
            saveButtonAsReplaceButton = false;

            foreach (RocketDesign rocketDesign in this.filteredRocketDesigns)
            {
                if (rocketDesign.Name == newFilterText)
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

            #region top-section
            filterTextInputText = GUILayout.TextField(filterTextInputText, filterTextInputStyle);

            if (GUI.changed)
            {
                HandleFilterTextInputChange(filterTextInputText);
            }
            #endregion

            #region middle-section

            #region left-column
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(_windowPosition.width / 3));

            OnWindowFilterLabelsColumn();

            GUILayout.EndVertical();
            #endregion

            #region middle-column
            GUILayout.BeginVertical(GUILayout.Height(150f), GUILayout.ExpandWidth(true));

            OnWindowDrawFilteredRocketDesignsColumn();

            GUILayout.EndVertical();
            #endregion

            #region right-column
            GUILayout.BeginVertical(GUILayout.Width(_windowPosition.width / 3));

            OnWindowDrawNewLabelsColumn();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            #endregion
            #endregion

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

        private void OnWindowFilterLabelsColumn()
        {
            GUILayout.Label("Column 1");
        }

        private void OnWindowDrawFilteredRocketDesignsColumn()
        {
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
        }

        private void OnWindowDrawNewLabelsColumn()
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            if (_newRocketDesign.Labels.Count == 0)
            {
                GUILayout.Label("No labels");
            }
            else
            {
                newRocketDesignNewLabelsScrollPosition = GUILayout.BeginScrollView(newRocketDesignNewLabelsScrollPosition, newRocketDesignNewLabelsScrollViewStyle);

                foreach (var labelName in _newRocketDesign.Labels)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("x", newRocketDesignNewLabelRemoveButtonStyle))
                    {
                        // TODO
                        _newRocketDesign.Labels = _newRocketDesign.Labels.Where(label => label != labelName).ToList();
                    }

                    GUILayout.Label(labelName);

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();

            GUILayout.Label("Attach new label:");
            GUILayout.BeginHorizontal();
            newRocketDesignNewLabelInputText = GUILayout.TextField(newRocketDesignNewLabelInputText, newRocketDesignNewLabelTextFieldStyle);

            if (GUILayout.Button("Add", newRocketDesignNewLabelAddButtonStyle))
            {
                // TODO
                _newRocketDesign.Labels = _newRocketDesign.Labels.Concat(new[] { newRocketDesignNewLabelInputText }).ToList();

                newRocketDesignNewLabelInputText = "";
            }
            GUILayout.EndHorizontal();
        }


        private void InitStyle()
        {
            rocketDesignSaverWindowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                fixedWidth = 600,
                stretchHeight = true
            };

            filterTextInputStyle = new GUIStyle(HighLogic.Skin.textField);

            filteredRocketDesignScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView)
            {
                stretchWidth = true,
            };
            filteredRocketDesignButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
                fixedHeight = 100,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            newRocketDesignNewLabelsScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            newRocketDesignNewLabelRemoveButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                fixedWidth = 20,
            };
            newRocketDesignNewLabelTextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                fixedWidth = 150,
            };
            newRocketDesignNewLabelAddButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
            };

            saveOrReplaceButtonStyle = new GUIStyle(HighLogic.Skin.button);
            saveOrReplaceButtonStyle.fixedWidth = 100;
            cancelButtonStyle = new GUIStyle(HighLogic.Skin.button);
            cancelButtonStyle.fixedWidth = 100;
        }
    }
}
