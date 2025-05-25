using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IReserveRepository
    {
        public IList<Reserve> GetReserves();
        public (ReserveBookResult, Reserve?) ReserveBook(Guid borrowerId, Guid bookId);
        public CheckAvailabilityResult CheckBookAvailability(Guid borrowerId, Guid bookId);

    }
}
