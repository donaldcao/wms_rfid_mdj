using THOK.Common.Ef.EntityRepository;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using System.Linq;

namespace THOK.SMS.Dal.EntityRepository
{
    public class SupplyPositionStorageRepository : RepositoryBase<SupplyPositionStorage>, ISupplyPositionStorageRepository
    {
        public IQueryable<SupplyPositionStorage> GetQueryableIncludeSupplyPosition()
        {
            return this.dbSet.Include("SupplyPosition")
                             .AsQueryable<SupplyPositionStorage>();
        }
    }
}
