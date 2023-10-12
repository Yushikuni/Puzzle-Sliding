using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;

    private int emptyLocationLocal = -1;
    private int sizeGame;

    private bool isShuffling = false;
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
        emptyLocationLocal = (sizeGame * sizeGame) - 1;
        sizeGame = 4;
        CreateGamePieces(0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isShuffling /*&& CheckingGameComlition()*/)
        {
            isShuffling = true;
            StartCoroutine(WaitForShuffle(0.5f));
        }
        if(Input.GetKey(KeyCode.Space)) 
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
       /* if(((i % sizeGame) != checkCollum) && ((i + offset) == emptyLocationLocal))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            (pieces[i].localPosition, pieces[i + offset].localPosition) = (pieces[i + offset].localPosition, pieces[i].localPosition);

            emptyLocationLocal = i;

            return true;
        }
        return false;*/
       return true;
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
        /*
        //int shuffleCount = 100;
        int last = 0;
        int count = 0;
        int rnd = Random.Range(0, sizeGame * sizeGame);
        while (count < (sizeGame * sizeGame * sizeGame)) 
        { 
            if (rnd == last) 
            { 
                continue; 
            }

            last = emptyLocationLocal;

            if(SwapIsValid(rnd, -sizeGame, sizeGame))
            { 
                count++; 
            }
            else if (SwapIsValid(rnd, +sizeGame, sizeGame))
            { 
                count++; 
            }
            else if((SwapIsValid(rnd, -1, 0)))
            { 
                count++;
            }
            else if ((SwapIsValid(rnd, +1, sizeGame - 1)))
            {
                count++;
            }
        }
        */

        Debug.Log("Every day I am shoffling");
    }
}
