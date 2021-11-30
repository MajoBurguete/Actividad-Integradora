using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class Robot
{
    public int id;
    public int x;
    public int y;
    public bool hasBox;
    public Box currentBox;

    override public string ToString()
    {
        return "Id: " + id + ", X: " + x + ", Y: " + y;
    }
}

[System.Serializable]
public class Stack
{
    public int id;
    public int x;
    public int y;
    public int boxNumber;

    override public string ToString()
    {
        return "Id: " + id + ", X: " + x + ", Y: " + y + ", BoxN: " + boxNumber;
    }
}


[System.Serializable]
public class Box
{
    public int id;
    public int x;
    public int y;
    public bool active;
    public bool isStack;
    public override string ToString()
    {
        return "Id: " + id + ", State: " + active + ", X: " + x + ", Y: " + y;
    }
}

[System.Serializable]
class AgentCollection
{
    public Robot[] robots;
    public Box[] boxes;
    public Stack[] stacks;
}


public class GameManager : MonoBehaviour
{
    string simulationURL;
    public GameObject prefabRobot;
    public GameObject prefabBox;
    public GameObject prefabBox2;
    public GameObject prefabBox3;
    public GameObject prefabBox4;
    public GameObject prefabBox5;
    List<GameObject> listaRobots;
    List<GameObject> listaBoxes;
    List<GameObject> listaStacks  = new List<GameObject>();
    List<Stack> stacksV = new List<Stack>();
    private float waitTime = 0.5f;
    private float timer = 0.0f;
    int start = 0;


    void Start()
    {
        StartCoroutine(ConnectToMesa());
        StartCoroutine(UpdatePositions());
    }

    IEnumerator ConnectToMesa()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/games", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                simulationURL = www.GetResponseHeader("Location");
                Debug.Log("Connected to simulation through Web API");
                Debug.Log(simulationURL);
            }
        }

    }

    IEnumerator UpdatePositions()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(simulationURL))
        {
            if (simulationURL != null)
            {
                // Request and wait for the desired page.
                yield return www.SendWebRequest();

                Debug.Log(www.downloadHandler.text);
                Debug.Log("Data has been processed");
                AgentCollection agents = JsonUtility.FromJson<AgentCollection>(www.downloadHandler.text);
                if (start == 0)
                {
                    instantiateGameObjects(agents.robots, agents.boxes);
                }
                else
                {
                    for (int i = 0; i < agents.robots.Length; i++)
                    {
                        positionRobot(agents.robots[i], listaRobots[i]);
                    }
                    for (int i = 0; i < agents.boxes.Length; i++)
                    {
                        positionBox(agents.boxes[i], listaBoxes[i]);
                    }
                    for (int j = 0; j < agents.stacks.Length; j++)
                    {
                        Debug.Log(agents.stacks[j].ToString());
                        positionStack(agents.stacks[j]);
                    }
                }
                start++;
            }
        }
    }

    void positionRobot(Robot robot, GameObject robotObj)
    {
        robotObj.transform.position = new Vector3(robot.x, 1.5f, robot.y);
    }
    void positionBox(Box box, GameObject boxObj)
    {
        if (box.active)
        {
            boxObj.SetActive(true);
        }
        else
        {
            boxObj.SetActive(false);
        }
    }

    void positionStack (Stack stack)
    {
        GameObject prefab = prefabBox;

        if (stack.boxNumber == 2)
        {
            listaBoxes.Add(Instantiate(prefab, new Vector3(stack.x, 2.14f, stack.y), Quaternion.identity));
        }
        if (stack.boxNumber == 3)
        {
            listaBoxes.Add(Instantiate(prefab, new Vector3(stack.x, 3.265f, stack.y), Quaternion.identity));
        }
        if (stack.boxNumber == 4)
        {
            listaBoxes.Add(Instantiate(prefab, new Vector3(stack.x, 4.37f, stack.y), Quaternion.identity));
        }
        if (stack.boxNumber == 5)
        {
            listaBoxes.Add(Instantiate(prefab, new Vector3(stack.x, 5.501f, stack.y), Quaternion.identity));
        }

    }

    bool alreadyExists(Stack stack)
    {
        for(int i = 0; i < stacksV.Count; i++)
        {
            if(stack.id == stacksV[i].id)
            {
                return true;
            }
        }

        return false;
    }


    void instantiateGameObjects(Robot[] robots, Box[] boxes)
    {
        listaRobots = new List<GameObject>();
        listaBoxes = new List<GameObject>();

        for (int i = 0; i < robots.Length; i++)
        {
            Robot robot = robots[i];
            GameObject prefab = prefabRobot;
            listaRobots.Add(Instantiate(prefab, new Vector3(robot.x, 1.5f, robot.y), Quaternion.identity));
        }

        for (int i = 0; i < boxes.Length; i++)
        {
            Debug.Log(boxes[i].ToString());
            Box box = boxes[i];
            GameObject prefab = prefabBox;
            listaBoxes.Add(Instantiate(prefab, new Vector3(box.x, 1.0f, box.y), Quaternion.identity));
        }

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            StartCoroutine(UpdatePositions());
            timer = timer - waitTime;
        }
    }
}

