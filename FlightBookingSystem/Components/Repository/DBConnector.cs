using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

using MySql.Data.MySqlClient;
using CsvHelper;
using FlightBookingSystem.Components.Model;
using CsvHelper.Configuration;

namespace FlightBookingSystem.Components.Repository
{
    public class DBConnector
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        // Constructor for default values
        public DBConnector()
        {
            Initialize();
        }

        // Initialize connection values
        private void Initialize()
        {
            server = "localhost";
            database = "flightbookingsystem";
            uid = "root";
            password = "";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);  // Use connection variable here
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
                // Handle connection errors based on the error code
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

        // Insert data into the database from CSV files
        public void InsertAirportsFromCsv(string filePath)
        {
            if (OpenConnection())
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false //no headers so we set to false. We create a new instance because its value is normally readonly
                };

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var airports = csv.GetRecords<Airport>();

                    foreach (var airport in airports)
                    {
                        var query = "SELECT COUNT(*) FROM airports WHERE code = @Code";
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Code", airport.Code);
                            var count = Convert.ToInt32(command.ExecuteScalar());

                            if (count == 0)
                            {
                                var insertQuery = "INSERT INTO airports (code, name) VALUES (@Code, @Name)";
                                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Code", airport.Code);
                                    insertCommand.Parameters.AddWithValue("@Name", airport.Name);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                CloseConnection();
            }
        }

        public void InsertFlightsFromCsv(string filePath)
        {
            if (OpenConnection())
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false // no headers so we set to false
                };

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var records = csv.GetRecords<FlightCsvRecord>().ToList();

                    foreach (var record in records)
                    {
                        var flightId = record.FlightId;
                        var airline = record.Airline;
                        var sourceCode = record.Source;
                        var destinationCode = record.Destination;
                        var day = record.Day;
                        var time = record.Time;
                        var totalSeats = Convert.ToInt32(record.TotalSeats);
                        var cost = Convert.ToDecimal(record.Cost);

                        // Retrieve Airport objects from the database using the airport codes
                        var sourceAirport = GetAirportByCode(sourceCode);
                        var destinationAirport = GetAirportByCode(destinationCode);

                        // Check if the flight already exists in the database
                        var query = "SELECT COUNT(*) FROM flights WHERE flight_id = @FlightId";
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FlightId", flightId);
                            var count = Convert.ToInt32(command.ExecuteScalar());

                            if (count == 0)
                            {
                                // Insert the flight into the database with the correct column names
                                var insertQuery = "INSERT INTO flights (flight_id, day, source_Code, destination_Code, airline, time, total_Seats, booked_Seats, cost) " +
                                                  "VALUES (@FlightId, @Day, @SourceCode, @DestinationCode, @Airline, @Time, @TotalSeats, @BookedSeats, @Cost)";
                                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@FlightId", flightId);
                                    insertCommand.Parameters.AddWithValue("@Day", day);
                                    insertCommand.Parameters.AddWithValue("@SourceCode", sourceAirport.Code);  // Using source airport code
                                    insertCommand.Parameters.AddWithValue("@DestinationCode", destinationAirport.Code);  // Using destination airport code
                                    insertCommand.Parameters.AddWithValue("@Airline", airline);
                                    insertCommand.Parameters.AddWithValue("@Time", time);
                                    insertCommand.Parameters.AddWithValue("@TotalSeats", totalSeats);
                                    insertCommand.Parameters.AddWithValue("@BookedSeats", 0);  // Default to 0 if not in CSV
                                    insertCommand.Parameters.AddWithValue("@Cost", cost);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                CloseConnection();
            }
        }


        // helper function to get an airport by its code
        private Airport GetAirportByCode(string airportCode)
        {
            var query = "SELECT * FROM airports WHERE code = @Code";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Code", airportCode);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Airport(reader["code"].ToString(), reader["name"].ToString());
                    }
                }
            }
            return null;//if no airport found
        }


        public void InsertReservationsFromCsv(string filePath)
        {
            if (OpenConnection())
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false //no headers so we set to false. We create a new instance because its value is normally readonly
                };

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var reservations = csv.GetRecords<Reservation>();

                    foreach (var reservation in reservations)
                    {
                        var query = @"
                        SELECT COUNT(*) FROM reservations 
                        WHERE FlightCode = @FlightCode AND name = @Name AND citizenship = @Citizenship";
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FlightCode", reservation.FlightCode);
                            command.Parameters.AddWithValue("@Name", reservation.Name);
                            command.Parameters.AddWithValue("@Citizenship", reservation.Citizenship);
                            var count = Convert.ToInt32(command.ExecuteScalar());

                            if (count == 0)
                            {
                                var insertQuery = @"
                                INSERT INTO reservations 
                                (FlightCode, airline, day, time, cost, name, citizenship) 
                                VALUES 
                                (@FlightCode, @Airline, @Day, @Time, @Cost, @Name, @Citizenship)";
                                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@FlightCode", reservation.FlightCode);
                                    insertCommand.Parameters.AddWithValue("@Airline", reservation.Airline);
                                    insertCommand.Parameters.AddWithValue("@Day", reservation.Day);
                                    insertCommand.Parameters.AddWithValue("@Time", reservation.Time);
                                    insertCommand.Parameters.AddWithValue("@Cost", reservation.Cost);
                                    insertCommand.Parameters.AddWithValue("@Name", reservation.Name);
                                    insertCommand.Parameters.AddWithValue("@Citizenship", reservation.Citizenship);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                CloseConnection();
            }
        }

        // Load data for UI
        internal List<Airport> LoadAirports()
        {
            string query = "SELECT * FROM Airports";
            List<Airport> airports = new List<Airport>();

            if (OpenConnection())
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
                CloseConnection();
            }

            return airports;
        }

        internal List<Flight> LoadFlights(List<Airport> airports)
        {
            string query = "SELECT * FROM Flights";
            List<Flight> flights = new List<Flight>();

            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
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
                CloseConnection();
            }

            return flights;
        }

        internal List<Reservation> LoadReservations()
        {
            string query = "SELECT * FROM Reservations";
            List<Reservation> reservations = new List<Reservation>();

            if (OpenConnection())
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
                CloseConnection();
            }

            return reservations;
        }
    }



//Call InitializeDatabase on program startup to ensure data is inputted when program is being used
    public class StartUpDB
    {
        private readonly DBConnector _dbConnector;

        public StartUpDB(DBConnector dbConnector)
        {
            _dbConnector = dbConnector;
        }

        public void InitializeDatabase() //for start up we want to initialize the database by calling the insert functions to fill up our database with data.
        {
            //insert necessary data into database, using file path as parameter
            _dbConnector.InsertAirportsFromCsv("C:\\FlightSystemFinal\\FlightBookingSystem\\Resources\\Raw\\airports.csv");
            _dbConnector.InsertFlightsFromCsv("C:\\FlightSystemFinal\\FlightBookingSystem\\Resources\\Raw\\flights.csv");
            _dbConnector.InsertReservationsFromCsv("C:\\FlightSystemFinal\\FlightBookingSystem\\Resources\\Raw\\reservations.csv");
        }
    }
}