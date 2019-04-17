using UnityEngine;

public class CashierAI : EmployeeAI 
{
    
    public int registerPriority = 1;
    public bool AddWaitingRegister(RunRegister r)
    {
        if (employee.taskManager.getNumberOfTasks() > 5)
            return false;
        Task takeOrder =  Task.CreateTask(transform);
        takeOrder.type = Task.Type.Register;
        takeOrder.taskName = "Take Order";
        takeOrder.task = r.EmployeeCreateTicket;
        r.onComplete = (delegate ()
        {
            emanager.StationTaskComplete(r);
            Complete();
        });
        takeOrder.priority = registerPriority;
        LevelManager.GridNode node = lmanager.registerPostion[0];
        takeOrder.setLocation(node.position);
        FoodPickup fp = (FoodPickup) emanager.GetStation<FoodPickup>();
        Task addTicket = Task.CreateTask(transform);
        addTicket.type = Task.Type.Combine;
        addTicket.taskName = "Add Ticket to Food Pickup";
        addTicket.task = (delegate() {
            fp.currentEmployee = employee;
            fp.EmployeeAddAllTickets();            
            });
        fp.onComplete = (delegate ()
        {
            Complete();
        });
        node = lmanager.pickupPositionEmployee[0];
        addTicket.setLocation(node.position);
        addTicket.priority = 0;
        employee.taskManager.AddTask(takeOrder);
        employee.taskManager.AddTask(addTicket);
        return true;
    }

}