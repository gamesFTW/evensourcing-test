﻿using UnityEngine;
using System.Collections;
using UnibusEvent;

public class CardController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Unibus.Subscribe<string>("CARD_PLAY_AS_MANA", onCardPlayAsMana);
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void onCardPlayAsMana(string id)
    {
        await ServerApi.PlayCardAsMana(id);
    }
}