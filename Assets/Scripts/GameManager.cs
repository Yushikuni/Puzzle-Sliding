using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameBoard;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;

    private int emptyLocationLocal;
    private int sizeGame;

    private bool isShuffling = false;
    /*int,int indexToCoords(int tileIndex);
    int CoordsToIndex(int X, int Y);*/
    /*
     For more pictures:
    Hello, is it possible to make a tutorial where you do this with 3 or more pictures? Like after a picture is solved, the level is completed, and a new picture puzzle comes in
    Odpov�d�t
    Firnox
�2 odpov�di
        @FirnoxGames
        @FirnoxGames
        p�ed 6 m�s�ci
        Hey, the part of the code where the image is assigned to the piece is the mr.sharedMaterial = newMat line. So to do this what you could do to do this is change the definition of this into a list: [SerializeField] private List<Material> newMat; And assign several materials with your different images on and assign them in the inspector. Then in the WaitShuffle method after waiting but before the shuffle choose a random image (or the next one) and assign it to each of the pieces:
        foreach (Transform piece in pieces):
          MeshRenderer mr = piece.GetComponent<MeshRenderer>();
          mr.sharedMaterial = newMat[index_of_the_material_you_have_chosen];
     */
    /*
     TODO:
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
        pieces = new List<Transform>();
        
        sizeGame = 5;        
        emptyLocationLocal = (sizeGame * sizeGame) - 1;
                
        CreateGamePieces(0.01f);

        SwapPieces(24, 23);
        //Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isShuffling && CheckingGameComlition())
        {
            isShuffling = true;
            StartCoroutine(WaitForShuffle(0.5f));
           // Debug.Log("Stop shuffle");
            
        }
        if (Input.GetMouseButtonDown(0)) // 0 zna�� lev� tla��tko my�i
        {
            // Zde m��ete p�idat vol�n� funkce pro pohyb d�lk� nebo jinou reakci na stisk my�i
            // Nap��klad: HandleMouseClick();
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit == hit.transform)
            {
                //Debug.Log("Uvnitr fce hit je na true");
                for (int i = 0; i < pieces.Count; i++)
                {
                    //Debug.Log("Uvnitr For cyklu");
                    if (pieces[i] == hit.transform)
                    {
                        //Debug.Log("kousek na kteryx jsem klikla je roven hit.transform");
                        if (SwapIsValid(i, -sizeGame, sizeGame)) 
                        {
                            //Debug.Log("Tu je brak na i, -sizeGame, sizeGame "); 
                            break; 
                        }
                        if (SwapIsValid(i, +sizeGame, sizeGame)) 
                        { 
                            //Debug.Log("Tu je brak na i, +sizeGame, sizeGame "); 
                            break; 
                        }
                        if (SwapIsValid(i, -1, 0)) 
                        { 
                            //Debug.Log("Tu je brak na SwapIsValid(i, -1, 0) "); 
                            break; 
                        }
                        if (SwapIsValid(i, +1, sizeGame - 1)) 
                        { 
                            //Debug.Log("Tu je brak na i, +1, sizeGame - 1) "); 
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
        // celkom 6 podminek
        //pozice 23 a offset je 5
        bool isNotOffset = i + offset < (sizeGame * sizeGame); // jsi v poli //OK
        bool anotherBool = i >= 0;
        bool isOffset = i < (sizeGame * sizeGame); //jsi v poli
        bool nani = (i + offset >= 0); //OK
        bool co1 = (i + offset == emptyLocationLocal); //OK 
        bool co2 = (i % sizeGame != checkCollum); //pravy a levy sloupec
        
        bool result = co1 && co2 && isNotOffset && nani && isOffset && anotherBool;
        return result;
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
        int shuffleCount = 100; // Po�et tah� pro zam�ch�n�
        for (int i = 0; i < shuffleCount; i++)
        {
            // Zde provedete n�hodn� platn� tah, nap��klad jako v p�edchoz�m k�du
            int randomMove = Random.Range(0, 4);

            if (randomMove == 0 && SwapIsValid(emptyLocationLocal, emptyLocationLocal - sizeGame, sizeGame))
            {
                SwapPieces(emptyLocationLocal, emptyLocationLocal - sizeGame);
            }
            else if (randomMove == 1 && SwapIsValid(emptyLocationLocal, emptyLocationLocal + sizeGame, sizeGame))
            {
                SwapPieces(emptyLocationLocal, emptyLocationLocal + sizeGame);
            }
            else if (randomMove == 2 && SwapIsValid(emptyLocationLocal, emptyLocationLocal - 1, 0))
            {
                SwapPieces(emptyLocationLocal, emptyLocationLocal - 1);
            }
            else if (randomMove == 3 && SwapIsValid(emptyLocationLocal, emptyLocationLocal + 1, sizeGame - 1))
            {
                SwapPieces(emptyLocationLocal, emptyLocationLocal + 1);
            }

        }

        //Debug.Log("Every day I am shoffling");
    }
    //exception out of index range
    private void SwapPieces(int fromIndex, int toIndex)
    {
        // Provede v�m�nu d�lk� v seznamu d�lk� a aktualizaci jejich pozice.
        (pieces[fromIndex], pieces[toIndex]) = (pieces[toIndex], pieces[fromIndex]);
        (pieces[fromIndex].localPosition, pieces[toIndex].localPosition) = (pieces[toIndex].localPosition, pieces[fromIndex].localPosition);
        emptyLocationLocal = toIndex;
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
