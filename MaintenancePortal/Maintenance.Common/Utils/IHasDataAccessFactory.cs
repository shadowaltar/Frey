using Maintenance.Common.Data;

namespace Maintenance.Common.Utils
{
    public interface IHasDataAccessFactory<T> where T : DataAccess
    {
        IDataAccessFactory<T> DataAccessFactory { get; set; }
    }
}