using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{



    [SerializeField]
    public GameObject wheel;
    public GameObject squarePrefab;
    public GameObject spotPrefab;
    public GameObject spawnPrefab;
    public GameObject gridPrefab;
    public Transform wheelSquares;

    //prefab for controlling movement while falling
    GameObject squareSpawn = null;
    //for random expand spawns
    GameObject randSpawn = null;
    
    public int randSpawnCount;
    //checker for player spawn

    public int maxScore;
    [SerializeField]
    public int scoreUpper;
    [SerializeField]
    public int expandMoves;

   
    [SerializeField]
    private int moves = 0;
    public int Moves
    {
        get
        {
            return moves;
        }

        set
        {
            moves = value;
        }
    }
    //Vertical transform of top spot
    public GameObject currentSpot;
    // spawn point
    public GameObject currentSpawn;
    //next spot to turn green
    public int LastSpot { get; set; }



    //All the spots around the wheel
    public List<GameObject> spots;
    public List<GameObject> spawns;
    public GameObject[,] grids;

    //spawn cooldown
    private float coolDown;

    // number of objects
    public int nBottom;

    //Next square's score
    public Text nextScore;
    public Text upper;
    public Text nextShrink;
    public static int next_score;
    public Slider slider;

    //scores
    public int scores;
    public Text ScoreText;

    // Obj list for pop checkrow
    List<GameObject> rowObjs;
   
    List<RandValues> rands;
    //list of randSpawns
    List<GameObject> randSpawns;


    //For getspots
    Vector3 center;
    float rad;

    void Start()
    {
        //Apply all the numbers 
        maxScore = 3;
        expandMoves = 3;
        // count of randomSpawns 
        randSpawnCount = 3;
        scoreUpper = (int)Mathf.Pow(2, maxScore);
        nBottom = 10;
        spots = new List<GameObject>();
        spawns = new List<GameObject>();
        grids = new GameObject[nBottom, 5];

        scores = 0;
        ScoreText.text = scores.ToString();
        upper.text = string.Format("upper: {0}", scoreUpper);
        nextShrink.text = string.Format("next shrink: {0}", expandMoves - Moves);
        slider.value = (expandMoves - Moves) / expandMoves;


        //Random next score to appear (2^3 max <-----)
        next_score = (int)Mathf.Pow(2, Random.Range(1, 4));
        nextScore.text = next_score.ToString();

        //Initialize level (spots)
        GetSpots();


        rowObjs = new List<GameObject>();
    }


    void Update()
    {

        //if inside outer ring and not blocked by extend and is not red   OR is the same score as next one
        if (currentSpot.transform.childCount <= 5 && currentSpot.transform.GetChild(0).GetComponent<SpriteRenderer>().color != new Color32(255, 0, 0, 255)
                        || (currentSpot.transform.childCount == 6 && next_score == currentSpot.transform.GetChild(currentSpot.transform.childCount - 1).GetComponent<Square>().Score) /*&& !currentSpot.GetComponent<Spot>().Blocked*/)
        {
            if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.None && Time.time > coolDown)
            {
                ClickSpawn();
            }
        }

            //Turn left
            if (SwipeManager.Instance.IsSwiping(SwipeDirection.Left) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
               
                        wheel.transform.Rotate(Vector3.forward, 360 / nBottom);
           
            }
            //Turn right
            else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Right) || Input.GetKeyDown(KeyCode.RightArrow))
            {
               
                        wheel.transform.Rotate(Vector3.forward, -360 / nBottom);
               
               
            }
       
    }

    private void ClickSpawn()
    {
     
            //Cooldown for spawn 0.5sec
            coolDown = Time.time + 0.5f;

            //spawn a square
            squareSpawn = Instantiate(squarePrefab, currentSpawn.transform.position, Quaternion.identity);
            squareSpawn.GetComponent<Square>().Score = next_score;
            //get score for next turn (non-inclusive)
            next_score = (int)Mathf.Pow(2, Random.Range(1, maxScore + 1));
            nextScore.text = next_score.ToString();
            squareSpawn.GetComponent<Square>().IsSpawn = true;

    }

    public void Merge(GameObject first)
    {
        int tmp = first.GetComponent<Square>().Score *= 2;
        first.GetComponent<Square>().ApplyStyle(tmp);

        if (tmp > scoreUpper)
        {
            scoreUpper *= 2;
            Instance.upper.text = string.Format("upper: {0}", scoreUpper);

        }
    }

    public Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        //Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    //Sets up spots for spawns
    public void GetSpots()
    {
        rad = wheel.transform.GetChild(0).GetComponent<CircleCollider2D>().radius;
        center = wheel.transform.position;


        //Spots, spawns and grid for movement
        SpawnSpots(spotPrefab, rad, 1, spots);
        SpawnSpots(spawnPrefab, rad + 5.5f, 1, spawns);

        currentSpot = spots[0];
        currentSpawn = spawns[0];


        SpawnSpots(gridPrefab, rad + 0.55f, 5, null, grids);

    }

    private void SpawnSpots(GameObject prefab, float rad, int count, List<GameObject> lists = null, GameObject[,] grids = null)
    {

        for (int i = 0; i < nBottom; i++)
        {
            for (int j = 0; j < count; j++)
            {

                int a = 360 / nBottom * i;
                var pos = RandomCircle(center, rad + 0.9f * j, a);
                GameObject tmp = Instantiate(prefab, pos, Quaternion.LookRotation(Vector3.back));
                tmp.name = i.ToString();

                int toggle = 0;

                if (prefab.CompareTag("spot"))
                {
                    toggle = 0;
                }
                else if (prefab.CompareTag("spawn"))
                {
                    toggle = 1;
                }
                else if (prefab.CompareTag("grid"))
                {
                    toggle = 2;
                }

                tmp.transform.SetParent(wheel.transform.GetChild(toggle));
                tmp.transform.LookAt(center, Vector3.right);
                tmp.transform.Rotate(0, 90, 0);
                if (grids != null)
                {
                    grids[i, j] = tmp;
                    tmp.transform.SetParent(GameManager.Instance.spawns[i].transform);
                    tmp.name = (j + 1).ToString();
                }
                else
                {
                    lists.Add(tmp);

                }
            }
        }


    }
    //Checks for 3 in a row
    public void CheckRow(int spotIndex, int squareIndex, int checkScore)
    {
        rowObjs.Clear();

        //iterator for list
        int index = spotIndex;
        //noMoves = false;
        //index of start of rowObjs (outside nbottom numbers to be safe)
        int startIndex = nBottom + 10;
        int endIndex = nBottom + 11;
        int firstIndex = 0;
        int nextIndex = 0;

        // for later checking 
        GameObject tmpSquare = spots[spotIndex].transform.GetChild(squareIndex).gameObject;

        //add placed square to rowOBjs
        rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);


        do
        {
            //if there's no start yet
            if (startIndex > nBottom)
            {
                //passing through 0
                if (index - 1 < 0)
                {
                    index = nBottom - 1;
                }
                else
                    index = index - 1;

                //check next left one after getting index-1
                if (index - 1 < 0)
                {
                    firstIndex = nBottom - 1;
                }
                else
                    firstIndex = index - 1;


                //if there's square with same squareIndex
                if (spots[index].transform.childCount > squareIndex)
                {
                    //if its score is the same
                    if (spots[index].transform.GetChild(squareIndex).GetComponent<Square>().Score == checkScore)
                    {
                        //if there's nothing to the left
                        if (spots[firstIndex].transform.childCount < squareIndex + 1)
                        {
                            rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                            //if never set up yet
                            if (startIndex > nBottom)
                            {
                                // start pf a row
                                startIndex = index;
                                index = spotIndex;
                                continue;
                            }
                        }
                        else
                        {
                            //or there's not that score
                            if (spots[firstIndex].transform.GetChild(squareIndex).GetComponent<Square>().Score != checkScore)
                            {
                                rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                                //if never set up yet
                                if (startIndex > nBottom)
                                {
                                    startIndex = index;
                                    index = spotIndex;
                                    continue;
                                }
                            }
                            else if (spots[firstIndex].transform.GetChild(squareIndex).GetComponent<Square>().Score == checkScore)
                            {
                                rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                                startIndex = nBottom + 10;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        //if score isnt same - move back to spotIndex and continue
                        startIndex = spotIndex;
                        index = spotIndex;
                        continue;
                    }
                }
                else
                {
                    //if score isnt same - move back to spotIndex and continue
                    startIndex = spotIndex;
                    index = spotIndex;
                    continue;
                }

            }
            //if we found left end
            else if (startIndex < nBottom)
            {
                //passing through nBottom (0)
                if (index + 1 > nBottom - 1)
                {
                    index = 0;
                }
                else
                    index = index + 1;
                //check next one after setting index+1
                if (index + 1 > nBottom - 1)
                {
                    nextIndex = 0;
                }
                else
                    nextIndex = index + 1;
                //if there's square with same squareIndex
                if (spots[index].transform.childCount > squareIndex)
                {
                    //if its score is the same
                    if (spots[index].transform.GetChild(squareIndex).GetComponent<Square>().Score == checkScore)
                    {
                        //if there's nothing to the right
                        if (spots[nextIndex].transform.childCount < squareIndex + 1)
                        {
                            rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                            //if never set up yet
                            if (endIndex > nBottom)
                            {
                                // start pf a row
                                endIndex = index;
                                break;
                            }
                        }
                        else
                        {
                            //or there's not that score
                            if (spots[nextIndex].transform.GetChild(squareIndex).GetComponent<Square>().Score != checkScore)
                            {
                                rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                                //if never set up yet
                                if (endIndex > nBottom)
                                {
                                    endIndex = index;
                                    break;
                                }
                            }
                            else if (spots[nextIndex].transform.GetChild(squareIndex).GetComponent<Square>().Score == checkScore)
                            {
                                rowObjs.Add(spots[index].transform.GetChild(squareIndex).gameObject);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        //if score isnt same - move back to spotIndex and continue
                        endIndex = spotIndex;
                        break;
                    }
                }
                else
                {
                    //if score isnt same - move back to spotIndex and continue
                    endIndex = spotIndex;
                    break;
                }
            }
        }
        while (endIndex > nBottom + 1);
        if (rowObjs.Count < 3)
        {
            rowObjs.Clear();
            // expand moves++ if this happened by player
            if (tmpSquare.GetComponent<Square>().IsSpawn)
            {

                ExpandMoves();

                if (Moves > expandMoves - 1)
                {

                    Expand();
                    Moves = 0;
                    slider.value = 1;

                    //expandMoves += expandMoves/2;
                    nextShrink.text = string.Format("next shrink: {0}", expandMoves - Moves);
                    slider.value = (float)(expandMoves - Moves) / expandMoves;
                }
            }
        }
        else
        {
            Pop(rowObjs);
        }

       
    }

    // update moves
    public void ExpandMoves()
    {
        Moves++;
        nextShrink.text = string.Format("next shrink: {0}", expandMoves - Moves);
        slider.value = (float)(expandMoves - Moves) / expandMoves;
    }

    //Kill all adjacent squares
    public void Pop(List<GameObject> rowObjs)
    {
        foreach (GameObject rowObj in rowObjs)
        {
            //Update the score
            scores += rowObj.GetComponent<Square>().Score;
            ScoreText.text = scores.ToString();

            if (rowObj.transform.parent != null)
            {
                //rowObj.transform.position += new Vector3(0, 0, 10);
                if (!rowObj.transform.parent.GetComponent<Spot>().Blocked)
                {
                    rowObj.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
                }

            }

            //Make upper square drop
            if (rowObj.transform.parent.childCount - rowObj.transform.GetSiblingIndex() > 1)
            {
               
                rowObj.transform.parent.GetChild(rowObj.transform.GetSiblingIndex() + 1).gameObject.GetComponent<Square>().Touched = false;
            }
            //Make this square drop
            rowObj.GetComponent<Square>().Touched = false;
            rowObj.GetComponent<Collider2D>().isTrigger = true;


            
            //Detach this square from parent
            rowObj.transform.parent = null;

        }
        rowObjs.Clear();

      
    }
    // struct to hold randomSpawn values
    public struct RandValues
    {

        public int Rng { get; set; }
        public int RandScore { get; set; }
    }

    public void Expand()
    {
        
        RandValues tmp = new RandValues();
        rands = new List<RandValues>();

        //randSpawn is upper Power -1 always
        int upperPow = (int)Mathf.Log(scoreUpper, 2)-1;
        List<int> randList = new List<int>();
        //get free spots


        foreach(GameObject spot in spots)
        {
            if (spot.transform.GetChild(0).GetComponent<SpriteRenderer>().color != new Color32(255, 0, 0, 255) 
                    && spot.name != currentSpot.name)
            {
                randList.Add(int.Parse(spot.name));
            }
        }
        //if there's atleast 3 spots to spawn randoms
        if (randList.Count >= randSpawnCount)
        {
            rands.Clear();
        
            for (int i = 0; i < randSpawnCount; i++)
            {
                tmp.Rng = randList[Random.Range(0, randList.Count)];
                tmp.RandScore = 0;

                if (rands.Count >= 1 && Contains(rands, tmp))
                {
                    while (Contains(rands, tmp))
                    {
                        tmp.Rng = randList[Random.Range(0, randList.Count)];
                        tmp.RandScore = 0;
                    }
                    
                }

                tmp.RandScore = (int)Mathf.Pow(2, Random.Range(1, upperPow + 1));
                rands.Add(tmp);
                //Debug.Log(rands[rands.Count - 1].Rng + " " + rands[rands.Count - 1].RandScore);

            }
            


            //spawn all
            foreach (RandValues rand in rands)
            {
                //Debug.Log("========");
                SpawnRandom(rand.Rng, rand.RandScore);
            }
        }
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111remove this if something's wrong
        else
        {
            rands.Clear();
        }
    }

    //Check if list of classes contains same rng
    bool Contains(List<RandValues> list, RandValues check)
    {
        foreach (RandValues n in list)
        {
            if (n.Rng == check.Rng)
            {
               
                return true;
            }
             
        }
        return false;
    }


    // Spawn randomSquare
    public void SpawnRandom(int rng, int randScore)
    {
        randSpawns = new List<GameObject>();
        //Debug.Log("SPAWN");
        //spawn a square at random spot with random score
        randSpawn = Instantiate(squarePrefab, spawns[rng].transform.position, Quaternion.identity);
        randSpawn.GetComponent<Square>().ExpandSpawn = true;
        randSpawn.GetComponent<Square>().Score = randScore;
        randSpawn.transform.SetParent(spots[rng].transform);
        randSpawn.name = randSpawn.transform.GetSiblingIndex().ToString();
        

        //Rotate spawns towards center
        Vector3 diff = randSpawn.transform.parent.position - randSpawn.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        randSpawn.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        //make spot red if 6th child
        if (randSpawn.transform.parent.childCount == 6)
        {
            randSpawn.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        }

        randSpawns.Add(randSpawn);
    }
  


    public void GameOver()
    {
        if (currentSpot.transform.childCount == 6)
        {
            //full spot colors red and opens another one
            currentSpot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);


        }
        int reds = 0;




        foreach (GameObject spot in spots)
        {
            if (spot.transform.GetChild(0).GetComponent<SpriteRenderer>().color == new Color32(255, 0, 0, 255) && !spot.GetComponent<Spot>().Blocked)
            {
                if (spot.transform.GetChild(spot.transform.childCount - 1) != null /*&& spot.transform.GetChild(spot.transform.childCount - 1).GetComponent<Square>().Score != next_score*/)
                {
                    reds++;
                }

            }
            else if (spot.GetComponent<Spot>().Blocked)
            {

                reds++;
            }
        }
        if (reds == spots.Count /*&& (next_score != currentSpot.transform.GetChild(currentSpot.transform.childCount - 1).GetComponent<Square>().Score)*/)
        {
            //noMoves = true;
            nextScore.text = "GAME OVER";
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!GAMOVER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

    }







}