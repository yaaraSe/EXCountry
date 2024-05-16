using MongoDB.Driver;
using Repository.Models;
using Repository.Services;
using System.Diagnostics.Metrics;

namespace Repository
{
    public class MongoRepository
    {
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Country> countries;
        public CountryService countryService { get; set; }

        public MongoRepository(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            database = client.GetDatabase(databaseSettings.DatabaseName);
            countries = database.GetCollection<Country>(databaseSettings.CollectionsNames.Countries);
            countryService = new CountryService(countries);

        }
    }
}