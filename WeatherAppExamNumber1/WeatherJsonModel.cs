using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAppExamNumber1
{
    public class WeatherJsonModel
    {
        public Main? main { get; set; }
        public string? name { get; set; }

        public class Main
        {
            public double temp { get; set; }
            public int humidity { get; set; }
        }
    }
}
