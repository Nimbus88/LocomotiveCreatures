using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
    public GameObject instance;
    public bool isRunning;
    public Genome genome;
    public LegController left;
    public LegController right;

    Vector3 initialPosition;
    Vector3 finalPosition;
    Vector3 prevPos;

    float upsidedown = 0;
    float rightsideup = 0;

    float idling = 0;
    float moving = 0;
    float backwards = 0;
    float forward = 0;
    float centered = 0;
    float arching = 0;
    float laying = 0;
    float standing = 0;

    public void Start()
    {
        instance = transform.parent.parent.gameObject;
        isRunning = false;
        initialPosition = transform.position;
    }

    public void startEval()
    {
        transform.localPosition = new Vector3(0, 0.26f, 0);
        initialPosition = transform.parent.parent.GetChild(2).GetChild(0).position;
        prevPos = initialPosition;
        isRunning = true;
    }

    public void endEval()
    {
        finalPosition = transform.position;
        isRunning = false;
    }

    //Distance score at end of evaluation
    public float Distance()
    {
        float d = finalPosition.x - initialPosition.x;
        float dw = d;
        dw *= dw < 0 ? -0.15f : dw;
        return dw;
    }

    public float D()
    {
        return finalPosition.x - initialPosition.x;
    }

    //Current Distance Score during evaluation
    public float CurDistance()
    {
        float d = transform.position.x;
        float dw = d;
        dw *= dw < 0 ? -0.15f : dw;
        return dw;
    }

    //Is the creature moving?
    public float IdleScore()
    {
        float d = moving - idling;
        float t = d < 0 ? 1 : d;
        return t;
    }

    //How much is the creature arching?
    public float PostureScore()
    {
        float o = centered - arching;
        float t = o < 0 ? 1 : o;
        return t;
    }

    //Facing in the right direction?
    public float FacingScore()
    {
        float f = forward - backwards;
        float t = f < 0 ? 1 : f;
        return t;
    }

    //Is the creature laying?
    public float StandingScore() {
        float f = standing - laying;
        float t = f < 0 ? 1 : f;
        return t;
    }

    //Current fitness during evalutation
    public float CurScore()
    {
        float d = CurDistance();
        float p = PostureScore();
        float f = FacingScore();
        float i = IdleScore();
        float s = StandingScore();
        float total = d + (f + p + i + s)/4;
        return total;

    }

    //Fitness at end of evaluation
    public float GetScore()
    {
        float d = Distance();
        float p = PostureScore();
        float f = FacingScore();
        float i = IdleScore();
        float s = StandingScore();
        float total = d + (f + p + i + s)/4;
        return total;

    }

    public void Update()
    {
        if (isRunning)
        {
            //Move creature
            left.position = genome.left.EvaluateAt(Time.time);
            right.position = genome.right.EvaluateAt(Time.time);

            //Calculate fitness
            if (transform.eulerAngles.z > 60 && transform.eulerAngles.z < 95)
            {
                arching += 1;
            }

            if (transform.eulerAngles.z <= 15 && transform.eulerAngles.z >= -15)
            {
                centered += 1;
            }

            if (transform.localPosition.y < -0.75f)
            {
                laying += 0;
            }
            else
            {
                standing += 1;
            }

            if (right.transform.position.x < left.transform.position.x)
            {
                backwards += 1;
            }
            else
            {
                forward += 1;
            }

            Vector3 curPos = transform.position;
            float diff = Mathf.Abs(initialPosition.x - curPos.x);
            if ((int)Mathf.Round(diff) == 0)
            {
                idling += 1;
            }
            else
            {
                moving += 1;
            }
            prevPos = curPos;
        }
    }
}

[System.Serializable]
public struct Genome
{
    //first 4 indexes are for left leg
    //last 4 index for right leg
    public float[] genes;
    public GenomeLeg left;
    public GenomeLeg right;

