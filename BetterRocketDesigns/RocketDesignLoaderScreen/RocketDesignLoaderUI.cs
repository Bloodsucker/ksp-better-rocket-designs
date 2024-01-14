using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignLoaderScreen
{
    internal class RocketDesignLoaderUI: MonoBehaviour
    {
        private RocketDesign _selectedRocketDesign;

        public event Action<string> OnTextFilterChanged;
        public event Action<string, bool> OnFilterLabelToggled;
        public event Action<string, bool> OnFilterCapabilityToggled;
        public event Action OnCancelButtonClicked;
        public event Action<RocketDesign> OnLoadButtonClicked;

        private List<RocketDesign> filteredRocketDesigns;

        private int _windowId;
        private Rect _windowPosition;
        private string filterTextInputText;
        private Dictionary<string, bool> _filterLabelsSelection;
        private Dictionary<string, bool> _filterCapabilitiesSelection;
        private IReadOnlyCollection<string> _cachedFilterLabels;
        private IReadOnlyCollection<string> _cachedFilterCapabilities;
        private Vector2 filteredRocketDesignScrollPosition;
        private Vector2 filterLabelsScrollPosition;
        private Vector2 filterCapabilitiesScrollPosition;
        private int filteredRocketDesignsSelectedGrid;
        private List<GUIContent> fileteredRocketDesignsContentGrid;
        private Vector2 _detailedLabelsScrollPosition;
        private Vector2 _detailedCapabilitiesScrollPosition;

        private GUIStyle rocketDesignSaverWindowStyle;
        private GUIStyle filterTextInputStyle;
        private GUIStyle filteredRocketDesignButtonStyle;
        private GUIStyle loadButtonStyle;
        private GUIStyle cancelButtonStyle;
        private GUIStyle filteredRocketDesignScrollViewStyle;
        private GUIStyle filterLabelsScrollViewStyle;
        private GUIStyle filterCapabilitiesScrollViewStyle;
        private GUIStyle _filterCapabilitySelectionToggleStyle;
        private GUIStyle _filterLabelSelectionToggleStyle;
        private GUIStyle _detailedLabelsScrollViewStyle;
        private GUIStyle _detailedCapabilitisScrollViewStyle;
        private GUIStyle _detailNameLabelStyle;

        public void Init(IReadOnlyCollection<string> cachedLabels, IReadOnlyCollection<string> cachedCapabilities)
        {
            _cachedFilterLabels = cachedLabels;
            _cachedFilterCapabilities = cachedCapabilities;

            _filterLabelsSelection = new Dictionary<string, bool>();
            foreach (var label in _cachedFilterLabels)
            {
                _filterLabelsSelection.Add(label, false);
            }

            _filterCapabilitiesSelection = new Dictionary<string, bool>();
            foreach (var capabilities in _cachedFilterCapabilities)
            {
                _filterCapabilitiesSelection.Add(capabilities, false);
            }
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _windowPosition = new Rect(0, 0, 700, 450);
            _windowPosition.x = (Screen.width - _windowPosition.width) / 2;
            _windowPosition.y = (Screen.height - _windowPosition.height) / 2;

            filterTextInputText = "";
            filteredRocketDesignsSelectedGrid = -1;

            InitStyle();
        }

        public void UpdateFilterResults(List<RocketDesign> rocketDesigns)
        {
            filteredRocketDesigns = rocketDesigns;

            fileteredRocketDesignsContentGrid = new List<GUIContent>();

            filteredRocketDesignsSelectedGrid = -1;
            _selectedRocketDesign = null;

            for (int i = 0; i < this.filteredRocketDesigns.Count; i++)
            {
                RocketDesign rocketDesign = this.filteredRocketDesigns[i];

                string buttonText = $"<b><size={HighLogic.Skin.font.fontSize * 1.5}>{rocketDesign.Name}</size></b>";

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
            }
        }

        private void HandleFilterTextInputChange(string newFilterText)
        {
            _selectedRocketDesign = null;

            OnTextFilterChanged.Invoke(newFilterText);
        }

        private void HandleExistingRocketdesignButtonClick(RocketDesign rocketDesign)
        {
            _selectedRocketDesign = rocketDesign;
        }

        private void OnGUI()
        {
            _windowPosition = GUILayout.Window(_windowId, _windowPosition, OnWindow, "Load Rocket Design as", rocketDesignSaverWindowStyle);
        }

        private void OnWindow(int windowId)
        {
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                filterTextInputText = GUILayout.TextField(filterTextInputText, filterTextInputStyle);

                if (GUI.changed)
                {
                    HandleFilterTextInputChange(filterTextInputText);
                }

                GUILayout.FlexibleSpace();
            }

            #region body-section
            GUILayout.BeginHorizontal();

            #region left-column
            GUILayout.BeginVertical(GUILayout.Width(200));
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
            OnWindowRocketDesignDetailsColumn();
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
            #endregion

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUI.enabled = _selectedRocketDesign != null;
            if (GUILayout.Button("Add to Editor", loadButtonStyle))
            {
                OnLoadButtonClicked.Invoke(_selectedRocketDesign);
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancel", cancelButtonStyle))
            {
                OnCancelButtonClicked.Invoke();
            }

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
        private void OnWindowFilterLabelsColumn()
        {
            GUILayout.Label("Label filter (OR)");

            filterLabelsScrollPosition = GUILayout.BeginScrollView(filterLabelsScrollPosition, filterLabelsScrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(150));

            foreach (var filterLabel in _cachedFilterLabels)
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

            filterCapabilitiesScrollPosition = GUILayout.BeginScrollView(filterCapabilitiesScrollPosition, filterCapabilitiesScrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(150));

            foreach (var filterCapability in _cachedFilterCapabilities)
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
            GUILayout.Label("Choose and Load:");
            filteredRocketDesignScrollPosition = GUILayout.BeginScrollView(filteredRocketDesignScrollPosition, filteredRocketDesignScrollViewStyle, GUILayout.Width(300), GUILayout.MaxWidth(300));

            int newFilteredRocketDesignsSelectedGrid = GUILayout.SelectionGrid(filteredRocketDesignsSelectedGrid, fileteredRocketDesignsContentGrid.ToArray(), 1, filteredRocketDesignButtonStyle);
            if (newFilteredRocketDesignsSelectedGrid != filteredRocketDesignsSelectedGrid)
            {
                filteredRocketDesignsSelectedGrid = newFilteredRocketDesignsSelectedGrid;
                HandleExistingRocketdesignButtonClick(filteredRocketDesigns[filteredRocketDesignsSelectedGrid]);
            }

            GUILayout.EndScrollView();
        }

        private void OnWindowRocketDesignDetailsColumn()
        {
            GUILayout.Label("Details");

            if (_selectedRocketDesign == null) return;

            GUILayout.Label(_selectedRocketDesign.Name, _detailNameLabelStyle);

            GUILayout.Label("Labels:");
            _detailedLabelsScrollPosition = GUILayout.BeginScrollView(_detailedLabelsScrollPosition, _detailedLabelsScrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(100));

            foreach(var label in _selectedRocketDesign.Labels)
            {
                GUILayout.Label(label);
            }

            GUILayout.EndScrollView();

            GUILayout.Label("Capabilities:");
            _detailedCapabilitiesScrollPosition = GUILayout.BeginScrollView(_detailedCapabilitiesScrollPosition, _detailedCapabilitisScrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(100));

            foreach (var capabilityKvp in _selectedRocketDesign.Capabilities)
            {
                GUILayout.Label($"{capabilityKvp.Key}: {capabilityKvp.Value} t");
            }

            GUILayout.EndScrollView();
        }


        private void InitStyle()
        {
            rocketDesignSaverWindowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                stretchHeight = true
            };

            filterTextInputStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                fixedWidth = 300,
            };

            filterLabelsScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _filterLabelSelectionToggleStyle = new GUIStyle(HighLogic.Skin.toggle);
            filterCapabilitiesScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _filterCapabilitySelectionToggleStyle = new GUIStyle(HighLogic.Skin.toggle);

            filteredRocketDesignScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);

            filteredRocketDesignButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                stretchWidth = true,
                fixedHeight = 80,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                fontStyle = FontStyle.Normal,
            };

            _detailNameLabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = HighLogic.Skin.font.fontSize * 2,
            };

            _detailedLabelsScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _detailedCapabilitisScrollViewStyle = new GUIStyle(HighLogic.Skin.scrollView);

            loadButtonStyle = new GUIStyle(HighLogic.Skin.button);
            loadButtonStyle.fixedWidth = 100;
            cancelButtonStyle = new GUIStyle(HighLogic.Skin.button);
            cancelButtonStyle.fixedWidth = 100;
        }
    }
}
