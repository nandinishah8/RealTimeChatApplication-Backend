using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly MinimalChatContext _context;
        public LogRepository(MinimalChatContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime)
        {
            IEnumerable<Logs> Log;

            if (customEndTime.HasValue)
            {
                Log = _context.Log
                 .Where(log => log.Timestamp >= customStartTime && log.Timestamp <= customEndTime);
            }
            else
            {
                Log = _context.Log.Where(log => log.Timestamp <= customStartTime);
            }

            return Log
                .Select(u => new
                {
                    id = u.Id,
                    ip = u.IP,
                    username = u.Username,
                    RequestBody = u.RequestBody.Replace("\n", "").Replace("\"", "").Replace("\r", ""),
                    TimeStamp = u.Timestamp,
                });
        }
    }
}
