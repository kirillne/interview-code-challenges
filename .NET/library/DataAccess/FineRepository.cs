using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class FineRepository : IFineRepository
    {
        public Fine AddFine(LibraryContext context, Borrower borrower, decimal amount)
        {
            var fine = new Fine
            {
                Borrower = borrower,
                Amount = amount,
                IssueDateTime = DateTime.Now,
                Id = Guid.NewGuid()
            };
            context.Fines.Add(fine);
            return fine;
        }

        public IList<Fine> GetFines()
        {
            using (var context = new LibraryContext())
            {
                return context.Fines
                    .Include(f => f.Borrower)
                    .ToList();
            }
        }
    }
}
