namespace OneBeyondApi.Model
{
    public class BorrowerLoans
    {
        public Borrower Borrower { get; set; }

        public IEnumerable<LoanedBook> Books { get; set; }
    
        public class LoanedBook
        {
            public string BookName { get; set; }
            public Guid BookStockId { get; set; }
        }
    }
}
