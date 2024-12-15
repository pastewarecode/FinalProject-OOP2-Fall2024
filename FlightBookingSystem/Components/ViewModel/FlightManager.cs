using FlightBookingSystem.Components.Model;
using FlightBookingSystem.Components.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.ViewModel
{
    internal class FlightManager
    {
        // using Singleton design pattern for flight manager class
        public static FlightManager INSTANCE = new FlightManager();

        private List<Airport> AirportList = new List<Airport>();
        private List<Flight> FlightList = new List<Flight>();
        private List<Reservation> ReservationList = new List<Reservation>();

        private FlightManager()
        {

        }

        public void AddAirport(string code, string name)
        {
            Airport newAirport = new Airport(code, name);

            if (AirportList.Contains(newAirport)) return;

            AirportList.Add(newAirport);
        }

        public void AddFlight(string flight_id, string airline, Airport source, Airport destination, string day, string time, int totalseats, decimal cost)
        {
            Flight f = new Flight(flight_id, day, source, destination, airline, time, totalseats, cost);
            if (FlightList.Contains(f)) return;
            FlightList.Add(f);
        }

        public Airport GetAirportByCode(string code)
        {
            foreach (Airport airport in AirportList)
            {
                if (airport.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) return airport;
            }
            return null;
        }

        public static List<string> GetAiportCodeList()
        {
            List<string> airportName = new List<string>();
            foreach (Airport airport in INSTANCE.AirportList)
            {
                airportName.Add(airport.Name);
            }
            return airportName;
        }

        public List<Flight> FindFlights(string src, string dest, string day)
        {
            if (FlightList.Count == 0)
            {
                DBManager.INSTANCE.RefreshFlights();
            }

            List<Flight> list = new List<Flight>();
            foreach (var flight in FlightList)
            {
                if (flight.Source.Name.Equals(src) &&
                    flight.Destination.Name.Equals(dest) &&
                    (flight.Day.Equals(day) || day.Equals("Any")))
                {
                    list.Add(flight);
                }
            }
            return list;
        }

        //added find reservations method
        public List<Reservation> FindReservations(string flightCode, string airline, string name)
        {
            List<Reservation> filteredReservations = new List<Reservation>();

            foreach (var reservation in ReservationList)
            {
                bool matches = true;

                if (!string.IsNullOrEmpty(flightCode) && !reservation.FlightCode.Contains(flightCode))
                {
                    matches = false;
                }

                if (!string.IsNullOrEmpty(airline) && !reservation.Airline.Contains(airline))
                {
                    matches = false;
                }

                if (!string.IsNullOrEmpty(name) && !reservation.Name.Contains(name))
                {
                    matches = false;
                }

                if (matches)
                {
                    filteredReservations.Add(reservation);
                }
            }

            return filteredReservations;
        }
    }
}
