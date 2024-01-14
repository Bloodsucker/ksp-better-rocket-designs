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

        private bool _isRocketDesignSaverWindowOpen = false;
        private bool _isRocketDesignLoaderWindowOpen = false;
        private RocketDesignSaverController rocketDesignSaverController;
        private RocketDesignLoaderController _rocketDesignLoaderController;
        private Texture2D loadRocketDesignToolbarIcon;
        private Texture2D saveAsRocketDesignToolbarIcon;

        private void Start()
        {
            loadRocketDesignToolbarIcon = GameDatabase.Instance.GetTexture("BetterRocketDesigns/Textures/open-64p", false);
            saveAsRocketDesignToolbarIcon = GameDatabase.Instance.GetTexture("BetterRocketDesigns/Textures/save-as-64p", false);

            _isRocketDesignSaverWindowOpen = false;
            _isRocketDesignLoaderWindowOpen = false;

            string targetDirectory = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder);
            RocketDesignLoader rocketDesignLoader = new RocketDesignLoader(targetDirectory);
            _rocketDesignManager = new RocketDesignManager(rocketDesignLoader);
            _rocketDesignManager.LoadAllRocketDesigns();

            _rocketDesignLoaderToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnRocketDesignLoaderToolbarButtonIn,
                OnRocketDesignLoaderToolbarButtonOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                loadRocketDesignToolbarIcon);

            GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
        }

        private void ShowSaveToolbarButton()
        {
            if(_isRocketDesignSaverWindowOpen || _isRocketDesignLoaderWindowOpen)
            {
                return;
            }

            _rocketDesignSaverToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnRocketDesignSaverToolbarButtonIn,
                OnRocketDesignSaverToolbarButtonOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                saveAsRocketDesignToolbarIcon);
        }

        private void HideSaveToolbarButton()
        {
            if(_rocketDesignSaverToolbarButton)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_rocketDesignSaverToolbarButton);
                _rocketDesignSaverToolbarButton = null;
            }
        }

        private void OnRocketDesignLoaderToolbarButtonIn()
        {
            if(_isRocketDesignLoaderWindowOpen || _isRocketDesignSaverWindowOpen)
            {
                _rocketDesignLoaderToolbarButton?.SetFalse(true);
                return;
            }

            _isRocketDesignLoaderWindowOpen = true;

            RocketDesignLoaderUI rocketDesignLoaderUI = new GameObject("RocketDesignLoaderUI")
                .AddComponent<RocketDesignLoaderUI>();

            rocketDesignLoaderUI.Init(_rocketDesignManager.getCachedLabels(), _rocketDesignManager.getCachedCapabilities());

            _rocketDesignLoaderController = new RocketDesignLoaderController(rocketDesignLoaderUI, _rocketDesignManager);

            _rocketDesignLoaderController.OnComplete += delegate
            {
                _isRocketDesignLoaderWindowOpen = false;
                _rocketDesignLoaderToolbarButton?.SetFalse(true);
            };
        }

        private void OnRocketDesignLoaderToolbarButtonOut()
        {
            _rocketDesignLoaderController?.Close();
            _isRocketDesignLoaderWindowOpen = false;
            _rocketDesignLoaderController = null;
        }


        private void OnRocketDesignSaverToolbarButtonIn()
        {
            if (detachedPart == null || _isRocketDesignSaverWindowOpen || _isRocketDesignLoaderWindowOpen)
            {
                _rocketDesignSaverToolbarButton?.SetFalse(true);
                return;
            };

            _isRocketDesignSaverWindowOpen = true;

            ConfigNode cn = CraftTools.TransformAsConfigNode(detachedPart);
            ConfigNodeAdapter configNode = new ConfigNodeAdapter(cn);
            RocketDesign newRocketDesign = new RocketDesign(configNode);

            RocketDesignSaverUI rocketDesignSaverUI = new GameObject("RocketDesignSaverUI")
                 .AddComponent<RocketDesignSaverUI>();

            rocketDesignSaverUI.Init(newRocketDesign, _rocketDesignManager.getCachedLabels(), _rocketDesignManager.getCachedCapabilities());

            rocketDesignSaverController = new RocketDesignSaverController(newRocketDesign, rocketDesignSaverUI, _rocketDesignManager);

            rocketDesignSaverController.OnComplete += delegate
            {
                _isRocketDesignSaverWindowOpen = false;
                _rocketDesignSaverToolbarButton?.SetFalse(true);
            };
        }

        private void OnRocketDesignSaverToolbarButtonOut()
        {
            rocketDesignSaverController?.Close();
            _isRocketDesignSaverWindowOpen = false;
            rocketDesignSaverController = null;
        }

        private void OnDestroy()
        {
            GameEvents.onEditorPartEvent.Remove(OnEditorPartEvent);

            HideSaveToolbarButton();

            ApplicationLauncher.Instance.RemoveModApplication(_rocketDesignLoaderToolbarButton);
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
                    ShowSaveToolbarButton();
                    break;
                case ConstructionEventType.PartDropped:
                case ConstructionEventType.PartAttached:
                case ConstructionEventType.PartDeleted:
                    Debug.Log("Part dragged STOP: " + Enum.GetName(typeof(ConstructionEventType), eventType));
                    detachedPart = null;
                    HideSaveToolbarButton();
                    break;
            }
        }

        private void DummyMethod()
        {
            // Do nothing
        }
    }
}
