using BlazorSMHI.Shared.Entities;

namespace BlazorSMHI.Client.Services
{
    public interface INotifierService
    {
        event Func<string, Machine, Task>? Notify;
        Task RefreshDailyStats(string key, Machine machine);
    }
}