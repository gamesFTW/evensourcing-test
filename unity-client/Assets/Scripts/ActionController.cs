﻿using UnityEngine;
using System;

[Serializable]
public class EndTurnAction
{
    public int currentTurn;
    public string endedPlayerId;
    public string startedPlayerId;
    public MovingPoints[] cardsMovingPointsUpdated;
    public string[] cardsUntapped;
    public string[] cardsDrawn;
}

public class MovingPoints
{
    public string id;
    public int currentMovingPoints;
}

[Serializable]
public class PlayCardAsManaAction
{
    public string cardId;
    public string playerId;
    public Boolean taped;
}

public class ActionController : MonoBehaviour
{
    private CardManger cardManger;

    void Awake()
    {
        cardManger = this.GetComponent<CardManger>();
    }

    public void ProcessAction(string type, int index, string message)
    {
        if (type == "EndTurnAction")
        {
            SocketData<EndTurnAction> data = JsonUtility.FromJson<SocketData<EndTurnAction>>(message);
            this.OnEndTurnAction(data.actions[index]);
        }

        if (type == "PlayCardAsManaAction")
        {
            Debug.Log(message);

            SocketData<PlayCardAsManaAction> data = JsonUtility.FromJson<SocketData<PlayCardAsManaAction>>(message);
            this.OnPlayCardAsManaAction(data.actions[index]);


            Debug.Log(JsonUtility.ToJson(data));
        }
    }

    public void OnEndTurnAction(EndTurnAction action)
    {
        cardManger.DrawCards(action.endedPlayerId, action.cardsDrawn);
    }

    public void OnPlayCardAsManaAction(PlayCardAsManaAction action)
    {
        cardManger.PlayCardAsMana(action.playerId, action.cardId, action.taped);
    }
}
