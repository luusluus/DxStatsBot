using DXStats.Interfaces;

namespace DXStats.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DxStatsDbContext context;

        public UnitOfWork(DxStatsDbContext context)
        {
            this.context = context;
        }
        public void Complete()
        {
            context.SaveChanges();
        }
    }
}
