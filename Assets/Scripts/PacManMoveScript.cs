using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PacManMoveScript : MonoBehaviour
{
    public float movespeed = 0.3f;
    Vector2 destination = Vector2.zero;
    public AudioSource chomp;
    public Vector2 moveDirection;
    public GameObject Blinky;
    public GameObject Pinky;
    public GameObject Inky;
    public GameObject Clyde;
    public TextMeshProUGUI score;
    public GameObject GameManager;
    public Vector2 startPos;
    [HideInInspector]
    public float timeSpent;
    public float slowTimer = 0f, SLOWTIMER = 4f;
    public GameObject Dots;
    private List<Vector3> locations = new List<Vector3>();

    void Start()
    {
        startPos = new Vector2(15, 8);
        movespeed = 0.3f;
        slowTimer = 0f;
        //GameManager.GetComponent<GameManagerScript>().ResetGame += StartGame;
        destination = transform.position;
        foreach (Transform child in Dots.transform)
        {
            locations.Add(child.transform.position);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(GameManager.GetComponent<GameManagerScript>().LoadScene(0));
        }
        // Come up with better name for 'p'
        if(slowTimer > 0)
        {
            slowTimer -= Time.fixedDeltaTime;
            movespeed = 0.15f;
        }
        else
        {
            movespeed = 0.3f;
        }
        Vector2 p = Vector2.MoveTowards(transform.position, destination, movespeed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // Check for Input if not moving
        if ((Vector2)transform.position == destination)
        {
            if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
                destination = (Vector2)transform.position + Vector2.up;
            if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
                destination = (Vector2)transform.position + Vector2.right;
            if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
                destination = (Vector2)transform.position - Vector2.up ;
            if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
                destination = (Vector2)transform.position - Vector2.right;
        }
        
        //works for now, counts moving in a wall as moving
        if ((Vector2)transform.position == destination && !Input.anyKey)
        {
            timeSpent += Time.fixedDeltaTime;
        }


        //To Fix: Plays on awake, stops/starts a lot (turned off because annoying)
        if ((Vector2)transform.position == destination && (Input.GetKey("up")))
        {
            //chomp.Play();
        }

        // Animation Parameters
        Vector2 dir = destination - (Vector2)transform.position;
        moveDirection = CheckMoveDirection(dir);
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    //Checks the direction Pacman is moving in. This is used in Inky and Pinky's pathfinding logic
    Vector2 CheckMoveDirection(Vector2 dir)
    {
        if (dir.x > 0.01)
        {
            moveDirection = Vector2.right;
        }
        if (dir.x < -0.01)
        {
            moveDirection = Vector2.left;
        }
        if (dir.y > 0.01)
        {
            moveDirection = Vector2.up;
        }
        if (dir.y < -0.01)
        {
            moveDirection = Vector2.down;
        }
        return moveDirection;
    }

    // Cast Line from 'next square in movedirection to 'Pac-Man'. True = hit pac man, ignores ghosts
    bool valid(Vector2 dir)
    {
        LayerMask layerMask = LayerMask.GetMask("Ghosts");
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos, ~layerMask);
        return (hit.collider == GetComponent<Collider2D>());
    }

    //Determines what happens when collided with. PacMan increases score when eating pellets, powers up due to power pellets and dies to ghosts
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerPellet"))
        {
            if(Random.Range(0, 100) >= 75)
            {
                GameManager.GetComponent<GameManagerScript>().PowerPelletCollected();
                GameManager.GetComponent<GameManagerScript>().powerPelletsCollected += 1;

                Destroy(collision.gameObject);
                score.GetComponent<ScoreScript>().ScorePowerPellet();
                StartCoroutine(GameManager.GetComponent<GameManagerScript>().CheckForGameEnd());
            }
            else
            {
                int randomIndex = Random.Range(0, locations.Count);
                Vector3 newPosition = locations[randomIndex];
                collision.transform.position = newPosition;
            }
        }

        if (collision.CompareTag("Pellet"))
        {
            GameManager.GetComponent<GameManagerScript>().pelletsCollected += 1;
            score.GetComponent<ScoreScript>().ScorePellet();
            StartCoroutine(GameManager.GetComponent<GameManagerScript>().CheckForGameEnd());
        }
        if (collision.CompareTag("Ghost") && collision.gameObject.GetComponent<GhostBehaviourScript>().frightened)
        {
            score.GetComponent<ScoreScript>().ScoreGhost();
        }

        if (collision.CompareTag("TeleportLeft"))
        {
            Vector2 tp = new Vector2(27, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
        if (collision.CompareTag("TeleportRight"))
        {
            Vector2 tp = new Vector2(2, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
    }

    public void slowed()
    {
        slowTimer = SLOWTIMER;
    }

    public void ResetGame()
    {
        gameObject.transform.position = startPos;
        destination = startPos;
    }
}
