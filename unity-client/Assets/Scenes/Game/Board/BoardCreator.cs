﻿using UnityEngine;
using UnibusEvent;
using System;
using DG.Tweening;

public class BoardCreator : MonoBehaviour
{
    public static readonly string UNIT_CLICKED_ON_BOARD = "UNIT_CLICKED_ON_BOARD";
    public static readonly string UNIT_MOUSE_ENTER_ON_BOARD = "UNIT_MOUSE_ENTER_ON_BOARD";
    public static readonly string UNIT_MOUSE_EXIT_ON_BOARD = "UNIT_MOUSE_EXIT_ON_BOARD";
    public static readonly string CLICKED_ON_VOID_TILE = "CLICKED_ON_VOID_TILE";

    public int Width;
    public int Height;

    public GameObject TilePrefab;
    public GameObject UnitPrefab;
    public GameObject AreaPrefab;

    private GameObject[,] Tiles;
    private GameObject[,] Units;

    private float tileWidth;
    private float tileHeight;

    public void CreateUnit(CardDisplay cardDisplay, Point position)
    {
        GameObject unit = Instantiate<GameObject>(UnitPrefab, this.transform);
        unit.transform.SetParent(this.transform);
        unit.transform.localPosition = PointerToIcometric(position, tileWidth, tileHeight);

        UnitDisplay unitDisplay = unit.GetComponent<UnitDisplay>();
        unitDisplay.CardData = cardDisplay.cardData;
        unitDisplay.CardDisplay = cardDisplay;
        cardDisplay.UnitDisplay = unitDisplay;

        Units[position.x, position.y] = unit as GameObject;
    }

    public void CreateArea(AreaData areaData)
    {
        GameObject area = Instantiate<GameObject>(AreaPrefab, this.transform);
        area.transform.SetParent(this.transform);
        area.transform.localPosition = PointerToIcometric(new Point(areaData.x, areaData.y), tileWidth, tileHeight);

        var areaDispay = area.GetComponent<AreaDisplay>();
        areaDispay.areaData = areaData;
        areaDispay.Init();
    }

    public void MoveUnit(CardDisplay cardDisplay, Point position, Point[] path)
    {
        UnitDisplay unitDisplay = cardDisplay.UnitDisplay;

        Sequence moveSequence = DOTween.Sequence();
        foreach(var point in path) {
            moveSequence.Append(
                unitDisplay.transform.DOLocalMove(PointerToIcometric(point, tileWidth, tileHeight), 0.5f)
            );
        }

        this.UpdatePositions(unitDisplay, position);
    }

    public void PushUnit(CardDisplay cardDisplay, Point position)
    {
        UnitDisplay unitDisplay = cardDisplay.UnitDisplay;

        unitDisplay.transform.DOLocalJump(PointerToIcometric(position, tileWidth, tileHeight), 0.5f, 1, 0.5f);

        this.UpdatePositions(unitDisplay, position);
    }

    public void KillUnit(CardDisplay cardDisplay)
    {
        UnitDisplay unitDisplay = cardDisplay.UnitDisplay;

        Point position = GetUnitsPosition(unitDisplay);
        Units[position.x, position.y] = null;

        Destroy(unitDisplay.gameObject);
    }

    public GameObject GetTileByUnit(GameObject card)
    {
        UnitDisplay unitDisplay = card.GetComponent<UnitDisplay>();

        Point unitPosition = GetUnitsPosition(unitDisplay);

        return Tiles[unitPosition.x, unitPosition.y];
    }

    public bool CheckCardsAdjacency(GameObject firstCard, GameObject secondCard)
    {
        Point firstCardPoint = GetUnitsPosition(firstCard.GetComponent<UnitDisplay>());
        Point secondCardPoint = GetUnitsPosition(secondCard.GetComponent<UnitDisplay>());

        int xDistance = Math.Abs(firstCardPoint.x - secondCardPoint.x);
        int yDistance = Math.Abs(firstCardPoint.y - secondCardPoint.y);

        if (xDistance + yDistance < 2)
        {
            return true;
        }

        return false;
    }

    private void UpdatePositions(UnitDisplay unitDisplay, Point position)
    {
        Point oldPosition = GetUnitsPosition(unitDisplay);

        Units[oldPosition.x, oldPosition.y] = null;
        Units[position.x, position.y] = unitDisplay.gameObject as GameObject;
    }

    private void Awake()
    {
        CreateTiles();
    }

    private void Start()
    {
        Unibus.Subscribe<Point>(TileDisplay.TILE_MOUSE_LEFT_CLICK, OnTileMouseLeftClick);
        Unibus.Subscribe<Point>(TileDisplay.TILE_MOUSE_ENTER, OnTileMouseEnter);
        Unibus.Subscribe<Point>(TileDisplay.TILE_MOUSE_EXIT, OnTileMouseExit);
    }

    private void CreateTiles ()
    {
        Tiles = new GameObject[Width + 1, Height + 1];
        Units = new GameObject[Width + 1, Height + 1];

        for (int x = 1; x <= Width; x++)
        {
            for (int y = 1; y <= Height; y++)
            {
                GameObject tile = Instantiate<GameObject>(TilePrefab, this.transform);
                tile.transform.SetParent(this.transform);

                RectTransform rt = (RectTransform)tile.transform;
                tileWidth = rt.rect.width;
                tileHeight = rt.rect.height;

                // Show text
                //TextMeshPro text = tile.transform.Find("Text").gameObject.GetComponent<TextMeshPro>();
                //text.SetText(x.ToString() + ";" + y.ToString());

                tile.transform.localPosition = PointerToIcometric(new Point(x, y), tileWidth, tileHeight);

                TileDisplay tileDisplay = tile.GetComponent<TileDisplay>();
                tileDisplay.x = x;
                tileDisplay.y = y;

                Tiles[x, y] = tile as GameObject;
            }
        }
    }

    private Point GetUnitsPosition(UnitDisplay unitDisplay)
    {
        for (int x = 1; x <= Width; x++)
        {
            for (int y = 1; y <= Height; y++)
            {
                if (Units[x, y] == unitDisplay.gameObject)
                {
                    return new Point(x, y);
                }
            }
        }

        return null;
    }

    void OnTileMouseLeftClick(Point position)
    {
        GameObject unit = Units[position.x, position.y];

        if (unit)
        {
            UnitDisplay unitDisplay = unit.GetComponent<UnitDisplay>();

            Unibus.Dispatch<UnitDisplay>(UNIT_CLICKED_ON_BOARD, unitDisplay);
        } else
        {
            Unibus.Dispatch<Point>(CLICKED_ON_VOID_TILE, position);
        }
    }

    void OnTileMouseEnter(Point position)
    {
        GameObject unit = Units[position.x, position.y];

        if (unit)
        {
            UnitDisplay unitDisplay = unit.GetComponent<UnitDisplay>();

            Unibus.Dispatch<UnitDisplay>(UNIT_MOUSE_ENTER_ON_BOARD, unitDisplay);
        }
    }

    void OnTileMouseExit(Point position)
    {
        GameObject unit = Units[position.x, position.y];

        if (unit)
        {
            UnitDisplay unitDisplay = unit.GetComponent<UnitDisplay>();

            Unibus.Dispatch<UnitDisplay>(UNIT_MOUSE_EXIT_ON_BOARD, unitDisplay);
        }
    }

    Vector3 PointerToIcometric(Point position, float tileWidth, float tileHeight)
    {
        float x = position.x;
        float y = position.y;

        return new Vector2(
            (x - y) * (tileWidth / 2),
            (x + y) * (tileHeight / 2)
        );
    }
}
