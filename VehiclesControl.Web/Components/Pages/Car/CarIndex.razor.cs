using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Xml.Linq;
using VehiclesControl.Domain.Outs;
using static MudBlazor.CategoryTypes;

namespace VehiclesControl.Web.Components.Pages.Car
{
    public partial class CarIndex
    {
        [Inject]
        public ApiRequest ApiRequest { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private IEnumerable<CarResponse> CarResponses { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        private bool _readOnly { get; set; }
        private bool _isCellEditMode { get; set; }
        private List<string> _events { get; set; } = new();
        private bool _editTriggerRowClick { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadCars();
        }
        protected async Task LoadCars()
        {
            var res = await ApiRequest.GetFromJsonAsync<ApiResponse<List<CarResponse>>>("/api/v1/Cars");
            if (res != null && res.Data != null)
            {
                CarResponses = res.Data;
            }
        }
        void StartedEditingItem(CarResponse item)
        {
            
        }

        void CanceledEditingItem(CarResponse item)
        {
            
        }

        void CommittedItemChanges(CarResponse item)
        {
            Snackbar.Add($"Edited Car Id: {item.Id}", Severity.Success);
        }
    }
}
