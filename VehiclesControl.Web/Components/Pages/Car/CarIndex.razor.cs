using Microsoft.AspNetCore.Components;
using MudBlazor;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Web.Components.Pages.Car
{
    public partial class CarIndex
    {
        [Inject] public ApiRequest ApiRequest { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] private IDialogService DialogService { get; set; }

        private IEnumerable<CarResponse> CarResponses { get; set; }
        private MudTable<CarResponse> _table { get; set; }
        private CarResponse CarResponse { get; set; }
        private string SearchString { get; set; } = string.Empty;
        private string IdFilter { get; set; }
        private string WheelsFilter { get; set; }
        private string ColorFilter { get; set; }

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

         private IEnumerable<CarResponse> FilteredCars => CarResponses?
            .Where(x => string.IsNullOrWhiteSpace(SearchString) || x.Id.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
                        || x.Wheels.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
                        || x.Color.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(IdFilter) || x.Id.ToString().Contains(IdFilter, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(WheelsFilter) || x.Wheels.ToString().Contains(WheelsFilter, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(ColorFilter) || x.Color.Contains(ColorFilter, StringComparison.OrdinalIgnoreCase));

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
