using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameBoard;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;
    //private List<Moves> possibleMoves;

    private int emptyLocationLocal;
    public int sizeGame;

    private bool isShuffling = false;
    /*int,int indexToCoords(int tileIndex);
    int CoordsToIndex(int X, int Y);*/
    /*
     For more pictures:
    Hello, is it possible to make a tutorial where you do this with 3 or more pictures? Like after a picture is solved, the level is completed, and a new picture puzzle comes in
    Odpovìdìt
    Firnox
·2 odpovìdi
        @FirnoxGames
        @FirnoxGames
        pøed 6 mìsíci
        Hey, the part of the code where the image is assigned to the piece is the mr.sharedMaterial = newMat line. So to do this what you could do to do this is change the definition of this into a list: [SerializeField] private List<Material> newMat; And assign several materials with your different images on and assign them in the inspector. Then in the WaitShuffle method after waiting but before the shuffle choose a random image (or the next one) and assign it to each of the pieces:
        foreach (Transform piece in pieces):
          MeshRenderer mr = piece.GetComponent<MeshRenderer>();
          mr.sharedMaterial = newMat[index_of_the_material_you_have_chosen];
     */
    /*
     TODO:
    ----------------------------------------------------------------
    1. Fixnout metodu Shuffle s tím co mám

    ----------------------------------------------------------------
    1. Fix mouse button
    2. fixing freezing of unity 3d in some methods
    3. Add more pictures!
    4. Fix visiblity of chunks
     */
    private void CreateGamePieces(float Gap)
    {
        float width = 1 / (float)sizeGame;
        for (int row = 0; row < sizeGame; row++) 
        { 
            for (int col = 0; col< sizeGame; col++) 
            {
                Transform piece = Instantiate(piecePrefab, gameBoard);
                
                pieces.Add(piece); 

                piece.localPosition = new Vector3(-1 + (2 * width * col) + width, +1 - (2 * width * row) - width, 0);
                piece.localScale = ((2 * width) - Gap) * Vector3.one;
                piece.name = $"{(row * sizeGame) + col}";
                if((row == sizeGame - 1) && (col == sizeGame - 1))
                {
                    //emptyLocationLocal = (sizeGame * sizeGame) - 1;
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
        if(sizeGame > 0) 
        {
            pieces = new List<Transform>();
            emptyLocationLocal = (sizeGame * sizeGame) - 1;

            CreateGamePieces(0.01f);

            //StartCoroutine(WaitForShuffle(0.5f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WaitForShuffle(1f));
        /*
        if (!isShuffling && CheckingGameComlition())
        {
            isShuffling = true;
            StartCoroutine(WaitForShuffle(0.5f));
            //Debug.Log("Stop shuffle");
            
        }*/
        if (Input.GetMouseButtonDown(0)) // 0 mark left mouse button
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit == hit.transform)
            {
                Debug.Log("Uvnitr fce hit je na true");
               for (int i = 0; i < pieces.Count; i++)
               {
                    //Debug.Log("Uvnitr For cyklu");
                    if (pieces[i] == hit.transform)
                    {
                        if(SwapIsValid(i, -sizeGame, sizeGame))
                        {
                            Debug.Log("T/F: SwapIsValid(i, -sizeGame, sizeGame): " + SwapIsValid(i, -sizeGame, sizeGame));  // down to up ^
                            Debug.Log("EmptyLocationLocal on i: " + emptyLocationLocal + i.ToString());
                            //SwapPieces(/*ColorIndex, BlankIndex*/)
                            SwapPieces(emptyLocationLocal, i);
                            break;
                        }
                        else if (SwapIsValid(i, +sizeGame, sizeGame))
                        {
                            Debug.Log("T/F: SwapIsValid(i, +sizeGame, sizeGame): " + SwapIsValid(i, +sizeGame, sizeGame));  // up to down ˇ
                            Debug.Log("EmptyLocationLocal on i: " + emptyLocationLocal + i.ToString());
                            //SwapPieces(/*ColorIndex, BlankIndex*/)
                            SwapPieces(emptyLocationLocal, i);
                            break;
                        }

                        else if (SwapIsValid(i, -1, 0))
                        {
                            Debug.Log("T/F: SwapIsValid(i, -1, 0):               " + SwapIsValid(i, -1, 0));                // right to left <-
                            Debug.Log("EmptyLocationLocal on i: " + emptyLocationLocal + i.ToString());
                            //SwapPieces(/*ColorIndex, BlankIndex*/)
                            SwapPieces(emptyLocationLocal, i);
                            break;
                        }
                        else if (SwapIsValid(i, +1, sizeGame - 1))
                        {
                            Debug.Log("T/F: SwapIsValid(i, +1, sizeGame - 1):    " + SwapIsValid(i, +1, sizeGame - 1));     // left to right ->
                            Debug.Log("EmptyLocationLocal on i: " + emptyLocationLocal + i.ToString());
                            //SwapPieces(/*ColorIndex, BlankIndex*/)
                            SwapPieces(emptyLocationLocal, i);
                            break;
                        }
                    }
               }
            }
        }
    }

    //swap je validni pokud je cerne policko vedle necerneho a zaroven neni mimo herni pole
    private bool SwapIsValid(int i, int offset, int checkCollum)
    {
        int origin = i;
        int destination = i + offset;
        //pravy a levy sloupec
        bool collCheck = (origin % sizeGame != checkCollum);
        //pole nejsou  mimo zacatek hernho pole
        bool originBottomBounds = (origin >=0);
        bool detinationBottomBounds=(destination >=0);
        //pole nejsou mimo konec seznamu
        bool originOutOfBounds = (origin < (sizeGame * sizeGame));
        bool destinationOutOfBounds = (destination < (sizeGame * sizeGame));
        //Cilove pole je prazdne pole
        bool destionationIsEmpty = (destination == emptyLocationLocal); 
        return
        (collCheck && originBottomBounds && detinationBottomBounds && originOutOfBounds && destinationOutOfBounds && destionationIsEmpty);
           
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
        Shuffle();

        yield return new WaitForSeconds(waitingTime);  
    }
    //TODO: Do some Shuffling with cards do not use BRUTAL!
    private void Shuffle()
    {

        for (int i = 0; i < pieces.Count - 1; i++)
        {
            int randomMove = Random.Range(0, 4);
            //Debug.Log("Random mode range: " + randomMove);

            int targetIndex;
            if (randomMove == 0 && SwapIsValid(emptyLocationLocal, emptyLocationLocal - sizeGame, sizeGame))
            {
                targetIndex = emptyLocationLocal - sizeGame;
                SwapPieces(emptyLocationLocal, targetIndex);
            }
            else if (randomMove == 1 && SwapIsValid(emptyLocationLocal, emptyLocationLocal + sizeGame, sizeGame))
            {
                targetIndex = emptyLocationLocal + sizeGame;
                SwapPieces(emptyLocationLocal, targetIndex);
            }
            else if (randomMove == 2 && SwapIsValid(emptyLocationLocal, emptyLocationLocal - 1, 0))
            {
                targetIndex = emptyLocationLocal - 1;
                SwapPieces(emptyLocationLocal, targetIndex);
            }
            else if (randomMove == 3 && SwapIsValid(emptyLocationLocal, emptyLocationLocal + 1, sizeGame - 1))
            {
                targetIndex = emptyLocationLocal + 1;
                SwapPieces(emptyLocationLocal, targetIndex);
            }
        }
    }

    private void SwapPieces(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < pieces.Count && toIndex >= 0 && toIndex < pieces.Count)
        {
            // Provede výmìnu dílkù v seznamu dílkù a aktualizaci jejich pozice.
            (pieces[fromIndex], pieces[toIndex]) = (pieces[toIndex], pieces[fromIndex]);
            (pieces[fromIndex].localPosition, pieces[toIndex].localPosition) = (pieces[toIndex].localPosition, pieces[fromIndex].localPosition);
            emptyLocationLocal = toIndex;
        }            
    }

    /* int,int indexToCoords(int tileIndex)
     {
         return tileIndex;
     }
     int CoordsToIndex(int X, int Y)
     {
         return X,Y;
     }*/

}
