using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour {

    public Text ScoreText;

    public Color32[] gamecolors = new Color32[4];
    public GameObject endPanel;
    public Material stackmat;
    private const float Stack_moving_speed = 5.0f;
    private const float Bounds_size = 3.5f;
    private Vector3 desiredposition;
    private bool isMovingOnx = true;
    private float secondaryposition;
    private GameObject[] thestack;
    private int scoreCount = 0;
    private int stackIndex;
    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private Vector3 lasttilepositon;
    private const float Error_margin = 0.1f;
    private int combo = 0;
    private bool gameover = false;
    private float Stack_bounds_gain = 0.25f;
    private const int combo_start_gain = 3;


    private Vector2 stackbounds = new Vector2(Bounds_size, Bounds_size);

	private void Start () {

        thestack = new GameObject[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            thestack[i] = transform.GetChild(i).gameObject;
            ColorMesh(thestack[i].GetComponent<MeshFilter>().mesh);
           
        }
        stackIndex = transform.childCount - 1;
	}
	
	private void Update () {


        if (Input.GetMouseButtonDown(0))
        {
            if (gameover)
                return;

            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
                ScoreText.text = scoreCount.ToString();
            }
            else
            {
                EndGame();
            }
        }
        MoveTile();
        // moving the stack
        transform.position = Vector3.Lerp(transform.position, desiredposition,Stack_moving_speed*Time.deltaTime);

	}

    private void CreateRubble(Vector3 pos,Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponent<MeshRenderer>().material = stackmat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);

    }

    private Color32 Lerp4(Color32 a,Color32 b,Color32 c,Color32 d,float t )
    {
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
    }
   


    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        for(int i=0;i<vertices.Length; i++)
        {
            float f = Mathf.Sin(scoreCount * 0.25f);
            colors[i] = Lerp4(gamecolors[0], gamecolors[1], gamecolors[2], gamecolors[3],f);
            mesh.colors32 = colors;
        }
    }

    private void MoveTile()
    {

        tileTransition += Time.deltaTime * tileSpeed;
        if(isMovingOnx)
            thestack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * Bounds_size, scoreCount, secondaryposition);
        else
            thestack[stackIndex].transform.localPosition = new Vector3(secondaryposition, scoreCount, Mathf.Sin(tileTransition) * Bounds_size);
    }

    private void SpawnTile()
    {
        lasttilepositon = thestack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredposition = (Vector3.down) * scoreCount;
        thestack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        thestack[stackIndex].transform.localScale = new Vector3(stackbounds.x, 1, stackbounds.y);

        ColorMesh(thestack[stackIndex].GetComponent<MeshFilter>().mesh);

    }
    private bool PlaceTile()
    {
        Transform t = thestack[stackIndex].transform;
        if(isMovingOnx)
        {
            float deltax = lasttilepositon.x - t.position.x;
            if(Mathf.Abs(deltax)>Error_margin)
            {
                // cutting tile
                combo = 0;
                stackbounds.x -= Mathf.Abs(deltax);
                if (stackbounds.x <= 0)
                    return false;
                float middle = lasttilepositon.x + t.localPosition.x / 2;
                t.localScale=new Vector3(stackbounds.x, 1, stackbounds.y);
                CreateRubble(new Vector3((t.position.x>0)
                    ?t.position.x + (t.localScale.x / 2):t.position.x-(t.localScale.x/2)
                    ,t.position.y, t.position.z)
                    , new Vector3(Mathf.Abs(deltax), 1, t.localScale.z));
                t.localPosition = new Vector3(middle - (lasttilepositon.x / 2), scoreCount, lasttilepositon.z);
            }
            else
            {
                if(combo>combo_start_gain)
                {
                    stackbounds.x += Stack_bounds_gain;
                    if (stackbounds.x > Bounds_size)
                        stackbounds.x = Bounds_size;

                    float middle = lasttilepositon.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackbounds.x, 1, stackbounds.y);
                    t.localPosition = new Vector3(middle - (lasttilepositon.x / 2), scoreCount, lasttilepositon.z);

                }
                combo++;
                t.localPosition = lasttilepositon + Vector3.up;
            }
             
        }
        else
        {
            float deltaz = lasttilepositon.z - t.position.z;
            if (Mathf.Abs(deltaz) > Error_margin)
            {
                // cutting tile
                combo = 0;
                stackbounds.y -= Mathf.Abs(deltaz);
                if (stackbounds.y <= 0)
                    return false;
                float middle = lasttilepositon.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackbounds.x, 1, stackbounds.y);
                CreateRubble(new Vector3(t.position.x
                   , t.position.y
                   , (t.position.z > 0)
                   ? t.position.z + (t.localScale.z / 2) : t.position.z - (t.localScale.z / 2))
                   , new Vector3(t.localScale.x, 1, Mathf.Abs(deltaz)));


                t.localPosition = new Vector3(lasttilepositon.x, scoreCount,middle-( lasttilepositon.z/2));
            }
            else
            {
                if (combo > combo_start_gain)
                {
                    stackbounds.y += Stack_bounds_gain;
                    if (stackbounds.y > Bounds_size)
                        stackbounds.y = Bounds_size;
                    float middle = lasttilepositon.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackbounds.x, 1, stackbounds.y);
                    t.localPosition = new Vector3(middle - (lasttilepositon.x / 2), scoreCount, middle - (lasttilepositon.z / 2));
                      
                }
                combo++;
                t.localPosition = lasttilepositon + Vector3.up;
            }

        }



        secondaryposition = (isMovingOnx) ? t.localPosition.x : t.localPosition.z;

        isMovingOnx = !isMovingOnx;  // reversing value for first x axis movement and then z-axis movement and so on...
        
        return true;
    }
    private void EndGame()
    {
        if (PlayerPrefs.GetInt("score") < scoreCount)
            PlayerPrefs.SetInt("score", scoreCount);

        gameover = true;
        endPanel.SetActive(true);
        thestack[stackIndex].AddComponent<Rigidbody>();
         
    }
    public void OnButtonClick(string SceneName)
    {
        SceneManager.LoadScene(SceneName);


    }
}
 