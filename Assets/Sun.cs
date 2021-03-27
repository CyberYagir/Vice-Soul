using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public GameObject localPlayer;
    public float height, speed;
    public Light light;
    public LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        transform.position = new Vector3(localPlayer.transform.position.x, localPlayer.transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, mask);

        if (hit.collider != null)
        {
            if (Vector3.Distance(transform.position, hit.point) < 60)
            {
                if (localPlayer.transform.position.y > -205)
                {
                    Vector3 pos = new Vector3(localPlayer.transform.position.x, hit.point.y + height, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, pos, speed);
                }
                else
                {
                    Vector3 pos = new Vector3(localPlayer.transform.position.x, -160, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, pos, speed);
                }
            }
            else
            {
                Vector3 pos = new Vector3(localPlayer.transform.position.x, -160, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, pos, speed);
            }
        }
        if (localPlayer.transform.position.y > -160)
        {
            light.intensity = 0.8f;
        }
        if (localPlayer.transform.position.y < -220)
        {
            if (light.intensity > 0)
            {
                light.intensity -= 0.1f * Time.deltaTime;
            }
        }
        else
        {
            if (light.intensity < 1)
            {
                light.intensity += 0.1f * Time.deltaTime;
            }
        }
    }
}
