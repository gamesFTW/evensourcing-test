﻿using System.Collections;
using TMPro;
using UnibusEvent;
using UnityEngine;
using DG.Tweening;

public class CardDisplay : MonoBehaviour
{
    public UnitDisplay UnitDisplay;
	public CardData cardData;

    public Transform Placeholder;

    public GameObject artwork;

	public TextMeshPro nameText;
	public TextMeshPro descriptionText;

	public TextMeshPro manaText;
	public TextMeshPro damageText;
    public TextMeshPro currentHpText;
    public TextMeshPro maxHpText;
    public TextMeshPro currentMovingPoints;

    public bool isMovedAtInitialPosition = false;

    public static readonly string CARD_PLAY_AS_MANA = "CARD_PLAY_AS_MANA";
    public static readonly string CARD_SELECTED_TO_PLAY = "CARD_SELECTED_TO_PLAY";
    public static readonly string CARD_MOUSE_ENTER = "CARD_MOUSE_ENTER";
    public static readonly string CARD_MOUSE_EXIT = "CARD_MOUSE_EXIT";
    public static readonly string CARD_DIED = "CARD_DIED";

    private bool IsSelected = false;
    private bool IsZoomed = false;
    private Vector3 scale;

    private GameObject overGlowObject;
    private GameObject selectedGlowObject;

    public int CurrentHp
    {
        get { return cardData.currentHp; }
        set {
            cardData.currentHp = value;
            currentHpText.text = value.ToString();
            this.Shake();
        }
    }

    public int CurrentMovingPoints
    {
        get { return cardData.currentMovingPoints; }
        set
        {
            cardData.currentMovingPoints = value;
            currentMovingPoints.text = value.ToString();
        }
    }

    // Use this for initialization
    void Start () 
    {
		nameText.text = cardData.name;

		manaText.text = cardData.manaCost.ToString();
        damageText.text = cardData.damage.ToString();
        maxHpText.text = cardData.maxHp.ToString();
        currentHpText.text = cardData.currentHp.ToString();
        currentMovingPoints.text = cardData.currentMovingPoints.ToString();

        StartCoroutine(LoadSprite());

        this.overGlowObject = this.transform.Find("Front").Find("OverGlow").gameObject;
        this.selectedGlowObject = this.transform.Find("Front").Find("SelectedGlow").gameObject;

        this.FillDescription();
    }

    void Update()
    {
        CheckRightMouseDown();

        UpdateZIndex();
    }

    public void MoveAtInitialPosition(Vector3 position, Vector3 scale) {
        this.Move(position, scale, 0);

        isMovedAtInitialPosition = true;
    }

    public void Move(Vector3 position, Vector3 scale, float time = 1) {
        this.scale = scale;

        this.transform.DOScale(scale, time);
        this.transform.DOMove(position, time);
    }

    public void FaceUp() {
        this.transform.Find("Back").gameObject.SetActive(false);
        this.transform.Find("Front").gameObject.SetActive(true);
    }

    public void FaceDown() {
        this.transform.Find("Back").gameObject.SetActive(true);
        this.transform.Find("Front").gameObject.SetActive(false);
    }

    public void Tap()
    {
        cardData.tapped = true;
        this.transform.DORotate(new Vector3(0, 0, -90), 1);

        this.Shake();
    }

    public void Untap()
    {
        cardData.tapped = false;
        this.transform.DORotate(new Vector3(0, 0, 0), 1);
    }

    public void ZoomIn(float zoom)
    {
        this.transform.DOScale(new Vector3(this.scale.x * zoom, this.scale.y * zoom, this.scale.z * zoom), 0.2f);
        this.IsZoomed = true;
    }

    public void ZoomOut()
    {
        this.transform.DOScale(this.scale, 0.2f);
        this.IsZoomed = false;
    }

    public void Kill()
    {
        Unibus.Dispatch(CARD_DIED, this);
        this.ZoomOut();
        this.SelectedHighlightOff();
        this.OverHighlightOff();
    }

    private void FillDescription()
    {
        string descriptionText = "";
        if (this.cardData.abilities.range != null)
        {
            descriptionText += "Range " + this.cardData.abilities.range.range + "\n";
        }
        if (this.cardData.abilities.firstStrike)
        {
            descriptionText += "First strike\n";
        }
        if (this.cardData.abilities.armored != null)
        {
            descriptionText += "Armored " + this.cardData.abilities.armored.armor + "\n";
        }
        if (this.cardData.abilities.vampiric)
        {
            descriptionText += "Vampiric\n";
        }
        if (this.cardData.abilities.noEnemyRetaliation)
        {
            descriptionText += "No enemy retaliation\n";
        }
        if (this.cardData.abilities.piercing)
        {
            descriptionText += "Cleave\n";
        }
        if (this.cardData.abilities.speed != null)
        {
            descriptionText += "Speed " + this.cardData.abilities.speed.speed + "\n";
        }
        if (this.cardData.abilities.flanking != null)
        {
            descriptionText += "Flanking " + this.cardData.abilities.flanking.damage + "\n";
        }
        if (this.cardData.abilities.push != null)
        {
            descriptionText += "Push " + this.cardData.abilities.push.range + "\n";
        }
        if (this.cardData.abilities.ricochet)
        {
            descriptionText += "Ricochet\n";
        }

        this.descriptionText.text = descriptionText;
    }

    void CheckRightMouseDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit && hit.collider.gameObject == this.gameObject)
            {
                OnRightMouseClicked();
            }
        }
    }

    void OnMouseDown()
    {
        OnLeftMouseClicked();
    }

    void OnMouseEnter()
    {
        Unibus.Dispatch(CARD_MOUSE_ENTER, this);
    }

    void OnMouseExit()
    {
        Unibus.Dispatch(CARD_MOUSE_EXIT, this);
    }
    
    public void SelectedHighlightOn()
    {
        IsSelected = true;
        this.selectedGlowObject.SetActive(true);
        this.overGlowObject.SetActive(false);

        if (this.UnitDisplay)
        {
            this.UnitDisplay.DisableTeamColor();
        }
    }

    public void SelectedHighlightOff()
    {
        IsSelected = false;
        this.selectedGlowObject.SetActive(false);

        if (this.UnitDisplay)
        {
            this.UnitDisplay.EnableTeamColor();
        }

    }

    public void OverHighlightOn()
    {
        if (!IsSelected)
        {
            this.overGlowObject.SetActive(true);
            this.UnitDisplay.DisableTeamColor();
        }
    }

    public void OverHighlightOff()
    {
        if (!IsSelected)
        {
            this.overGlowObject.SetActive(false);
            this.UnitDisplay.EnableTeamColor();
        }
    }

    private void OnLeftMouseClicked()
    {
        Unibus.Dispatch(CARD_SELECTED_TO_PLAY, this);
    }

    private void OnRightMouseClicked()
    {
        Unibus.Dispatch(CARD_PLAY_AS_MANA, this);
    }

    public IEnumerator LoadSprite()
    {
        WWW www = new WWW(Config.LOBBY_SERVER_URL + cardData.image);
        yield return www;

        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5F, 0.5F));

        artwork.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void UpdateZIndex()
    {
        Vector3 position = transform.localPosition;
        float z = (float)(position.x * 0.001);

        if (IsZoomed)
        {
            z = -20;
        }

        this.transform.localPosition = new Vector3(position.x, position.y, z);
    }

    public void Shake()
    {
        if (this.UnitDisplay)
        {
            this.UnitDisplay.Shake();
        }
    }
}
