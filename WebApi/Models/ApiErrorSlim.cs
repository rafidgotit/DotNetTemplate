namespace Sugary.WepApi.Models
{
    public class ApiErrorSlim
    {
        internal ApiErrorSlim(string message, InternalErrorCode internalError = InternalErrorCode.None)
        {
            this.Message = message;
            this.InternalErrorCode = internalError;
        }
        public string Message { get; }

        public InternalErrorCode InternalErrorCode { get; }
    }
}
