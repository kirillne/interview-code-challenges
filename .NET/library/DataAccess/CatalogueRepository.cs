using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class CatalogueRepository : ICatalogueRepository
    {
        public CatalogueRepository()
        {
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

        public ReturnBookResult ReturnBook(Guid bookStockId)
        {
            using (var context = new LibraryContext())
            {
                var bookStock = context.Catalogue
                    .Include(x => x.OnLoanTo)
                    .FirstOrDefault(x => x.Id == bookStockId);

                if (bookStock == null)
                {
                    return ReturnBookResult.BookNotFound;
                }
                if(bookStock.OnLoanTo == null)
                {
                    return ReturnBookResult.BookNotOnLoan;
                }

                bookStock.OnLoanTo = null;
                bookStock.LoanEndDate = null;
                context.SaveChanges();
                return ReturnBookResult.Success;
            }
        }
    }
}
