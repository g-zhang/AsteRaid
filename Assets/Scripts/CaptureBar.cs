using UnityEngine;
using System.Collections;

public class CaptureBar : MonoBehaviour {
    [Header("Config")]
    public float maxLen = 2f;
    public float currVal = .5f; // 1 = all red, 0 = all blue

    GameObject redBarObj;
    GameObject blueBarObj;

    // Use this for initialization
    void Start()
    {
        redBarObj = transform.Find("Red").gameObject;
        blueBarObj = transform.Find("Blue").gameObject;
        //maxLen = baseBarObj.transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Vector3 scale = redBarObj.transform.localScale;
        scale.z = currVal * maxLen;
        redBarObj.transform.localScale = scale;

        Vector3 bpos = blueBarObj.transform.localPosition;
        Vector3 bscale = blueBarObj.transform.localScale;
        bpos.x = scale.z;
        bscale.z = maxLen - scale.z;
        blueBarObj.transform.localPosition = bpos;
        blueBarObj.transform.localScale = bscale;
    }
}
