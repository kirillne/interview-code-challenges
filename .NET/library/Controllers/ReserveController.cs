using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;

namespace OneBeyondApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReserveController : ControllerBase
    {
        private ILogger<ReserveController> _logger;
        private IReserveRepository _reserveRepository;

        public ReserveController(ILogger<ReserveController> logger, IReserveRepository reserveRepository)
        {
            _logger = logger;
            _reserveRepository = reserveRepository;
        }

        [HttpPost]
        [Route("ReserveBook")]
        public IActionResult ReserveBook(Guid borrowerId, Guid bookId)
        {
            var (result, reserve) = _reserveRepository.ReserveBook(borrowerId, bookId);
            return result switch
            {
                ReserveBookResult.Success => Ok(reserve),
                ReserveBookResult.BookNotFound => NotFound("Book not found"),
                ReserveBookResult.BorrowerNotFound => NotFound("Borrower not found"),
                ReserveBookResult.BorrowerAlreadyHasReservedBook => BadRequest("Borrower already has reserved book"),
                ReserveBookResult.BookIsImmediatelyAvailable => BadRequest("Book is immediately available, no need to reserve"),
                ReserveBookResult.BookIsLoanedToTheBorrower => BadRequest("Book is currently on loan to the borrower"),
                _ => StatusCode(500, "An unexpected error occurred")
            };
        }

        [HttpGet]
        [Route("GetReserves")]
        public IList<Reserve> GetReserves()
        {
            return _reserveRepository.GetReserves();
        }

        [HttpGet]
        [Route("CheckBookAvailability")]
        public IActionResult CheckBookAvailability(Guid borrowerId, Guid bookId)
        {
            var result = _reserveRepository.CheckBookAvailability(borrowerId, bookId);
            return result.Status switch
            {
                CheckAvailabilityResult.BookAvailabilityStatus.BookNotFound => NotFound("Book not found"),
                CheckAvailabilityResult.BookAvailabilityStatus.BorrowerNotFound => NotFound("Borrower not found"),
                CheckAvailabilityResult.BookAvailabilityStatus.BorrowerHasTheBook => BadRequest("Borrower already has the book on loan"),
                CheckAvailabilityResult.BookAvailabilityStatus.BookIsAvailableImmediately => Ok(result),
                CheckAvailabilityResult.BookAvailabilityStatus.BookIsAvailableAfterReservation => Ok(result),
                _ => StatusCode(500, "An unexpected error occurred")
            };
        }
    }
}
