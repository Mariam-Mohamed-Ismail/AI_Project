using DeliveryDriver.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platform2D
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject settingsMenu;
        //[SerializeField] private GameObject displayMenu;
        [SerializeField] private GameObject audioMenu;
        [SerializeField] private GameObject controlsMenu;
        Stack<GameObject> menuStack;
        GameObject currentMenu;
        private void Awake()
        {
            menuStack = new Stack<GameObject>();
            menuStack.Push(mainMenu);
            currentMenu = menuStack.Peek();

        }

        private void Start()
        {
            currentMenu.SetActive(true);
            //displayMenu.SetActive(false);
            audioMenu.SetActive(false);
            controlsMenu.SetActive(false);
            settingsMenu.SetActive(false);
            AudioManager.Instance.Play("MenuMusic");
        }

        public void Settings()
        {
            currentMenu.SetActive(false);
            settingsMenu.SetActive(true);
            currentMenu = settingsMenu;
            menuStack.Push(settingsMenu);
        }
        public void Display()
        {
            currentMenu.SetActive(false);
            //displayMenu.SetActive(true);
            //currentMenu = displayMenu;
            //menuStack.Push(displayMenu);
        }
        public void Audio()
        {
            currentMenu.SetActive(false);
            audioMenu.SetActive(true);
            currentMenu = audioMenu;
            menuStack.Push(audioMenu);
        }

        public void Controls()
        {
            currentMenu.SetActive(false);
            controlsMenu.SetActive(true);
            currentMenu = controlsMenu;
            menuStack.Push(controlsMenu);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Back()
        {
            currentMenu.SetActive(false);
            menuStack.Pop();
            currentMenu = menuStack.Peek();
            currentMenu.SetActive(true);
        }

        public void NewGame()
        {
            GameSceneManager.Instance.LoadNextLevel();
        }

    }
}

