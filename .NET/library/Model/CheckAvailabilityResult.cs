
namespace OneBeyondApi.Model
{
    public record CheckAvailabilityResult(CheckAvailabilityResult.BookAvailabilityStatus Status, DateTime? AvailableFrom = null)
    {
        public enum BookAvailabilityStatus
        {
            BookNotFound,
            BorrowerNotFound,
            BorrowerHasTheBook,
            BookIsAvailableImmediately,
            BookIsAvailableAfterReservation
        }
    }
}
