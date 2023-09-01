namespace MinimalChatApplication.Interfaces
{
    public interface ILogRepository
    {
        Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime);

    }
}
