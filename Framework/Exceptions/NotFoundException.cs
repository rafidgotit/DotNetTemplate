using System.Net;

namespace Framework.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string title) : base(title, httpStatusCode: HttpStatusCode.NotFound)
        {
        }
    }

    public class ResourceObjectNotFoundException : NotFoundException
    {
        public ResourceObjectNotFoundException(string resourceName, object id) : base($"Object not found for resource: {resourceName}.")
        {
            this.Detail = $"Object with id: '{id}' not found for resource: {resourceName}.";
        }
    }

    public class BadQueryParameterValueException : ApiException
    {
        public string QueryParameterName { get; }

        public BadQueryParameterValueException(string queryParamName)
            : base($"Invalid input to query string parameterer: '{queryParamName}'.", httpStatusCode: HttpStatusCode.BadRequest)
        {
            this.QueryParameterName = queryParamName;
        }

        public BadQueryParameterValueException(string queryParamName, string message)
            : base($"Invalid input to query string parameterer '{queryParamName}': {message}.", httpStatusCode: HttpStatusCode.BadRequest)
        {
            this.QueryParameterName = queryParamName;
        }
    }

    public class NotFoundQueryParameterException : ApiException
    {
        public NotFoundQueryParameterException(string queryParamName) :
            base($"Missing query string parameterer: '{queryParamName}'.", httpStatusCode: HttpStatusCode.BadRequest)
        {

        }
    }

    public class BadInputParameterValueException : ApiException
    {
        public string InputParameterName { get; }

        public BadInputParameterValueException(string inputParamName)
            : base($"Invalid value to input string parameterer: '{inputParamName}'.", httpStatusCode: HttpStatusCode.BadRequest)
        {
            this.InputParameterName = inputParamName;
        }

        public BadInputParameterValueException(string inputParamName, string message)
            : base($"Invalid value to input string parameterer '{inputParamName}': {message}.", httpStatusCode: HttpStatusCode.BadRequest)
        {
            this.InputParameterName = inputParamName;
        }
    }
    public class QuotaExceededException : ApiException
    {
        public QuotaExceededException() : base("You have exceeded your quota. Please upgrade you licence."
            , httpStatusCode: HttpStatusCode.Unauthorized)
        {

        }
    }
}
