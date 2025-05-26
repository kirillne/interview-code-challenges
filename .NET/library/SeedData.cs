using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;

namespace OneBeyondApi
{
    public class SeedData
    {
        public static void SetInitialData()
        {
            var ernestMonkjack = new Author
            {
                Name = "Ernest Monkjack"
            };
            var sarahKennedy = new Author
            {
                Name = "Sarah Kennedy"
            };
            var margaretJones = new Author
            {
                Name = "Margaret Jones"
            };

            var clayBook = new Book
            {
                Name = "The Importance of Clay",
                Format = BookFormat.Paperback,
                Author = ernestMonkjack,
                ISBN = "1305718181"
            };

            var agileBook = new Book
            {
                Name = "Agile Project Management - A Primer",
                Format = BookFormat.Hardback,
                Author = sarahKennedy,
                ISBN = "1293910102"
            };

            var rustBook = new Book
            {
                Name = "Rust Development Cookbook",
                Format = BookFormat.Paperback,
                Author = margaretJones,
                ISBN = "3134324111"
            };

            var popularBook = new Book
            {
                Name = "Popular Book",
                Format = BookFormat.Paperback,
                Author = ernestMonkjack,
                ISBN = "1234567890"
            };

            var daveSmith = new Borrower
            {
                Name = "Dave Smith",
                EmailAddress = "dave@smithy.com"
            };

            var lianaJames = new Borrower
            {
                Name = "Liana James",
                EmailAddress = "liana@gmail.com"
            };

            var firstBorrower = new Borrower
            {
                Name = "First Borrower",
                EmailAddress = "first@gmail.com"
            };

            var secondBorrower = new Borrower
            {
                Name = "Second Borrower",
                EmailAddress = "second@gmail.com"
            };

            var thirdBorrower = new Borrower
            {
                Name = "Third Borrower",
                EmailAddress = "third@gmail.com"
            };

            var bookOnLoanUntilToday = new BookStock {
                Book = clayBook,
                OnLoanTo = daveSmith,
                LoanEndDate = DateTime.Now.Date
            };

            var bookNotOnLoan = new BookStock
            {
                Book = clayBook,
                OnLoanTo = null,
                LoanEndDate = null
            };

            var bookOnLoanUntilNextWeek = new BookStock
            {
                Book = agileBook,
                OnLoanTo = lianaJames,
                LoanEndDate = DateTime.Now.Date.AddDays(7)
            };

            var rustBookStock = new BookStock
            {
                Book = rustBook,
                OnLoanTo = null,
                LoanEndDate = null
            };

            var popularBookOnShortLoan = new BookStock
            {
                Book = popularBook,
                OnLoanTo = daveSmith,
                LoanEndDate = DateTime.Now.Date.AddDays(7)
            };

            var popularBookOnLongLoan = new BookStock
            {
                Book = popularBook,
                OnLoanTo = lianaJames,
                LoanEndDate = DateTime.Now.Date.AddDays(40)
            };

            var popularBookFirstReserve = new Reserve
            {
                Book = popularBook,
                Borrower = firstBorrower,
                ReserveDateTime = DateTime.Now
            };

            var popularBookSecondReserve = new Reserve
            {
                Book = popularBook,
                Borrower = secondBorrower,
                ReserveDateTime = DateTime.Now.AddSeconds(1)
            };

            using (var context = new LibraryContext())
            {
                context.Authors.Add(ernestMonkjack);
                context.Authors.Add(sarahKennedy);
                context.Authors.Add(margaretJones);


                context.Books.Add(clayBook);
                context.Books.Add(agileBook);
                context.Books.Add(rustBook);
                context.Books.Add(popularBook);

                context.Borrowers.Add(daveSmith);
                context.Borrowers.Add(lianaJames);
                context.Borrowers.Add(firstBorrower);
                context.Borrowers.Add(secondBorrower);
                context.Borrowers.Add(thirdBorrower);

                context.Catalogue.Add(bookOnLoanUntilToday);
                context.Catalogue.Add(bookNotOnLoan);
                context.Catalogue.Add(bookOnLoanUntilNextWeek);
                context.Catalogue.Add(rustBookStock);
                context.Catalogue.Add(popularBookOnShortLoan);
                context.Catalogue.Add(popularBookOnLongLoan);

                context.Reserves.Add(popularBookFirstReserve);
                context.Reserves.Add(popularBookSecondReserve);

                context.SaveChanges();

            }
        }
    }
}
