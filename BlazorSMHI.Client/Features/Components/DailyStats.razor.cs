using BlazorSMHI.Client.Services;
using BlazorSMHI.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSMHI.Client.Features.Components
{
    public partial class DailyStats : IDisposable
    {
        public List<Machine> Machines { get; set; } = new List<Machine>();

        public int CountTotal { get; set; }
        public int CountOnline { get; set; }
        public int CountOffline { get; set; }
        public int NumberOfReadings { get; set; }

        public string RefreshHoverClass { get; set; } = string.Empty;

        public string Visible { get; set; } = "visible";

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IMCDataService MCDataService { get; set; } = default!;

        [Inject]
        INotifierService Notifier { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await RetrieveData();
            Notifier.Notify += OnNotify;
        }

        public async Task OnNotify(string key, Machine machine)
        {
            await InvokeAsync(() =>
            {
                var localMachine = Machines.FirstOrDefault(m => m.Id == machine.Id);

                switch (key)
                {
                    case "delete":
                        Machines.Remove(localMachine!);
                        break;
                    case "status":
                        localMachine!.Status = machine.Status;
                        break;
                    case "add":
                        Machines.Add(machine);
                        break;
                }

                SetStats();
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            Notifier.Notify -= OnNotify;
        }

        private async Task RetrieveData()
        {
            var machines = await MCDataService.GetAllMachines();
            Machines = machines != null ? machines.ToList() : new List<Machine>();
            SetStats();
        }

        private void SetStats()
        {
            CountTotal = Machines.Count();
            CountOnline = Machines.Where(m => m.Status == MachineStatus.Online).Count();
            CountOffline = CountTotal - CountOnline;
            NumberOfReadings = 0;

            foreach (Machine m in Machines)
            {
                DateTime now = DateTime.Now;
                NumberOfReadings += m.MachineData.Where(d => d.Time.Date == now.Date).Count();
            }
        }

        private async Task RefreshStats()
        {
            Visible = "invisible";
            Thread.Sleep(300); // To give a sense of a refresh animation
            await RetrieveData();
            Visible = "visible";
        }

        private void MouseOver(MouseEventArgs e) { RefreshHoverClass = "refresh-hover"; }
        private void MouseOut(MouseEventArgs e) { RefreshHoverClass = ""; }
    }
}