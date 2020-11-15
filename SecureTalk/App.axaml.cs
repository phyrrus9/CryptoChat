using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTalk
{
    public class App : Application
    {
        public static Models.User ActiveUser;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            SecureTalk.DataContext.DbPath = @"C:\Users\ethan\client.db3";
            using (DataContext db = new())
                db.Database.Migrate();

            Services.API.Configure();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
