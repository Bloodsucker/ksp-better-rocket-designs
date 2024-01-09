using System;
using System.Collections.Generic;
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

            _ui.UpdateFilteredRocketDesigns(rocketDesignManager.GetCachedRocketDesigns());
        }

        public void HandleCancelClicked()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
            OnComplete.Invoke();
        }

        private void HandleFilterChanged(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(filterCriteria );

            _ui.UpdateFilteredRocketDesigns(filteredRocketDesigns);
        }

        private void HandleSaveButtonClicked(string name)
        {
            _newRocketDesign.Name = name;

            _rocketDesignManager.SaveOrReplaceAsRocketDesign(_newRocketDesign);

            MonoBehaviour.Destroy(_ui.gameObject);

            OnComplete.Invoke();
        }

        public void Close()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
        }
    }
}
