namespace VMTLauncher
{
    /// <summary>
    /// Centralized color palette for the dark blue minimalist theme.
    /// All UI components reference these colors for consistency.
    /// </summary>
    public static class ThemeColors
    {
        // ─── Background Layers ───────────────────────────────────────
        public static readonly Color BackgroundDark    = Color.FromArgb(18, 25, 38);     // #121926 - Deepest
        public static readonly Color BackgroundMain    = Color.FromArgb(22, 31, 46);     // #161F2E - Primary
        public static readonly Color BackgroundCard    = Color.FromArgb(28, 39, 58);     // #1C273A - Card/Panel
        public static readonly Color BackgroundInput   = Color.FromArgb(20, 28, 42);     // #141C2A - Input fields
        public static readonly Color BackgroundHover   = Color.FromArgb(35, 48, 70);     // #233046 - Hover state

        // ─── Accent & Interactive ────────────────────────────────────
        public static readonly Color AccentBlue        = Color.FromArgb(59, 130, 246);   // #3B82F6 - Primary accent
        public static readonly Color AccentBlueHover   = Color.FromArgb(37, 99, 235);    // #2563EB - Accent hover
        public static readonly Color AccentBlueSoft    = Color.FromArgb(59, 130, 246, 40); // Subtle glow
        public static readonly Color AccentGreen       = Color.FromArgb(34, 197, 94);    // #22C55E - Start/Success
        public static readonly Color AccentGreenHover  = Color.FromArgb(22, 163, 74);    // #16A34A
        public static readonly Color AccentOrange      = Color.FromArgb(249, 115, 22);   // #F97316 - Warning
        public static readonly Color AccentRed         = Color.FromArgb(239, 68, 68);    // #EF4444 - Error/Stop

        // ─── Text ────────────────────────────────────────────────────
        public static readonly Color TextPrimary       = Color.FromArgb(226, 232, 240);  // #E2E8F0
        public static readonly Color TextSecondary     = Color.FromArgb(148, 163, 184);  // #94A3B8
        public static readonly Color TextMuted         = Color.FromArgb(100, 116, 139);  // #64748B
        public static readonly Color TextHighlight     = Color.FromArgb(255, 255, 255);  // Pure white

        // ─── Borders & Dividers ──────────────────────────────────────
        public static readonly Color Border            = Color.FromArgb(40, 54, 78);     // #28364E
        public static readonly Color BorderLight       = Color.FromArgb(51, 65, 85);     // #334155
        public static readonly Color BorderAccent      = Color.FromArgb(59, 130, 246, 100); // Accent glow

        // ─── Progress Bar ────────────────────────────────────────────
        public static readonly Color ProgressTrack     = Color.FromArgb(30, 41, 59);     // #1E293B
        public static readonly Color ProgressFill      = Color.FromArgb(59, 130, 246);   // Matches accent

        // ─── Fonts ───────────────────────────────────────────────────
        public static readonly Font FontTitle          = new("Segoe UI Semibold", 18f, FontStyle.Bold);
        public static readonly Font FontSubtitle       = new("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontLabel          = new("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        public static readonly Font FontBody           = new("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FontSmall          = new("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font FontButton         = new("Segoe UI Semibold", 10f, FontStyle.Bold);
        public static readonly Font FontPatchNotes     = new("Cascadia Code", 9.5f, FontStyle.Regular);
        public static readonly Font FontVersion        = new("Segoe UI Semibold", 11f, FontStyle.Bold);
    }
}
