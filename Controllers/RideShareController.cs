using Microsoft.AspNetCore.Mvc;

namespace AdessoRideShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideShareController : ControllerBase
    {
        private static List<Travel> Travels = new List<Travel>();
        private static int PeopleCount = 0;

        private readonly ILogger<RideShareController> _logger;

        public RideShareController(ILogger<RideShareController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        /* Kayýt eklemek için oluþturulmuþtur */
        public ActionResult Post(Travel entity)
        {
            if (String.IsNullOrEmpty(entity.Start) || String.IsNullOrEmpty(entity.Destination))
            {
                return BadRequest("Nereden Nereye bilgisi girmelisiniz");
            }
            if (entity.NumberOfSeat == 0)
            {
                return BadRequest("Koltuk sayýsý 0 olamaz");
            }

            var tempEntity = new Travel
            {
                Start = entity.Start.ToUpper(),
                Date = entity.Date == null ? DateTime.Now : entity.Date,
                Destination = entity.Destination.ToUpper(),
                NumberOfSeat = entity.NumberOfSeat,
                Id = Travels.Count != 0 ? Travels.Max(x => x.Id) + 1 : 1,
                Active = null //yayýna alýnmamýþ
            };

            Travels.Add(tempEntity);

            return Ok(tempEntity);
        }

        [HttpPut("{Id}")]
        /* Yayýna alma iþlemi */
        public ActionResult Put(int Id)
        {
            var travel = Travels.FirstOrDefault(p => p.Id == Id);
            if (travel is null)
            {
                return NotFound();
            }

            travel.Active = true;
            return Ok("Yayýna alma iþlemi baþarýlý");

        }

        [HttpDelete("{Id}")]
        // Pasife alarak silme iþlemi gerçekleþtirildi
        public ActionResult Delete(int Id)
        {
            var travel = Travels.FirstOrDefault(p => p.Id == Id);
            if (travel is null) return NotFound();

            travel.Active = false;
            return Ok("Silme iþlemi baþarýlý");
        }

        [HttpGet("{start}/{destination}")]
        // Arama iþlemi 
        public ActionResult<List<Travel>> Get(string start, string destination)
        {
            if (start is not null && destination is not null)
            {
                return Travels.Where(p => p.Start.Contains(start.ToUpper()) && p.Destination.Contains(destination.ToUpper()) && p.Active == true).ToList();
            }
            else
            {
                return ValidationProblem("Nereden ve nereye bilgilerini girmelisiniz");
            }
        }

        [HttpPost("{Id}")]
        // Seyahate katýlma iþlemi
        public ActionResult Join(int Id)
        {
            var travel = Travels.FirstOrDefault(p => p.Id == Id);
            if (travel is null)
            {
                return NotFound();
            }
            if (travel.NumberOfSeat >= PeopleCount)
            {
                PeopleCount += 1;
            }
            else
            {
                return BadRequest("Ýþleminizi gerçekleþtiremiyoruz. Seyahatte yer kalmamýþtýr.");
            }
            return Ok("Katýlým Ýþleminiz Baþarýlý");
        }

        [HttpGet]
        // Tümünü getir
        public ActionResult<List<Travel>> GetAll()
        {
            return Travels.Where(p => p.Active == true).ToList();
        }


    }
}