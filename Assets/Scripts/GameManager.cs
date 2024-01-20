using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] matchSprites = new Sprite[6];
    [SerializeField] Sprite[] playerSprites = new Sprite[6];
    [SerializeField] Sprite[] opponentSprites = new Sprite[6];
    [SerializeField] Sprite emptySprite;

    Board opponentBoard;
    Board playerBoard;
    SpriteRenderer rollDisplay;
    int currentValue;
    int turns;
    bool playerTurn = true;
    bool dieReady;
    bool opponentReady = true;

    private void Start()
    {
        opponentBoard = GetComponentsInChildren<Board>()[0];
        playerBoard = GetComponentsInChildren<Board>()[1];
        rollDisplay = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(RollDie());
        turns = 0;
    }

    private void Update()
    {
        // have the computer go
        if (!playerTurn && dieReady && turns <= 18 && opponentReady)
        {
            StartCoroutine(OpponentTurn());
        }

        if (playerBoard.IsFull() && playerTurn || opponentBoard.IsFull() && !playerTurn)
        {
            Time.timeScale = 0;
        }
    }

    // ======================================================= //
    // ==================== PLAYER'S TURN ==================== //
    // ======================================================= //
    public void TriggerTileHit(Tile tile)
    {
        // see if it's the player's turn or not
        if (playerTurn)
        {
            // get the column we're working with
            int col = tile.column;

            // go through each row and see where to place the new die
            Tile[,] board = playerBoard.GetTiles();

            // make sure a slot was actually picked, if not, do nothing
            if (!FillNextSlot(board, col))
            {
                return;
            }

            playerTurn = false;
            tile.board.RefreshScore();
            StartCoroutine(RollDie());
        }
    }

    // ========================================================= //
    // ==================== OPPONENT'S TURN ==================== //
    // ========================================================= //
    IEnumerator OpponentTurn()
    {
        // although it's the opponent's turn, they're considered "not ready", as they're currently going
        opponentReady = false;
        int col;

        // artificially have the opponent "think"
        //yield return new WaitForSeconds(Random.value * 1 + 1);

        // now have the opponent fill in the slot
        col = EasyOpponent();
        FillNextSlot(opponentBoard.GetTiles(), col);

        // end the opponent's turn
        playerTurn = true;
        opponentReady = true;
        opponentBoard.RefreshScore();
        StartCoroutine(RollDie());
        yield return new WaitForEndOfFrame();
    }

    // =============================================================== //
    // ==================== OPPONENT DIFFICULTIES ==================== //
    // =============================================================== //
    int EasyOpponent()
    {
        // pick a random, open column
        // columns are open if the third slot is open
        Tile[,] t = opponentBoard.GetTiles();

        // start by getting all the open columns
        List<int> openCols = new();

        for (int i = 0; i < 3; i++)
            if (!t[i, 2].hasValue)
                openCols.Add(i);

        // grab one of them randomly
        int col = openCols[Random.Range(0, openCols.Count)];
        return col;
    }

    // ================================================= //
    // ==================== HELPERS ==================== //
    // ================================================= //

    /// <summary>
    /// Goes through the column and puts the die in the next open slot
    /// </summary>
    /// <param name="board">The board to check</param>
    /// <param name="col">The column to check</param>
    /// <returns>True if the die was placed, false if it couldn't</returns>
    bool FillNextSlot(Tile[,] board, int col)
    {
        for (int i = 0; i < 3; i++)
        {
            if (!board[col, i].hasValue)
            {
                board[col, i].value = currentValue;
                board[col, i].hasValue = true;

                // slot fill depends on whos turn it is
                Sprite sprite = (playerTurn) ? playerSprites[currentValue - 1] : opponentSprites[currentValue - 1];
                board[col, i].UpdateSprite(sprite);

                CancelOpposingDice(
                    playerTurn ? opponentBoard : playerBoard,
                    currentValue,
                    col
                );

                return true;
            }
        }
        return false;
    }

    void CancelOpposingDice(Board opposingBoard, int valuePlaced, int column)
    {
        // remove tiles
        Tile[,] opposingTiles = opposingBoard.GetTiles();
        for (int i = 0; i < 3; i++)
        {
            if (opposingTiles[column, i].hasValue && opposingTiles[column, i].value == valuePlaced)
            {
                opposingTiles[column, i].value = 0;
                opposingTiles[column, i].hasValue = false;
                opposingTiles[column, i].UpdateSprite(emptySprite);
            }
        }

        // move tiles up
        if (!opposingTiles[column, 0].hasValue && opposingTiles[column, 1].hasValue)
            opposingTiles[column, 0].SwapAssets(opposingTiles[column, 1]);
        if (!opposingTiles[column, 1].hasValue && opposingTiles[column, 2].hasValue)
            opposingTiles[column, 1].SwapAssets(opposingTiles[column, 2]);
        if (!opposingTiles[column, 0].hasValue && opposingTiles[column, 1].hasValue)
            opposingTiles[column, 0].SwapAssets(opposingTiles[column, 1]);

        opposingBoard.RefreshScore();
    }

    IEnumerator RollDie()
    {
        dieReady = false;
        for (int i = 0;i < 10;i++) 
        {
            currentValue = Random.Range(1, 7);
            rollDisplay.sprite = matchSprites[currentValue - 1];
            yield return new WaitForSeconds(0.1f);
        }
        dieReady = true;
    }
}
