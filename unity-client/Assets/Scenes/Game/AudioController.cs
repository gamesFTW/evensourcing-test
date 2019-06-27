﻿using System.Collections;
using System.Collections.Generic;
using UnibusEvent;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController : MonoBehaviour
{
    public static readonly string CARD_ATTACKED = "AudioController:CARD_ATTACKED";
    public static readonly string CARD_PLAYED = "AudioController:CARD_PLAYED";
    public static readonly string CARD_DIED = "AudioController:CARD_DIED";
    public static readonly string CARD_MOVED = "AudioController:CARD_MOVED";
    public static readonly string CARD_SELECTED = "AudioController:CARD_SELECTED";

    private AudioSource AudioSource;
    private Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        Unibus.Subscribe<CardDisplay>(AudioController.CARD_ATTACKED, OnCardAttacked);
        Unibus.Subscribe<CardDisplay>(AudioController.CARD_PLAYED, OnCardPlayed);
        Unibus.Subscribe<CardDisplay>(AudioController.CARD_DIED, OnCardDied);
        Unibus.Subscribe<CardDisplay>(AudioController.CARD_MOVED, OnCardMoved);
        Unibus.Subscribe<CardDisplay>(AudioController.CARD_SELECTED, OnCardSelected);

        this.AudioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddSounds(List<string> urls)
    {
        foreach(var url in urls)
        {
            if (url != null && !this.sounds.ContainsKey(url))
            {
                // Remove dublicates
                this.sounds.Add(url, null);
            }
        }

        foreach (KeyValuePair<string, AudioClip> entry in this.sounds)
        {
            StartCoroutine(this.LoadSound(entry.Key));
        }
    }

    private IEnumerator LoadSound(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(Config.LOBBY_SERVER_URL + url, AudioType.WAV))
        {
            yield return www.Send();

            if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                this.sounds.Remove(url);
                this.sounds.Add(url, clip);
            }
        }
    }

    private void OnCardAttacked(CardDisplay attackerCard)
    {
        this.Play(attackerCard, "attack");
    }

    private void OnCardPlayed(CardDisplay attackerCard)
    {
        this.Play(attackerCard, "play");
    }

    private void OnCardDied(CardDisplay attackerCard)
    {
        this.Play(attackerCard, "die");
    }

    private void OnCardMoved(CardDisplay attackerCard)
    {
        this.Play(attackerCard, "move");
    }

    private void OnCardSelected(CardDisplay attackerCard)
    {
        this.Play(attackerCard, "select");
    }

    private void Play(CardDisplay card, string soundName)
    {
        if (card.sounds.ContainsKey(soundName))
        {
            SoundData soundData = card.sounds[soundName];
            AudioClip clip = this.sounds[soundData.url];

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();

            var duration = clip.length;

            StartCoroutine(WaitForSound(duration, audioSource));
        }
    }

    private IEnumerator WaitForSound(float duration, AudioSource audioSource)
    {
        yield return new WaitForSeconds(duration);
        Destroy(audioSource);
    }
}