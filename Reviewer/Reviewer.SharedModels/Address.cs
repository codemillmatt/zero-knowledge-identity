using System;
using System.Collections.Generic;
namespace Reviewer.SharedModels
{
    public class Address
    {
        public Address()
        {
        }

        public string Id { get; set; }

        public string Line1 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }
}
