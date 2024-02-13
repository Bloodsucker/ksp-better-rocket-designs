using BetterRocketDesigns.ksp;
using KSP;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignLoaderScreen
{
    internal class RocketDesignLoaderController
    {
        private readonly RocketDesignLoaderUI _ui;
        private readonly RocketDesignManager _rocketDesignManager;
        public event Action OnComplete;

        private RocketDesignFilter _rocketDesignFilter = new RocketDesignFilter();

        public RocketDesignLoaderController(RocketDesignLoaderUI ui, RocketDesignManager rocketDesignManager) { 
            _ui = ui;
            _rocketDesignManager = rocketDesignManager;

            _ui.OnTextFilterChanged += HandleTextFilterChanged;
            _ui.OnFilterLabelToggled += HandleLabelFilterToggled;
            _ui.OnFilterCapabilityToggled += HandleFilterCapabilityToggled;
            _ui.OnLoadButtonClicked += HandleLoadButtonClicked;
            _ui.OnCancelButtonClicked += HandleCancelClicked;

            _ui.UpdateFilterResults(rocketDesignManager.GetCachedRocketDesigns());
        }

        private void HandleLabelFilterToggled(string filterLabel, bool isSelected)
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

            _ui.UpdateFilterResults(filteredRocketDesigns);
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

            _ui.UpdateFilterResults(filteredRocketDesigns);
        }

        private void HandleLoadButtonClicked(RocketDesign design)
        {
            if (design.ConfigNode is ConfigNodeAdapter adapter)
            {
                ConfigNode configNode = adapter.GetOriginalInstance();
                ShipTemplate subassembly = new ShipTemplate();
                subassembly.LoadShip(design.CraftPath, configNode);
                EditorLogic.fetch.SpawnTemplate(subassembly);

                MonoBehaviour.Destroy(_ui.gameObject);
                OnComplete.Invoke();
            }
            else
            {
                Debug.LogError("RocketDesign#ConfigNode is not ConfigNodeAdapter type.");
            }
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

            _ui.UpdateFilterResults(filteredRocketDesigns);
        }

        public void Close()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
        }
    }
}
