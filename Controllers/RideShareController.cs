using Microsoft.AspNetCore.Mvc;

namespace AdessoRideShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideShareController : ControllerBase
    {
        private List<Travel> Travels = new List<Travel>();
       
        private readonly ILogger<RideShareController> _logger;

        public RideShareController(ILogger<RideShareController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post(Travel entity)
        {
            var tempEntity = new Travel
            {
                Start = entity.Start.ToUpper(),
                Date = entity.Date,
                Destination = entity.Destination.ToUpper(),
                NumberOfSeat = entity.NumberOfSeat,
                Id = Travels.Max(x => x.Id) + 1,
                Active = null //yayýna alýnmamýþ
            };

            Travels.Add(tempEntity);  

            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpPut("{Id}")]
        public ActionResult Put(int Id)  // yayýna alma iþlemini activitesi true olduðunda olacak þekilde kurguladým.
        {
            var travel = Travels.FirstOrDefault(p => p.Id == Id);
            if (travel is null)
            {
                return NotFound();
            }

            travel.Active = true;
            return NoContent();
        }

        [HttpDelete("{Id}")]
        public ActionResult Delete(int Id) // tamamen silmek yerine travellarýn pasife alýnmasý ile tasarladým.
        {
            var travel = Travels.FirstOrDefault(p => p.Id == Id);
            if (travel is null) return NotFound();

            travel.Active = false;
            return NoContent();
        }

        [HttpGet("{start}/{destination}")]
        public ActionResult<List<Travel>> Get(string start, string destinantion)
        {
            if (start is not null && destinantion is not null)
            {
                return Travels.Where(p => p.Start.Contains(start.ToUpper()) && p.Destination.Contains(destinantion.ToUpper())).ToList();
            }
            else
            {
                return ValidationProblem("Nereden ve nereye bilgilerini girmelisiniz");
            }
        }

        [HttpGet]
        public ActionResult<List<Travel>> GetAll()
        {
            return Travels.Where(p => p.Active == true).ToList();
        }

    }
}