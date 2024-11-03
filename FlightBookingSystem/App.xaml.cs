
using FlightBookingSystem.Components.Repository;

namespace FlightBookingSystem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DBManager.INSTANCE.InitializeAsync();

            MainPage = new MainPage();
        }
    }
}
