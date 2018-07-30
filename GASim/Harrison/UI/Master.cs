using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Master : MonoBehaviour {

    public bool mine = true;
    public GA ga;
    public GAOrig tut;
    public SaveData save;
    public Text displayGen;


    //Tracking
    Transform bestCreat;
    Vector3 prevPos;
    float camz;

    // Use this for initialization
    void Start () {
        ga = GetComponent<GA>();
        tut = GetComponent<GAOrig>();
        save = GetComponent<SaveData>();
	}
	
	// Update is called once per frame
	void Update () {
        int g = mine ? ga.curGen : tut.curGen;
        displayGen.text = "Generation: " + g.ToString();

        //camera following best
        if ((ga.running || tut.running) && bestCreat)
        {
            Vector3 l = Vector3.Lerp(prevPos, bestCreat.position, Time.time);
            l.y = bestCreat.parent.parent.transform.position.y;
            Camera.main.transform.position = new Vector3(l.x, l.y, -10);
        }

    }

    //thread to track best creature (Harrison)
    IEnumerator trackBest()
    {
        while (ga.running)
        {
            if (Camera.main.GetComponent<MoveCam>().lockon)
            {
                if (ga.population.Count > 0)
                {
                    List<Creature> sort = ga.population.OrderByDescending(o => o.CurScore()).ToList();

                    Creature best = sort[0];
                    bestCreat = best.transform;
                    prevPos = Camera.main.transform.position;
                    camz = prevPos.z;
                }
            }
            else
            {
                bestCreat = null;
            }

            yield return new WaitForSeconds(2);
        }
    }

    //thread to track best creature (Alan)
    IEnumerator trackBestT()
    {
        while (tut.running)
        {
            if (Camera.main.GetComponent<MoveCam>().lockon)
            {
                if (tut.creatures.Count > 0)
                {
                    List<CreatOrig> sort = tut.creatures.OrderByDescending(o => o.CurScore()).ToList();

                    CreatOrig best = sort[0];
                    bestCreat = best.transform;
                    prevPos = Camera.main.transform.position;
                    camz = prevPos.z;
                }
            }
            else
            {
                bestCreat = null;
            }

            yield return new WaitForSeconds(2);
        }
    }

    public void StartSim()
    {
        if (mine)
        {
            ga.StartSim();
            StartCoroutine(trackBest());
        }
        else
        {
            tut.StartSim();
            StartCoroutine(trackBestT());
        }
    }
    public void StopSim()
    {
        if (mine)
        {
            ga.EndSim();
        }
        else
        {
            tut.EndSim();
        }
    }
}
