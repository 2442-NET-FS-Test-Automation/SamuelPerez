using Library.Data.Entities;

namespace Library.Api.Fulfillment;

public class BurstPlanner
{
    public IReadOnlyList<int> OrderByPriority(IEnumerable<Order> orders)
    {
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        foreach (Order o in orders)
        {
            pq.Enqueue(o.Id, o.Priority == Priority.Expedited ? 0 : 1);
        }

        var orderedByPriority = new List<int>();

        while (pq.TryDequeue(out int id, out _))
        {
            orderedByPriority.Add(id);
        }

        return orderedByPriority;
    }
}