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

        public RocketDesignSaverController(RocketDesign newRocketDesign, RocketDesignSaverUI ui, RocketDesignManager rocketDesignManager)
        {
            _newRocketDesign = newRocketDesign;

            _ui = ui;
            _rocketDesignManager = rocketDesignManager;

            _ui.OnFilterChanged += HandleFilterChanged;
            _ui.OnSaveButtonClicked += HandleSaveButtonClicked;
            _ui.OnCancelButtonClicked += HandleCancelClicked;
            _ui.OnAddNewCapabilityButtonClicked += HandleNewCapabilityAddButtonClicked;
            _ui.OnNewCapabilityRemoveButtonClicked += HandleNewCapabilityRemoveButtonClicked;
            _ui.OnAddNewLabelButtonClicked += HandleNewLabelAddButtonClicked;
            _ui.OnRemoveNewLabelButtonClicked += HandleNewLabelRemoveButtonClicked;

            _ui.UpdateFilteredRocketDesigns(rocketDesignManager.GetCachedRocketDesigns());
        }

        public void HandleCancelClicked()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
            OnComplete.Invoke();
        }

        private void HandleFilterChanged(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(filterCriteria);

            _ui.UpdateFilteredRocketDesigns(filteredRocketDesigns);
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
