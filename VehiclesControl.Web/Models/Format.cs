using System.Globalization;

namespace VehiclesControl.Web.Models;

public static class Format
{
    public static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    /// <summary>Audit timestamps are stored in UTC; SQL Server returns them without a Kind.</summary>
    public static DateTime ToLocal(DateTime value) =>
        (value.Kind == DateTimeKind.Unspecified ? System.DateTime.SpecifyKind(value, DateTimeKind.Utc) : value).ToLocalTime();

    /// <summary>Calendar dates (purchase, sale, due dates) — shown as stored, no time-zone shift.</summary>
    public static string Day(DateTime value) => value.ToString("d MMM yyyy", Turkish);

    public static string? Day(DateTime? value) => value is { } v ? Day(v) : null;

    public static string DateTime(DateTime value) => ToLocal(value).ToString("d MMM yyyy, HH:mm", Turkish);

    public static string? DateTime(DateTime? value) => value is { } v ? DateTime(v) : null;

    public static string Money(decimal value) => value.ToString("C0", Turkish);

    public static string? Money(decimal? value) => value is { } v ? Money(v) : null;

    /// <summary>Short money for chart axes: ₺5,2 Mn / ₺640 B.</summary>
    public static string Compact(decimal value) => Math.Abs(value) switch
    {
        >= 1_000_000 => $"₺{(value / 1_000_000m).ToString("0.#", Turkish)} Mn",
        >= 1_000 => $"₺{(value / 1_000m).ToString("0", Turkish)} B",
        _ => Money(value),
    };

    public static string Number(decimal value) => value.ToString("N0", Turkish);

    public static string Km(int value) => $"{value.ToString("N0", Turkish)} km";

    public static string MonthName(int year, int month) => new DateTime(year, month, 1).ToString("MMM", Turkish);

    public static int DaysSince(DateTime day) => (int)(System.DateTime.Today - day.Date).TotalDays;

    public static int DaysUntil(DateTime day) => (int)(day.Date - System.DateTime.Today).TotalDays;

    /// <summary>The API serves photos at /uploads/…; this app proxies them at /media/….</summary>
    public static string? Media(string? apiUrl) =>
        apiUrl is null ? null : apiUrl.StartsWith("/uploads/", StringComparison.Ordinal) ? "/media/" + apiUrl["/uploads/".Length..] : apiUrl;

    public static string Initials(string? name)
    {
        var parts = (name ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpper(Turkish),
            _ => string.Concat(parts[0][..1], parts[^1][..1]).ToUpper(Turkish),
        };
    }

    public static string DueText(DateTime due)
    {
        var days = DaysUntil(due);
        return days switch
        {
            < 0 => $"{-days} gün gecikti",
            0 => "Bugün",
            1 => "Yarın",
            _ => $"{days} gün kaldı",
        };
    }
}
