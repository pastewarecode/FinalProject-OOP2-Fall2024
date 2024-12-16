using FlightBookingSystem.Components.Repository;


namespace FlightBookingSystem
{
    public partial class App : Application
    {
        public App(StartUpDB startUpDB) //declaring startUpDB
        {
            InitializeComponent();

            DBManager.INSTANCE.InitializeAsync();

            //initialize database by calling method
            startUpDB.InitializeDatabase();

            MainPage = new MainPage();
        }
    }
}
