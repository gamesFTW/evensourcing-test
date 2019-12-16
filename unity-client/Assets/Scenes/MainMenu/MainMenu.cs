﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class MainMenu : MonoBehaviour
    {
        void Start()
        {
            var tutorialButton = this.transform.Find("Container/TutorialButton").GetComponent<Button>();
            var lobbyButton = this.transform.Find("Container/LobbyButton").GetComponent<Button>();
            var singlePlayerButton = this.transform.Find("Container/SinglePlayerButton").GetComponent<Button>();
            var multiPlayerButton = this.transform.Find("Container/MultiPlayerButton").GetComponent<Button>();

            CursorController.SetDefault();

            tutorialButton.onClick.AddListener(this.OnTutorialButtonClick);
            lobbyButton.onClick.AddListener(this.OnLobbyButtonClick);
            singlePlayerButton.onClick.AddListener(this.OnSinglePlayerButtonClick);
            multiPlayerButton.onClick.AddListener(this.OnMultiPlayerButtonClick);
        }

        private async void OnTutorialButtonClick()
        {
            this.transform.Find("Container").gameObject.SetActive(false);
            SinglePlayerGameData data = await LobbyServerApi.CreateTutorialGame();

            Main.StartGame(data.gameServerId, data.playerId, data.aiId);
        }

        private void OnLobbyButtonClick()
        {
            SceneManager.LoadScene("Lobby");
        }

        private void OnSinglePlayerButtonClick()
        {
            ChooseDeck.isSinglePlayer = true;
            SceneManager.LoadScene("ChooseDeck");
        }

        private void OnMultiPlayerButtonClick()
        {
            ChooseDeck.isSinglePlayer = false;
            SceneManager.LoadScene("ChooseDeck");
        }
    }
}
