using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideManager : MonoBehaviour
{
    public GameObject pillar1;
    public GameObject pillar2;
    public GameObject pillar3;
    public GameObject board;
    public GameObject ball;   // represent the location of the ride 
    public GameObject[] chairs;
    public GameObject[] seats;
    int[] seatsDelta;
    float[] seatsTimer;

    Light lt;


    int Speed = 30;
    bool leanBoard = false;
    float distance;
    bool cameraMoveBg = true;   // beginning of camera moving


    // Velocity magnitude
    public float velocityMagnitude = 0.0f;
    public float rotationSpeed = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        pillar1 = GameObject.Find("Pillar1");
        pillar2 = GameObject.Find("Pillar2");
        pillar3 = GameObject.Find("Pillar3");
        board = GameObject.Find("board");
        ball = GameObject.Find("soccer");
        chairs = new GameObject[6];
        for (int i = 0; i < chairs.Length; i++)
        {
            string temp = "chair" + (i + 1);
            chairs[i] = GameObject.Find(temp);
        }

        seats = new GameObject[24];
        for (int i = 0; i < seats.Length; i++)
        {
            string temp = "Cube" + (i + 1);
            seats[i] = GameObject.Find(temp);
        }
        seatsDelta = new int[seats.Length];
        for (int i = 0; i < seats.Length; i++)
        {
            seatsDelta[i] = 1;
        }
        seatsTimer = new float[seats.Length];
        for (int i = 0; i < seats.Length; i++)
        {
            var random = new Random();
            seatsTimer[i] = Random.Range(0.0f, 5.0f)*100;
        }

        // Light
        lt = GameObject.Find("Directional Light").GetComponent<Light>();
        lt.intensity = 1.0f;

        addLight(); // Add Point Light on each chair


        // Camera
        Camera.main.transform.localPosition = new Vector3(-1.5f, -1.2f, -17f);
        //Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 1.0f);
        Camera.main.transform.eulerAngles = (new Vector3(-26f, 2.0f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
        // Pillar2 and Pillar 3 go up 
        movePillarUp(pillar2);
        movePillarUp(pillar3);

        // Rotate and move the seats
        rotateBoard();
        rotateChairs();
        rotateSeats();
        moveSeatsUPAndDown();

        // Adjust main ligth
        gettingDark();

        // Move camera
        if (cameraMoveBg)
        {
            cameraMove();
        }
        else
        {
            // Following game object
            distance = Vector3.Distance(Camera.main.transform.position, ball.transform.position);
            Debug.Log(distance);
            if (distance > 19)
            {
                cameraFollow();
            }
        }

       

        // You can move the ride with arrow keys
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            //Camera.main.transform.Translate(Vector3.forward * Speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * Speed * Time.deltaTime);
        }
    }
    void movePillarUp(GameObject go)
    {
        float movetoY = 1.7f;
        if (go.transform.localPosition.y < movetoY)
            go.transform.localPosition += Vector3.up * 0.3f * Time.deltaTime;
        else
        {
            leanBoard = true;
        }
    }

    void rotateBoard()
    {
        board.transform.Rotate(board.transform.up, 20.0f * Time.deltaTime, Space.World);

        //if (leanBoard)
        //{
        //    if (board.transform.rotation.eulerAngles.z < 15)
        //        board.transform.Rotate( new Vector3(0, 0, 2.0f * Time.deltaTime));
        //    //else

        //}

    }

    void rotateChairs()
    {
        for (int i = 0; i < chairs.Length; i++)
        {
            chairs[i].transform.Rotate(new Vector3(0, 30.0f * Time.deltaTime, 0));
        }
    }

    void rotateSeats()
    {
        for (int i = 0; i < seats.Length; i++)
        {
            seats[i].transform.Rotate(new Vector3(0, 100.0f * Time.deltaTime, 0));
        }
    }

    void moveSeatsUPAndDown()
    {
        float seatHeight = 0;
        for (int i = 0; i < seats.Length; i++)
        {
            if (seatsTimer[i] < 0)
            {
                seatHeight = seats[i].transform.localScale.y;
                if (seatHeight < 1)
                {
                    seatsDelta[i] = 1;
                }
                else if (seatHeight > 4)
                {
                    seatsDelta[i] = -1;
                }
                seats[i].transform.localScale += new Vector3(0, 0.1f * seatsDelta[i], 0);
            }
            else
            {
                seatsTimer[i]--;
            }
            
        }
    }

    //////////////////////////////
    /*         Light            */
    //////////////////////////////

    void gettingDark()
    {
        if(lt.intensity > 0)
        {
            lt.intensity -= 0.005f;
        }
    }

    void addLight()
    {
        Color[] colors = { Color.green, Color.red, Color.cyan };
        for(int i=0; i<chairs.Length; i++)
        {
            // Make a game object
            GameObject lightGameObject = new GameObject("Point Light");

            // Add the light component
            Light lightComp = lightGameObject.AddComponent<Light>();

            // Set color randomly and set position
            lightComp.color = colors[(int)Random.Range(0.0f, (float)colors.Length)];
            lightComp.type = LightType.Point;
            lightComp.intensity = 1.5f;

            // Set the position (or any transform property)
            lightGameObject.transform.parent = chairs[i].transform;

            lightGameObject.transform.localPosition = new Vector3(0.0f, 6.4f, 0.0f);
            lightGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    //////////////////////////////
    /*         Camera           */
    //////////////////////////////

    void cameraMove()
    {
        float cPositionY = Camera.main.transform.localPosition.y;
        if (cPositionY < 15)
        {
            // First, camera shows the bottom of the ride, pillars
            // Then, camera rotates toward the ride, and goes up.
            // It shows wide scene until cPositionY reaches 10, 
            Camera.main.transform.Rotate(new Vector3(5.0f * Time.deltaTime, 0, 0));
            Camera.main.transform.localPosition += Vector3.up * 2.0f * Time.deltaTime;
            if (cPositionY < 10)
            {
                Camera.main.fieldOfView += 0.1f;
            }
        }
        else
        {
            cameraMoveBg = false;
        }

    }

    void cameraFollow()
    {
        Vector3 heading = ball.transform.position - Camera.main.transform.position;
        //Vector3 temp = new Vector3(ball.transform.position.x, Camera.main.transform.position.y, ball.transform.position.z);
        //Vector3 moving = temp - Camera.main.transform.position;

        Quaternion newrotation = Quaternion.LookRotation(heading);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, newrotation, 4.5f * Time.deltaTime);
        Camera.main.transform.Translate(Vector3.forward * 0.3f, Space.Self);
    }

}
