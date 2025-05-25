using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class ReserveRepository : IReserveRepository
    {
        public IList<Reserve> GetReserves()
        {
            using (var context = new LibraryContext())
            {
                return context.Reserves
                    .Include(r => r.Borrower)
                    .Include(r => r.Book)
                    .ToList();
            }
        }

        public (ReserveBookResult, Reserve?) ReserveBook(Guid borrowerId, Guid bookId)
        {
            using (var context = new LibraryContext())
            {
                var borrower = context.Borrowers.Find(borrowerId);
                if (borrower == null)
                {
                    return (ReserveBookResult.BorrowerNotFound, null);
                }
                var book = context.Books.Find(bookId);
                if (book == null)
                {
                    return (ReserveBookResult.BookNotFound, null);
                }
                if (context.Reserves.Any(r => r.Borrower.Id == borrowerId && r.Book.Id == bookId))
                {
                    return (ReserveBookResult.BorrowerAlreadyHasReservedBook, null);
                }

                var bookStocks = context.Catalogue.Where(bs => bs.Book.Id == bookId);
                   
                if (bookStocks.Any(x => x.OnLoanTo == null))
                {
                    return (ReserveBookResult.BookIsImmediatelyAvailable, null);
                }
                
                if(bookStocks.Any(x => x.OnLoanTo != null && x.OnLoanTo.Id == borrowerId))
                {
                    return (ReserveBookResult.BookIsLoanedToTheBorrower, null);
                }

                var reserve = new Reserve
                {
                    Id = Guid.NewGuid(),
                    Borrower = borrower,
                    Book = book,
                    ReserveDateTime = DateTime.Now
                };

                context.Reserves.Add(reserve);
                context.SaveChanges();
                return (ReserveBookResult.Success, reserve);
            }
        }
    }
}
