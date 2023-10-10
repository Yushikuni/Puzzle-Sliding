using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;

    private int emptyLocationLocal;
    private int sizeGame;

    private bool isShuffling = false;

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
                    emptyLocationLocal = (sizeGame * sizeGame) - 1;
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
        if(!isShuffling && CheckingGameComlition())
        {
            isShuffling = true;
            //StartCorountine(WaitForShuffle(0.5f));
        }
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
        if(((i % sizeGame) != checkCollum) && ((i + offset) == emptyLocationLocal))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            (pieces[i].localPosition, pieces[i + offset].localPosition) = (pieces[i + offset].localPosition, pieces[i].localPosition);

            emptyLocationLocal = i;

            return true;
        }
        return false;
    }

    // Only after move piece of picture call this method
    private bool CheckingGameComlition()
    {
        for (int i = 0; i < pieces.Count; i++ )
        {
            if (pieces[i].name != $"{i}")
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator WaitForShuffle(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        Shuffle();
        isShuffling = false;
    }
    //TODO: Do some Shuffling with cards do not use BRUTAL!
    private void Shuffle()
    {
        int shuffleCount = 100;

        for(int i=0; i < shuffleCount; ++i) 
        {
            int randomPieceIndex = Random.Range(0, pieces.Count);

            if (CanMovePiece(pieces[randomPieceIndex], out emptyLocationLocal))
            {
                int emptyRow = emptyLocationLocal / sizeGame;
                int emptyCol = emptyLocationLocal % sizeGame;

                int pieceRow = randomPieceIndex / sizeGame;
                int pieceCol = randomPieceIndex % sizeGame;

                // Zkontrolujeme, zda je vybraný kus sousedem prázdného místa
                if (Mathf.Abs(pieceRow - emptyRow) + Mathf.Abs(pieceCol - emptyCol) == 1)
                {
                    SwapIsValid(randomPieceIndex, 0, emptyLocationLocal);
                }
            }
        }
    }

    private bool CanMovePiece(Transform piece, out int emptyLocation)
    {
        emptyLocation = -1;
        int pieceIndex = pieces.IndexOf(piece);
        if (pieceIndex == -1)
        {
            // Pokud komponenta kusu není v seznamu, považujeme ji za neplatnou
            return false;
        }

        int emptyRow = emptyLocation / sizeGame;
        int emptyCol = emptyLocation % sizeGame;

        int pieceRow = pieceIndex / sizeGame;
        int pieceCol = pieceIndex % sizeGame;

        // Zkontrolujeme, zda je kus sousedem prázdného místa
        if (Mathf.Abs(pieceRow - emptyRow) + Mathf.Abs(pieceCol - emptyCol) == 1)
        {
            // Nastavíme prázdnou lokaci na index kusu
            emptyLocation = pieceIndex;
            return true;
        }

        return false;
    }
}
