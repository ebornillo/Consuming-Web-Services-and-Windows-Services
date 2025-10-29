using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeatherAppExamNumber1
{
    [XmlRoot("current")]
    public class WeatherXmlModel
    {
        [XmlElement("city")]
        public City? city { get; set; }

        [XmlElement("temperature")]
        public Temperature? temperature { get; set; }

        [XmlElement("humidity")]
        public Humidity? humidity { get; set; }
    }

    public class City
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }

    public class Temperature
    {
        [XmlAttribute("value")]
        public double Value { get; set; }
    }

    public class Humidity
    {
        [XmlAttribute("value")]
        public int Value { get; set; }
    }
}
