using System;
using System.Collections;
using UnityEngine;

public class EmployeeAI : MonoBehaviour 
{

    public LevelManager lmanager;
    public EmployeeManager emanager;
    public delegate void ReachedLocation();
    public ReachedLocation reached;
    public delegate void TaskComplete();
    public TaskComplete taskComplete;
    public Employee employee;

    public void AddLocation(Vector3 location, ReachedLocation onReached)
    {
        Debug.Log("Add location was called");
        reached = onReached;
        StartCoroutine(WaitUntilAtLocation(location));
    }
    IEnumerator WaitUntilAtLocation(Vector3 location)
    {
        Debug.Log("Waiting to arrive at " + location);
        yield return new WaitUntil(() => Vector3.Distance(location, transform.position) < .5f);
        if(reached != null)
        {
            Debug.Log("Reached was called");
            reached();
        }

        Debug.Log("Arrived");
        employee.agent.isStopped = true;
    }
    public virtual void  MoveToStation()
    {

    }
    public virtual void DoStationWork()
    {

    }
    public void Complete()
    {

        Debug.Log("Complete called on EAI");
        taskComplete();
    }
}