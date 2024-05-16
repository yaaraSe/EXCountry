using log4net;
using log4net.Config;
using log4net.Core;
using MongoDB.Driver;
using Repository;
using Repository.Models;
using Repository.Services;
using System.Net.Http.Headers;
using System.Text.Json;


namespace serverPortal.Utils
{
    public class ImportDataCountry
    {

        private readonly MongoRepository _mongoRepository;
        //  private readonly ILog log = LogManager.GetLogger(typeof(ImportDataCountry));

        public ImportDataCountry(MongoRepository mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task FetchCountriesAsync()
        {
            ////----log----------------
            ////  BasicConfigurator.Configure();
            //XmlConfigurator.Configure(new FileInfo("Logs/LoggerConfing.xml"));
            //log.Info("This is my first log message");
            //log.Debug("This is a debug message");
            //log.Info("This is an information message");
            //log.Warn("This is a warning message");
            //log.Error("This is an error message");
            //log.Fatal("This is a fatal message");

            ////log.Warn("This is a warning message");
            ////log.Info("This is an error message"); log.Warn("This is a warning message");
            ////log.Error("This is an error message"); log.Warn("This is a warning message");
            ////log.Error("This is an error message"); log.Warn("This is a warning message");
            ////log.Error("This is an error message"); log.Warn("This is a warning message");
            ////log.Error("This is an error message");
            ////log.Error("This is an error message");
            //var name = "Yaara";
            //log.Info($"Hello from {name}");
            //log.InfoFormat("Hello from {0}", name);

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://restcountries.com/v3.1/all");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("https://restcountries.com/v3.1/all");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        PropertyNameCaseInsensitive = true
                    };
                    List<Country> countries = JsonSerializer.Deserialize<List<Country>>(json, options);
                    Console.WriteLine(countries);
                    if (_mongoRepository.countryService.GetAllCountries().Count == 0)
                    {
                        foreach (var country in countries)
                        {
                            _mongoRepository.countryService.CreateCountry(country);
                            Console.WriteLine($"Region: {country.Name.Common}");
                        }
                    }

                }
                else
                {
                    throw new HttpRequestException($"{(int)response.StatusCode} ({response.ReasonPhrase})");
                }
            }


        }
    }
}