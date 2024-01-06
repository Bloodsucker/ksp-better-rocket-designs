using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverController
    {
        private readonly IConfigNodeAdapter _newConfigNode;
        private readonly RocketDesignSaverUI _ui;
        private readonly RocketDesignManager _rocketDesignManager;
        public event Action OnComplete;

        public RocketDesignSaverController(IConfigNodeAdapter newConfigNode, RocketDesignSaverUI ui, RocketDesignManager rocketDesignManager)
        {
            _newConfigNode = newConfigNode;

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
            RocketDesign newRocketDesign = new RocketDesign(_newConfigNode)
            {
                Name = name
            };

            _rocketDesignManager.SaveOrReplaceAsRocketDesign(newRocketDesign);

            MonoBehaviour.Destroy(_ui.gameObject);

            OnComplete.Invoke();
        }

        public void Close()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
        }
    }
}
