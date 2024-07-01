using System.Collections;
using _Building.Scripts.Game.Gameplay.Root;
using _Building.Scripts.Game.MainMenu.Root;
using _Building.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Building.Scripts
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private Coroutines _coroutines;
        private UIRootView _uiRoot;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
            _instance = new GameEntryPoint();
            _instance.StartGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
        }

        private void StartGame()
        {
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.GAMEPLAY)
            {
                _coroutines.StartCoroutine(LoadAndStartGameplay());
                return;
            }

            if (sceneName == Scenes.MAIN_MENU)
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
            }

            if (sceneName != Scenes.BOOT)
            {
                return;
            }
#endif
            
            // launch game
            _coroutines.StartCoroutine(LoadAndStartMainMenu());
            //_coroutines.StartCoroutine(LoadAndStartGameplay());
        }

        private IEnumerator LoadAndStartMainMenu()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAIN_MENU);

            yield return new WaitForSeconds(2);

            // container
            var sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot);
            
            // temporary bad code
            sceneEntryPoint.GoToGameplaySceneRequested += () =>
            {
                _coroutines.StartCoroutine(LoadAndStartGameplay());
            };
            
            _uiRoot.HideLoadingScreen();
        }
        
        private IEnumerator LoadAndStartGameplay()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);

            yield return new WaitForSeconds(2);

            // container
            var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_uiRoot);

            // temporary bad code
            sceneEntryPoint.GoToMainMenuSceneRequested += () =>
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
            };
            
            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}