using MudBlazor;

namespace VehiclesControl.Web.Theme;

/// <summary>
/// "Showroom" theme: ink (dark slate) as the primary, racing red as the brand accent, a dark
/// sidebar in both modes. Palette follows the ui-ux-pro-max "Automotive / Car Dealership" entry
/// (premium dark + action red); layout density follows its "Data-Dense Dashboard" style.
/// </summary>
public static class AppTheme
{
    private static readonly string[] Body = ["Inter", "system-ui", "-apple-system", "Segoe UI", "sans-serif"];
    private static readonly string[] Display = ["Outfit", "Inter", "system-ui", "sans-serif"];

    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0F172A",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#E5383B",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#D97706",
            Info = "#2563EB",
            Success = "#15803D",
            Warning = "#B45309",
            Error = "#DC2626",
            Dark = "#0F172A",
            Background = "#F5F6F8",
            BackgroundGray = "#EEF0F3",
            Surface = "#FFFFFF",
            AppbarBackground = "rgba(245,246,248,0.78)",
            AppbarText = "#0F172A",
            DrawerBackground = "#0B0F19",
            DrawerText = "#A7B0C0",
            DrawerIcon = "#7D879A",
            TextPrimary = "#0F172A",
            TextSecondary = "#5B6476",
            TextDisabled = "#9AA3B2",
            ActionDefault = "#5B6476",
            LinesDefault = "#E4E7EC",
            LinesInputs = "#CDD3DC",
            TableLines = "#EDF0F3",
            TableHover = "#F7F8FA",
            Divider = "#E4E7EC",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#F1F5F9",
            PrimaryContrastText = "#0B0F19",
            Secondary = "#F2545B",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#FBBF24",
            Info = "#60A5FA",
            Success = "#4ADE80",
            Warning = "#FBBF24",
            Error = "#F87171",
            Dark = "#0B0F19",
            Background = "#090C13",
            BackgroundGray = "#131824",
            Surface = "#0F131D",
            AppbarBackground = "rgba(9,12,19,0.78)",
            AppbarText = "#E5E9F0",
            DrawerBackground = "#06080D",
            DrawerText = "#A7B0C0",
            DrawerIcon = "#7D879A",
            TextPrimary = "#E5E9F0",
            TextSecondary = "#97A1B3",
            TextDisabled = "#5D6678",
            ActionDefault = "#97A1B3",
            LinesDefault = "#1D2332",
            LinesInputs = "#2B3346",
            TableLines = "#181E2B",
            TableHover = "#141925",
            Divider = "#1D2332",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = Body, FontSize = ".875rem" },
            H3 = new H3Typography { FontFamily = Display, FontWeight = "700", LetterSpacing = "-.02em" },
            H4 = new H4Typography { FontFamily = Display, FontWeight = "650", LetterSpacing = "-.02em", FontSize = "1.75rem" },
            H5 = new H5Typography { FontFamily = Display, FontWeight = "600", LetterSpacing = "-.01em" },
            H6 = new H6Typography { FontFamily = Display, FontWeight = "600", FontSize = "1.05rem", LetterSpacing = "-.005em" },
            Subtitle1 = new Subtitle1Typography { FontFamily = Body, FontWeight = "600" },
            Subtitle2 = new Subtitle2Typography { FontFamily = Body, FontWeight = "600" },
            Button = new ButtonTypography { FontFamily = Body, FontWeight = "600", TextTransform = "none", LetterSpacing = "0" },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "256px",
            AppbarHeight = "64px",
        },
    };
}
