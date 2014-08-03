using Trading.Common.Data;

namespace Trading.Common.Utils
{
    public interface IHasDataAccessFactory<T> where T : DataAccess
    {
        IDataAccessFactory<T> DataAccessFactory { get; set; }
    }
}