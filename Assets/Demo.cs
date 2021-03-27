using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public AnimationClip clip;
    private void Start()
    {
        StartCoroutine(wait());
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            StopAllCoroutines();
            Application.LoadLevel(1);
        }
    }
    public IEnumerator wait() {
        yield return new WaitForSeconds(clip.length); 
        Application.LoadLevel(1);
    }
}
