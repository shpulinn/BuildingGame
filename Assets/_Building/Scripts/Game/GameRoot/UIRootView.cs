using System;
using UnityEngine;

namespace _Building.Scripts
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;

        private void Awake()
        {
            HideLoadingScreen();
        }

        public void HideLoadingScreen()
        {
            _loadingScreen.SetActive(false);
        }

        public void ShowLoadingScreen()
        {
            _loadingScreen.SetActive(true);
        }
    }
}