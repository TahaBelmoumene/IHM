using System.Windows;
using QuestPDF.Infrastructure; // Ajouter ce using

namespace IHM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Configuration de la licence communautaire (Gratuite)
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}