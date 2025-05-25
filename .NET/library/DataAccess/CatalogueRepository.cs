using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class CatalogueRepository : ICatalogueRepository
    {
        private readonly LibrarySettings _librarySettings;
        private readonly IFineRepository _fineRepository;

        public CatalogueRepository(IOptions<LibrarySettings> options, IFineRepository fineRepository)
        {
            _librarySettings = options.Value;
            _fineRepository = fineRepository;
        }

        public List<BookStock> GetCatalogue()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .ToList();
                return list;
            }
        }

        public List<BookStock> SearchCatalogue(CatalogueSearch search)
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Author))
                    {
                        list = list.Where(x => x.Book.Author.Name.Contains(search.Author));
                    }
                    if (!string.IsNullOrEmpty(search.BookName))
                    {
                        list = list.Where(x => x.Book.Name.Contains(search.BookName));
                    }
                }

                return list.ToList();
            }
        }

        public List<BorrowerLoans> GetOnLoan()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .Include(x => x.OnLoanTo)
                    .Where(x => x.OnLoanTo != null)
                    .GroupBy(x => x.OnLoanTo)
                    .Select(x => new BorrowerLoans
                    {
                        Borrower = x.Key!,
                        Books = x.Select(b => new BorrowerLoans.LoanedBook
                        { 
                            BookName = b.Book.Name,
                            BookStockId = b.Id 
                        })
                    });

                return list.ToList();
            }
        }

        public (ReturnBookResult, Fine?) ReturnBook(Guid bookStockId)
        {
            using (var context = new LibraryContext())
            {
                var bookStock = context.Catalogue
                    .Include(x => x.OnLoanTo)
                    .FirstOrDefault(x => x.Id == bookStockId);

                if (bookStock == null)
                {
                    return (ReturnBookResult.BookNotFound, null);
                }
                if (bookStock.OnLoanTo == null)
                {
                    return (ReturnBookResult.BookNotOnLoan, null);
                }

                Fine? fine = null;

                if (bookStock.LoanEndDate < DateTime.Now)
                {
                    fine = _fineRepository.AddFine(context, bookStock.OnLoanTo, _librarySettings.LoanOverdueFineAmount);
                }

                bookStock.OnLoanTo = null;
                bookStock.LoanEndDate = null;
                context.SaveChanges();
                return fine == null ? (ReturnBookResult.Success, null) : (ReturnBookResult.FineIssued, fine);
            }
        }
    }
}
