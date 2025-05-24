namespace OneBeyondApi.Model
{
    public class BorrowerLoans
    {
        public Borrower Borrower { get; set; }

        public IEnumerable<string> BookNames { get; set; }
    }
}
