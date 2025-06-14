namespace Ordering.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) 
            : base($"Domain Eception: \"{message}\" throws from domain Layer.")
        { 
        }
    }
}
