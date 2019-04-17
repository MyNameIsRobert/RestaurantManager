using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using System;

public class Employee : Person 
{
    #region Variables
    public Station[] workStations;

    //Their preferred schedule and their actual work schedule
    public WorkSchedule preferredSchedule;
    public WorkSchedule actualSchedule;

    //Their current skills
    public Skills skills;    

    public List<Enums.EmployeeInterests> interests = new List<Enums.EmployeeInterests>();

    public List<EmployeeAI> aiControllers = new List<EmployeeAI>();

    public TaskListManager taskManager;

    public Transform hand;
    public NavMeshAgent agent;
    #endregion

    #region Structs
    [System.Serializable]
    public struct WorkSchedule
    {
        public bool any;
        public int[] hours, days, months;

        /// <summary>
        /// Compares the instance schedule with 'schedule'
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns>Percentage [0, 1] of similarity between 'schedule' and the current workschedule instance. Percentage is relative
        /// to the instance schedule. If workschedule instance or 'schedule' are 'any', then will automatically return 1</returns>
        public float CompareSchedule(WorkSchedule schedule)
        {
            if (any)
            {
                return 1;
            }
            int totalSchedule_Nodes = hours.Length + days.Length + months.Length;
            int correctSchedule_Nodes = 0;
            for (int i = 0; i < hours.Length; i++)
            {
                for (int j = 0; j < schedule.hours.Length; j++)
                {
                    if (hours[i] == schedule.hours[j])
                    {
                        correctSchedule_Nodes++;
                        break;
                    }
                }
            }
            for (int i = 0; i < days.Length; i++)
            {
                for (int j = 0; j < schedule.days.Length; j++)
                {
                    if (days[i] == schedule.days[j])
                    {
                        correctSchedule_Nodes++;
                        break;
                    }
                }
            }
            for (int i = 0; i < months.Length; i++)
            {
                for (int j = 0; j < schedule.months.Length; j++)
                {
                    if (months[i] == schedule.months[j])
                    {
                        correctSchedule_Nodes++;
                        break;
                    }
                }
            }

            return (float)correctSchedule_Nodes / (float)totalSchedule_Nodes;
        }
        public WorkSchedule(int[] h, int[] d, int[] m)
        {
            hours = h;
            days = d;
            months = m;
            any = false;
        }
        public WorkSchedule(bool a)
        {
            if(a)
            {
                any = a;
                hours = null;
                days = null;
                months = null;
            }
            else
            {
                any = a;
                hours = null;
                days = null;
                months = null;
            }
        }
        public static WorkSchedule emptySchedule = new WorkSchedule(null, null, null);
    }
    [System.Serializable]
    public struct Skills
    {
        //From -100 to 100 intially, can go beyond those limits by learning skills
        public int people, cooking, cleaning, workSpeed, moveSpeed, liftStrength;
        float people_F, cooking_F, cleaning_F, workSpeed_F, moveSpeed_F, liftStrength_F;
        public enum SkillType
        {
            People,
            Cooking,
            Cleaning,
            Speed,
            Strength
        }

        public void AddToSkill(SkillType type, float amount)
        {
            switch (type)
            {
                case Skills.SkillType.Cleaning:

                    break;
                case Skills.SkillType.Cooking:
                    break;
                case Skills.SkillType.People:
                    break;
                case Skills.SkillType.Speed:
                    break;
                case Skills.SkillType.Strength:
                    break;
            }
        }

        public void RemoveFromSkill(SkillType type, float amount)
        {
            switch (type)
            {
                case Skills.SkillType.Cleaning:
                    break;
                case Skills.SkillType.Cooking:
                    break;
                case Skills.SkillType.People:
                    break;
                case Skills.SkillType.Speed:
                    break;
                case Skills.SkillType.Strength:
                    break;
            }
        }

        
    }
    #endregion

    #region Constructors
    public Employee()
    {
        workStations = null;
        preferredSchedule = WorkSchedule.emptySchedule;
        actualSchedule = WorkSchedule.emptySchedule;
    }
    public Employee(bool random)
    {

    }
    #endregion

    #region Public Functions
    public void Randomize()
    {

    }
    public void LearnSkill(Skills.SkillType type, float amount)
    {
        skills.AddToSkill(type, amount);
    }
    public void UnLearnSkill(Skills.SkillType type, float amount)
    {
        skills.RemoveFromSkill(type, amount);
    }

    public override void CopyFrom(Person p)
    {
        base.CopyFrom(p);
        if (p is Employee)
        {
            Employee e = p as Employee;
            workStations = e.workStations;
            preferredSchedule = e.preferredSchedule;
            actualSchedule = e.actualSchedule;
            interests = e.interests;
            skills = e.skills;
        }
    }

    public void AddItemToHand(GameObject item)
    {
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;
    }
    #endregion

