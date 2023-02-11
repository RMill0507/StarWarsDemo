
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;


namespace StarWarsDemo
{
    public class Program
    {
        //An instance of the `HttpClient` class is created to make HTTP requests.
        static HttpClient client = new HttpClient();

        //A string that holds the URL of the Star Wars API that will be used.
        static string apiUrl = "https://swapi.dev/api/planets/";

        // A `StringBuilder` instance that will be used to store the CSV content.
        static StringBuilder csvContent = new StringBuilder();

        static void Main(string[] args)
        {
            //The first line of the CSV content is set to the header row.
            csvContent.AppendLine("Name,Diameter,Gravity,Climate,Population");

            //The `RetrievePlanets` method is called and the program waits for it to complete.
            RetrievePlanets().Wait();

            //The CSV content is written to a file named `planets.csv`.
            File.WriteAllText("planets.csv", csvContent.ToString());
        }
        //This method retrieves planet data from the Star Wars API.
        static async Task RetrievePlanets()
        {
            //The loop continues while the `apiUrl` string is not empty or null.
            while (!string.IsNullOrEmpty(apiUrl))
            {
                //The API response is fetched and stored as a string.
                string apiResponse = await client.GetStringAsync(apiUrl);
                
                //The JSON response is deserialized into a `StarWarsPlanetsResponse` object.  
                StarWarsPlanetsResponse planetsResponse = JsonConvert.DeserializeObject<StarWarsPlanetsResponse>(apiResponse);

                //For each planet in the `results` array, the relevant data is appended to the CSV content.
                foreach (Planet planet in planetsResponse.results)
                {
                    // The planet data is only added to the CSV content if the diameter, gravity, climate, and population are known
                    if (!string.IsNullOrEmpty(planet.diameter) &&
                        !string.IsNullOrEmpty(planet.gravity) &&
                        !string.IsNullOrEmpty(planet.climate) &&
                        !string.IsNullOrEmpty(planet.population) &&
                        planet.population != "unknown" &&
                        planet.climate != "unknown" &&
                        planet.gravity != "unknown" &&
                        planet.diameter != "unknown")
                    {
                        csvContent.AppendLine($"{planet.name},{planet.diameter},{planet.gravity},{planet.climate},{planet.population}");
                    }
                }
                //The URL of the next page of results is stored in the `apiUrl` string.
                apiUrl = planetsResponse.next;
            }
        }
    }

    class StarWarsPlanetsResponse// This class represents the structure of the Star Wars API response.
    {
        public string count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public Planet[] results { get; set; }
    }

    class Planet// this class to represent a single planet in the API response
    {
        public string name { get; set; }
        public string diameter { get; set; }
        public string gravity { get; set; }
        public string climate { get; set; }
        public string population { get; set; }
    }
}

