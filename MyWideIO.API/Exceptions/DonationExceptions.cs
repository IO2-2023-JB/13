namespace MyWideIO.API.Exceptions
{
    public class DonationException : BadRequestException
    {
        public DonationException(string? message) : base(message)
        {

        }
    }
    public class NotEnoughMoneyException : DonationException
    {
        public NotEnoughMoneyException() : base("Account balance is too small")
        {

        }
    }

    public class InvalidAmountException : DonationException
    {
        public InvalidAmountException() : base("Wrong amount")
        {

        }
    }


}
