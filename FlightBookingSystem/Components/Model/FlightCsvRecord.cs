using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Model
{
    public class FlightCsvRecord //class to input the correct data format into the database from the flights.csv file
    {
        public string FlightId { get; set; }
        public string Airline { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public int TotalSeats { get; set; }
        public decimal Cost { get; set; }
    }

}
