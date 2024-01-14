using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverUI : MonoBehaviour
    {
        public event Action<string> OnTextFilterChanged;
        public event Action<string> OnSaveButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action<string, bool> OnFilterLabelToggled;
        public event Action<string, bool> OnFilterCapabilityToggled;
        public event Action<RocketDesign> OnFilteredRocketDesignButtonClick;
        public event Action<string, float> OnAddNewCapabilityButtonClicked;
        public event Action<string> OnNewCapabilityRemoveButtonClicked;
        public event Action<string> OnAddNewLabelButtonClicked;
        public event Action<string> OnRemoveNewLabelButtonClicked;


        private RocketDesign _newRocketDesign;
        private List<RocketDesign> filteredRocketDesigns;
        private IReadOnlyCollection<string> _cachedFilterLabels;
        private IReadOnlyCollection<string> _cachedFilterCapabilities;
        private bool saveButtonAsReplaceButton;
        private List<GUIContent> fileteredRocketDesignsContentGrid = new List<GUIContent>();
        private readonly string[] _newRocketDesignToolbarOptions = { "Labels", "Capabilities"};

        private int _windowId;
        private Rect _windowPosition = new Rect(0, 0, 600, 300);
        private string filterTextInputText = "";
        private Dictionary<string, bool> _filterLabelsSelection;
        private Dictionary<string, bool> _filterCapabilitiesSelection;
        private Vector2 filterLabelsScrollPosition;
        private Vector2 filterCapabilitiesScrollPosition;
        private Vector2 filteredRocketDesignScrollPosition;
        private int filteredRocketDesignsSelectedGrid = -1;
        private Vector2 newRocketDesignNewLabelsScrollPosition;
        private string newRocketDesignNewLabelInputText = "";
        private int _newRocketDesignToolbarSelection = 0;
        private string newRocketDesignNewCapabilityNameInputText = "";
        private string newRocketDesignNewCapabilityValueInputText = "";
        private Vector2 newCapabilityScrollPosition;

        private GUIStyle rocketDesignSaverWindowStyle;
        private GUIStyle filterTextInputStyle;
        private GUIStyle filteredRocketDesignHScrollViewStyle;
        private GUIStyle filteredRocketDesignVScrollViewStyle;
        private GUIStyle filteredRocketDesignButtonStyle;
        private GUIStyle saveOrReplaceButtonStyle;
        private GUIStyle cancelButtonStyle;
        private GUIStyle _filterLabelSelectionToggleStyle;
        private GUIStyle filterLabelsScrollViewStyle;
        private GUIStyle filterCapabilitiesScrollViewStyle;
        private GUIStyle _filterCapabilitySelectionToggleStyle;
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

        public void Init(RocketDesign newRocketDesign, IReadOnlyCollection<string> cachedLabels, IReadOnlyCollection<string> cachedCapabilities)
        {
            _newRocketDesign = newRocketDesign;
            _cachedFilterLabels = cachedLabels;
            _cachedFilterCapabilities = cachedCapabilities;

            _filterLabelsSelection = new Dictionary<string, bool>();
            foreach(var label in _cachedFilterLabels)
            {
                _filterLabelsSelection.Add(label, false);
            }

            _filterCapabilitiesSelection = new Dictionary<string, bool>();
            foreach(var capabilities in _cachedFilterCapabilities)
            {
                _filterCapabilitiesSelection.Add(capabilities, false);
            }
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition.x = (Screen.width - _windowPosition.width) / 2;
            _windowPosition.y = (Screen.height - _windowPosition.height) / 2;
            saveButtonAsReplaceButton = false;

            InitStyle();
        }

        public void UpdateFilter(List<RocketDesign> rocketDesigns)
        {
            filteredRocketDesigns = rocketDesigns;

            fileteredRocketDesignsContentGrid = new List<GUIContent>();

            saveButtonAsReplaceButton = false;
            filteredRocketDesignsSelectedGrid = -1;

            for (int i = 0; i < this.filteredRocketDesigns.Count; i++)
            {
                RocketDesign rocketDesign = this.filteredRocketDesigns[i];

                string buttonText = $"<b>{rocketDesign.Name}</b>";

                if (rocketDesign.Labels.Count > 0)
                {
                    string labelsButtonText = string.Join(" - ", rocketDesign.Labels);
                    
                    string newLabelsButtonText = labelsButtonText.Substring(0, Math.Min(29, labelsButtonText.Length));
                    if (newLabelsButtonText.Length != labelsButtonText.Length) newLabelsButtonText += "…";
                    labelsButtonText = newLabelsButtonText;

                    buttonText += $"\n{labelsButtonText}";
                }

                if (rocketDesign.Capabilities.Count > 0)
                {
                    string capabilitiesButtonText = string.Join("; ", rocketDesign.Capabilities.Select(kvp => $"{kvp.Key}: {kvp.Value} t"));
                    
                    string newCapabilitiesButtonText = capabilitiesButtonText.Substring(0, Math.Min(26, capabilitiesButtonText.Length));
                    if (newCapabilitiesButtonText.Length != capabilitiesButtonText.Length) newCapabilitiesButtonText += "…";
                    capabilitiesButtonText = newCapabilitiesButtonText;

                    buttonText += $"\nCap: {capabilitiesButtonText}";
                }

                GUIContent buttonContent = new GUIContent
                {
                    text = buttonText,
                    tooltip = $"Select: {rocketDesign.Name}",
                };

                fileteredRocketDesignsContentGrid.Add(buttonContent);

                if (!saveButtonAsReplaceButton)
                {
                    saveButtonAsReplaceButton = rocketDesign.Name == filterTextInputText;
                    filteredRocketDesignsSelectedGrid = saveButtonAsReplaceButton ? i : -1;
                }
            }
        }

        private void HandleFilterTextInputChange(string newFilterText)
        {
            saveButtonAsReplaceButton = false;

            OnTextFilterChanged.Invoke(newFilterText);
        }

        private void HandleSaveButtonClick()
        {
            OnSaveButtonClicked.Invoke(filterTextInputText);
        }

        private void HandleCancelButtonClick()
        {
            OnCancelButtonClicked.Invoke();
        }

        private void HandleExistingRocketdesignButtonClick(RocketDesign rocketDesign)
        {
            OnFilteredRocketDesignButtonClick.Invoke(rocketDesign);
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

            #region body-section
            GUILayout.BeginHorizontal();

            #region left-column
            GUILayout.BeginVertical();
            OnWindowFilterLabelsColumn();
            GUILayout.EndVertical();
            #endregion

            #region middle-column
            GUILayout.BeginVertical();
            OnWindowDrawFilteredRocketDesignsColumn();
            GUILayout.EndVertical();
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
            GUILayout.Label("Label filter (OR)");

            filterLabelsScrollPosition = GUILayout.BeginScrollView(filterLabelsScrollPosition, filterLabelsScrollViewStyle, GUILayout.Width(200), GUILayout.Height(150));

            foreach(var filterLabel in _cachedFilterLabels)
            {
                _filterLabelsSelection.TryGetValue(filterLabel, out bool isSelected);
                isSelected = GUILayout.Toggle(isSelected, filterLabel, _filterLabelSelectionToggleStyle);

                if (GUI.changed)
                {
                    _filterLabelsSelection.Remove(filterLabel);
                    _filterLabelsSelection.Add(filterLabel, isSelected);

                    OnFilterLabelToggled.Invoke(filterLabel, isSelected);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.Label("Capability filter (OR)");

            filterCapabilitiesScrollPosition = GUILayout.BeginScrollView(filterCapabilitiesScrollPosition, filterCapabilitiesScrollViewStyle, GUILayout.Width(200), GUILayout.Height(150));

            foreach(var filterCapability in _cachedFilterCapabilities)
            {
                _filterCapabilitiesSelection.TryGetValue(filterCapability, out bool isSelected);
                isSelected = GUILayout.Toggle(isSelected, filterCapability, _filterCapabilitySelectionToggleStyle);

                if (GUI.changed)
                {
                    _filterCapabilitiesSelection.Remove(filterCapability);
                    _filterCapabilitiesSelection.Add(filterCapability, isSelected);

                    OnFilterCapabilityToggled.Invoke(filterCapability, isSelected);
                }
            }

            GUILayout.EndScrollView();
        }

        private void OnWindowDrawFilteredRocketDesignsColumn()
        {
            GUILayout.Label("Copy all Metadata from:");
            filteredRocketDesignScrollPosition = GUILayout.BeginScrollView(filteredRocketDesignScrollPosition, false, true, filteredRocketDesignHScrollViewStyle, filteredRocketDesignVScrollViewStyle, GUILayout.Width(200), GUILayout.MaxWidth(200));

            int newFilteredRocketDesignsSelectedGrid = GUILayout.SelectionGrid(filteredRocketDesignsSelectedGrid, fileteredRocketDesignsContentGrid.ToArray(), 1, filteredRocketDesignButtonStyle);
            if (newFilteredRocketDesignsSelectedGrid != filteredRocketDesignsSelectedGrid)
            {
                filteredRocketDesignsSelectedGrid = newFilteredRocketDesignsSelectedGrid;
                HandleExistingRocketdesignButtonClick(filteredRocketDesigns[filteredRocketDesignsSelectedGrid]);
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

            filterLabelsScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _filterLabelSelectionToggleStyle = new GUIStyle(HighLogic.Skin.toggle);
            filterCapabilitiesScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _filterCapabilitySelectionToggleStyle = new GUIStyle(HighLogic.Skin.toggle);

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

            filteredRocketDesignHScrollViewStyle = new GUIStyle(HighLogic.Skin.horizontalScrollbar);
            filteredRocketDesignVScrollViewStyle = new GUIStyle(HighLogic.Skin.verticalScrollbar);

            filteredRocketDesignButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
                fixedHeight = 70,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                fontStyle = FontStyle.Normal,
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
