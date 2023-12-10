using UnityEngine;
using KSP.UI.Screens;
using System;
using BetterRocketDesigns.RocketDesignSaver;
using BetterRocketDesigns.ksp;

namespace BetterRocketDesigns
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BetterRocketDesigns : MonoBehaviour
    {
        private ApplicationLauncherButton toolbarButton;
        private Part detachedPart;

        private RocketDesignSaverUI rocketDesignSaverUI;
        private RocketDesignSaverController rocketDesignSaverController;

        private void Start()
        {
            toolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnToolbarButtonToggleIn,
                OnToolbarButtonToggleOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                GameDatabase.Instance.GetTexture("Path/To/Your/notfound", false));

            GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
        }

        private void OnToolbarButtonToggleIn()
        {
            Debug.Log("RandomPartExplosion OnToolbarButtonToggleIn");

            if (detachedPart == null)
            {
                toolbarButton.SetFalse(false);
                return;
            };

            // Convert Part into a configNode
            ShipConstruct draggedShipConstruct = new ShipConstruct();
            draggedShipConstruct.Add(detachedPart);
            ConfigNode actualConfigNode = draggedShipConstruct.SaveShip();
            IConfigNodeAdapter configNode = new ConfigNodeAdapter(actualConfigNode);

            string targetDirectory = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder);
            RocketDesignLoader rocketDesignLoader = new RocketDesignLoader(targetDirectory);
            RocketDesignManager rocketDesignManager = new RocketDesignManager(rocketDesignLoader);

            rocketDesignManager.LoadAllRocketDesigns();

            RocketDesignSaverUI rocketDesignSaverUI = new GameObject("RocketDesignSaverUI")
                .AddComponent<RocketDesignSaverUI>();

            rocketDesignSaverController = new RocketDesignSaverController(configNode, rocketDesignSaverUI, rocketDesignManager);

            rocketDesignSaverUI.OnCancelButtonClicked += delegate {
                rocketDesignSaverController = null;
            };
        }

        private void OnToolbarButtonToggleOut()
        {
            rocketDesignSaverController?.HandleCancelClicked();
            rocketDesignSaverController = null;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to avoid memory leaks
            GameEvents.onEditorPartEvent.Remove(OnEditorPartEvent);
        }

        private void OnEditorPartEvent(ConstructionEventType eventType, Part part)
        {
            if (eventType != ConstructionEventType.PartDragging)
                Debug.Log("OnEditorPartEvent: " + Enum.GetName(typeof(ConstructionEventType), eventType));

            switch (eventType)
            {
                case ConstructionEventType.PartPicked:
                case ConstructionEventType.PartDetached:
                case ConstructionEventType.PartCopied:
                    Debug.Log("Part dragged START: " + Enum.GetName(typeof(ConstructionEventType), eventType));
                    detachedPart = part;
                    break;
                case ConstructionEventType.PartDropped:
                case ConstructionEventType.PartAttached:
                case ConstructionEventType.PartDeleted:
                    Debug.Log("Part dragged STOP: " + Enum.GetName(typeof(ConstructionEventType), eventType));
                    detachedPart = null;
                    break;
            }
        }

        private void SaveDraggedPartAsCraftFile()
        {
            // Get the EditorLogic instance
            EditorLogic editorLogic = EditorLogic.fetch;

            // Check if a part is currently selected
            if (detachedPart)
            {
                // Create a ShipConstruct with the root part and its connections
                ShipConstruct draggedShipConstruct = new ShipConstruct();
                draggedShipConstruct.Add(detachedPart);

                // Convert the ShipConstruct to a ConfigNode
                ConfigNode craftConfigNode = draggedShipConstruct.SaveShip();

                // Define the target directory for the .craft file
                string targetDirectory = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder, "RocketDesign");

                // Ensure the target directory exists, create it if necessary
                if (!System.IO.Directory.Exists(targetDirectory))
                {
                    System.IO.Directory.CreateDirectory(targetDirectory);
                }

                // Define the target file path
                string craftFilePath = System.IO.Path.Combine(targetDirectory, "my_new_vessel.craft");

                // Save the ConfigNode to the .craft file
                if (craftConfigNode != null)
                {
                    craftConfigNode.Save(craftFilePath);

                    Debug.Log("Craft file saved at: " + craftFilePath);
                }
                else
                {
                    Debug.LogError("Failed to convert detached part to ConfigNode.");
                }
            }
            else
            {
                Debug.LogWarning("No part is currently selected in the editor.");
            }
        }

        private void DummyMethod()
        {
            // Do nothing
        }
    }
}
