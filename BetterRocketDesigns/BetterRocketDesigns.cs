using UnityEngine;
using KSP.UI.Screens;
using System;
using BetterRocketDesigns.RocketDesignSaverScreen;
using BetterRocketDesigns.ksp;
using BetterRocketDesigns.utils;
using BetterRocketDesigns.RocketDesignLoaderScreen;

namespace BetterRocketDesigns
{
    enum ToolbarButtonMode
    {
        SAVE_AS,
        LOAD,
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BetterRocketDesigns : MonoBehaviour
    {
        private RocketDesignManager _rocketDesignManager;
        private ApplicationLauncherButton _toolbarButton;
        private Part detachedPart;

        private bool _isRocketDesignSaverWindowOpen = false;
        private bool _isRocketDesignLoaderWindowOpen = false;
        private RocketDesignSaverController rocketDesignSaverController;
        private RocketDesignLoaderController _rocketDesignLoaderController;
        private Texture2D loadToolbarButtonIcon;
        private Texture2D saveAsToolbarButtonIcon;
        private ToolbarButtonMode _toolbarButtonMode;

        private void Start()
        {
            loadToolbarButtonIcon = GameDatabase.Instance.GetTexture("BetterRocketDesigns/Textures/open-64p", false);
            saveAsToolbarButtonIcon = GameDatabase.Instance.GetTexture("BetterRocketDesigns/Textures/save-as-64p", false);

            _isRocketDesignSaverWindowOpen = false;
            _isRocketDesignLoaderWindowOpen = false;

            string targetDirectory = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder);
            RocketDesignLoader rocketDesignLoader = new RocketDesignLoader(targetDirectory);
            _rocketDesignManager = new RocketDesignManager(rocketDesignLoader);
            _rocketDesignManager.LoadAllRocketDesigns();

            _toolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnToolbarButtonIn,
                OnToolbarButtonOut,
                DummyMethod, DummyMethod,
                DummyMethod, DummyMethod,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                loadToolbarButtonIcon);

            GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);

            SetToolbarButtonMode(ToolbarButtonMode.LOAD);
        }

        private void SetToolbarButtonMode(ToolbarButtonMode toolbarButtonMode)
        {
            if (toolbarButtonMode == ToolbarButtonMode.LOAD)
            {
                _toolbarButton.SetTexture(loadToolbarButtonIcon);
            } else
            {
                _toolbarButton.SetTexture(saveAsToolbarButtonIcon);
            }

            _toolbarButtonMode = toolbarButtonMode;
        }

        private void OpenLoaderWindow()
        {
            _isRocketDesignLoaderWindowOpen = true;

            RocketDesignLoaderUI rocketDesignLoaderUI = new GameObject("RocketDesignLoaderUI")
                .AddComponent<RocketDesignLoaderUI>();

            rocketDesignLoaderUI.Init(_rocketDesignManager.getCachedLabels(), _rocketDesignManager.getCachedCapabilities());

            _rocketDesignLoaderController = new RocketDesignLoaderController(rocketDesignLoaderUI, _rocketDesignManager);

            _rocketDesignLoaderController.OnComplete += delegate
            {
                _isRocketDesignLoaderWindowOpen = false;
                _toolbarButton.SetFalse(true);
            };
        }

        private void OpenSaveAsWindow()
        {
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
                _toolbarButton.SetFalse(true);
            };
        }

        private void OnToolbarButtonIn()
        {
            if(_toolbarButtonMode == ToolbarButtonMode.LOAD)
            {
                OpenLoaderWindow();
            }
            else
            {
                OpenSaveAsWindow();
            }
        }

        private void OnToolbarButtonOut()
        {
            _rocketDesignLoaderController?.Close();
            _isRocketDesignLoaderWindowOpen = false;
            _rocketDesignLoaderController = null;

            rocketDesignSaverController?.Close();
            _isRocketDesignSaverWindowOpen = false;
            rocketDesignSaverController = null;

            if(detachedPart != null)
            {
                SetToolbarButtonMode(ToolbarButtonMode.SAVE_AS);
            }
            else
            {
                SetToolbarButtonMode(ToolbarButtonMode.LOAD);
            }
        }

        private void OnDestroy()
        {
            GameEvents.onEditorPartEvent.Remove(OnEditorPartEvent);
            ApplicationLauncher.Instance.RemoveModApplication(_toolbarButton);
        }

        private void OnEditorPartEvent(ConstructionEventType eventType, Part part)
        {
            switch (eventType)
            {
                case ConstructionEventType.PartPicked:
                case ConstructionEventType.PartDetached:
                case ConstructionEventType.PartCopied:
                    detachedPart = part;

                    if(!_isRocketDesignLoaderWindowOpen && !_isRocketDesignSaverWindowOpen)
                    {
                        SetToolbarButtonMode(ToolbarButtonMode.SAVE_AS);
                    }

                    break;
                case ConstructionEventType.PartDropped:
                case ConstructionEventType.PartAttached:
                case ConstructionEventType.PartDeleted:
                    detachedPart = null;

                    if(!_isRocketDesignSaverWindowOpen && !_isRocketDesignLoaderWindowOpen)
                    {
                        SetToolbarButtonMode(ToolbarButtonMode.LOAD);
                    }

                    break;
            }
        }

        private void DummyMethod()
        {
            // Do nothing
        }
    }
}
