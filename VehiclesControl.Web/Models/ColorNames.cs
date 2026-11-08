namespace VehiclesControl.Web.Models;

/// <summary>Turns free-text colour names (Turkish or English) into CSS colours for the swatches.</summary>
public static class ColorNames
{
    public static readonly string[] Presets = ["Beyaz", "Siyah", "Gri", "Gümüş", "Kırmızı", "Mavi", "Lacivert", "Yeşil", "Turuncu", "Bordo", "Kahverengi", "Bej"];

    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kırmızı"] = "#E53935", ["kirmizi"] = "#E53935", ["red"] = "#E53935",
        ["mavi"] = "#1E88E5", ["blue"] = "#1E88E5",
        ["siyah"] = "#111827", ["black"] = "#111827",
        ["beyaz"] = "#F8FAFC", ["white"] = "#F8FAFC",
        ["gri"] = "#9CA3AF", ["grey"] = "#9CA3AF", ["gray"] = "#9CA3AF", ["füme"] = "#4B5563", ["fume"] = "#4B5563",
        ["gümüş"] = "#C0C7D1", ["gumus"] = "#C0C7D1", ["silver"] = "#C0C7D1",
        ["yeşil"] = "#43A047", ["yesil"] = "#43A047", ["green"] = "#43A047",
        ["sarı"] = "#FBC02D", ["sari"] = "#FBC02D", ["yellow"] = "#FBC02D",
        ["turuncu"] = "#FB8C00", ["orange"] = "#FB8C00",
        ["mor"] = "#8E24AA", ["purple"] = "#8E24AA",
        ["pembe"] = "#EC407A", ["pink"] = "#EC407A",
        ["kahverengi"] = "#6D4C41", ["brown"] = "#6D4C41",
        ["lacivert"] = "#1A237E", ["navy"] = "#1A237E",
        ["bordo"] = "#7B1E2B", ["burgundy"] = "#7B1E2B",
        ["turkuaz"] = "#00ACC1", ["turquoise"] = "#00ACC1",
        ["bej"] = "#D7C4A3", ["beige"] = "#D7C4A3",
    };

    public static string ToCss(string? name)
    {
        var key = (name ?? string.Empty).Trim();
        if (Map.TryGetValue(key, out var css))
            return css;

        return key.Length is 4 or 7 && key[0] == '#' && key[1..].All(Uri.IsHexDigit) ? key : "var(--mud-palette-gray-light)";
    }
}
