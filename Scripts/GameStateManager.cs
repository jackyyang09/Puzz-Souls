using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    float orbSize = 180;

    [SerializeField]
    float orbPickedSize;

    [Tooltip("Usually keep at 3 for match 3 games")]
    [SerializeField]
    int matchRequirement = 3;

    [SerializeField]
    GameObject[] orbPrefabs;

    [SerializeField]
    float maxMoveTime;

    [SerializeField]
    float remainingMoveTime;

    [SerializeField]
    RectTransform gemBoard;

    [SerializeField]
    int boardHorizontal = 6;

    [SerializeField]
    int boardVertical = 5;

    List<GameObject> comboOrbs;

    GemBehaviour[,] orbBoard;
    bool[,] orbChecked;

    public static GameStateManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != this)
        {
            if (instance != null) Destroy(instance);
            instance = this;
        }
    }

    private void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        orbBoard = new GemBehaviour[boardHorizontal, boardVertical];
        orbChecked = new bool[boardHorizontal, boardVertical];
        for (int y = 0; y < boardVertical; y++)
        {
            for (int x = 0; x < boardHorizontal; x++)
            {
                Vector2 pos = new Vector2(-450 + x * orbSize, -390 + y * orbSize);
                GameObject orb = Instantiate(orbPrefabs[Random.Range(0, orbPrefabs.Length)], Vector3.zero, Quaternion.identity, transform);
                orb.GetComponent<RectTransform>().anchoredPosition = pos;
                GemBehaviour gem = orb.GetComponent<GemBehaviour>();
                gem.SetTilePosition(new Vector2(x, y));
                orbBoard[x, y] = gem;
            }
        }
    }

    public void ReadBoard()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GemBehaviour gem = transform.GetChild(i).GetComponent<GemBehaviour>();
            Vector2 index = gem.GetTileIndex();
            orbBoard[(int)index.x, (int)index.y] = gem;
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public Vector2 TileToRectSpace(int x, int y)
    {
        return new Vector2(-450 + x * orbSize, -390 + y * orbSize);
    }

    public Vector2 TileToRectSpace(Vector2 tile)
    {
        return new Vector2(-450 + tile.x * orbSize, -390 + tile.y * orbSize);
    }

    public void FindMatches()
    {
        ReadBoard();
        StartCoroutine(ClearOrbs());
    }

    IEnumerator ClearOrbs()
    {
        for (int y = 0; y < boardVertical; y++)
        {
            for (int x = 0; x < boardHorizontal; x++)
            {
                if (!orbChecked[x, y])
                {
                    comboOrbs = new List<GameObject>();
                    int vertical = 0;
                    vertical += CheckUpwards(new Vector2(x, y), orbBoard[x, y].GetOrbType());
                    vertical += CheckDownwards(new Vector2(x, y - 1), orbBoard[x, y].GetOrbType());

                    if (vertical >= matchRequirement)
                    {
                        foreach (GameObject g in comboOrbs)
                        {
                            Destroy(g);
                        }
                        yield return new WaitForSeconds(2);
                    }

                    comboOrbs = new List<GameObject>();

                    int horizontal = 0;
                    horizontal += CheckLeftwards(new Vector2(x, y), orbBoard[x, y].GetOrbType());
                    horizontal += CheckRightwards(new Vector2(x + 1, y), orbBoard[x, y].GetOrbType());

                    if (horizontal >= matchRequirement)
                    {
                        foreach (GameObject g in comboOrbs)
                        {
                            Destroy(g);
                        }
                        yield return new WaitForSeconds(2);
                    }
                }
            }
        }
    }

    int CheckUpwards(Vector2 index, OrbType orbType)
    {
        if (index.y >= 0 && index.y < boardVertical)
        {
            GemBehaviour thisOrb = orbBoard[(int)index.x, (int)index.y];
            if (thisOrb.GetOrbType() == orbType)
            {
                comboOrbs.Add(thisOrb.gameObject);
                return 1 + CheckUpwards(index + Vector2.up, orbType);
            }
        }
        return 0;
    }

    int CheckDownwards(Vector2 index, OrbType orbType)
    {
        if (index.y >= 0 && index.y < boardVertical)
        {
            GemBehaviour thisOrb = orbBoard[(int)index.x, (int)index.y];
            if (thisOrb.GetOrbType() == orbType)
            {
                comboOrbs.Add(thisOrb.gameObject);
                return 1 + CheckDownwards(index + Vector2.down, orbType);
            }
        }
        return 0;
    }

    int CheckLeftwards(Vector2 index, OrbType orbType)
    {
        if (index.x >= 0 && index.x < boardHorizontal)
        {
            GemBehaviour thisOrb = orbBoard[(int)index.x, (int)index.y];
            if (thisOrb.GetOrbType() == orbType)
            {
                comboOrbs.Add(thisOrb.gameObject);
                return 1 + CheckLeftwards(index + Vector2.left, orbType);
            }
        }
        return 0;
    }

    int CheckRightwards(Vector2 index, OrbType orbType)
    {
        if (index.x >= 0 && index.x < boardHorizontal)
        {
            GemBehaviour thisOrb = orbBoard[(int)index.x, (int)index.y];
            if (thisOrb.GetOrbType() == orbType)
            {
                comboOrbs.Add(thisOrb.gameObject);
                return 1 + CheckRightwards(index + Vector2.right, orbType);
            }
        }
        return 0;
    }
}