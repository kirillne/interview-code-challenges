using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface ICatalogueRepository
    {
        public List<BookStock> GetCatalogue();
        public List<BookStock> SearchCatalogue(CatalogueSearch search);
        public List<BorrowerLoans> GetOnLoan();
        public (ReturnBookResult, Fine?) ReturnBook(Guid bookStockId);
    }
}
