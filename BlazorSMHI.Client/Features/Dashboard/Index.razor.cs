using Microsoft.AspNetCore.Components;
using BlazorSMHI.Client.Services;
using BlazorSMHI.Shared.Entities;

namespace BlazorSMHI.Client.Features.Dashboard
{
    public partial class Index : IDisposable
    {
        public List<Machine> Machines { get; set; } = new List<Machine>();

        [Inject]
        public IMCDataService MCDataService { get; set; } = default!;

        [Inject]
        INotifierService Notifier { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Machines = (await MCDataService.GetAllMachines()).ToList();
            Notifier.Notify += OnNotify;
        }

        public async Task OnNotify(string key, Machine machine)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            Notifier.Notify -= OnNotify;
        }

        private void ToggleMachine(object checkedValue, Machine machine)
        {
            machine.Status = (bool)checkedValue ? MachineStatus.Online : MachineStatus.Offline;
            MCDataService.UpdateMachine(machine);
            Notifier.RefreshDailyStats("status", machine);
        }

        private void DeleteMachine(Machine machine)
        {
            MCDataService.DeleteMachine(machine.Id);
            Machines.Remove(machine);
            Notifier.RefreshDailyStats("delete", machine);
        }
    }
}