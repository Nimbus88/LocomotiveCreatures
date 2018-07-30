using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * GUI functionality and settings
 */ 
public class UIStuff : MonoBehaviour {

    public Master m;

    public GameObject startB;
    public GameObject stopB;

    public InputField pop;
    public InputField gen;
    public InputField time;
    public InputField file;
    public Slider slide;
    public Text s_val;
    public Dropdown imp;


    public SaveData save;

	// Update is called once per frame
	void Update () {

        s_val.text = slide.value.ToString();
        if (save.saved)
        {
            strt = true;
            startB.SetActive(true);
            stopB.SetActive(false);
        }
        
    }

    bool strt = true;
    public void toggleSim()
    {
        strt = !strt;
        if (strt)
        {
            m.StopSim();
        }
        else
        {
            save.saved = false;
            int p = int.Parse(pop.text);
            int g = int.Parse(gen.text);
            float t = float.Parse(time.text);
            float r = slide.value;
            bool i = imp.value == 0 ? true : false;

            if (p < 0 || g < 0 || t < 0) { return; }

            m.save.filename = file.text;
            m.mine = i;
            if (m.mine)
            {
                m.ga.SetParams(p, g, t, r);
            }
            else
            {
                m.tut.setParams(p, g, t);
            }
            m.StartSim();

            startB.SetActive(false);
            stopB.SetActive(true);
        }
    }
}
