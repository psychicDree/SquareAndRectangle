using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;
public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button createButton;
    private Transform _startPosition;
    private GameObject _squarePrefab;
    private int _gridSize = 5;
    private Square[,] _grid;
    private Vector2Int _gridCoords;    
    private void Start()
    {
        _squarePrefab = Resources.Load("SquarePrefab") as GameObject;
        _startPosition = transform.Find("StartPosition");
        inputField.onValueChanged.AddListener((str) =>
        {
            if (int.Parse(str) > 5)
            {
                _gridSize = int.Parse(str);
            }
            else
            {
                _gridSize = 5;
            }
            createButton.onClick.AddListener(InitGrid);
        }); 
    }

    private void InitGrid()
    {
        _startPosition.transform.position = new Vector3(-(_gridSize / 2), -(_gridSize / 2), 0);
        GameObject.Find("Canvas").SetActive(false);
        _grid = new Square[_gridSize, _gridSize];
        
        for (int y = 0; y < _gridSize; ++y)
        {
            for (int x = 0; x < _gridSize; ++x)
            {
                Vector2Int squareCoords = new Vector2Int(x,y);
                Square square = Instantiate(_squarePrefab).GetComponent<Square>();
                square.Init();
                square.transform.position = CalculatePositionFromCoords(squareCoords);
                square.SetOccupiedPosition(squareCoords);
                SetSquareOnBoard(squareCoords, square);
            }
        }
        SetUpSquare();
    }

    private Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return _startPosition.position + new Vector3(coords.x * 1, coords.y * 1,0f );
    }
    private void SetSquareOnBoard(Vector2Int coords, Square piece)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            _grid[coords.x, coords.y] = piece;
    }
    private bool CheckIfCoordinatesAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= _gridSize || coords.y >= _gridSize)
            return false;
        return true;
    }
    public Square GetSquare(Vector2Int coords)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            return _grid[coords.x, coords.y];
        return null;
    }

    private void SetUpSquare()
    {
        resetIfSet:
        var square = UnityTools.GetRandomValue<Square>(_grid, new Random());
        if(square.HasSet) goto resetIfSet;
        
        Random random = new Random();
        int value = random.Next(2, _gridSize);
        square.SetData(value);
        AddCoordinateOfSquare(square, value);
        Debug.Log(value);
        if (UnityTools.IsPowerOfTwo(value))
        {
            AddCornerCoordinateOfSquare(square);
            restart:
            GridTypeWithCorner type = UnityTools.RandomEnumValue<GridTypeWithCorner>();
            square.UseGridOfType(type);
            if(square.usedCoords.Count == 0)
                goto restart;
            square.usedCoords.Add(square.GetOccupiedPosition());
            // foreach (var sq in FindObjectsOfType<Square>())
            //     if(IsOverlapping(sq.usedCoords,square.usedCoords)) goto restart;
        }
        else
        {
            restart:
            square.usedCoords = new List<Vector2Int>();
            GridType type = UnityTools.RandomEnumValue<GridType>();
            square.UseGridOfType(type);
            if(square.usedCoords.Count == 0)
                goto restart;
            square.usedCoords.Add(square.GetOccupiedPosition());
            // foreach (var sq in FindObjectsOfType<Square>())
            //     if(IsOverlapping(sq.usedCoords,square.usedCoords)) goto restart;
        }
        square.HasSet = true;
        goto resetIfSet;
    }

    private bool IsOverlapping(List<Vector2Int> list1, List<Vector2Int> list2)
    {
        return (from item1 in list1 from item2 in list2 where item1 == item2 select item1).Any();
    }
    void AddCoordinateOfSquare(Square sq, int value)
        {
            Debug.Log(value);
            for (int i = 1; i < value; i++)
            {
                Vector2Int leftAdj = new Vector2Int(sq.GetOccupiedPosition().x + i, sq.GetOccupiedPosition().y);
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x+ (value - 1), sq.GetOccupiedPosition().y))) sq._selectableCoords.LeftCoords.Add(leftAdj);
            }

            for (int i = 1; i < value; i++)
            {
                Vector2Int rightAdj = new Vector2Int(sq.GetOccupiedPosition().x - i, sq.GetOccupiedPosition().y);
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x- (value - 1), sq.GetOccupiedPosition().y))) sq._selectableCoords.RightCoords.Add(rightAdj);
            }

            for (int i = 1; i < value; i++)
            {
                Vector2Int upAdj = new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y + i);
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y + (value - 1)))) sq._selectableCoords.UpCoords.Add(upAdj);
            }

            for (int i = 1; i < value; i++)
            {
                Vector2Int downAdj = new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y - i);
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y - (value - 1)))) sq._selectableCoords.DownCoords.Add(downAdj);
            }
        }
    void AddCornerCoordinateOfSquare(Square square)  
    {
        Vector2Int leftForUpAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y);
        Vector2Int leftUpAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y + 1); 
        Vector2Int upForLeftAdj = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1);
        if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y)) && 
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y + 1)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y)))
        {
            square._selectableCoords.UpLeftCornerCoords.Add(leftForUpAdj);
            square._selectableCoords.UpLeftCornerCoords.Add(leftUpAdj);
            square._selectableCoords.UpLeftCornerCoords.Add(upForLeftAdj);
        }
        Vector2Int rightAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y);
        Vector2Int rightUpAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y + 1);
        Vector2Int upForRightAdj = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1);
        if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y + 1)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1)))
        {
            square._selectableCoords.UpRightCornerCoords.Add(rightAdj);
            square._selectableCoords.UpRightCornerCoords.Add(rightUpAdj);
            square._selectableCoords.UpRightCornerCoords.Add(upForRightAdj);
        }
        
        Vector2Int downLeftAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y);
        Vector2Int leftDownAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y - 1); 
        Vector2Int downAdj1 = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1);
        if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y - 1)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1)))
        {
            square._selectableCoords.DownLeftCornerCoords.Add(downLeftAdj);
            square._selectableCoords.DownLeftCornerCoords.Add(leftDownAdj);
            square._selectableCoords.DownLeftCornerCoords.Add(downAdj1);
        }
        
        Vector2Int right = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y);
        Vector2Int rightDownAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y - 1);
        Vector2Int downAdj2 = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1);
        if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y - 1)) &&
            CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1)))
        {
            square._selectableCoords.DownRightCornerCoords.Add(right);
            square._selectableCoords.DownRightCornerCoords.Add(rightDownAdj);
            square._selectableCoords.DownRightCornerCoords.Add(downAdj2);
        }
    }
    
}
