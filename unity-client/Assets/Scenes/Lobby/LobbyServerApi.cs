﻿using System;
using System.Threading.Tasks;


namespace Lobby
{
    [Serializable]
    public class GamesData
    {
        public GameData[] Games;
    }

    [Serializable]
    public class GameData
    {
        public string _id;
        public string gameServerId;
    }

    [Serializable]
    public class DecksData
    {
        public DeckData[] Decks;
    }

    [Serializable]
    public class DeckData
    {
        public string _id;
        public string name;
    }

    public class LobbyServerApi
    {
        public async static Task<GamesData> GetGames<GamesData>()
        {
            return await HttpRequest.Get<GamesData>(Config.LOBBY_SERVER_URL + "publications/Games");
        }

        public async static Task<DecksData> GetDecks<DecksData>()
        {
            return await HttpRequest.Get<DecksData>(Config.LOBBY_SERVER_URL + "publications/Decks");
        }

        public async static Task CreateGame(string player1DeckId, string player2DeckId)
        {

            var values = new
            {
                deckId1 = player1DeckId,
                deckId2 = player2DeckId
            };

            await HttpRequest.Post(Config.LOBBY_SERVER_URL + "methods/createGame", values);
        }
    }
}
