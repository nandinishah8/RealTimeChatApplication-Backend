using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<ActionResult<IEnumerable<Logs>>> GetLogs(DateTime? startTime, DateTime? endTime)
        {
            var logs = await _logRepository.GetLogs(startTime, endTime);

            if (logs == null)
            {
                return new NotFoundObjectResult(new { message = "Logs not found" });
            }

            return new OkObjectResult(logs);
        }
    }
}
