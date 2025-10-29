using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using WeatherAppExamNumber1;

namespace WeatherConsoleApp
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string? apiKey;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== OpenWeatherMap API Integration (Console) ===\n");

            // Load configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            apiKey = config["OpenWeatherMap:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key is missing. Add it using 'Manage User Secrets' in Visual Studio.");
                return;
            }

            Console.Write("Enter city name: ");
            string? city = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("City name cannot be empty.");
                return;
            }

            Console.WriteLine("\nChoose format (1 = JSON, 2 = XML): ");
            string? choice = Console.ReadLine();


            if (choice == "1")
                await GetWeatherJson(city);
            else if (choice == "2")
                await GetWeatherXml(city);
            else
                Console.WriteLine("Invalid choice.");
        }

        private static async Task GetWeatherJson(string city)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                string response = await client.GetStringAsync(url);

                var weather = JsonSerializer.Deserialize<WeatherJsonModel>(response);

                if (weather == null || weather.main == null)
                {
                    Console.WriteLine("Unable to parse weather data.");
                    return;
                }

                Console.WriteLine("\n--- JSON FORMAT RESULT ---");
                Console.WriteLine($"City: {weather.name}");
                Console.WriteLine($"Temperature: {weather.main.temp} °C");
                Console.WriteLine($"Humidity: {weather.main.humidity}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching JSON data: {ex.Message}");
            }
        }

        private static async Task GetWeatherXml(string city)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&appid={apiKey}&units=metric";
                string response = await client.GetStringAsync(url);

                var serializer = new XmlSerializer(typeof(WeatherXmlModel));
                using var reader = new StringReader(response);
                var weather = (WeatherXmlModel?)serializer.Deserialize(reader);

                if (weather?.city == null || weather.temperature == null || weather.humidity == null)
                {
                    Console.WriteLine("Unable to parse XML weather data.");
                    return;
                }

                Console.WriteLine("\n--- XML FORMAT RESULT ---");
                Console.WriteLine($"City: {weather.city.Name}");
                Console.WriteLine($"Temperature: {weather.temperature.Value} °C");
                Console.WriteLine($"Humidity: {weather.humidity.Value}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching XML data: {ex.Message}");
            }
        }
    }
}
