using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[ExecuteInEditMode]
public class WorkInEditor : MonoBehaviour
{
    public bool isWork;


    private void Update()
    {
        if (isWork)
        {
            //string[] strings = File.ReadAllLines(@"C:\items.txt");
            //for (int i = 0; i < strings.Length; i++)
            //{
            //    GetComponent<WorldManager>().items[i].descriptionen = strings[i];
            //}
            //isWork = !isWork;
        }
    }
}
