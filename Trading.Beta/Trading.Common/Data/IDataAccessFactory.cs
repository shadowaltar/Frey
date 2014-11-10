namespace Trading.Common.Data
{
    /// <summary>
    /// Interface for a DataAccess's factory class, for IMAP system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccessFactory<out T> where T : DataAccess
    {
        T New();
        T NewTransaction();
    }
}