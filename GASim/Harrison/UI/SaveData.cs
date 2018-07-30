using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/**
 * Record data of simulation
 */ 
public class SaveData : MonoBehaviour {

    public string filename;
    public bool saved;
	
    public void test()
    {
        var file = File.CreateText("test.txt");
        file.Write("{0}", -1);
        for (int i = 0; i < 5; i++)
        {
            file.Write("\t{0}", i);
        }
        file.Close();
    }
	public void saveData(List<List<float>> creatures)
    {
        var file = File.CreateText(filename+".dat");
        file.WriteLine("Best\tAverage\tWorst");

        foreach (List<float> gen in creatures)
        {
            if (gen.Count == 0) { continue; }
            float sum = 0;
            for (int i = 0; i < gen.Count; i++)
                sum += gen[i];

            float avg = sum / gen.Count;
            float best = gen[0];
            float worst = gen[gen.Count - 1];
            gen.RemoveAt(0);
            gen.RemoveAt(gen.Count - 1);
            file.WriteLine("{0}\t{1}\t{2}", best, avg, worst);
        }

        file.Close();
        Debug.Log("Saved");
        creatures.Clear();
        saved = true;
    }
}
