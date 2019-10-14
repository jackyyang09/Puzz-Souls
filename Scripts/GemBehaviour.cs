using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbType
{
    Estus,
    Sword,
    Shield,
    Boot,
    Jammer
}

public class GemBehaviour : MonoBehaviour
{
    [SerializeField]
    Vector2 indexOnBoard;

    [SerializeField]
    OrbType orbType = OrbType.Estus;

    bool orbPicked;

    RectTransform rect;

    GameStateManager game;

    int savedSiblingIndex = -1;

    GameObject lastSwapped = null;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();

        game = GameStateManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (orbPicked) {
            transform.position = Input.mousePosition;
        }
    }

    public void PickUpOrb()
    {
        orbPicked = true;
        savedSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        gameObject.name = "THIS";
    }

    public void ReleaseOrb()
    {
        orbPicked = false;
        transform.SetSiblingIndex(savedSiblingIndex);
        ApplyPositionOnBoard();
        game.FindMatches();
    }

    public void SetTilePosition(Vector2 newIndex)
    {
        indexOnBoard = newIndex;
    }

    public void ApplyPositionOnBoard() {
        rect.anchoredPosition = game.TileToRectSpace(indexOnBoard);
    }

    public Vector2 GetTileIndex()
    {
        return indexOnBoard;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!orbPicked) return;
        //if (lastSwapped == collision.gameObject) return;
        GemBehaviour neighbour = collision.GetComponent<GemBehaviour>();

        Vector2 newTilePos = neighbour.GetTileIndex();

        neighbour.SetTilePosition(indexOnBoard);
        neighbour.ApplyPositionOnBoard();

        indexOnBoard = newTilePos;
    }

    public OrbType GetOrbType()
    {
        return orbType;
    }
}