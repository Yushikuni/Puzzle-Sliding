using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

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
        sizeGame = 10;
        CreateGamePieces(0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
