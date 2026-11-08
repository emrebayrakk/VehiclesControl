using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Web.Models;

namespace VehiclesControl.Web.Services;

/// <summary>Every endpoint the UI calls, in one place.</summary>
public sealed class GalleryApi(ApiClient api)
{
    public const long MaxPhotoBytes = 8 * 1024 * 1024;

    // ----- Account -----
    public Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request) => api.PostAsync<LoginResponse>("api/v1/auth/login", request);
    public Task<ApiResult<UserResponse>> UpdateProfileAsync(UpdateProfileRequest request) => api.PutAsync<UserResponse>("api/v1/auth/profile", request);

    // ----- Super admin -----
    public Task<ApiResult<AdminSummaryResponse>> GetAdminSummaryAsync() => api.GetAsync<AdminSummaryResponse>("api/v1/admin/summary");
    public Task<ApiResult<List<GalleryResponse>>> GetGalleriesAsync(string? search) => api.GetAsync<List<GalleryResponse>>(Url("api/v1/admin/galleries", ("search", search)));
    public Task<ApiResult<GalleryResponse>> CreateGalleryAsync(CreateGalleryRequest request) => api.PostAsync<GalleryResponse>("api/v1/admin/galleries", request);
    public Task<ApiResult<GalleryResponse>> UpdateGalleryAsync(long id, GalleryRequest request) => api.PutAsync<GalleryResponse>($"api/v1/admin/galleries/{id}", request);
    public Task<ApiResult<GalleryResponse>> SetGalleryActiveAsync(long id, bool active) => api.PatchAsync<GalleryResponse>($"api/v1/admin/galleries/{id}/active", new ActiveRequest(active));

    // ----- Own gallery & team -----
    public Task<ApiResult<GalleryResponse>> GetMyGalleryAsync() => api.GetAsync<GalleryResponse>("api/v1/gallery");
    public Task<ApiResult<GalleryResponse>> UpdateMyGalleryAsync(GalleryRequest request) => api.PutAsync<GalleryResponse>("api/v1/gallery", request);
    public Task<ApiResult<List<UserResponse>>> GetTeamAsync() => api.GetAsync<List<UserResponse>>("api/v1/gallery/team");
    public Task<ApiResult<UserResponse>> AddTeamMemberAsync(TeamMemberRequest request) => api.PostAsync<UserResponse>("api/v1/gallery/team", request);
    public Task<ApiResult<UserResponse>> SetMemberActiveAsync(long id, bool active) => api.PatchAsync<UserResponse>($"api/v1/gallery/team/{id}/active", new ActiveRequest(active));

    // ----- Dashboard & notifications -----
    public Task<ApiResult<DashboardResponse>> GetDashboardAsync() => api.GetAsync<DashboardResponse>("api/v1/dashboard");
    public Task<ApiResult<NotificationListResponse>> GetNotificationsAsync() => api.GetAsync<NotificationListResponse>("api/v1/notifications");
    public Task<ApiResult<bool>> MarkNotificationReadAsync(long id) => api.PostAsync<bool>($"api/v1/notifications/{id}/read");
    public Task<ApiResult<bool>> MarkAllNotificationsReadAsync() => api.PostAsync<bool>("api/v1/notifications/read-all");

    // ----- Vehicles -----
    public Task<ApiResult<PagedResult<VehicleListItem>>> GetVehiclesAsync(VehicleQuery q) =>
        api.GetAsync<PagedResult<VehicleListItem>>(Url("api/v1/vehicles", ("search", q.Search), ("status", q.Status), ("page", q.Page), ("pageSize", q.PageSize), ("sortBy", q.SortBy), ("descending", q.Descending)));
    public Task<ApiResult<List<VehicleOption>>> GetVehicleOptionsAsync(string? search, bool availableOnly, CancellationToken ct = default) =>
        api.GetAsync<List<VehicleOption>>(Url("api/v1/vehicles/options", ("search", search), ("availableOnly", availableOnly)), ct);
    public Task<ApiResult<List<string>>> GetBrandsAsync() => api.GetAsync<List<string>>("api/v1/vehicles/brands");
    public Task<ApiResult<VehicleDetail>> GetVehicleAsync(long id) => api.GetAsync<VehicleDetail>($"api/v1/vehicles/{id}");
    public Task<ApiResult<VehicleDetail>> CreateVehicleAsync(VehicleRequest request) => api.PostAsync<VehicleDetail>("api/v1/vehicles", request);
    public Task<ApiResult<VehicleDetail>> UpdateVehicleAsync(long id, VehicleRequest request) => api.PutAsync<VehicleDetail>($"api/v1/vehicles/{id}", request);
    public Task<ApiResult<bool>> DeleteVehicleAsync(long id) => api.DeleteAsync<bool>($"api/v1/vehicles/{id}");
    public Task<ApiResult<VehicleDetail>> SetVehicleStatusAsync(long id, VehicleStatus status) => api.PatchAsync<VehicleDetail>($"api/v1/vehicles/{id}/status", new VehicleStatusRequest(status));
    public Task<ApiResult<List<PhotoResponse>>> DeletePhotoAsync(long id, long photoId) => api.DeleteAsync<List<PhotoResponse>>($"api/v1/vehicles/{id}/photos/{photoId}");
    public Task<ApiResult<List<PhotoResponse>>> SetCoverPhotoAsync(long id, long photoId) => api.PutAsync<List<PhotoResponse>>($"api/v1/vehicles/{id}/photos/{photoId}/cover");

    public async Task<ApiResult<List<PhotoResponse>>> UploadPhotosAsync(long id, IReadOnlyList<IBrowserFile> files)
    {
        using var form = new MultipartFormDataContent();
        var streams = new List<Stream>();
        try
        {
            foreach (var file in files)
            {
                var stream = file.OpenReadStream(MaxPhotoBytes);
                streams.Add(stream);
                var part = new StreamContent(stream);
                part.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType);
                form.Add(part, "files", file.Name);
            }

            return await api.PostFormAsync<List<PhotoResponse>>($"api/v1/vehicles/{id}/photos", form);
        }
        finally
        {
            foreach (var stream in streams)
                await stream.DisposeAsync();
        }
    }

    // ----- Customers -----
    public Task<ApiResult<PagedResult<CustomerResponse>>> GetCustomersAsync(PagedQuery q, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<CustomerResponse>>(Url("api/v1/customers", ("search", q.Search), ("page", q.Page), ("pageSize", q.PageSize), ("sortBy", q.SortBy), ("descending", q.Descending)), ct);
    public Task<ApiResult<CustomerResponse>> CreateCustomerAsync(CustomerRequest request) => api.PostAsync<CustomerResponse>("api/v1/customers", request);
    public Task<ApiResult<CustomerResponse>> UpdateCustomerAsync(long id, CustomerRequest request) => api.PutAsync<CustomerResponse>($"api/v1/customers/{id}", request);
    public Task<ApiResult<bool>> DeleteCustomerAsync(long id) => api.DeleteAsync<bool>($"api/v1/customers/{id}");

    // ----- Sales -----
    public Task<ApiResult<SaleListResponse>> GetSalesAsync(SaleQuery q) =>
        api.GetAsync<SaleListResponse>(Url("api/v1/sales", ("search", q.Search), ("from", q.From), ("to", q.To), ("customerId", q.CustomerId), ("page", q.Page), ("pageSize", q.PageSize), ("sortBy", q.SortBy), ("descending", q.Descending)));
    public Task<ApiResult<SaleResponse>> CreateSaleAsync(SaleRequest request) => api.PostAsync<SaleResponse>("api/v1/sales", request);
    public Task<ApiResult<bool>> CancelSaleAsync(long id) => api.DeleteAsync<bool>($"api/v1/sales/{id}");

    // ----- Expenses -----
    public Task<ApiResult<ExpenseListResponse>> GetExpensesAsync(ExpenseQuery q) =>
        api.GetAsync<ExpenseListResponse>(Url("api/v1/expenses", ("search", q.Search), ("vehicleId", q.VehicleId), ("category", q.Category), ("from", q.From), ("to", q.To), ("page", q.Page), ("pageSize", q.PageSize), ("sortBy", q.SortBy), ("descending", q.Descending)));
    public Task<ApiResult<ExpenseResponse>> CreateExpenseAsync(ExpenseRequest request) => api.PostAsync<ExpenseResponse>("api/v1/expenses", request);
    public Task<ApiResult<ExpenseResponse>> UpdateExpenseAsync(long id, ExpenseRequest request) => api.PutAsync<ExpenseResponse>($"api/v1/expenses/{id}", request);
    public Task<ApiResult<bool>> DeleteExpenseAsync(long id) => api.DeleteAsync<bool>($"api/v1/expenses/{id}");

    // ----- Reminders -----
    public Task<ApiResult<List<ReminderResponse>>> GetRemindersAsync(ReminderQuery q) =>
        api.GetAsync<List<ReminderResponse>>(Url("api/v1/reminders", ("scope", q.Scope), ("vehicleId", q.VehicleId)));
    public Task<ApiResult<ReminderResponse>> CreateReminderAsync(ReminderRequest request) => api.PostAsync<ReminderResponse>("api/v1/reminders", request);
    public Task<ApiResult<ReminderResponse>> UpdateReminderAsync(long id, ReminderRequest request) => api.PutAsync<ReminderResponse>($"api/v1/reminders/{id}", request);
    public Task<ApiResult<ReminderResponse>> CompleteReminderAsync(long id) => api.PostAsync<ReminderResponse>($"api/v1/reminders/{id}/complete");
    public Task<ApiResult<bool>> DeleteReminderAsync(long id) => api.DeleteAsync<bool>($"api/v1/reminders/{id}");

    private static string Url(string path, params (string Key, object? Value)[] query)
    {
        var values = new Dictionary<string, string?>();
        foreach (var (key, value) in query)
        {
            var text = value switch
            {
                null => null,
                string s when string.IsNullOrWhiteSpace(s) => null,
                DateTime d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                bool b => b ? "true" : "false",
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString(),
            };
            if (text is not null)
                values[key] = text;
        }

        return values.Count == 0 ? path : QueryHelpers.AddQueryString(path, values);
    }
}
