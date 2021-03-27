using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News : MonoBehaviour
{
    public Text text0;
    public Text text1;
    public void Awake()
    {
        StartCoroutine(get());
    }

    IEnumerator get()
    {
        WWWForm form = new WWWForm();
        WWW www = new WWW("http://terrtest.000webhostapp.com/news0.txt", form);
        yield return www;
        if (www.text == "")
        {
            text0.text = "Error";
        }
        text0.text = "";
        //text0.text = www.text;
        var strings = www.text.Split('\n');

        int count = 0;
        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i][0] != '-')
            {
                text0.text += ((strings.Length - count).ToString("000") + " " + strings[i] + "\n");
                count++;
            }
            else
            {
                text0.text += ("    " + strings[i].Remove(0, 1) + "\n"); ;
            }
        }

        form = new WWWForm();
        www = new WWW("http://terrtest.000webhostapp.com/news1.txt", form);
        yield return www;
        text1.text = www.text;
        if (www.text == "")
        {
            text1.text = "Error";
        }
    }
}
