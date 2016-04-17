using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipColor : MonoBehaviour
{

    List<Color> original;
    List<Material> mats;

    bool isChanging = false;
    float currColorTime = 0f;

    // Use this for initialization
    void Start()
    {
        mats = new List<Material>(GetComponent<Renderer>().materials);
        original = new List<Color>(mats.Count);
        foreach (Material mat in mats)
        {
            original.Add(mat.color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currColorTime > 0f)
        {
            currColorTime -= Time.deltaTime;
        }
        else if (isChanging)
        {
            isChanging = false;
            currColorTime = 0f;
            for (int i = 0; i < mats.Count; i++)
            {
                mats[i].color = original[i];
            }
        }
    }

    public void FlashColor(Color color, float time)
    {
        foreach (Material mat in mats)
        {
            mat.color = color;
        }
        currColorTime = time;
        isChanging = true;
    }
}
