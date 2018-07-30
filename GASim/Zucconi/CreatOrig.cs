using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Code was from Alan Zucconi's Unity Tutorial
 * https://www.alanzucconi.com/2016/04/13/evolutionary-computation-2/
 * https://www.alanzucconi.com/2016/04/20/evolutionary-computation-3/
 * 
 * Some functions and lines were added for Zucconi's implementation
 * to work with UI and recording features
 */
public class CreatOrig : MonoBehaviour {

    //Added
    bool isRunning;
    Vector3 finalPosition;
    public void startEval()
    {
        transform.localPosition = new Vector3(0, 0.26f, 0);
        initialPosition = transform.parent.parent.GetChild(2).GetChild(0).position;
        isRunning = true;
    }

    public void endEval()
    {
        finalPosition = transform.position;
        isRunning = false;
    }

    public float CurScore()
    {
        // Walking score
        float walkingScore = transform.position.x - initialPosition.x;

        // Balancing score
        bool headUp =
        head.transform.eulerAngles.z < 0 + 30 ||
        head.transform.eulerAngles.z > 360 - 30;
        bool headDown =
        head.transform.eulerAngles.z > 180 - 45 &&
        head.transform.eulerAngles.z < 180 + 45;

        return walkingScore * (headDown ? 0.5f : 1f) + (headUp ? 2f : 0f);
    }

    public float TotDist()
    {
        return transform.position.x - initialPosition.x;
    }
    /////////////

    public GenomeT genome;

    public GameObject head;

    public LegController left;
    public LegController right;

    private Vector3 initialPosition;
    public void Start()
    {
        initialPosition = transform.position;
    }

    public float GetScore()
    {
        // Walking score
        float walkingScore = finalPosition.x - initialPosition.x;

        // Balancing score
        bool headUp =
        head.transform.eulerAngles.z < 0 + 30 ||
        head.transform.eulerAngles.z > 360 - 30;
        bool headDown =
        head.transform.eulerAngles.z > 180 - 45 &&
        head.transform.eulerAngles.z < 180 + 45;

        return walkingScore * (headDown ? 0.5f : 1f) + (headUp ? 2f : 0f);
    }

    // Update is called once per frame
    void Update () {
        if (isRunning)
        {
            left.position = genome.left.EvaluateAt(Time.time);
            right.position = genome.right.EvaluateAt(Time.time);
        }
    }
}

[System.Serializable]
public struct GenomeLegT
{
    public float m;
    public float M;
    public float o;
    public float p;

    public float EvaluateAt(float time)
    {
        return (M - m) / 2 * (1 + Mathf.Sin((time + o) * Mathf.PI * 2 / p)) + m;
    }

    public GenomeLegT Clone()
    {
        GenomeLegT leg = new GenomeLegT();
        leg.m = m;
        leg.M = M;
        leg.o = o;
        leg.p = p;
        return leg;
    }

    public void Mutate()
    {
        switch (Random.Range(0, 3 + 1))
        {
            case 0:
                m += Random.Range(-0.1f, 0.1f);
                m = Mathf.Clamp(m, -1f, +1f);
                break;
            case 1:
                M += Random.Range(-0.1f, 0.1f);
                M = Mathf.Clamp(M, -1f, +1f);
                break;
            case 2:
                p += Random.Range(-0.25f, 0.25f);
                p = Mathf.Clamp(p, 0.1f, 2f);
                break;
            case 3:
                o += Random.Range(-0.25f, 0.25f);
                o = Mathf.Clamp(o, -2f, 2f);
                break;
        }
    }
}

[System.Serializable]
public struct GenomeT
{
    public GenomeLegT left;
    public GenomeLegT right;

    public static GenomeT RandGenome ()
    {
        GenomeT g = new GenomeT();
        g.left = new GenomeLegT();
        g.right = new GenomeLegT();
        g.left.m = Random.Range(-1f, +1f);
        g.left.M = Random.Range(-1f, +1f);
        g.left.o = Random.Range(0.1f, 2f);
        g.left.p = Random.Range(-2f, +2f);
        g.right.m = Random.Range(-1f, +1f);
        g.right.M = Random.Range(-1f, +1f);
        g.right.o = Random.Range(0.1f, 2f);
        g.right.p = Random.Range(-2f, +2f);
        return g;
    }

    public GenomeT Clone()
    {
        GenomeT genome = new GenomeT();
        genome.left = left.Clone();
        genome.right = right.Clone();
        return genome;
    }

    public void Mutate()
    {
        if (Random.Range(0f, 1f) > 0.5f)
            left.Mutate();
        else
            right.Mutate();
    }
}