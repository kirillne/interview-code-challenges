namespace OneBeyondApi.Model
{
    public enum ReserveBookResult
    {
        Success,
        BookNotFound,
        BorrowerNotFound,
        BorrowerAlreadyHasReservedBook,
        BookIsImmediatelyAvailable,
        BookIsLoanedToTheBorrower,
    }
}
