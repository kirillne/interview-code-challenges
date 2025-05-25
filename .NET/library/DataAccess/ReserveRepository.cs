using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class ReserveRepository : IReserveRepository
    {
        private LibrarySettings _librarySettings;

        public ReserveRepository(IOptions<LibrarySettings> options)
        {
            _librarySettings = options.Value;
        }

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
                    Borrower = borrower,
                    Book = book,
                    ReserveDateTime = DateTime.Now
                };

                context.Reserves.Add(reserve);
                context.SaveChanges();
                return (ReserveBookResult.Success, reserve);
            }
        }

        public CheckAvailabilityResult CheckBookAvailability(Guid borrowerId, Guid bookId)
        {
            using (var context = new LibraryContext())
            {
                var borrower = context.Borrowers.Find(borrowerId);
                if (borrower == null)
                {
                    return new CheckAvailabilityResult(CheckAvailabilityResult.BookAvailabilityStatus.BorrowerNotFound);
                }

                var bookStocks = context.Catalogue.Include(x => x.OnLoanTo).Where(bs => bs.Book.Id == bookId).ToList();

                if (!bookStocks.Any())
                {
                    return new CheckAvailabilityResult(CheckAvailabilityResult.BookAvailabilityStatus.BookNotFound);
                }

                if (bookStocks.Any(x => x.OnLoanTo == null))
                {
                    return new CheckAvailabilityResult(CheckAvailabilityResult.BookAvailabilityStatus.BookIsAvailableImmediately);
                }

                if(bookStocks.Any(x => x.OnLoanTo != null && x.OnLoanTo.Id == borrowerId))
                {
                    return new CheckAvailabilityResult(CheckAvailabilityResult.BookAvailabilityStatus.BorrowerHasTheBook);
                }

                var reserves = context.Reserves.Include(x => x.Borrower).Where(x => x.Book.Id == bookId).OrderBy(x => x.ReserveDateTime).ToList()
                    .TakeWhile(x => x.Borrower.Id != borrowerId).ToList();
                
                foreach(var reserve in reserves)
                {
                    var currentFirstAvailableStock = bookStocks.MinBy(x => x.LoanEndDate);
                    currentFirstAvailableStock!.LoanEndDate += TimeSpan.FromDays(_librarySettings.LoanDurationInDays); 
                }

                var firstAvailableStock = bookStocks.MinBy(x => x.LoanEndDate);
                return new CheckAvailabilityResult(
                    CheckAvailabilityResult.BookAvailabilityStatus.BookIsAvailableAfterReservation,
                    firstAvailableStock?.LoanEndDate);

            }
        }

    }
}
