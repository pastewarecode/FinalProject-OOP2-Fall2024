using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using FlightBookingSystem.Components.Model;
using System.Data.Common;

namespace FlightBookingSystem.Components.Repository
{
    public class DBConnector
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //constructor
        public DBConnector()
        {
            Initialize();
        }

        //initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "FlightsBookingSystem";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        //add and load airport data
        internal void AddAirport(Airport airport)
        {
            string query = "INSERT INTO Airports (code, name) VALUES (@code, @name)";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@code", airport.Code);
                cmd.Parameters.AddWithValue("@name", airport.Name);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        internal List<Airport> LoadAirports()
        {
            string query = "SELECT * FROM Airports";
            List<Airport> airports = new List<Airport>();

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    airports.Add(new Airport(
                        dataReader["code"].ToString(),
                        dataReader["name"].ToString()
                    ));
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return airports;
        }

        //add and load flight data
        internal void AddFlight(Flight flight)
        {
            string query = "INSERT INTO Flights (flight_id, day, source, destination, airline, time, total_seats, cost) " +
                           "VALUES (@flightId, @day, @source, @destination, @airline, @time, @totalSeats, @cost)";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@flightId", flight.FlightId);
                cmd.Parameters.AddWithValue("@day", flight.Day);
                cmd.Parameters.AddWithValue("@source", flight.Source.Code); // Only the airport code is stored
                cmd.Parameters.AddWithValue("@destination", flight.Destination.Code); // Only the airport code is stored
                cmd.Parameters.AddWithValue("@airline", flight.Airline);
                cmd.Parameters.AddWithValue("@time", flight.Time);
                cmd.Parameters.AddWithValue("@totalSeats", flight.TotalSeats);
                cmd.Parameters.AddWithValue("@cost", flight.Cost);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        internal List<Flight> LoadFlights(List<Airport> airports)
        {
            string query = "SELECT * FROM Flights";
            List<Flight> flights = new List<Flight>();

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    // Find the corresponding Airport objects for source and destination
                    Airport source = airports.FirstOrDefault(a => a.Code == dataReader["source"].ToString());
                    Airport destination = airports.FirstOrDefault(a => a.Code == dataReader["destination"].ToString());

                    flights.Add(new Flight(
                        dataReader["flight_id"].ToString(),
                        dataReader["day"].ToString(),
                        source,
                        destination,
                        dataReader["airline"].ToString(),
                        dataReader["time"].ToString(),
                        int.Parse(dataReader["total_seats"].ToString()),
                        decimal.Parse(dataReader["cost"].ToString())
                    ));
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return flights;
        }


        //add and load reservation data
        internal void AddReservation(Reservation reservation)
        {
            string query = "INSERT INTO Reservations (flight_code, airline, day, time, cost, name, citizenship) " +
                           "VALUES (@flightCode, @airline, @day, @time, @cost, @name, @citizenship)";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@flightCode", reservation.FlightCode);
                cmd.Parameters.AddWithValue("@airline", reservation.Airline);
                cmd.Parameters.AddWithValue("@day", reservation.Day);
                cmd.Parameters.AddWithValue("@time", reservation.Time);
                cmd.Parameters.AddWithValue("@cost", reservation.Cost);
                cmd.Parameters.AddWithValue("@name", reservation.Name);
                cmd.Parameters.AddWithValue("@citizenship", reservation.Citizenship);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        internal List<Reservation> LoadReservations()
        {
            string query = "SELECT * FROM Reservations";
            List<Reservation> reservations = new List<Reservation>();

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    reservations.Add(new Reservation(
                        dataReader["flight_code"].ToString(),
                        dataReader["airline"].ToString(),
                        dataReader["day"].ToString(),
                        dataReader["time"].ToString(),
                        decimal.Parse(dataReader["cost"].ToString()),
                        dataReader["name"].ToString(),
                        dataReader["citizenship"].ToString()
                    ));
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return reservations;
        }
    }
}