    private void Start()
    {
        taskManager = new TaskListManager(this);
        EmployeeAI[] cont = transform.GetComponentsInChildren<EmployeeAI>();
        foreach (EmployeeAI a in cont)
        {
            aiControllers.Add(a);
            a.employee = this;
        }
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {

    }
    public bool CheckForAI<T>() where T: EmployeeAI
    {
        foreach(EmployeeAI a in aiControllers)
        {
            if (a is T)
                return true;
        }
        return false;
    }
    //Returns a script attached to the Employee if it is of the 
    //type of the specified derrived class of EmployeeAI (i.e CashierAI)
    public EmployeeAI GetAI<T>() where T: EmployeeAI
    {
        foreach (EmployeeAI a in aiControllers)
            if (a is T)
                return a;
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Register"))
        {
            Transform currentTransform = other.transform;
            RunRegister register;
            register = currentTransform.GetComponent<RunRegister>();
            if (!register)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<RunRegister>())
                    {
                        register = currentTransform.GetComponent<RunRegister>();
                        break;
                    }
                }
            }
            if (register)
            {
                register.currentEmployee = this;
            }
        }
        if (other.CompareTag("Pickup"))
        {

            Transform currentTransform = other.transform;
            FoodPickup pickup = currentTransform.GetComponent<FoodPickup>();
            if (!pickup)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<FoodPickup>())
                    {
                        pickup = currentTransform.GetComponent<FoodPickup>();
                        break;
                    }
                }
            }
            if (pickup)
            {
                pickup.currentEmployee = this;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Register"))
        {
            Transform currentTransform = other.transform;
            RunRegister register;
            register = currentTransform.GetComponent<RunRegister>();
            if (!register)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<RunRegister>())
                    {
                        register = currentTransform.GetComponent<RunRegister>();
                        break;
                    }
                }
            }
            if (register)
            {
                register.currentEmployee = null;
            }
        }
        if (other.CompareTag("Pickup"))
        {

            Transform currentTransform = other.transform;
            FoodPickup pickup = currentTransform.GetComponent<FoodPickup>();
            if (!pickup)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<FoodPickup>())
                    {
                        pickup = currentTransform.GetComponent<FoodPickup>();
                        break;
                    }
                }
            }
            if (pickup)
            {
                pickup.currentEmployee = null;
            }

        }
    }
    public void StartCheckingForTasks()
    {
        StartCoroutine(CheckForNewTask());
    }
    IEnumerator CheckForNewTask()
    {
        yield return new WaitUntil(() => taskManager.getNumberOfTasks() > 0);
        taskManager.StepToNextTask();
    }
}

[System.Serializable]
public class TaskListManager
{
    [SerializeField]
    Employee employee;
    [SerializeField]
    List<Task> currentTasks;
    public bool checkingForTasks = false;
    public TaskListManager(Employee e)
    {
        currentTasks = new List<Task>();
        employee = e;
        employee.StartCheckingForTasks();
    }
    
    /// <summary>
    /// Sorts all the tasks by priority then by previous index
    /// </summary>
    void SortTasks()
    {
        List<Task> totalTasks = new List<Task>(), tempList = new List<Task>();
        int highestPriority = 0;
        foreach(Task t in currentTasks)
        {
            if (t.priority > highestPriority)
                highestPriority = t.priority;
        }
        int currentTaskIndex = 0;
        for(int i = highestPriority; i >= 0; i--)
        {
            tempList.Clear();
            foreach(Task t in currentTasks)
            {
                if(t.priority == highestPriority)
                {
                    tempList.Add(t);
                }
            }
            int lowestIndex = 959595, highestIndex = -1;
            foreach(Task t in tempList)
            {
                if (t.previousTaskIndex < lowestIndex)
                    lowestIndex = t.previousTaskIndex;
                if (t.previousTaskIndex > highestIndex)
                    highestIndex = t.previousTaskIndex;
            }
            for(int j = lowestIndex; j <= highestIndex; j++)
            {
                foreach (Task t in tempList)
                    if (t.previousTaskIndex == j)
                    {
                        totalTasks.Add(t);
                        t.previousTaskIndex = currentTaskIndex;
                        currentTaskIndex++;
                    }
            }
        }
    }
    public void AddTask(Task t)
    {
        currentTasks.Add(t);
        t.previousTaskIndex = currentTasks.Count - 1;
        SortTasks();
    }
    public int getNumberOfTasks()
    {
        return currentTasks.Count;
    }
    public void StepToNextTask()
    {
        if (getNumberOfTasks() < 1)
        {
            employee.StartCheckingForTasks();
        }
        if (currentTasks.Count == 0)
        {
            return;
        }
        Debug.Log("Stepping to next task");
        Task task = currentTasks[0];
        employee.agent.isStopped = false;
        employee.agent.SetDestination(task.getLocation());
        EmployeeAI eAI = employee.aiControllers[0];
        eAI.AddLocation(task.getLocation(), delegate ()
        {
            task.Do();
            eAI.taskComplete = (delegate ()
            {
                StepToNextTask();
                task.Complete();
            });
        });
            
        currentTasks.RemoveAt(0);
        GameObject.Destroy(task.gameObject);
        
            
    }
}
