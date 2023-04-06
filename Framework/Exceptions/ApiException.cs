using System.Net;

namespace Framework.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }
        /// <summary>
        /// a short, human-readable summary of the problem that SHOULD NOT change from occurrence to occurrence of the problem.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// a human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string Detail { get; protected set; }

        public ApiException(string title, string detail = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
            : base(title)
        {
            this.HttpStatusCode = httpStatusCode;
            this.Title = title;
            this.Detail = detail;
        }
    }
}
