using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Model
{
    internal class Reservation
    {
        private string _FlightCode;
        private string _Airline;
        private string _Day;
        private string _Time;
        private decimal _Cost;
        private string _Name;
        private string _Citizenship;

        public string FlightCode
        {
            get { return _FlightCode; }
            set { _FlightCode = value; }
        }

        public string Airline
        {
            get { return _Airline; }
            set { _Airline = value; }
        }

        public string Day
        {
            get { return _Day; }
            set { _Day = value; }
        }

        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        public decimal Cost
        {
            get { return _Cost; }
            set { _Cost = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Citizenship
        {
            get { return _Citizenship; }
            set { _Citizenship = value; }
        }

        public Reservation(string flightCode, string airline, string day, string time, decimal cost, string name, string citizenship)
        {
            FlightCode = flightCode;
            Airline = airline;
            Day = day;
            Time = time;
            Cost = cost;
            Name = name;
            Citizenship = citizenship;
        }
    }
}
