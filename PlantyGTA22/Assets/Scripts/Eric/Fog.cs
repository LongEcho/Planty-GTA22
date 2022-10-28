using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public int Nebelverdrangswert;
    public GameObject fogHolder;

    bool checkPlant = true;



    // Update is called once per frame
    void Update()
    {
        if (checkPlant)
        {
            StartCoroutine("Check");
        }

    }

    IEnumerator Check()
    {
        Debug.Log("cHECK");
        checkPlant = false;
        yield return new WaitForSeconds(5);
        if (PlayerPrefs.GetInt("Nebelverdrangswert", 0) >= Nebelverdrangswert)
        {
            Debug.Log("deaktivatwe");
            fogHolder.SetActive(false);
        }
        checkPlant = true;
    }
}