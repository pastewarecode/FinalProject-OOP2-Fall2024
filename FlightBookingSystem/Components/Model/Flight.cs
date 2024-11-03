using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Model
{
    internal class Flight : IEquatable<Flight?>
    {
        private string _flight_id;
        private string _day;
        private Airport _source;
        private Airport _destination;
        private string _airline;
        private string _time;
        private int _totalseats;
        private int _bookedseats;
        private decimal _cost;


        public string FlightId { get { return _flight_id; } }
        public string Day { get { return _day; } }
        public Airport Source { get { return _source; } }
        public Airport Destination { get { return _destination; } }
        public string Airline { get { return _airline; } }
        public string Time { get { return _time; } }
        public int TotalSeats { get { return _totalseats; } }
        public int BookedSeats { get { return _bookedseats; } }
        public decimal Cost { get { return _cost; } }
        public int AvailableSeats { get { return TotalSeats - BookedSeats; } }

        public Flight(string flight_id, string day, Airport source, Airport destination, string airline, string time, int totalseats, decimal cost)
        {
            _flight_id = flight_id;
            _day = day;
            _source = source;
            _destination = destination;
            _airline = airline;
            _time = time;
            _totalseats = totalseats;
            _bookedseats = 0;
            _cost = cost;
        }


        /// <summary>
        /// Returns string representation of the flight object. Update the implementation as per requirement
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FlightId} : {Airline}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Flight);
        }

        public bool Equals(Flight? other)
        {
            return other is not null &&
                   FlightId == other.FlightId &&
                   Day == other.Day &&
                   Time == other.Time;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FlightId, Day, Time);
        }

        public static bool operator ==(Flight? left, Flight? right)
        {
            return EqualityComparer<Flight>.Default.Equals(left, right);
        }

        public static bool operator !=(Flight? left, Flight? right)
        {
            return !(left == right);
        }
    }
}
