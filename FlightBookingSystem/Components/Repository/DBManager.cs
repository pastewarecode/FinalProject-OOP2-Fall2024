using FlightBookingSystem.Components.Exceptions;
using FlightBookingSystem.Components.Model;
using FlightBookingSystem.Components.ViewModel;
using FlightBookingSystem.Components.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Repository
{
    internal class DBManager
    {
        private static string AiportFile = "airports.csv";
        private static string FlightFile = "flights.csv";
        private static string ReservationFile = "reservations.csv";
        private FlightManager flightManager;


        // using Singleton design pattern for flight manager class
        public static DBManager INSTANCE { get; private set; } = new DBManager();

        private DBManager() 
        {
            flightManager = FlightManager.INSTANCE;
        }

        public async Task InitializeAsync()
        {

            loadAirports();
            loadFlights();

        }

        public void RefreshFlights() 
        {
            loadFlights();
        }

        public void RefreshAirports()
        {
            loadAirports();
        }

        private async Task loadAirports()
        {
            
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(AiportFile);
                using var reader = new StreamReader(stream);

                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] parts = line.Split(",");
                    flightManager.AddAirport(parts[0], parts[1]);
                    line = reader.ReadLine();
                }
            }
            catch (FileNotFoundException)
            {
                throw new Exception("File not found");
            }
            catch (NullReferenceException)
            {
                throw new Exception("Reference NULL");
            }
            
        }


        private async Task loadFlights()
        {
            try 
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(FlightFile);
                using var reader = new StreamReader(stream);

                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] parts = line.Split(',');
                    flightManager.AddFlight(parts[0],
                        parts[1],
                        GetAirport(parts[2]),
                        GetAirport(parts[3]),
                        parts[4],
                        parts[5],
                        int.Parse(parts[6]),
                        decimal.Parse(parts[7]));

                    line = reader.ReadLine();

                }
            }
            catch (FileNotFoundException)
            {
                throw new Exception("File not found");
            }
            catch (NullReferenceException)
            {
                throw new Exception("Reference NULL");
            }
        }


        private Airport GetAirport(string code)
        {
            Airport airport = flightManager.GetAirportByCode(code);
            if(airport == null )
            {
                throw new InvalidAirportInfo("Wrong Airport code provided");
            }
            return airport;
        }


        //private void writetest()
        //{

        //    using (FileStream fs = new FileStream(ReservationFile, FileMode.Open, FileAccess.Read)) ;

        //    foreach(string l in flightManager.GetAiportCodeList())
        //    {

        //    }

        //}


    }

    
}
