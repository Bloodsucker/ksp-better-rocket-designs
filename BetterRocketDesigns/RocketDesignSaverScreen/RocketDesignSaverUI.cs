using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverUI : MonoBehaviour
    {
        public event Action<string> OnFilterChanged;
        public event Action<string> OnSaveButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action<string, float> OnAddNewCapabilityButtonClicked;
        public event Action<string> OnNewCapabilityRemoveButtonClicked;
        public event Action<string> OnAddNewLabelButtonClicked;
        public event Action<string> OnRemoveNewLabelButtonClicked;

        private RocketDesign _newRocketDesign;
        private List<RocketDesign> filteredRocketDesigns;
        private bool saveButtonAsReplaceButton;
        private readonly string[] _newRocketDesignToolbarOptions = { "Labels", "Capabilities"};

        private int _windowId;
        private string filterTextInputText = "";
        private Rect _windowPosition = new Rect(0, 0, 600, 300);
        private Vector2 filteredRocketDesignScrollPosition;
        private Vector2 newRocketDesignNewLabelsScrollPosition;
        private string newRocketDesignNewLabelInputText = "";
        private int _newRocketDesignToolbarSelection = 0;
        private string newRocketDesignNewCapabilityNameInputText = "";
        private string newRocketDesignNewCapabilityValueInputText = "";
        private Vector2 newCapabilityScrollPosition;

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
        private GUIStyle _newRocketDesignToolbarStyle;
        private GUIStyle newRocketDesignNewCapabilityNameTextFieldStyle;
        private GUIStyle newRocketDesignNewCapabilityValueTextFieldStyle;
        private GUIStyle newRocketDesignNewCapabilityAddButtonStyle;
        private GUIStyle newCapabilityScrollViewStyle;
        private GUIStyle newCapabilityRemoveButtonStyle;

        public void Init(RocketDesign newRocketDesign)
        {
            _newRocketDesign = newRocketDesign;
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition.x = (Screen.width - _windowPosition.width) / 2;
            _windowPosition.y = (Screen.height - _windowPosition.height) / 2;
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
            GUILayout.BeginHorizontal();
            filterTextInputText = GUILayout.TextField(filterTextInputText, filterTextInputStyle);

            if (GUI.changed)
            {
                HandleFilterTextInputChange(filterTextInputText);
            }

            GUI.enabled = filterTextInputText.Length != 0;

            if (GUILayout.Button(saveButtonAsReplaceButton ? "Replace" : "Save", saveOrReplaceButtonStyle))
            {
                HandleSaveButtonClick();
            }

            GUI.enabled = true;

            if (GUILayout.Button("Cancel", cancelButtonStyle))
            {
                HandleCancelButtonClick();
            }
            GUILayout.EndHorizontal();
            #endregion

            #region middle-section
            GUILayout.BeginHorizontal();

            #region left-column
            GUILayout.BeginVertical(GUILayout.Width(200));
            OnWindowFilterLabelsColumn();
            GUILayout.EndVertical();
            #endregion

            #region middle-column
            OnWindowDrawFilteredRocketDesignsColumn();
            #endregion

            #region right-column
            GUILayout.BeginVertical(GUILayout.Width(200));
            OnWindowDrawNewLabelsColumn();
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
            #endregion

            GUI.DragWindow();
        }

        private void OnWindowFilterLabelsColumn()
        {
            GUILayout.Label("Column 1");
        }

        private void OnWindowDrawFilteredRocketDesignsColumn()
        {
            filteredRocketDesignScrollPosition = GUILayout.BeginScrollView(filteredRocketDesignScrollPosition, filteredRocketDesignScrollViewStyle, GUILayout.Width(200), GUILayout.Height(300));

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

            _newRocketDesignToolbarSelection = GUILayout.Toolbar(_newRocketDesignToolbarSelection, _newRocketDesignToolbarOptions, _newRocketDesignToolbarStyle);
            if (_newRocketDesignToolbarSelection == 0)
            {
                OnWindowDrawNewLabelBlock();
            }
            else
            {
                OnWindowDrawNewCapabilityBlock();
            }

            GUILayout.EndVertical();
        }

        private void OnWindowDrawNewLabelBlock()
        {
            GUILayout.Label("Add new Label:");
            GUILayout.BeginHorizontal();
            newRocketDesignNewLabelInputText = GUILayout.TextField(newRocketDesignNewLabelInputText, newRocketDesignNewLabelTextFieldStyle);

            GUI.enabled = newRocketDesignNewLabelInputText.Length != 0;
            if (GUILayout.Button("+", newRocketDesignNewLabelAddButtonStyle))
            {
                OnAddNewLabelButtonClicked.Invoke(newRocketDesignNewLabelInputText);

                newRocketDesignNewLabelInputText = "";
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // TODO Search "global" or "found" Labels -box.

            if (_newRocketDesign.Labels.Count == 0)
            {
                GUILayout.Label("No assigned Labels");
            }
            else
            {
                newRocketDesignNewLabelsScrollPosition = GUILayout.BeginScrollView(newRocketDesignNewLabelsScrollPosition, newRocketDesignNewLabelsScrollViewStyle);

                foreach (var labelName in _newRocketDesign.Labels)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("-", newRocketDesignNewLabelRemoveButtonStyle))
                    {
                        OnRemoveNewLabelButtonClicked.Invoke(labelName);
                    }

                    GUILayout.Label(labelName);

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }

        private void OnWindowDrawNewCapabilityBlock()
        {
            GUILayout.Label("Add new payload Capability:");
            GUILayout.BeginHorizontal();
            newRocketDesignNewCapabilityNameInputText = GUILayout.TextField(newRocketDesignNewCapabilityNameInputText, newRocketDesignNewCapabilityNameTextFieldStyle, GUILayout.ExpandWidth(true));
            newRocketDesignNewCapabilityValueInputText = GUILayout.TextField(newRocketDesignNewCapabilityValueInputText, newRocketDesignNewCapabilityValueTextFieldStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("t", GUILayout.ExpandWidth(false));

            bool isNewCapabilityValueFloatParseSuccessful = float.TryParse(newRocketDesignNewCapabilityValueInputText, out float newCapabilityValue);

            GUI.enabled = newRocketDesignNewCapabilityNameInputText.Length != 0 && isNewCapabilityValueFloatParseSuccessful;
            if (GUILayout.Button("+", newRocketDesignNewCapabilityAddButtonStyle) && isNewCapabilityValueFloatParseSuccessful)
            {
                OnAddNewCapabilityButtonClicked.Invoke(newRocketDesignNewCapabilityNameInputText, newCapabilityValue);

                newRocketDesignNewCapabilityNameInputText = "";
                newRocketDesignNewCapabilityValueInputText = "";
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // TODO Search "global" or "found" Capabilities -box.

            if (_newRocketDesign.Capabilities.Count == 0)
            {
                GUILayout.Label("No assigned Capabilities");
            }
            else
            {
                newCapabilityScrollPosition = GUILayout.BeginScrollView(newCapabilityScrollPosition, newCapabilityScrollViewStyle);

                foreach (var kvp in _newRocketDesign.Capabilities)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("-", newCapabilityRemoveButtonStyle))
                    {
                        OnNewCapabilityRemoveButtonClicked.Invoke(kvp.Key);
                    }

                    GUILayout.Label($"{kvp.Key}: {kvp.Value} t");

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }


        private void InitStyle()
        {
            rocketDesignSaverWindowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                stretchHeight = true
            };

            filterTextInputStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                fixedHeight = HighLogic.Skin.button.CalcHeight(new GUIContent(), 20),
                alignment = TextAnchor.MiddleLeft,
            };

            saveOrReplaceButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                fixedWidth = 100,
            };
            cancelButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                fixedWidth = 100,
            };

            filteredRocketDesignScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);

            filteredRocketDesignButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
                fixedHeight = 100,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            _newRocketDesignToolbarStyle = new GUIStyle(HighLogic.Skin.button);


            newRocketDesignNewLabelTextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                stretchWidth = true,
                fixedHeight = HighLogic.Skin.button.CalcHeight(new GUIContent(), 20),
                alignment = TextAnchor.MiddleLeft,
            };
            newRocketDesignNewLabelAddButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = false,
                fixedWidth = 30
            };
            newRocketDesignNewLabelsScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            newRocketDesignNewLabelRemoveButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = false,
                fixedWidth = 30
            };

            newRocketDesignNewCapabilityNameTextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                stretchWidth = true,
                fixedHeight = HighLogic.Skin.button.CalcHeight(new GUIContent(), 20),
                alignment = TextAnchor.MiddleLeft,
            };
            newRocketDesignNewCapabilityValueTextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                stretchWidth = true,
                fixedHeight = HighLogic.Skin.button.CalcHeight(new GUIContent(), 20),
                alignment = TextAnchor.MiddleLeft,
            };
            newRocketDesignNewCapabilityAddButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = false,
                fixedWidth = 30
            };
            newCapabilityScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            newCapabilityRemoveButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = false,
                fixedWidth = 30
            };
        }
    }
}
