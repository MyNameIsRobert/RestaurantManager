using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EmployeeManager : MonoBehaviour
{
    public List<Employee> hiredEmployees = new List<Employee>();

    public List<Employee> appliedEmployees = new List<Employee>();

    public List<Employee> currentWorkingEmployees;

    public List<Station> workableStations = new List<Station>();
    public bool acceptingApplications;

    public float applicant_Modifier = 1;
    public float stationCheck_StepTime = .15f;
    public int maxApplicants;
    TimeManager tmanager;
    public GameObject emptyApplicant_Prefab;
    public Enums.StationType[] stationPriorities;
    public List<bool> stationTasked = new List<bool>();
    IEnumerator ApplicantManager()
    {
        while(true)
        {
            float randomStep = Random.Range(tmanager.realTime_ToHour_Seconds / 2, tmanager.realTime_ToHour_Seconds);
            if(!acceptingApplications)
            {
                yield return new WaitUntil(() => acceptingApplications);
            }
            int numOfNewApplicants = (int)(Random.Range(0, 5) * applicant_Modifier);
            for(int i = 0; i < numOfNewApplicants; i++)
            {
                if(appliedEmployees.Count >= maxApplicants)
                {
                    break;
                }
                Employee e = new Employee();
                e.Randomize();
                Employee go = Instantiate(emptyApplicant_Prefab.GetComponent<Employee>(), transform);
                go.CopyFrom(e);
                appliedEmployees.Add(go);
            }

            yield return new WaitForSeconds(randomStep);

            if((float)appliedEmployees.Count / (float)maxApplicants > .75f)
            {
                int removeAmount = Random.Range(0, 3);
                for (int i = 0; i < removeAmount; i++)
                    appliedEmployees.RemoveAt(Random.Range(0, appliedEmployees.Count - 1));
            }

        }

    }
    public void StationTaskComplete(Station s)
    {
        stationTasked[workableStations.IndexOf(s)] = false;
    }
    IEnumerator StationManager()
    {
        while (true)
        {
            int count = 0;
            foreach(Station s in workableStations)
            {
                switch (s.stationType)
                {
                    case Enums.StationType.Combine:
                        break;
                    case Enums.StationType.Container:
                        break;
                    case Enums.StationType.FoodPickup:
                        break;
                    case Enums.StationType.Grill:
                        break;
                    case Enums.StationType.Register:
                        if (s is RunRegister)
                        {
                            RunRegister r = s as RunRegister;
                            if (r.customer && !r.currentEmployee)
                            {
                                Debug.Log("Waiting customer at " + r);
                                if (!stationTasked[count])
                                {
                                    foreach (Employee e in currentWorkingEmployees)
                                    {
                                        if (e.CheckForAI<CashierAI>())
                                        {
                                            CashierAI cAI = (CashierAI)e.GetAI<CashierAI>();
                                            if (cAI.AddWaitingRegister(r))
                                            {
                                                stationTasked[count] = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                        else
                            Debug.Log(s.gameObject);
                        break;
                }
                count++;
                
            }
            yield return new WaitForSeconds(stationCheck_StepTime);
        }
    }
    void SortStations()
    {

    }
    private void Start() 
    {
        tmanager = FindObjectOfType<TimeManager>();
        StartCoroutine(ApplicantManager());
        StartCoroutine(StationManager());
        Station[] s = FindObjectsOfType<Station>();
        foreach (Station st in s)
        {
            workableStations.Add(st);
            stationTasked.Add(false);
        }
    }
    public Station GetStation<T>() where T: Station
    {
        foreach (Station s in workableStations)
            if (s is T)
                return s as T;
        return null;
    }
}
