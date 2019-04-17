using System;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    Vector3 locationOfTask;
    public string taskName;
    public enum Type
    {
        Register,
        Combine,
        Cook,
        Clean,
        CallFood
    }
    public Type type;
    public static GameObject blankPrefab;
    /// <summary>
    /// This is the delegate for a function that is to be called for the task's actual code
    /// This will be overriden when creating the task with whatever function is actually required
    /// of that task i.e. Find the location of a container with a specific ingredient, then send that Container and location
    /// to it's next task, which is to pickup that item. This delegate can be used to string together various tasks through 
    /// a custom TaskList manager, and will make dynamically programming for tasks much much easier.
    /// </summary>
    public DoTask task;
    public DoTask taskComplete;
    /// <summary>
    /// The higher the priority the sooner the employee will do it. By default 
    /// all tasks will have a priority of 0
    /// </summary>
    public int priority;
    /// <summary>
    /// The task's spot in the last array it was in (so it retains it's priority within it's own priority group)
    /// (i.e. if there have been 4 tasks of priority 0 added, but a task of priority 1 is added, we still want to
    /// retain the same order of those first 4 tasks, this is what that number is used for)
    /// </summary>
    public int previousTaskIndex;
    /// <summary>
    /// The delegate's declaration
    /// </summary>
    public delegate void DoTask();
    
    public Vector3 getLocation(){ return locationOfTask; }
    public void setLocation(Vector3 location) { locationOfTask = location; }
    /// <summary>
    /// This function's only purpose is to allow anything outside it's scope to call the delegated function
    /// </summary>
    public void Do()
    {
        Debug.Log("Do was called for " + taskName);
        task();
    }
    public void Complete()
    {
        Debug.Log("TaskComplete was called");
        if (taskComplete == null)
            return;
        taskComplete();
    }
    
    /// <summary>
    /// The only way to set the Task delegate. Note that this will override any other 
    /// delegations made, as the delegate can only hold one function, on purpose
    /// </summary>
    /// <param name="func">An 'Action' (() =>func) to set the delegate to</param>
    public void  SetTaskDelegate(DoTask func)
    {
        Debug.Log("Task was delegated to " + func.ToString());
        task = func;
    }
    private void Start()
    {
        //task = new DoTask(NoTask);
    }
    private void Awake()
    {
        blankPrefab = Resources.Load("Blank Task") as GameObject;
    }
    void NoTask()
    {
        Debug.Log("No Task!");
    }

    public static Task CreateTask(Transform parent)
    {
        return Instantiate(blankPrefab.GetComponent<Task>(), parent) as Task;
    }

}
