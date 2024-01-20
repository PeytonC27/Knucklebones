using UnityEngine;

public class Tile : MonoBehaviour
{
    public int column;
    public int value;
    public bool hasValue = false;
    public Board board;

    GameManager gameManager;
    SpriteRenderer spriteRenderer;
    GameObject border;
    public bool isPlayableTile = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isPlayableTile)
        {
            border = transform.GetChild(0).gameObject;
            border.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                gameManager.TriggerTileHit(hit.transform.gameObject.GetComponent<Tile>());
            }
        }
    }

    /// <summary>
    /// Changes the sprite of this tile
    /// </summary>
    /// <param name="newSprite"></param>
    public void UpdateSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    public GameObject GetParent()
    {
        return board.gameObject;
    }

    /// <summary>
    /// Swaps two tiles in the same column on the same board
    /// </summary>
    /// <param name="other"></param>
    public void SwapAssets(Tile other)
    {
        int tempVal = other.value;
        bool tempHasValue = other.hasValue;
        Sprite tempSprite = other.spriteRenderer.sprite;

        other.value = value;
        other.hasValue = hasValue;
        other.UpdateSprite(spriteRenderer.sprite);

        value = tempVal;
        hasValue = tempHasValue;
        spriteRenderer.sprite = tempSprite;
    }

    public void EnableBorder()
    {
        border.SetActive(true);
    }

    public void DisableBorder()
    {
        border.SetActive(false);
    }
}
