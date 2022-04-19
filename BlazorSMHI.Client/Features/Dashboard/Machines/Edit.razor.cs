using BlazorSMHI.Client.Services;
using BlazorSMHI.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorSMHI.Client.Features.Dashboard.Machines
{
    public partial class Edit : IDisposable
    {
        [Inject]
        public IMCDataService MCDataService { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        INotifierService Notifier { get; set; } = default!;

        [Parameter]
        public string MachineId { get; set; } = string.Empty;

        public Machine Machine { get; set; } = new Machine();
        public List<MachineType> MachineTypes { get; set; } = new List<MachineType>();
        public List<MachineLocation> MachineLocations { get; set; } = new List<MachineLocation>();

        protected string Message { get; set; } = string.Empty;
        protected string StatusClass { get; set; } = string.Empty;
        protected bool Saved { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Saved = false;
            Notifier.Notify += OnNotify;
            MachineTypes = (await MCDataService.GetAllMachineTypes()).ToList();
            MachineLocations = (await MCDataService.GetAllMachineLocations()).ToList();

            Guid.TryParse(MachineId, out var machineId);

            if (machineId == Guid.Empty)
            {
                Machine = new Machine
                {
                    Status = MachineStatus.Offline,
                    MachineTypeId = MachineTypes.Count > 0 ? MachineTypes.First().Id : default!,
                    MachineLocationId = MachineLocations.Count > 0 ? MachineLocations.First().Id : default!
                };
            }
            else
            {
                Machine = await MCDataService.GetMachineDetails(machineId) ?? new Machine();
            }
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

        protected async Task HandleValidSubmit()
        {
            Saved = false;

            if (Machine.Id == Guid.Empty)
            {
                Machine.MachineType = MachineTypes.First(t => t.Id == Machine.MachineTypeId);
                Machine.MachineLocation = MachineLocations.First(t => t.Id == Machine.MachineLocationId);
                var addedMachine = await MCDataService.AddMachine(Machine);
                if (addedMachine != null)
                {
                    StatusClass = "alert-success";
                    Message = "New machine added successfully!";
                    Saved = true;
                    await Notifier.RefreshDailyStats("add", Machine);
                }
                else
                {
                    StatusClass = "alert-danger";
                    Message = "Something went wrong adding the new machine, please try again.";
                    Saved = false;
                }
            }
            else
            {
                Machine.MachineType = MachineTypes.First(t => t.Id == Machine.MachineTypeId);
                Machine.MachineLocation = MachineLocations.First(t => t.Id == Machine.MachineLocationId);
                await MCDataService.UpdateMachine(Machine);
                StatusClass = "alert-success";
                Message = "Machine updated successfully!";
                Saved = true;
            }

        }

        protected void HandleInvalidSubmit()
        {
            StatusClass = "alert-danger";
            Message = "There are some validation errors, please try again.";
        }

        protected void NavigateToDashboard()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}