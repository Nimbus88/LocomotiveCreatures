using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GA : MonoBehaviour {
    public bool running;
    bool stop;

    public int pop_size;
    public int max_gen;
    public float eval_length;
    [Range(0.0f,1.0f)]
    public float mutateRate;

    public List<Creature> population;
    public List<List<float>> generations;

    public GameObject container;
    public GameObject instance;


    public int curGen = 0;
    Vector3 placeOffset = new Vector3(0, -5, 0);

    IEnumerator sim;

    // Use this for initialization
    void Start()
    {
        generations = new List<List<float>>();
    }

    //Set simulation parameters
    public void SetParams(int p, int g, float t, float r)
    {
        pop_size = p;
        max_gen = g;
        eval_length = t;
        mutateRate = r;
    }

    public void StartSim()
    {
        //make sure everything is empty
        population.Clear();
        foreach (Transform t in container.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        running = true;
        stop = false;
        //Start a thread to run a simulation
        StartCoroutine(Simulate());
    }

    public void EndSim()
    {
        running = false;
        stop = true;

        //make sure everything is empty
        population.Clear();
        foreach (Transform t in container.transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    IEnumerator Simulate()
    {

        //rand pop
        population = randomPopulation(pop_size);
        yield return new WaitForSeconds(1f);
        int g = 1;
        while (g <= max_gen && !stop)
        {
            curGen = g;
            //eval pop & calculate fitness
            foreach (Creature c in population)
                c.startEval();

            //Thread wait until evalation time ends
            yield return new WaitForSeconds(eval_length);
            //stop eval
            foreach (Creature c in population)
                c.endEval();

            //sort pop by fitness (high -> low)
            List<Creature> sorted = population.OrderByDescending(o => o.GetScore()).ToList();
            //save distance
            //Debug.Log(sorted[0].D());
            List<float> popScore = new List<float>();
            foreach (Creature c in sorted)
            {
                popScore.Add(c.D());
            }
            generations.Add(popScore);
            //selection
            List<Creature> selected = selection(sorted);
            //repopulate
            List<Genome> newGenes = repopulate(selected);
            population = newPop(newGenes, g + 1);
            g++;
            yield return new WaitForSeconds(1);
        }
        transform.GetComponent<SaveData>().saveData(generations);
        EndSim();
    }

    List<Creature> randomPopulation(int size)
    {
        List<Creature> pop = new List<Creature>();
        for (int i = 0; i < size; i++)
        {
            GameObject o = Instantiate(instance, placeOffset*i, Quaternion.identity, container.transform);
            Creature c = o.transform.GetChild(0).GetChild(0).GetComponent<Creature>();
            c.genome = new Genome();
            c.genome.randGenes();

            pop.Add(c);  
        }
        return pop;
    }

    List<Creature> newPop (List<Genome> genes, int gen)
    {
        population.Clear();
        foreach(Transform t in container.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        List<Creature> pop = new List<Creature>();
        int i = 0;
        foreach (Genome g in genes)
        {
            GameObject o = Instantiate(instance, placeOffset * i, Quaternion.identity, container.transform);
            Creature c = o.transform.GetChild(0).GetChild(0).GetComponent<Creature>();
            c.genome = g;

            pop.Add(c);
            i++;
        }
        return pop;
    }

    //random selection based on fitness
    List<Creature> selection(List<Creature> sorted)
    {
        List<float> scores = new List<float>();
        foreach (Creature c in sorted)
        {
            float score = c.GetScore();
            scores.Add(score);
        }

        int half = sorted.Count / 2;
        List<Creature> ret = new List<Creature>();
        while (ret.Count < half)
        {
            //normalize scores
            float sum = 0;
            List<float> tempScores = new List<float>();
            foreach (float f in scores)
            {
                sum += f;
            }
            for (int o = 0; o < tempScores.Count; o++)
            {
                tempScores[o] /= sum;
            }

            //find creature to add to new pop
            float r = Random.Range(0, 1);
            int i;
            for (i = 0; i < tempScores.Count; i++)
            {
                if (r <= 0) { break; }
                r -= tempScores[i];
            }
            Creature c = sorted[i];
            ret.Add(c);
            sorted.RemoveAt(i);
            scores.RemoveAt(i);
        }
        return ret;
    }

    //crossover and mutate 2 pairs
    List<Genome> repopulate(List<Creature> selected)
    {
        List<Genome> ret = new List<Genome>();
        //List<Genome> pars = new List<Genome>();
        int size = selected.Count;
        while (selected.Count > 1)
        {
            int r1 = Random.Range(0, selected.Count);
            int r2 = Random.Range(0, selected.Count);
            while (r1 == r2)
            {
                r2 = Random.Range(0, selected.Count);
            }
            Genome p1 = selected[r1].genome.Clone();
            Genome p2 = selected[r2].genome.Clone();
            Genome[] children = Genome.crossover2Pnt(p1,p2);
            Genome c1 = children[0];
            Genome c2 = children[1];
            c1.Mutate(mutateRate);
            c2.Mutate(mutateRate);
            ret.Add(p1); //pars.Add(p1);
            ret.Add(p2); //pars.Add(p2);
            ret.Add(c1);
            ret.Add(c2);

            selected.RemoveAt(r1);
            if (r1 < r2) { r2--; }
            selected.RemoveAt(r2);
        }

        if (selected.Count == 1)
        {
            //int i = Random.Range(0, pars.Count);
            Genome p = selected[0].genome.Clone();
            Genome c = p.Clone();
            c.Mutate(mutateRate);
            ret.Add(p);
            ret.Add(c);
        }

        return ret;
    }
}
