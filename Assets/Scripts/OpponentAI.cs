using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OpponentAI : MonoBehaviour
{
    Board opponentboard;
    Board playerBoard;

    public void SetBoards(Board playerBoard, Board opponentBoard)
    {
        this.playerBoard = playerBoard; 
        this.opponentboard = opponentBoard;
    }

    /// <summary>
    /// Calculates the right column to place the die
    /// </summary>
    /// <returns></returns>
    public int PlaceDie(bool hardMode, int roll)
    {
        if (hardMode)
            return HardOpponent(roll);
        else
            return EasyOpponent();
    }

    // ========================================================= //
    // ==================== EASY DIFFICULTY ==================== //
    // ========================================================= //
    int EasyOpponent()
    {
        // pick a random, open column
        // columns are open if the third slot is open
        Tile[,] t = opponentboard.GetTiles();

        // start by getting all the open columns
        List<int> openCols = new();

        for (int i = 0; i < 3; i++)
            if (!t[i, 2].hasValue)
                openCols.Add(i);

        if (openCols.Count == 0)
            return 0;

        // grab one of them randomly
        int col = openCols[UnityEngine.Random.Range(0, openCols.Count)];
        return col;
    }

    // ========================================================= //
    // ==================== HARD DIFFICULTY ==================== //
    // ========================================================= //
    int HardOpponent(int roll)
    {
        // key is the column, value is the row
        Dictionary<int, int> openCols = new();

        // get a view of the board
        int[,] oScoreGrid = opponentboard.GetScoreGrid();
        int[,] pScoreGrid = playerBoard.GetScoreGrid();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (oScoreGrid[i,j] == 0)
                {
                    openCols.Add(i, j);
                    break;
                }
            }
        }

        // evaluate what happens in each column if you place the die there
        int bestPointDiff = int.MinValue;
        int bestPlacement = 0;
        bool tieBreaker = false;
        int[,] tempPlayerGrid = (int[,]) pScoreGrid.Clone();
        int[,] tempOpponentGrid = (int[,]) oScoreGrid.Clone();

        HashSet<int> tieBreakerList = new();

        int playerScore, opponentScore, diff;
        foreach (var pair in openCols)
        {
            // temporarily place the die, and evaluate as if the die was actually placed
            tempOpponentGrid[pair.Key, pair.Value] = roll;
            CancelOpposingDice(tempPlayerGrid, pair.Key, roll);

            // determine the point differential, and remember the better outcome
            playerScore = EvaluateTotalScore(tempPlayerGrid);
            opponentScore = EvaluateTotalScore(tempOpponentGrid);

            diff = opponentScore - playerScore;
            if (diff > bestPointDiff)
            {
                bestPlacement = pair.Key;
                bestPointDiff = diff;
            }
            else if (diff == bestPointDiff)
            {
                tieBreakerList.Add(pair.Key);
                tieBreakerList.Add(bestPlacement);
                tieBreaker = true;
            }

            // reset the boards as if the move didn't happen
            tempOpponentGrid = (int[,]) oScoreGrid.Clone();
            tempPlayerGrid = (int[,]) pScoreGrid.Clone();
        }

        if (tieBreaker)
            return tieBreakerList.ToList()[UnityEngine.Random.Range(0, tieBreakerList.Count)];
        return bestPlacement;
    }

    // ======================================================== //
    // ==================== HELPER METHODS ==================== //
    // ======================================================== //
    void CancelOpposingDice(int[,] board, int column, int valuePlaced)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[column, i] == valuePlaced)
            {
                board[column, i] = 0;
            }
        }
    }

    int EvaluateCol(int a, int b, int c)
    {
        if (a == b && b == c && a != 0)
            return (a + b + c) * 3;
        else if (a == b && b != c && a != 0)
            return (a + b) * 2 + c;
        else if (a == c && b != c && a != 0)
            return (a + c) * 2 + b;
        else if (b == c && a != c && b != 0)
            return (b + c) * 2 + a;
        else
            return (a + b + c);
    }

    int EvaluateTotalScore(int[,] scoreGrid)
    {
        int score = 0;
        for (int i = 0; i < scoreGrid.GetLength(0); i++)
        {
            score += EvaluateCol(scoreGrid[i, 0],
                scoreGrid[i, 1],
                scoreGrid[i, 2]
                );
        }
        return score;
    }
}
