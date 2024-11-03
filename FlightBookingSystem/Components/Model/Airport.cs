using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBookingSystem.Components.Model
{
    internal class Airport
    {

        private string _code;
        private string _name;

        public string Code { get { return _code; } }
        public string Name { get { return _name; } }

        public Airport(string code, string name)
        {
            _code = code;
            _name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Airport airport &&
                   Code == airport.Code &&
                   Name == airport.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, Name);
        }

        public static bool operator ==(Airport? left, Airport? right)
        {
            return EqualityComparer<Airport>.Default.Equals(left, right);
        }

        public static bool operator !=(Airport? left, Airport? right)
        {
            return !(left == right);
        }
    }
}
