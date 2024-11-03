using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Exceptions
{
    public class InvalidAirportInfo : ApplicationException
    {
        public InvalidAirportInfo(string message) : base (message) { }
    }
}
