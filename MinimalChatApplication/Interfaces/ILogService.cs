using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface ILogService
    {
        Task<ActionResult<IEnumerable<Logs>>> GetLogs(DateTime? startTime, DateTime? endTime);
    }
}
