using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_shoe : MonoBehaviour
{
    public static int index = 0;
    public static bool animation_flag = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void click_change_shoe()
    {
        index = (index + 1) % 5;
        for (int i = 0; i < TCP.instance.R_shoes.Count; i++)
        {
            if (index == i)
            {
                TCP.instance.R_shoes[i].SetActive(true);
                TCP.instance.L_shoes[i].SetActive(true);
                Debug.Log(string.Format("SetActive Active index is: {0}", index));
            }
            else
            {
                TCP.instance.R_shoes[i].SetActive(false);
                TCP.instance.L_shoes[i].SetActive(false);
                Debug.Log(string.Format("SetActive deActive index is: {0}", index));
            }
        }
        
        Debug.Log(string.Format("Current index is: {0}", index));
    }

    public void click_right()
    {
        index = (index + 1) % 5;
        animation_flag = true;
    }

    public void click_left()
    {
        if (index == 0)
            index = 4;
        else
            index = (index - 1) % 5;
        animation_flag = true;
    }
}
