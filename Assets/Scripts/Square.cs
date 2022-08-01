using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SquareSelectableCoords
{
	public List<Vector2Int> LeftCoords;
	public List<Vector2Int> RightCoords;
	public List<Vector2Int> UpCoords;
	public List<Vector2Int> DownCoords;
	public List<Vector2Int> UpLeftCornerCoords;
	public List<Vector2Int> UpRightCornerCoords;
	public List<Vector2Int> DownLeftCornerCoords;
	public List<Vector2Int> DownRightCornerCoords;
}
public class Square : MonoBehaviour
{
	public int _value;
	private TMP_Text _text;
	private SpriteRenderer _renderer;
	public Vector2Int _occupiedPos;
	public SquareSelectableCoords _selectableCoords;
	public List<Vector2Int> usedCoords;
	public bool HasSet;
	public Square linkedSquare;
	public bool IsMaster { get; set; }

	public void Init()
	{
		_text = transform.Find("Value").GetComponent<TMP_Text>();
		_text.text = "";
		_renderer = transform.Find("SquareSprite").GetComponent<SpriteRenderer>();
		
	}

	public void SetOccupiedPosition(Vector2Int coords)
	{
		_occupiedPos = coords;
	}

	public void SetData(int value)
	{
		this._value = value;
		_renderer.color = Color.grey;
		_text.text = value.ToString();
	}

	public void SetSolutionColor(Color color)
	{
		if(!IsMaster)
			_renderer.color = color;
	}
	public int GetSquareValue()
	{
		return _value;
	}
	public Vector2Int GetOccupiedPosition()
	{
		return _occupiedPos;
	}

	public void ResetSelectedData()
	{
		_value = 0;
		_renderer.color = Color.green;
		_text.text = "";
		_selectableCoords = new SquareSelectableCoords();
	}
	public void UseGridOfType(GridType type)
	{
		usedCoords = GetGrid(type);
	}
	public void UseGridOfType(GridTypeWithCorner type)
	{
		usedCoords = GetGridWithCorner(type);
	}
	public List<Vector2Int> GetGrid(GridType type)
	{
		switch (type)
		{
			case GridType.up :
				return _selectableCoords.UpCoords;
			case GridType.down :
				return _selectableCoords.DownCoords;
			case GridType.left :
				return _selectableCoords.LeftCoords;
			case GridType.right :
				return _selectableCoords.RightCoords;
		}
		return default;
	}
	public List<Vector2Int> GetGridWithCorner(GridTypeWithCorner type)
	{
		switch (type)
		{
			case GridTypeWithCorner.up :
				return _selectableCoords.UpCoords;
			case GridTypeWithCorner.down :
				return _selectableCoords.DownCoords;
			case GridTypeWithCorner.left :
				return _selectableCoords.LeftCoords;
			case GridTypeWithCorner.right :
				return _selectableCoords.RightCoords;
			case GridTypeWithCorner.upleft:
				return _selectableCoords.UpLeftCornerCoords;
			case GridTypeWithCorner.upright:
				return _selectableCoords.UpRightCornerCoords;
			case GridTypeWithCorner.downleft:
				return _selectableCoords.DownLeftCornerCoords;
			case GridTypeWithCorner.downright:
				return _selectableCoords.DownRightCornerCoords;
		}
		return default;
	}
}
