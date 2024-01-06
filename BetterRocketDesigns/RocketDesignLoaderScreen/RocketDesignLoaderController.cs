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

        public RocketDesignLoaderController(RocketDesignLoaderUI ui, RocketDesignManager rocketDesignManager) { 
            _ui = ui;
            _rocketDesignManager = rocketDesignManager;

            this._ui.OnFilterChanged += HandleFilterChanged;
            this._ui.OnLoadButtonClicked += HandleLoadButtonClicked;
            this._ui.OnCancelButtonClicked += HandleCancelClicked;

            this._ui.UpdateFilteredRocketDesigns(rocketDesignManager.GetCachedRocketDesigns());
        }

        private void HandleLoadButtonClicked(RocketDesign design)
        {
            if (design.ConfigNode is ConfigNodeAdapter adapter)
            {
                ConfigNode configNode = adapter.GetOriginalInstance();
                ShipTemplate subassembly = new ShipTemplate();
                subassembly.LoadShip(configNode);
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

        private void HandleFilterChanged(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = _rocketDesignManager.Filter(filterCriteria);

            this._ui.UpdateFilteredRocketDesigns(filteredRocketDesigns);
        }

        public void Close()
        {
            MonoBehaviour.Destroy(_ui.gameObject);
        }
    }
}
