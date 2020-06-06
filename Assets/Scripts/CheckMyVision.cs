﻿using UnityEngine;

public class CheckMyVision : MonoBehaviour
{
    //How sensitive we are about vision/Line of sight?
    public enum enmSensitivity { HIGH, LOW};

    //Variable to check sensitivity
    public enmSensitivity sensitivity = enmSensitivity.HIGH;

    //Are we able to see the target right now?
    public bool targetInSight = false;

    //Field of Vision
    public float fieldOfVision = 45f;

    //We need a reference to our target here as well
    private Transform target = null;

    //Reference to our eyes - yet to add
    public Transform myEyes = null;

    //My transform component?
    private Transform npcTransform = null;

    //My sphere collider
    private SphereCollider sphereCollider = null;

    //Last known sighting of object?
    public Vector3 lastKnownSighting = Vector3.zero;

    private void Awake()
    {
        npcTransform = GetComponent<Transform>();
        sphereCollider = GetComponent<SphereCollider>();
        lastKnownSighting = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); //Okay we shall tag this later

    }

    bool InMyFieldOfVision()
    {
        Vector3 dirToTarget = target.position - myEyes.position;

        //Get angle between forward and view direction
        float angle = Vector3.Angle(myEyes.forward, dirToTarget);

        //Let us check if within field of view
        if (angle <= fieldOfVision)
        {
           // Debug.Log("Player in my field of vision");
            return true;
        }
        else
            return false;
    }
    
    //We need a function to check line of sight
    bool ClearLineofSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(myEyes.position, 
            (target.position - myEyes.position).normalized,
            out hit, sphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                //Debug.Log("Player Clear Line of Sight");
                return true;
            }
            
        }
        return false;


    }

    void UpdateSight()
    {
        switch (sensitivity)
        {
            case enmSensitivity.HIGH:
                targetInSight = InMyFieldOfVision() && ClearLineofSight();
                break;
            case enmSensitivity.LOW:
                targetInSight = InMyFieldOfVision() || ClearLineofSight();
                break;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();

        //Update last known sighting
        if (targetInSight)
            lastKnownSighting = target.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
          return;
      targetInSight = false;
    }

}
