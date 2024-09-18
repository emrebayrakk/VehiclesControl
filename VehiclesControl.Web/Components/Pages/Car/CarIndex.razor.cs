using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting.Server;
using MudBlazor;
using System.Net.Http;
using System.Xml.Linq;
using VehiclesControl.Domain.Entities;
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

        private MudTable<CarResponse> _table { get; set; }

        [Inject] private IDialogService DialogService { get; set; }
        private CarResponse CarResponse { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadCars();
        }
        protected async Task LoadCars()
        {
            var res = await ApiRequest.GetFromJsonAsync<ApiResponse<List<CarResponse>>>("/api/v1/Cars/with-dapper");
            if (res != null && res.Data != null)
            {
                CarResponses = res.Data;
            }
        }
        private void PageChanged(int i)
        {
            _table.NavigateTo(i - 1);
        }
        void StartedEditingItem(CarResponse item)
        {
            
        }

        void CanceledEditingItem(CarResponse item)
        {
            
        }

        async void CommittedItemChanges(CarResponse item)
        {
            var res = await UpdateCar(item);
            if (res)
            {
                Snackbar.Add($"Edited Car Id: {item.Id}", Severity.Success);
                await LoadCars();
            }
            else
            {
                Snackbar.Add($"Error! Not Edited Car Id: {item.Id}", Severity.Error);
                await LoadCars();
            }
            
        }

        public async Task<bool> UpdateCar(CarResponse item)
        {
            var mappedCar = new Domain.Entities.Car
            {
                Id = item.Id,
                Color = item.Color,
                HeadlightsOn = item.HeadlightsOn,
                Wheels = item.Wheels,
                UpdatedDate = DateTime.Now,
                CreatedDate = CarResponse.CreatedDate
            };
            var res = await ApiRequest.PutAsync<ApiResponse<long>,Domain.Entities.Car>("/api/v1/Cars/with-dapper-updated", mappedCar);
            if (res != null && res.Data != null && res.Data != -1)
            {
                await LoadCars();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteCar(CarResponse item)
        {
            var url = $"/api/v1/Cars/with-dapper-delete/{CarResponse.Id}";
            var res = await ApiRequest.DeleteAsync<ApiResponse<long>>(url);
            if (res != null && res.Data != null && res.Data != -1)
            {
                await LoadCars();
                return true;
            }
            return false;
        }
        private async Task OnButtonClicked()
        {
            var parameters = new DialogParameters<DialogTemplate_Dialog>
            {
                { x => x.Car, CarResponse },
            };

            var dialog = await DialogService.ShowAsync<DialogTemplate_Dialog>("Delete or Update Car", parameters);
            var result = await dialog.Result;
            if (result != null)
            {
                if (!result.Canceled)
                {
                    var res = await UpdateCar(CarResponse);
                    if (res)
                    {
                        Snackbar.Add("Car Updated", Severity.Success);
                    }
                }
                if (result.Canceled)
                {
                    var res = await DeleteCar(CarResponse);
                    if (res)
                    {
                        await LoadCars();
                        Snackbar.Add("Car Deleted", Severity.Error);
                    }
                }
            }
            else
            {
                Snackbar.Add("Error!", Severity.Error);
            }
        }
    }
}
