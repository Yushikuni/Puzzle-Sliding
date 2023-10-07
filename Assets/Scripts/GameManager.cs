using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;

    private int emptyLocation;
    private int sizeGame;

    private void CreateGamePieces(float Gap)
    {
        float width = 1 / (float)sizeGame;
        for (int row = 0; row < sizeGame; row++) 
        { 
            for (int col = 0; col< sizeGame; col++) 
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                
                pieces.Add(piece); 

                piece.localPosition = new Vector3(-1 + (2 * width * col) + width, +1 - (2 * width * row) - width, 0);
                piece.localScale = ((2 * width) - Gap) * Vector3.one;
                piece.name = $"{(row * sizeGame) + col}";
                if((row == sizeGame - 1) && (col == sizeGame - 1))
                {
                    emptyLocation = (sizeGame * sizeGame) - 1;
                    piece.gameObject.SetActive(false); 
                }
                else
                {
                    float halfGap = Gap / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4]; //UV -> (0,1) (1,1) (0,0) (1,0)
                    uv[0] = new Vector2((width * col)+ halfGap, 1 - ((width * (row + 1)) - halfGap));
                    uv[1] = new Vector2((width * (col + 1)) - halfGap, 1 - ((width * (row + 1)) - halfGap));
                    uv[2] = new Vector2((width * col) + halfGap, 1 - ((width * row) + halfGap)); 
                    uv[3] = new Vector2((width * (col + 1)) - halfGap, 1 - ((width * row) + halfGap));
                    mesh.uv = uv;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new List<Transform>();
        sizeGame = 4;
        CreateGamePieces(0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToViewportPoint(Input.mousePosition),Vector2.zero);

            if(hit)
            {
                for(int i = 0; i < pieces.Count; i++) 
                {
                    if (pieces[i] == hit.transform)
                    {
                        if (SwapIsValid(i, -sizeGame, sizeGame)) { break; }
                        if (SwapIsValid(i, +sizeGame, sizeGame)) { break; }
                        if (SwapIsValid(i, -1, 0)) { break; }
                        if (SwapIsValid(i, +1, sizeGame - 1)) { break; }
                    }
                }
            }
        }
    }

    private bool SwapIsValid(int i, int offset, int checkCollum)
    {
        if(((i % sizeGame) != checkCollum) && ((i + offset) == emptyLocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            (pieces[i].localPosition, pieces[i + offset].localPosition) = (pieces[i + offset].localPosition, pieces[i].localPosition);

            emptyLocation = i;

            return true;
        }
        return false;
    }
}
