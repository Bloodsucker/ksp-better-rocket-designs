using UnityEngine;
using KSP.UI.Screens;
using System;
using BetterRocketDesigns.RocketDesignSaverScreen;
using BetterRocketDesigns.ksp;
using BetterRocketDesigns.utils;
using BetterRocketDesigns.RocketDesignLoaderScreen;

namespace BetterRocketDesigns
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BetterRocketDesigns : MonoBehaviour
    {
        private RocketDesignManager _rocketDesignManager;
        private ApplicationLauncherButton _rocketDesignSaverToolbarButton;
        private ApplicationLauncherButton _rocketDesignLoaderToolbarButton;
        private Part detachedPart;

        private RocketDesignSaverController rocketDesignSaverController;
        private RocketDesignLoaderController _rocketDesignLoaderController;

        private void Start()
        {
            string targetDirectory = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder);
            RocketDesignLoader rocketDesignLoader = new RocketDesignLoader(targetDirectory);
            _rocketDesignManager = new RocketDesignManager(rocketDesignLoader);
            _rocketDesignManager.LoadAllRocketDesigns();

            _rocketDesignSaverToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnRocketDesignSaverToolbarButtonIn,
                OnRocketDesignSaverToolbarButtonOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                Tools.MakeTexture(32, 32, Color.blue));

            _rocketDesignLoaderToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnRocketDesignLoaderToolbarButtonIn,
                OnRocketDesignLoaderToolbarButtonOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                Tools.MakeTexture(32, 32, Color.yellow));

            GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
        }
        private void OnRocketDesignLoaderToolbarButtonIn()
        {
            RocketDesignLoaderUI rocketDesignLoaderUI = new GameObject("RocketDesignLoaderUI")
                .AddComponent<RocketDesignLoaderUI>();

            _rocketDesignLoaderController = new RocketDesignLoaderController(rocketDesignLoaderUI, _rocketDesignManager);

            rocketDesignLoaderUI.OnCancelButtonClicked += delegate {
                _rocketDesignLoaderToolbarButton.SetFalse(true);
            };
        }

        private void OnRocketDesignLoaderToolbarButtonOut()
        {
            _rocketDesignLoaderController?.HandleCancelClicked();
            _rocketDesignLoaderController = null;
        }


        private void OnRocketDesignSaverToolbarButtonIn()
        {
            if (detachedPart == null)
            {
                _rocketDesignSaverToolbarButton.SetFalse(true);
                return;
            };

            // Convert Part into a configNode
            ShipConstruct draggedShipConstruct = new ShipConstruct();
            draggedShipConstruct.Add(detachedPart);
            IConfigNodeAdapter configNode = new ConfigNodeAdapter(draggedShipConstruct.SaveShip());

            RocketDesignSaverUI rocketDesignSaverUI = new GameObject("RocketDesignSaverUI")
                .AddComponent<RocketDesignSaverUI>();

            rocketDesignSaverController = new RocketDesignSaverController(configNode, rocketDesignSaverUI, _rocketDesignManager);

            rocketDesignSaverUI.OnCancelButtonClicked += delegate {
                _rocketDesignSaverToolbarButton.SetFalse(true);
            };
        }

        private void OnRocketDesignSaverToolbarButtonOut()
        {
            rocketDesignSaverController?.HandleCancelClicked();
            rocketDesignSaverController = null;
        }

        private void OnDestroy()
        {
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

        private void DummyMethod()
        {
            // Do nothing
        }
    }
}
