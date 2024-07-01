using System;
using _Building.Scripts.Game.MainMenu.Root.View;
using UnityEngine;

namespace _Building.Scripts.Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        public event Action GoToGameplaySceneRequested; 

        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public void Run(UIRootView uiRoot)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            uiScene.GoToGameplayButtonClicked += () =>
            {
                GoToGameplaySceneRequested?.Invoke();
            };
        }
    }
}