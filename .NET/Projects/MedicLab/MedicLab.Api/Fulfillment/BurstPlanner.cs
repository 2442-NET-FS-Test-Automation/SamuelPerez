using MedicLab.Data.Entities;

namespace MedicLab.Api.Fulfillment;

public class BurstPlanner
{
    public IReadOnlyList<int> OrderByPriority(IEnumerable<AppointmentOrder> appointmentOrders)
    {
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();
        
        foreach ( AppointmentOrder appointmentOrder in appointmentOrders)
        {
            pq.Enqueue(appointmentOrder.AppointmentOrderId, appointmentOrder.Priority == Priority.Expedited ? 0 : 1);
        }

        List<int> orderedByPriority = new List<int>();

        while (pq.TryDequeue(out int appointmentOrderId, out _))
        {
            orderedByPriority.Add(appointmentOrderId);
        }

        return orderedByPriority;
    }
}