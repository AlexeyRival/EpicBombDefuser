using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Segment : MonoBehaviour
{
    public GameObject indicator,display;
    public bool Solved;
    public bool failed;
    public int size = 4;
    public string Answer;
    public string tryedAnswer;
    public string emptyDisplayValue = "______";
    public int counter = 0;
    public BlockType blockType;
    public AudioClip good, bad,click;
    public AudioSource source;
    public bool istimer;
    public int time=15;
    public float defactotime=15;
    public Bomb bomb;
    public void Add(string symbol)
    {
        if (Solved||istimer) return;
        tryedAnswer += symbol;
        ++counter;
        source.clip = click;
        if (display)
        {
            display.GetComponent<Text>().text = tryedAnswer;
        }
        if (tryedAnswer == Answer)
        {
            source.clip = good;
            Solved = true;
            indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        }
        else if (counter == size)
        {
            if (display) display.GetComponent<Text>().text = emptyDisplayValue;

            tryedAnswer = "";
            counter = 0;
            source.clip = bad;
        }
        
        source.Play();
    }
    public void Update()
    {
        if (istimer)
        {
            defactotime -= Time.deltaTime;
            if ((int)defactotime < time) {
                --time;
                display.GetComponent<Text>().text = $"{time / 60 / 10}{time / 60 % 10}:{time % 60 / 10}{time % 60 % 10}";
                source.clip = click;
                source.Play();
            }
            if (time <= 0) 
            {
                failed = true;
            }
        }
    }
}
