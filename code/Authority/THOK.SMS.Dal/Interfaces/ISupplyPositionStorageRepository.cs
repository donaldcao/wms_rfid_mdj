using THOK.Common.Ef.Interfaces;
using THOK.SMS.DbModel;
using System.Linq;

namespace THOK.SMS.Dal.Interfaces
{
    public interface ISupplyPositionStorageRepository : IRepository<SupplyPositionStorage>
    {
        IQueryable<SupplyPositionStorage> GetQueryableIncludeSupplyPosition();
    }
}
