using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script assumes the user has been drawing a magic circle.
//On release of the mouse, it estimates the shape that was being drawn, and sets a new LineRenderer with that shape.
//For development purposes, the shapes will be hard coded. In a future version, I hope to check which shape is closest
public class EstimateShape : MonoBehaviour
{
    private SpawnOnClick spawnScript;


    //Good numbers are 0.2 and 0.9, respectively
    public float circleSensitivityScalar; //The public float that adjusts how tightly we look for a circle
    public float percentNeededForValidity; //The public float that dictates how many points to consider, eliminates some outliers


    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spawnScript = GetComponent<SpawnOnClick>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (lineRenderer == null) //this could happen if the instantiation of the lineRenderer in SpawnOnClick happens after this script starts
            {
                lineRenderer = GetComponent<LineRenderer>();
            }


            Vector3 centroid = spawnScript.getCentroidCoords();

            bool validity = isCircle(lineRenderer, centroid);

            if(validity)
            {
                Debug.Log("It's a circle!");
                //If valid, then draw the calculated sphere
            }
            else
            {
                Debug.Log("This is not a circle...");
                //Else fail the creation of the circle
            }
        }
    }

    private bool isCircle(LineRenderer lineRendererInput, Vector3 centroid)
    {
        //Get average radius from center
        //Will potentially implement an option to remove outliers later.
        //This will likely also be influenced by a sensitivity function in the game options.

        float sumDistances = 0.0f;

        for (int i = 0; i<lineRendererInput.positionCount; i++)
        {
            sumDistances += Vector3.Distance(centroid, lineRendererInput.GetPosition(i));
        }

        float avgDistance = sumDistances / lineRendererInput.positionCount;

        int valid = 0;

        for (int i = 0;i<lineRendererInput.positionCount;i++)
        {
            float dist = Vector3.Distance(centroid, lineRendererInput.GetPosition(i));
            //Debug.Log("Distance between position and centroid: " + dist);
            //Debug.Log("Scaled distance needed: " + circleSensitivityScalar * avgDistance);

            if ( Math.Abs(dist  - avgDistance) 
                < circleSensitivityScalar*avgDistance)
            {
                valid++;
            }
        }

        Debug.Log("Num Valid of total = " + valid + " / " + lineRendererInput.positionCount);
        
        if( ((float)valid / (float)lineRenderer.positionCount) >= percentNeededForValidity)
        {
            return true;
        }

        return false;
    }
}
