using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] matchSprites = new Sprite[6];
    [SerializeField] Sprite[] playerSprites = new Sprite[6];
    [SerializeField] Sprite[] opponentSprites = new Sprite[6];
    [SerializeField] Sprite emptySprite;

    [Header("End Screen")]
    [SerializeField] Canvas endScreen;

    [Header("Sound")]
    [SerializeField] GameObject audioManager;


    AudioManager audioPlayer;
    Board opponentBoard;
    Board playerBoard;
    SpriteRenderer rollDisplay;
    OpponentAI ai;

    int currentValue;
    bool playerTurn = true;
    bool dieReady;
    bool opponentReady = true;

    EndScreenManager endScreenManager;

    private void Start()
    {
        Debug.Log(HardMode.isHardMode);

        // get the sound manager
        audioPlayer = audioManager.GetComponent<AudioManager>();

        // set up the boards
        opponentBoard = GetComponentsInChildren<Board>()[0];
        playerBoard = GetComponentsInChildren<Board>()[1];

        Color opponentColor = new Color32(221, 91, 113, 255); // #dd5b71
        Color playerColor = new Color32(119, 179, 254, 255);  // #77b3fe

        opponentBoard.SetScoreboardColor(opponentColor);
        playerBoard.SetScoreboardColor(playerColor);

        // setup the AI
        ai = GetComponent<OpponentAI>();
        ai.SetBoards(playerBoard, opponentBoard);

        // set up the UIs
        rollDisplay = GetComponentInChildren<SpriteRenderer>();

        endScreenManager = endScreen.GetComponent<EndScreenManager>();
        endScreenManager.DisableEndScreen();

        // start the game
        StartCoroutine(RollDie());
    }

    private void Update()
    {
        // have the computer go
        if (!playerTurn && dieReady && opponentReady)
        {
            StartCoroutine(OpponentTurn());
        }

        if (playerBoard.IsFull() && playerTurn || opponentBoard.IsFull() && !playerTurn)
        {

            // display stats
            string winner = playerBoard.Score > opponentBoard.Score ? "You" : "The opponent";
            endScreenManager.EnableEndScreen(winner, playerBoard.Score, opponentBoard.Score);

            // show the restart button and the darker background
            endScreen.transform.GetChild(2).gameObject.SetActive(true);
            endScreen.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    // ======================================================= //
    // ==================== PLAYER'S TURN ==================== //
    // ======================================================= //
    public void TriggerTileHit(Tile tile)
    {
        // see if it's the player's turn or not
        if (playerTurn && dieReady)
        {
            // get the column we're working with
            int col = tile.column;

            // go through each row and see where to place the new die
            Board board = playerBoard;

            // make sure a slot was actually picked, if not, do nothing
            if (!FillNextSlot(board, col))
            {
                return;
            }

            playerTurn = false;
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
        yield return new WaitForSeconds(Random.value * 0.5f + 0.5f);

        // now have the opponent fill in the slot
        col = ai.PlaceDie(HardMode.isHardMode, currentValue);
        FillNextSlot(opponentBoard, col);

        // end the opponent's turn
        playerTurn = true;
        opponentReady = true;
        StartCoroutine(RollDie());
        yield return new WaitForEndOfFrame();
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
    bool FillNextSlot(Board board, int col)
    {
        Tile[,] tiles = board.GetTiles();
        for (int i = 0; i < 3; i++)
        {
            if (!tiles[col, i].hasValue)
            {
                tiles[col, i].value = currentValue;
                tiles[col, i].hasValue = true;

                // slot fill depends on whos turn it is
                Sprite sprite = (playerTurn) ? playerSprites[currentValue - 1] : opponentSprites[currentValue - 1];
                tiles[col, i].UpdateSprite(sprite);

                bool capture = CancelOpposingDice(
                    playerTurn ? opponentBoard : playerBoard,
                    currentValue,
                    col
                );

                bool bonusTriggered = board.RefreshScore(currentValue, col);

                if (capture)
                    audioPlayer.PlayCaptureSound();
                else
                    audioPlayer.PlayDiePlacement(bonusTriggered);

                return true;
            }
        }
        return false;
    }

    bool CancelOpposingDice(Board opposingBoard, int valuePlaced, int column)
    {
        // remove tiles
        bool wasCancellation = false;
        Tile[,] opposingTiles = opposingBoard.GetTiles();
        for (int i = 0; i < 3; i++)
        {
            if (opposingTiles[column, i].hasValue && opposingTiles[column, i].value == valuePlaced)
            {
                opposingTiles[column, i].value = 0;
                opposingTiles[column, i].hasValue = false;
                opposingTiles[column, i].UpdateSprite(emptySprite);
                wasCancellation = true;
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
        return wasCancellation;
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
