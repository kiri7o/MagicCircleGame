using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnOnClick : MonoBehaviour
{
    public float distance;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public float minAddLineDistance;

    private Vector3 centroidCoords;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 0;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        if (Input.GetMouseButtonDown(0))
        {
            //On first mouse press
            lineRenderer.positionCount++;
            Vector3 firstSpot = addRayFromCamera();
            lineRenderer.SetPosition(0, firstSpot);
        }

        if(Input.GetMouseButton(0))
        {
            //As mouse held down
            //If the distance between the last point and the current point is great enough,
            //then add a new point to the line renderer

            Vector3 newSpot = addRayFromCamera();
            if(Vector3.Distance(newSpot,lineRenderer.GetPosition(lineRenderer.positionCount-1)) > minAddLineDistance)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount-1, newSpot);
            }
            centroidCoords = calculateCentroid(lineRenderer);
            renderCentroid(centroidCoords);
        }

        //When the mouse is released, delete all points.

        if(Input.GetMouseButtonUp(0))
        {
            lineRenderer.positionCount = 0;
            GameObject centroidSphere = GameObject.Find("centroidTest");
            if(centroidSphere != null )
            {
                GameObject.Destroy(centroidSphere);
            }
        }

        
    }

    private Vector3 addRayFromCamera()
    {
        //First, get ray from camera to mouse position

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Then get ray through center of camera
        Ray fromCamera = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        //Get a point along that centered camera ray a fixed distance from the camera (set in unity)
        Vector3 planePoint = fromCamera.origin + (fromCamera.direction * distance);

        //spawn plane on planepoint, normal to camera ray
        Plane drawPlane = new Plane(Camera.main.transform.forward.normalized, planePoint);

        float planeDist;

        //set planeDist to the distance of our mouse pointer ray from camera to plane
        drawPlane.Raycast(ray, out planeDist);


        Vector3 returnRay = ray.origin + (ray.direction * planeDist);

        return returnRay;
    }

    private Vector3 calculateCentroid(LineRenderer lineRenderer)
    {
        float sumx = 0.0f;
        float sumy = 0.0f;
        float sumz = 0.0f;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            sumx += lineRenderer.GetPosition(i).x;
            sumy += lineRenderer.GetPosition(i).y;
            sumz += lineRenderer.GetPosition(i).z;
        }

        sumx /= lineRenderer.positionCount;
        sumy /= lineRenderer.positionCount;
        sumz /= lineRenderer.positionCount;

        return new Vector3(sumx, sumy, sumz);
    }
    private void renderCentroid(Vector3 centroid)
    {

        GameObject sphere = GameObject.Find("centroidTest");
        if (sphere == null)
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "centroidTest";
        }
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        sphere.transform.position = centroid;

    }

    public Vector3 getCentroidCoords ()
    {
        return centroidCoords;
    }

}
