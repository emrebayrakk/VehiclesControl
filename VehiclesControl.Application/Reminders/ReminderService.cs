using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Reminders;

public interface IReminderService
{
    Task<ApiResponse<List<ReminderResponse>>> GetAsync(ReminderQuery query, CancellationToken ct = default);
    Task<ApiResponse<ReminderResponse>> CreateAsync(ReminderRequest request, CancellationToken ct = default);
    Task<ApiResponse<ReminderResponse>> UpdateAsync(long id, ReminderRequest request, CancellationToken ct = default);
    Task<ApiResponse<ReminderResponse>> CompleteAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default);
}

public sealed class ReminderService(IAppDbContext db, TimeProvider clock) : IReminderService
{
    private DateTime Today => clock.GetLocalNow().Date;

    public async Task<ApiResponse<List<ReminderResponse>>> GetAsync(ReminderQuery query, CancellationToken ct = default)
    {
        var reminders = db.Reminders.AsNoTracking();
        if (query.VehicleId is { } vehicleId)
            reminders = reminders.Where(r => r.VehicleId == vehicleId);

        var today = Today;
        reminders = query.Scope switch
        {
            ReminderScope.Upcoming => reminders.Where(r => !r.IsCompleted && r.DueDate >= today).OrderBy(r => r.DueDate),
            ReminderScope.Overdue => reminders.Where(r => !r.IsCompleted && r.DueDate < today).OrderBy(r => r.DueDate),
            ReminderScope.Completed => reminders.Where(r => r.IsCompleted).OrderByDescending(r => r.CompletedDate),
            _ => reminders.OrderBy(r => r.IsCompleted).ThenBy(r => r.DueDate),
        };

        return ApiResponse<List<ReminderResponse>>.Success(await reminders.Take(500).Select(Projections.Reminder).ToListAsync(ct));
    }

    public async Task<ApiResponse<ReminderResponse>> CreateAsync(ReminderRequest request, CancellationToken ct = default)
    {
        if (!await db.Vehicles.AnyAsync(v => v.Id == request.VehicleId, ct))
            return ApiResponse<ReminderResponse>.NotFound("Araç bulunamadı.");

        var reminder = new Reminder();
        Apply(request, reminder);
        db.Reminders.Add(reminder);
        await db.SaveChangesAsync(ct);
        return ApiResponse<ReminderResponse>.Created(await GetByIdAsync(reminder.Id, ct));
    }

    public async Task<ApiResponse<ReminderResponse>> UpdateAsync(long id, ReminderRequest request, CancellationToken ct = default)
    {
        var reminder = await db.Reminders.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (reminder is null)
            return ApiResponse<ReminderResponse>.NotFound("Hatırlatıcı bulunamadı.");

        if (!await db.Vehicles.AnyAsync(v => v.Id == request.VehicleId, ct))
            return ApiResponse<ReminderResponse>.NotFound("Araç bulunamadı.");

        var dueChanged = reminder.DueDate != (request.DueDate ?? reminder.DueDate).Date;
        Apply(request, reminder);
        if (dueChanged)
            reminder.NotifiedAt = null;

        await db.SaveChangesAsync(ct);
        return ApiResponse<ReminderResponse>.Success(await GetByIdAsync(id, ct));
    }

    public async Task<ApiResponse<ReminderResponse>> CompleteAsync(long id, CancellationToken ct = default)
    {
        var reminder = await db.Reminders.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (reminder is null)
            return ApiResponse<ReminderResponse>.NotFound("Hatırlatıcı bulunamadı.");

        reminder.IsCompleted = true;
        reminder.CompletedDate = Today;
        await db.SaveChangesAsync(ct);
        return ApiResponse<ReminderResponse>.Success(await GetByIdAsync(id, ct));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var reminder = await db.Reminders.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (reminder is null)
            return ApiResponse<bool>.NotFound("Hatırlatıcı bulunamadı.");

        db.Reminders.Remove(reminder);
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }

    private Task<ReminderResponse> GetByIdAsync(long id, CancellationToken ct) =>
        db.Reminders.Where(r => r.Id == id).Select(Projections.Reminder).FirstAsync(ct);

    private static void Apply(ReminderRequest r, Reminder reminder)
    {
        reminder.VehicleId = r.VehicleId;
        reminder.Type = r.Type;
        reminder.DueDate = (r.DueDate ?? DateTime.Today).Date;
        reminder.Notes = Text.Clean(r.Notes);
    }
}
