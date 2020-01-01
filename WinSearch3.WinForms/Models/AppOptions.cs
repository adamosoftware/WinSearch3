using JsonSettings.Library;
using WinForms.Library;
using WinForms.Library.Models;

namespace WinSearch3.Models
{
    public class AppOptions : SettingsBase
    {
        public override string Filename 
            => PathUtil.EnvironmentPath(System.Environment.SpecialFolder.LocalApplicationData, "WinSearch", "settings.json");

        public FormPosition FormPosition { get; set; }

        public string Locations { get; set; }
        public string SearchFilename { get; set; }
        public string Extensions { get; set; }
        public string Contents { get; set; }
    }
}
