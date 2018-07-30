using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Code was from Alan Zucconi's Unity Tutorial
 * https://www.alanzucconi.com/2016/04/27/evolutionary-computation-4/
 * 
 * Some functions and lines were added for Zucconi's implementation
 * to work with UI and recording features
 */
public class GAOrig : MonoBehaviour {

    //Added
    public List<List<float>> generationsL = new List<List<float>>();
    public GameObject container;
    public bool running;
    bool stop;
    public void StartSim()
    {
        //make sure everything is empty
        creatures.Clear();
        foreach (Transform t in container.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        running = true;
        stop = false;
        StartCoroutine(Simulation());
    }

    public void EndSim()
    {
        running = false;
        stop = true;

        //make sure everything is empty
        creatures.Clear();
        foreach (Transform t in container.transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }
    public void setParams(int p, int g, float t)
    {
        variations = p;
        generations = g;
        simulationTime = t;
    }

    public int curGen = 0;
    //////////////////

    public int generations = 100;
    public float simulationTime = 15f;
    public int variations = 100;
    private GenomeT bestGenome;

    public Vector3 distance = new Vector3(0, -5, 0);
    public GameObject prefab;
    public List<CreatOrig> creatures = new List<CreatOrig>();

    public IEnumerator Simulation()
    {
        bestGenome = GenomeT.RandGenome();
        for (int i = 1; i <= generations; i++)
        {
            curGen = i;
            //Added
            if (stop) { break; }
            //////////////

            CreateCreatures();
            yield return new WaitForSeconds(1);
            StartSimulation();
            yield return new WaitForSeconds(simulationTime);

            StopSimulation();
            EvaluateScore();

            //Added
            creatures.OrderByDescending(o => o.GetScore()).ToList();
            List<float> popScore = new List<float>();
            foreach (CreatOrig c in creatures)
            {
                popScore.Add(c.TotDist());
            }
            generationsL.Add(popScore);
            //end Add

            DestroyCreatures();

            yield return new WaitForSeconds(1);
        }
        transform.GetComponent<SaveData>().saveData(generationsL);
        EndSim();
    }

    public void CreateCreatures()
    {
        for (int i = 0; i < variations; i++)
        {
            // Mutate the genome
            GenomeT genome = bestGenome.Clone();
            genome.Mutate();

            // Instantiate the creature
            Vector3 position = Vector3.zero + distance * i;
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, container.transform);
            CreatOrig creature = obj.transform.GetChild(0).GetChild(0).GetComponent<CreatOrig>();

            creature.genome = genome;
            creatures.Add(creature);
        }
    }

    public void StartSimulation()
    {
        foreach (CreatOrig creature in creatures)
            creature.startEval();
    }

    public void StopSimulation()
    {
        foreach (CreatOrig creature in creatures)
            creature.endEval();
    }

    public void DestroyCreatures()
    {
        foreach (CreatOrig creature in creatures)
            Destroy(creature.transform.parent.parent.gameObject);

        creatures.Clear();
    }

    private float bestScore = 0;

    public void EvaluateScore()
    {
        foreach (CreatOrig creature in creatures)
        {
            float score = creature.GetScore();
            if (score > bestScore)
            {
                bestScore = score;
                bestGenome = creature.genome.Clone();
            }
        }
    }
}
