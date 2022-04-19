using BlazorSMHI.Shared.Entities;

namespace BlazorSMHI.Client.Services
{
    public class NotifierService : INotifierService
    {

        public async Task RefreshDailyStats(string key, Machine machine)
        {
            if (Notify != null)
            {
                await Notify.Invoke(key, machine);
            }
        }

        public event Func<string, Machine, Task>? Notify;
    }
}
