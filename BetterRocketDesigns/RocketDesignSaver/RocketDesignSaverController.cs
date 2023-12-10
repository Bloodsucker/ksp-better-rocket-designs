using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns.RocketDesignSaver
{
    internal class RocketDesignSaverController
    {
        private readonly IConfigNodeAdapter newConfigNode;
        private readonly RocketDesignSaverUI ui;
        private readonly RocketDesignManager rocketDesignManager;

        public RocketDesignSaverController(IConfigNodeAdapter newConfigNode, RocketDesignSaverUI ui, RocketDesignManager rocketDesignManager)
        {
            this.newConfigNode = newConfigNode;

            this.ui = ui;
            this.rocketDesignManager = rocketDesignManager;

            this.ui.OnFilterChanged += HandleFilterChanged;
            this.ui.OnSaveButtonClicked += HandleSaveButtonClicked;
            this.ui.OnCancelButtonClicked += HandleCancelClicked;

            this.ui.Init();
            this.ui.UpdateFilteredRocketDesigns(rocketDesignManager.GetCachedRocketDesigns());
        }

        public void HandleCancelClicked()
        {
            MonoBehaviour.Destroy(ui.gameObject);
        }

        private void HandleFilterChanged(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = rocketDesignManager.Filter(filterCriteria );

            this.ui.UpdateFilteredRocketDesigns(filteredRocketDesigns);
        }

        private void HandleSaveButtonClicked(string name)
        {
            newConfigNode.SetValue("ship", name);

            RocketDesign newRocketDesign = new RocketDesign
            {
                Name = newConfigNode.GetValue("ship"),
                ConfigNode = newConfigNode
            };

            rocketDesignManager.SaveOrReplaceAsRocketDesign(newRocketDesign);

            MonoBehaviour.Destroy(ui.gameObject);
        }
    }
}
