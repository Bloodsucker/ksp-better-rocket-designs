using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverController
    {
        private readonly RocketDesign _newRocketDesign;
        private readonly RocketDesignSaverUI _ui;
        private readonly RocketDesignManager _rocketDesignManager;
        public event Action OnComplete;

        private RocketDesignFilter _rocketDesignFilter = new RocketDesignFilter();

        public RocketDesignSaverController(RocketDesign newRocketDesign, RocketDesignSaverUI ui, RocketDesignManager rocketDesignManager)
        {
            _newRocketDesign = newRocketDesign;

            _ui = ui;
            _rocketDesignManager = rocketDesignManager;

            _ui.OnTextFilterChanged += HandleTextFilterChanged;
            _ui.OnSaveButtonClicked += HandleSaveButtonClicked;
            _ui.OnCancelButtonClicked += HandleCancelClicked;
            _ui.OnFilterLabelToggled += HandleFilterLabelToggled;
            _ui.OnFilterCapabilityToggled += HandleFilterCapabilityToggled;
            _ui.OnFilteredRocketDesignButtonClick += HandleFilteredRocketDesignButtonClicked;
            _ui.OnAddNewCapabilityButtonClicked += HandleNewCapabilityAddButtonClicked;
            _ui.OnNewCapabilityRemoveButtonClicked += HandleNewCapabilityRemoveButtonClicked;
            _ui.OnAddNewLabelButtonClicked += HandleNewLabelAddButtonClicked;
            _ui.OnRemoveNewLabelButtonClicked += HandleNewLabelRemoveButtonClicked;

            _ui.UpdateFilter(rocketDesignManager.GetCachedRocketDesigns());
        }

        private void HandleFilteredRocketDesignButtonClicked(RocketDesign selectedRocketDesign)
        {
            _newRocketDesign.Labels = selectedRocketDesign.Labels;
            _newRocketDesign.Capabilities = selectedRocketDesign.Capabilities;
        }

        private void HandleFilterCapabilityToggled(string filterCapability, bool isSelected)
        {
            if (_rocketDesignFilter.FilterCapabilities.Contains(filterCapability))
            {
                _rocketDesignFilter.FilterCapabilities.Remove(filterCapability);
            }

            if (isSelected)
            {
                _rocketDesignFilter.FilterCapabilities.Add(filterCapability);
            }

            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(_rocketDesignFilter);

            _ui.UpdateFilter(filteredRocketDesigns);
        }

        private void HandleFilterLabelToggled(string filterLabel, bool isSelected)
        {
            if (_rocketDesignFilter.FilterLabels.Contains(filterLabel))
            {
                _rocketDesignFilter.FilterLabels.Remove(filterLabel);
            }

            if (isSelected)
            {
                _rocketDesignFilter.FilterLabels.Add(filterLabel);
            }

            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(_rocketDesignFilter);

            _ui.UpdateFilter(filteredRocketDesigns);
        }

        public void HandleCancelClicked()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
            OnComplete.Invoke();
        }

        private void HandleTextFilterChanged(string filterCriteria)
        {
            _rocketDesignFilter.TextFilter = filterCriteria;

            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(_rocketDesignFilter);

            _ui.UpdateFilter(filteredRocketDesigns);
        }

        private void HandleSaveButtonClicked(string name)
        {
            _newRocketDesign.Name = name;

            _rocketDesignManager.SaveOrReplaceAsRocketDesign(_newRocketDesign);

            MonoBehaviour.Destroy(_ui.gameObject);

            OnComplete.Invoke();
        }

        private void HandleNewLabelAddButtonClicked(string labelName)
        {
            List<string> labels = _newRocketDesign.Labels.ToList();

            if(labels.IndexOf(labelName) != -1)
            {
                return;
            }

            labels.Add(labelName);

            _newRocketDesign.Labels = labels;
        }

        private void HandleNewLabelRemoveButtonClicked(string labelName)
        {
            List<string> labels = _newRocketDesign.Labels.ToList();
            labels.Remove(labelName);

            _newRocketDesign.Labels = labels;
        }

        private void HandleNewCapabilityAddButtonClicked(string newCapabilityName, float newCapabilityValue)
        {
            Dictionary<string, float> clonedCapabilities = new Dictionary<string, float>(_newRocketDesign.Capabilities
                .ToDictionary(entry => entry.Key, entry => entry.Value));
            clonedCapabilities.Add(newCapabilityName, newCapabilityValue);
            _newRocketDesign.Capabilities = clonedCapabilities;
        }
        private void HandleNewCapabilityRemoveButtonClicked(string capabilityName)
        {
            Dictionary<string, float> clonedCapabilities = new Dictionary<string, float>(_newRocketDesign.Capabilities
                .ToDictionary(entry => entry.Key, entry => entry.Value));
            clonedCapabilities.Remove(capabilityName);
            _newRocketDesign.Capabilities = clonedCapabilities;
        }

        public void Close()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
        }
    }
}
