using MongoDB.Driver;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class CountryService
    {
        private readonly IMongoCollection<Country> _countries;
        public CountryService(IMongoCollection<Country> countries)
        {
            _countries = countries;
        }
        public List<Country> GetAllCountries()
        {
            List<Country> countries = _countries.Find(_ => true).ToList();
            return countries;
        }
        public Country GetCountryById(string id)
        {
            try
            {
                var country = _countries.Find(country => country.Id == id).SingleAsync();
                return country.Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public void CreateCountry(Country country)
        {
            _countries.InsertOne(country);
        }

        public void UpdateCountry(string id, Country country)
        {

            ReplaceOneResult result = _countries.ReplaceOne(country => country.Id == id, country);
        }
    }
}
