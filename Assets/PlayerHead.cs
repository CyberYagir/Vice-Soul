using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public Transform parent;
    public Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (parent.localScale.x > 0)
        {
            transform.localScale = startScale;
            transform.right = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            transform.localRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, Mathf.Clamp(transform.localRotation.z, -0.5f, 0.3f), transform.localRotation.w);
        }
        if (parent.localScale.x < 0)
        {
            transform.localScale = new Vector3(-startScale.x, -startScale.y, startScale.z);
            transform.right = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(transform.eulerAngles.z,138,250));
        }
    }
}
