using System;
using TMPro;
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

    public int Score { get { return score; } }

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
                board[i, j].GetComponent<SpriteRenderer>().sortingOrder = 1;
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
    public bool RefreshScore()
    {
        score = 0;
        bool bonusTriggered = false;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            Tile t1 = board[x, 0];
            Tile t2 = board[x, 1];
            Tile t3 = board[x, 2];

            score += ScoreCheck(t1, t2, t3);
        }

        scoreDisplay.text = score.ToString();
        return bonusTriggered;
    }

    /// <summary>
    /// Adds up the current score of the board
    /// <param name="lastValue">The last die placed</param>
    /// <param name="column">The column where the die was placed</param>
    /// </summary>
    public bool RefreshScore(int lastValue, int column)
    {
        score = 0;
        bool bonusTriggered = false;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            Tile t1 = board[x, 0];
            Tile t2 = board[x, 1];
            Tile t3 = board[x, 2];

            if (x == column)
                bonusTriggered = CheckIfBonusTriggered(t1, t2, t3, lastValue);

            score += ScoreCheck(t1, t2, t3);
        }

        scoreDisplay.text = score.ToString();
        return bonusTriggered;
    }

    /// <summary>
    /// Checks whether the most recent value in a specific column triggered a bonus
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="lastValue"></param>
    /// <param name="column"></param>
    private bool CheckIfBonusTriggered(Tile t1, Tile t2, Tile t3, int lastValue)
    {
        // when the column only has 2 die
        if (t2.hasValue && !t3.hasValue)
            if (t1.value == t2.value && t1.value == lastValue)
                return true;
        // when the colum has all three
        if (t3.hasValue)
            if ((t2.value == t3.value || t1.value == t3.value) && t3.value == lastValue)
                return true;
        return false;
    }

    public int ScoreCheck(Tile t1, Tile t2, Tile t3)
    {
        int a = t1.value;
        int b = t2.value;
        int c = t3.value;

        // go through every combo
        if (a == b && b == c && a != 0)
        {
            t1.EnableBorder();
            t2.EnableBorder();
            t3.EnableBorder();
            return (a + b + c) * 3;
        }
        else if (a == b && b != c && a != 0)
        {
            t1.EnableBorder();
            t2.EnableBorder();
            t3.DisableBorder();
            return (a + b) * 2 + c;
        }
        else if (a == c && b != c && a != 0)
        {
            t1.EnableBorder();
            t2.DisableBorder();
            t3.EnableBorder();
            return (a + c) * 2 + b;
        }
        else if (b == c && a != c && b != 0)
        {
            t1.DisableBorder();
            t2.EnableBorder();
            t3.EnableBorder();
            return (b + c) * 2 + a;
        }
        else
        {
            t1.DisableBorder();
            t2.DisableBorder();
            t3.DisableBorder();
            return (a + b + c);
        }
    }

    public bool IsFull()
    {
        for (int x = 0; x < board.GetLength(0); x++)
            for (int y = 0; y < board.GetLength(1); y++)
                if (!board[x, y].hasValue)
                    return false;
        return true;
    }

    public void EnableBorder(Tile t)
    {
        t.EnableBorder();
    }

    public void DisableBorder(Tile t)
    {
        t.DisableBorder();
    }

    public void SetScoreboardColor(Color color)
    {
        scoreDisplay.faceColor = color;
    }

    public int[,] GetScoreGrid()
    {
        int[,] scoreGrid = new int[3, 3];
        for (int x = 0; x < board.GetLength(0); x++)
            for (int y = 0; y < board.GetLength(1); y++)
                scoreGrid[x, y] = board[x, y].value;

        return scoreGrid;
    }
}
