using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisTestAPICore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        readonly IConnectionMultiplexer _redis;
        readonly IDatabase _db;

        public ApiController(IConnectionMultiplexer redis)
        {
            _redis = redis;

            _db = redis.GetDatabase();
        }

        //[HttpGet]
        //[Route("add-data")]
        //public IActionResult Get()
        //{
            
        //    var array =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        ID = index,

        //        DateCreated = DateTime.Now,

        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();

        //    foreach(var item in array)
        //    {
        //        _db.StringSet(item.ID.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(item), _ttl);
        //    }

            
        //    return Ok("Added 5 rows");
        //}

        [HttpGet]
        [Route("probabilistic-read")]
        public IActionResult Get(int key, double beta = 1, int ttl = 1)
        {
            var now = DateTime.Now;
            var timeToLive = _db.KeyTimeToLive(key.ToString());
            var timespan = TimeSpan.FromMinutes(ttl);


            if (timeToLive == null || (now - (timeToLive * beta * Math.Log(GetRandomNumber(0, 1))) >= now.Add(timeToLive.Value)))
            {

                var newvalue = new WeatherForecast
                {
                    ID = key,

                    DateCreated = DateTime.Now,

                    Date = DateTime.Now.AddDays(key),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };


                _db.StringSet(newvalue.ID.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(newvalue), timespan);
                return Ok($"[TTL: {timespan}] {Newtonsoft.Json.JsonConvert.SerializeObject(newvalue).ToString()}");
            }
            else
            {
                var result = _db.StringGet(key.ToString());
                return Ok($"[TTL: {timeToLive}] {result.ToString()}");
            }

        }


        private double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }



    }
}