    public void randGenes()
    {
        genes = new float[8];

        genes[0] = Random.Range(-1f, +1f);
        genes[1] = Random.Range(-1f, +1f);
        genes[2] = Random.Range(0.1f, 2f);
        genes[3] = Random.Range(-2f, +2f);
        genes[4] = Random.Range(-1f, +1f);
        genes[5] = Random.Range(-1f, +1f);
        genes[6] = Random.Range(0.1f, 2f);
        genes[7] = Random.Range(-2f, +2f);

        applyGenes();
    }

    public void applyGenes()
    {
        left = new GenomeLeg();
        right = new GenomeLeg();
        left.min = genes[0];
        left.max = genes[1];
        left.x_shift = genes[2];
        left.period = genes[3];
        right.min = genes[4];
        right.max = genes[5];
        right.x_shift = genes[6];
        right.period = genes[7];
    }

    public void updateGenes()
    {
        left.min = genes[0];
        left.max = genes[1];
        left.x_shift = genes[2];
        left.period = genes[3];
        right.min = genes[4];
        right.max = genes[5];
        right.x_shift = genes[6];
        right.period = genes[7];
    }

    public Genome Clone()
    {
        Genome genome = new Genome();
        genome.genes = (float[])genes.Clone();
        genome.left = left.Clone();
        genome.right = right.Clone();
        return genome;
    }

    public static Genome[] crossover (Genome a, Genome b)
    {
        Genome[] ret = new Genome[2];
        int startI = Random.Range(0, 8);
        Genome n1 = a.Clone();
        Genome n2 = b.Clone();

        for (int i = startI; i < 8-startI; i++)
        {
            n1.genes[i] = b.genes[i];
            n2.genes[i] = a.genes[i];
        }

        n1.applyGenes();
        n2.applyGenes();
        ret[0] = n1; ret[1] = n2;

        return ret;
    }
    public static Genome[] crossover2Pnt(Genome a, Genome b)
    {
        Genome[] ret = new Genome[2];
        int startI = Random.Range(0, 8);
        int endI = Random.Range(startI, 8);
        Genome n1 = a.Clone();
        Genome n2 = b.Clone();

        for (int i = startI; i < endI; i++)
        {
            n1.genes[i] = b.genes[i];
            n2.genes[i] = a.genes[i];
        }

        n1.applyGenes();
        n2.applyGenes();
        ret[0] = n1; ret[1] = n2;

        return ret;
    }

    //Mutate each gene based on rate
    public void Mutate(float rate)
    {
        for (int i = 0; i < 8; i++) {
            if (Random.Range(0f, 1f) > rate) { continue; }
            mut_val(i);
        }
        updateGenes();
    }

    //Mutate a gene by an amount, bounded to some range
    void mut_val(int i)
    {
        switch (i)
        {
            case 0:
            case 4:
                genes[i] += Random.Range(-0.1f, 0.1f);
                genes[i] = Mathf.Clamp(genes[i], -1f, +1f);
                break;
            case 1:
            case 5:
                genes[i] += Random.Range(-0.1f, 0.1f);
                genes[i] = Mathf.Clamp(genes[i], -1f, +1f);
                break;
            case 2:
            case 6:
                genes[i] += Random.Range(-0.25f, 0.25f);
                genes[i] = Mathf.Clamp(genes[i], 0.1f, 2f);
                break;
            case 3:
            case 7:
                genes[i] += Random.Range(-0.25f, 0.25f);
                genes[i] = Mathf.Clamp(genes[i], -2f, 2f);
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public struct GenomeLeg
{
    public float min;
    public float max;
    public float x_shift;
    public float period;

    public float EvaluateAt(float time)
    {
        return (max - min) / 2 * (1 + Mathf.Sin((time + x_shift) * Mathf.PI * 2 / period)) + min;
    }

    public GenomeLeg Clone()
    {
        GenomeLeg leg = new GenomeLeg();
        leg.min = min;
        leg.max = max;
        leg.x_shift = x_shift;
        leg.period = period;
        return leg;
    }
}