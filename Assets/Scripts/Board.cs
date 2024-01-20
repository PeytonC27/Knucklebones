using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Canvas scoreBoard;
    [SerializeField] float[] yCoordinates = { -3, -6, -9 };
    float[] xCoordinates = { -5, 0, 5 };


    TMP_Text scoreDisplay;
    Tile[,] board;
    int score;

    protected void Start()
    {
        // get the board set up
        board = new Tile[3,3];
        SpawnTiles();

        // set up the scoreboard
        scoreBoard = Instantiate(scoreBoard, new Vector2(-15, yCoordinates[1]), Quaternion.identity, transform);
        scoreDisplay = scoreBoard.GetComponentInChildren<TMP_Text>();
        scoreDisplay.text = "0";
    }

    protected void SpawnTiles()
    {
        int i = 0, j = 0;
        foreach (int x in xCoordinates)
        {
            foreach (int y in yCoordinates) 
            {
                board[i, j] = Instantiate(tile, new Vector2(x, y), Quaternion.identity, transform);
                board[i, j].name = "Tile " + i + " " + j;
                board[i, j].column = i;
                board[i, j].board = this.GetComponent<Board>();
                j++;
            }
            j = 0;
            i++;
        }
    }

    public Tile[,] GetTiles()
    {
        return board;
    }

    /// <summary>
    /// Adds up the current score of the board
    /// </summary>
    public void RefreshScore()
    {
        score = 0;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            int a = board[x, 0].value;
            int b = board[x, 1].value;
            int c = board[x, 2].value;

            // all three same
            if (a == b && b == c)
                score += (a * 3) * 3;
            // doubles
            else if (a == b || a == c || b == c)
            {
                if (a == b)
                    score += (a + b) * 2 + c;
                else if (a == c)
                    score += (a + c) * 2 + b;
                else
                    score += (b + c) * 2 + a;
            }
            // only singles
            else 
                score += a + b + c;
        }

        scoreDisplay.text = score.ToString();
    }

    public bool IsFull()
    {
        for (int x = 0; x < board.GetLength(0); x++)
            for (int y = 0; y < board.GetLength(1); y++)
                if (!board[x, y].hasValue)
                    return false;
        return true;
    }
}
