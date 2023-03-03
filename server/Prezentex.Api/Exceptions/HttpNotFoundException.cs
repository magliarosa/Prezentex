namespace Prezentex.Api.Exceptions
{
    public class HttpNotFoundException : Exception
    {
        public override string? Message { get; }
        public HttpNotFoundException() { }

        public HttpNotFoundException(string message)
        {
            Message = message;
        }

    }
}
