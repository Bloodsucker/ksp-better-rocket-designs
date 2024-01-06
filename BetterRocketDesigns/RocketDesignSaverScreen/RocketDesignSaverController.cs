using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaverScreen
{
    internal class RocketDesignSaverController
    {
        private readonly IConfigNodeAdapter newConfigNode;
        private readonly RocketDesignSaverUI _ui;
        private readonly RocketDesignManager _rocketDesignManager;
        public event Action OnComplete;

        public RocketDesignSaverController(IConfigNodeAdapter newConfigNode, RocketDesignSaverUI ui, RocketDesignManager rocketDesignManager)
        {
            this.newConfigNode = newConfigNode;

            this._ui = ui;
            this._rocketDesignManager = rocketDesignManager;

            this._ui.OnFilterChanged += HandleFilterChanged;
            this._ui.OnSaveButtonClicked += HandleSaveButtonClicked;
            this._ui.OnCancelButtonClicked += HandleCancelClicked;

            this._ui.UpdateFilteredRocketDesigns(rocketDesignManager.GetCachedRocketDesigns());
        }

        public void HandleCancelClicked()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
            OnComplete.Invoke();
        }

        private void HandleFilterChanged(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(filterCriteria );

            this._ui.UpdateFilteredRocketDesigns(filteredRocketDesigns);
        }

        private void HandleSaveButtonClicked(string name)
        {
            newConfigNode.SetValue("ship", name);

            RocketDesign newRocketDesign = new RocketDesign
            {
                Name = newConfigNode.GetValue("ship"),
                ConfigNode = newConfigNode
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
