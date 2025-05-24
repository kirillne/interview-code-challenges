using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IFineRepository
    {
        public IList<Fine> GetFines();
        public Fine AddFine(LibraryContext context, Borrower borrower, decimal amount);
    }
}
