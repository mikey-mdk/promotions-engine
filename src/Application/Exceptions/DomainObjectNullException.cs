namespace PromotionsEngine.Application.Exceptions
{
    public class DomainObjectNullException : Exception
    {
        public DomainObjectNullException()
        {
        }

        public DomainObjectNullException(string message)
            : base(message)
        {
        }

        public DomainObjectNullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
