using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawned : MonoBehaviour
{
    public GameObject objectToSpawn;
    private PlacementIndicator placementIndicator;
    private GameObject obj;
    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (obj == null)
            {
                obj = Instantiate(objectToSpawn, placementIndicator.transform.position, placementIndicator.transform.rotation);
            }
            else
            {
                obj.transform.position = placementIndicator.transform.position;
            }
        }
    }
}
