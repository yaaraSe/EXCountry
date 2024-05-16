using log4net;
using log4net.Config;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.Models;
using serverPortal.Utils;
using System.Diagnostics.Metrics;

namespace serverPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : Controller
    {

        public readonly MongoRepository repository;
        private readonly ILog log = LogManager.GetLogger(typeof(CountryController));

        public CountryController(MongoRepository mongoRepository)
        {
            repository = mongoRepository;
            XmlConfigurator.Configure(new FileInfo("Logs/LoggerConfing.xml"));
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Country> countries = repository.countryService.GetAllCountries();
                log.Info(" Get all Countries");

                return Ok(countries);
            }
            catch (Exception ex)
            {
                log.Error("ex.Message: " + ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public ActionResult<Country> Get(string id)
        {
            try
            {
                Country country = repository.countryService.GetCountryById(id);
                log.Info(" Get Country " + country);
                return country;
            }
            catch (Exception ex)
            {
                log.Error("ex.Message: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Country> Create(Country country)
        {
            try
            {

                repository.countryService.CreateCountry(country);
                log.Info(" Create Country" + country.Id);
                return Ok($"Success Create country: {country.Id}");
            }
            catch (Exception ex)
            {
                log.Error("ex.Message: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Country> Update(string id, Country country)
        {
            try
            {
                repository.countryService.UpdateCountry(id, country);
                log.Info(" Update Country " + id);

                return country;
            }
            catch (Exception ex)
            {
                log.Error("ex.Message: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }


    }
}