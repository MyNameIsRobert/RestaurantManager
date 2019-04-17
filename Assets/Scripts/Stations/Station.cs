using UnityEngine;

public class Station : MonoBehaviour
{
    public PlayerController player;
    public Employee currentEmployee;
    public Enums.StationType stationType;
    public delegate void OnComplete();
    public OnComplete onComplete;

    public GameObject employeeConnectionCube, customerConnectionCube;

    public void SetOnComplete(OnComplete comp)
    {
        onComplete = comp;
    }
}
