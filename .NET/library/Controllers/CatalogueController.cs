using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly ICatalogueRepository _catalogueRepository;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueRepository catalogueRepository)
        {
            _logger = logger;
            _catalogueRepository = catalogueRepository;
        }

        [HttpGet]
        [Route("GetCatalogue")]
        public IList<BookStock> Get()
        {
            return _catalogueRepository.GetCatalogue();
        }

        [HttpPost]
        [Route("SearchCatalogue")]
        public IList<BookStock> Post(CatalogueSearch search)
        {
            return _catalogueRepository.SearchCatalogue(search);
        }

        [HttpGet]
        [Route("OnLoan")]
        public IList<BorrowerLoans> GetOnLoan()
        {
            return _catalogueRepository.GetOnLoan();
        }

        [HttpPost]
        [Route("ReturnBook/{bookStockId}")]
        public IActionResult ReturnBook(Guid bookStockId)
        {
            var result = _catalogueRepository.ReturnBook(bookStockId);

            return result switch
            {
                ReturnBookResult.Success => Ok(),
                ReturnBookResult.BookNotFound => NotFound("Book not found"),
                ReturnBookResult.BookNotOnLoan => BadRequest("Book is not currently on loan"),
                ReturnBookResult.FineIssued => Ok("Fine was issued"),
                _ => StatusCode(500, "An unexpected error occurred")
            };
        }
    }
}