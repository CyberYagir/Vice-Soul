using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeeInfo : MonoBehaviour
{
    public GameObject info;
    public GameObject oldinfo;
    public TMP_Text infoTextShadow;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((oldinfo == null && info != oldinfo) || info == null)
        {
            if (oldinfo != null)
            {
                oldinfo.GetComponent<MobInfo>().over = false;
            }
            GetComponent<PlayerUI>().infoText.gameObject.SetActive(false);
        }
        var mb = new MobInfo();
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, -20), Vector3.forward);
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.TryGetComponent<MobInfo>(out mb))
            {
                infoTextShadow.text = GetComponent<PlayerUI>().infoText.text;
                GetComponent<PlayerUI>().infoText.gameObject.SetActive(true);
                mb.over = true;
            }
        }
        oldinfo = info;
        if (mb != null)
        {
            info = mb.gameObject;
        }
        else
        {
            info = null;
        }
    }
}
