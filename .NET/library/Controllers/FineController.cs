using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FineController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly IFineRepository _fineRepository;

        public FineController(ILogger<BookController> logger, IFineRepository fineRepository)
        {
            _logger = logger;
            _fineRepository = fineRepository;   
        }

        [HttpGet]
        [Route("GetFines")]
        public IList<Fine> Get()
        {
            return _fineRepository.GetFines();
        }
    }
}