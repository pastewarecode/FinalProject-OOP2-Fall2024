using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Exceptions
{
    internal class InvalidFlightInfo : ApplicationException
    {
        public InvalidFlightInfo(string message) : base(message) { }
    }
}
