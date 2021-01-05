using System;
using System.Collections.Generic;

namespace DXStats.Domain.Entity
{
    public class Snapshot
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public ICollection<DayVolume> DayVolumes { get; set; }
        public ICollection<DayCompletedOrders> DayCompletedOrders { get; set; }

    }
}
