using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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
    private List<Square> _squares;
    private void Start()
    {
        _squarePrefab = Resources.Load("SquarePrefab") as GameObject;
        _startPosition = transform.Find("StartPosition");
        _squares = new List<Square>();
        inputField.onValueChanged.AddListener((str) =>
        {
            _gridSize = int.Parse(str) > 5 ? int.Parse(str) : 5;
            createButton.onClick.AddListener(InitGrid);
        }); 
    }

    private List<Task> tasks = new List<Task>();
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
                _squares.Add(square);
            }
        }

        StartCoroutine(StartSetup());
    }

    private void SetupUI()
    {
        foreach (var square in _squares)
        {
            if (square.IsMaster)
            {
                square.SetData(square.GetSquareValue());
                foreach (var s in square.usedCoords)
                {
                    GetSquare(s).SetSolutionColor(Color.green);
                }
            }
        }
    }

    IEnumerator StartSetup()
    {
        foreach (var square in _squares)
        {
            yield return StartCoroutine(SetUpSquare(square));
            StopCoroutine(SetUpSquare(square));
        }
        SetupUI();
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
        return coords.x >= 0 && coords.y >= 0 && coords.x < _gridSize && coords.y < _gridSize;
    }
    private Square GetSquare(Vector2Int coords)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            return _grid[coords.x, coords.y];
        return null;
    }
    private IEnumerator SetUpSquare(Square square)
    {
        if(square.HasSet) yield break;
        Random random = new Random();
        int value = random.Next(2, _gridSize);
        square.SetData(value);
        square.usedCoords = new List<Vector2Int>();
        if (UnityTools.IsPowerOfTwo(value) && value!= 2)
        {
            AddCoordinateOfSquare(square, value);
            AddCornerCoordinateOfSquare(square);
            restart:
            GridTypeWithCorner type = UnityTools.RandomEnumValue<GridTypeWithCorner>();
            square.UseGridOfType(type);
            if(square.usedCoords.Count == 0) goto restart;
            square.usedCoords.Add(square.GetOccupiedPosition());
            foreach (var coord in square.usedCoords)
            {
                GetSquare(coord).HasSet = true;
                GetSquare(coord).linkedSquare = square;
            }
        }
        else
        {
            AddCoordinateOfSquare(square, value);
            restart1:
            GridType type = UnityTools.RandomEnumValue<GridType>();
            square.UseGridOfType(type);
            if(square.usedCoords.Count == 0) goto restart1;
            square.usedCoords.Add(square.GetOccupiedPosition());
            foreach (var coord in square.usedCoords)
            {
                GetSquare(coord).HasSet = true;
                GetSquare(coord).linkedSquare = square;
            }
        }
        
        square.IsMaster = true;        square.HasSet = true;
    }
    void AddCoordinateOfSquare(Square sq, int value)
    {
        for (int i = 1; i < value; i++)
        {
            Vector2Int leftAdj = new Vector2Int(sq.GetOccupiedPosition().x + i, sq.GetOccupiedPosition().y);
            if(!GetSquare(leftAdj).HasSet)
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x+ (value - 1), sq.GetOccupiedPosition().y))) sq._selectableCoords.LeftCoords.Add(leftAdj);
        }

        for (int i = 1; i < value; i++)
        {
            Vector2Int rightAdj = new Vector2Int(sq.GetOccupiedPosition().x - i, sq.GetOccupiedPosition().y);
            if(!GetSquare(rightAdj).HasSet)
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x- (value - 1), sq.GetOccupiedPosition().y))) sq._selectableCoords.RightCoords.Add(rightAdj);
        }

        for (int i = 1; i < value; i++)
        {
            Vector2Int upAdj = new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y + i);
            if(!GetSquare(upAdj).HasSet)
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y + (value - 1)))) sq._selectableCoords.UpCoords.Add(upAdj);
        }

        for (int i = 1; i < value; i++)
        {
            Vector2Int downAdj = new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y - i);
            if(!GetSquare(downAdj).HasSet)
                if (CheckIfCoordinatesAreOnBoard(new Vector2Int(sq.GetOccupiedPosition().x, sq.GetOccupiedPosition().y - (value - 1)))) sq._selectableCoords.DownCoords.Add(downAdj);
        }   
    }
    void AddCornerCoordinateOfSquare(Square square)  
    {
        Vector2Int leftForUpAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y);
        Vector2Int leftUpAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y + 1); 
        Vector2Int upForLeftAdj = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1);
        if (!GetSquare(leftForUpAdj).HasSet && !GetSquare(leftUpAdj).HasSet && !GetSquare(leftUpAdj).HasSet)
        {
            if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y)) && 
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y + 1)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y)))
            {
                square._selectableCoords.UpLeftCornerCoords.Add(leftForUpAdj);
                square._selectableCoords.UpLeftCornerCoords.Add(leftUpAdj);
                square._selectableCoords.UpLeftCornerCoords.Add(upForLeftAdj);
            }
        }
            
        Vector2Int rightAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y);
        Vector2Int rightUpAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y + 1);
        Vector2Int upForRightAdj = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1);
        if (!GetSquare(rightAdj).HasSet && !GetSquare(rightUpAdj).HasSet && !GetSquare(upForRightAdj).HasSet)
        {
            if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y + 1)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y + 1)))
            {
                square._selectableCoords.UpRightCornerCoords.Add(rightAdj);
                square._selectableCoords.UpRightCornerCoords.Add(rightUpAdj);
                square._selectableCoords.UpRightCornerCoords.Add(upForRightAdj);
            }
        }
        
        Vector2Int downLeftAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y);
        Vector2Int leftDownAdj = new Vector2Int(square.GetOccupiedPosition().x + 1, square.GetOccupiedPosition().y - 1); 
        Vector2Int downAdj1 = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1);
        if (!GetSquare(downLeftAdj).HasSet && !GetSquare(leftDownAdj).HasSet && !GetSquare(downAdj1).HasSet)
        {
            if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1,
                    square.GetOccupiedPosition().y)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x + 1,
                    square.GetOccupiedPosition().y - 1)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x,
                    square.GetOccupiedPosition().y - 1)))
            {
                square._selectableCoords.DownLeftCornerCoords.Add(downLeftAdj);
                square._selectableCoords.DownLeftCornerCoords.Add(leftDownAdj);
                square._selectableCoords.DownLeftCornerCoords.Add(downAdj1);
            }
        }
        
        Vector2Int right = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y);
        Vector2Int rightDownAdj = new Vector2Int(square.GetOccupiedPosition().x - 1, square.GetOccupiedPosition().y - 1);
        Vector2Int downAdj2 = new Vector2Int(square.GetOccupiedPosition().x, square.GetOccupiedPosition().y - 1);
        if (!GetSquare(right).HasSet && !GetSquare(rightDownAdj).HasSet && !GetSquare(downAdj2).HasSet)
        {
            if (CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1,
                    square.GetOccupiedPosition().y)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x - 1,
                    square.GetOccupiedPosition().y - 1)) &&
                CheckIfCoordinatesAreOnBoard(new Vector2Int(square.GetOccupiedPosition().x,
                    square.GetOccupiedPosition().y - 1)))
            {
                square._selectableCoords.DownRightCornerCoords.Add(right);
                square._selectableCoords.DownRightCornerCoords.Add(rightDownAdj);
                square._selectableCoords.DownRightCornerCoords.Add(downAdj2);
            }
        }
    }
    
}
