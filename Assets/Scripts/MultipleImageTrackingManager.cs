using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultipleImageTrackingManager : MonoBehaviour
{
    GameObject Scene;
    private GameObject Target_1;
    private GameObject Target_2;
    GameObject Plane;
    private GameObject Seeker;
    float speed = 1;
    Vector3[] path;
    int targetIndex;
    ARTrackedImage _image;
    [SerializeField]
    private Text imageTrackedText;
    [SerializeField]
    private GameObject[] arObjectsToPlace;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);

    private ARTrackedImageManager m_TrackedImageManager;

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> trashes = new Dictionary<string, GameObject>();

    void Awake()
    {

        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();



        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);
        }
    }


    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }




    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            arObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        // Display the name of the tracked image in the canvas
        //imageTrackedText.text = trackedImage.referenceImage.name;


        // Assign and Place Game Object
        if (trackedImage.referenceImage.name == "Remplie")
        {
            //yield return new WaitForSeconds(1);
            Plane = arObjects[trackedImage.referenceImage.name].transform.Find("Plane").gameObject;
            Seeker = arObjects[trackedImage.referenceImage.name].transform.Find("Seeker").gameObject;
            Target_1 = arObjects[trackedImage.referenceImage.name].transform.Find("Target_1").gameObject;
            Plane.GetComponent<Renderer>().material.color = Color.red;
            //yield return new WaitForSeconds(2); 
           
                PathRequestManager.RequestPath(Seeker.transform.position, Target_1.transform.position, OnPathFound);
        }
        else
        {
            Plane = arObjects[trackedImage.referenceImage.name].transform.Find("Plane").gameObject;
            Seeker = arObjects[trackedImage.referenceImage.name].transform.Find("Seeker").gameObject;
            Target_1 = arObjects[trackedImage.referenceImage.name].transform.Find("Target_1").gameObject;
            Target_2 = arObjects[trackedImage.referenceImage.name].transform.Find("Target_2").gameObject;
            trashes.Add(Target_1.name, Target_1);
            trashes.Add(Target_2.name, Target_2);
            Plane.GetComponent<Renderer>().material.color = Color.yellow;

            foreach (KeyValuePair<string, GameObject> trash in trashes)
            {
                PathRequestManager.RequestPath(Seeker.transform.position, trash.Value.transform.position, OnPathFound);
            }
                

        }

        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);


        Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {

        if (arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name];
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            goARObject.transform.localScale = scaleFactor;
            foreach (GameObject go in arObjects.Values)
            {
                Debug.Log($"Go in arObjects.Values: {go.name}");
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
        }
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        //Delay
        yield return new WaitForSeconds(5);
        Vector3 currentWaypoint = path[0];
        while (true)
        {

            if (Seeker.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Seeker.transform.position = Vector3.MoveTowards(Seeker.transform.position, currentWaypoint, speed / 2 * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}