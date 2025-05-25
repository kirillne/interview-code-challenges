namespace OneBeyondApi.Model
{
    public class Reserve
    {
        public Guid Id { get; set; }    
        public Borrower Borrower { get; set; }
        public Book Book { get; set; }
        public DateTime ReserveDateTime { get; set; }
    }
}
