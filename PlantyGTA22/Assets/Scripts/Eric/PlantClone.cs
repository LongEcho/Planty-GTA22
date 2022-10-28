using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlantClone : MonoBehaviour
{
    public GameObject[] PlantsTypes;
    //public GameObject[] Spawnpoints;
    //public GameObject FruitPrefab;
    RaycastHit hit;
    int currentType;
    [HideInInspector] public float waterGot;
    bool watered;
    bool watered1;
    [HideInInspector] public bool canEarn;
    public GameObject ViewpointPrefab;
    Viewpoint viewpoint;
    bool finishedGrowing;

    [Header("Parameter")]
    public int nebelverdr�ngung;
    public float maxGrowTime;
    //maxGrowTime divided with PlantsTypes.Length
    float growTimePerType;
    //private int fruitCount;
    //public float fruitGrowTime;
    public float waterNeeded;

    [Header("Spreading")]
    public float spreadTime;
    public float minspreadRange;
    public float maxspreadRange;
    //how many times can spread
    public int spreadCount;
    public string SpawnPlantPrefab;

    [Header("Seeds")]
    public string seedName;
    public int seedCount;
    public int seedTime;
    public string playerTag = "GameController";
    
    bool spawnSeeds;
    bool collectSeeds;

    // Start is called before the first frame update
    void Start()
    {
        spawnSeeds = false;

        //gets to the ground
        Ray ray = new Ray(transform.position, -Vector3.up);

        Debug.DrawRay(transform.position, Vector3.down * 100, Color.red);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Ground")
            {
                float heightAboveGround = hit.distance;
                transform.localPosition = new Vector3(transform.localPosition.x, -heightAboveGround + 0.1f, transform.localPosition.z);
            }
        }

        growTimePerType = maxGrowTime / PlantsTypes.Length;

        viewpoint = Instantiate(ViewpointPrefab, transform.position, transform.rotation).GetComponent<Viewpoint>();
        viewpoint.PointText = "Pour!";
        PlantsTypes[currentType].SetActive(true);
    }

    private void Update()
    {
        if (waterGot > waterNeeded && !watered && !watered1)
        {
            watered = true;
            watered1 = true;
        }
        if (watered == true)
        {
            Invoke("Evolve", growTimePerType);
            viewpoint.PointText = "Grows";
            watered = false;
        }
        if (spawnSeeds)
        {
            StartCoroutine("SpawnSeeds");
        }

        RandomFlipValue();
    }

    public void Evolve()
    {
        if (PlantsTypes.Length > 0 && PlantsTypes.Length > currentType + 1)
        {
            PlantsTypes[currentType].SetActive(false);
            currentType += 1;
            PlantsTypes[currentType].SetActive(true);




            //Restarts this Progress
            Invoke("Evolve", growTimePerType);
        }

        //if finished
        if (PlantsTypes.Length < currentType + 2)
        {
            PlayerPrefs.SetInt("Nebelverdrangswert", PlayerPrefs.GetInt("Nebelverdrangswert", 0) + nebelverdr�ngung);

            if (!finishedGrowing)
            {
                //Invoke("SpawnFruits", fruitGrowTime);
                StartCoroutine("SpawnPlants");
                spawnSeeds = true;
                finishedGrowing = true;
            }
        }
    }

    //List<GameObject> fruits = new List<GameObject>();
    //GameObject fruit;

    //void SpawnFruits()
    //{
    //    for (int i = 0; i < fruitCount; i++)
    //    {
    //        fruit = Instantiate(FruitPrefab, Spawnpoints[i].transform.position, Spawnpoints[i].transform.rotation);
    //        fruits.Add(fruit);
    //        canEarn = true;
    //        viewpoint.PointText = "Earn Fruits!";
    //        if (i + 1 == fruitCount)
    //        {
    //            Invoke("SpawnPlants", spreadTime);
    //            spreadCount -= 1;
    //        }
    //    }
    //}

    //void SpawnPlants()
    //{
    //    if (spreadCount > 0)
    //    {
    //        Invoke("SpawnFruits", fruitGrowTime);
    //        viewpoint.PointText = "Grows";
    //    }
    //    for (int i = 0; i < fruitCount; i++)
    //    {
    //        Vector3 position = transform.position + new Vector3(Random.Range(minspreadRange, maxspreadRange) * randomValue, 0, Random.Range(minspreadRange, maxspreadRange) * randomValue);
    //        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    //        Instantiate(Resources.Load(SpawnPlantPrefab), position, rotation);
    //    }
    //    //for (int i = 0; i < fruits.Count; i++)
    //    //{
    //    //    Destroy(fruits[i]);
    //    //}
    //}


    int randomValue;
    void RandomFlipValue()
    {
        randomValue = Random.Range(-1, 2);
        if (randomValue == 0)
            return;
    }

    //public void DestroyPlant()
    //{
    //    for (int i = 0; i < fruits.Count; i++)
    //    {
    //        Destroy(fruits[i]);
    //    }
    //    viewpoint.DestroyUI();
    //    Destroy(gameObject);
    //}

    IEnumerator SpawnPlants()
    {

        if (spreadCount >= 0)
        {
            for (int i = 0; i < spreadCount; i++)
            {
                yield return new WaitForSeconds(spreadTime);

                Vector3 spawn_position = transform.position + new Vector3(Random.Range(minspreadRange, maxspreadRange), 0, Random.Range(minspreadRange, maxspreadRange));

                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                Instantiate(Resources.Load(SpawnPlantPrefab), spawn_position, rotation);
            }
        }
    }

    IEnumerator SpawnSeeds()
    {
        spawnSeeds = false;
        yield return new WaitForSeconds(seedTime);
        collectSeeds = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && collectSeeds)
        {
            PlayerPrefs.SetInt(seedName, PlayerPrefs.GetInt(seedName, 0) + seedCount);
            collectSeeds = false;
            spawnSeeds = true;
        }
    }
}