namespace RestfulApiWrapper.Exceptions
{
    public class ValidationException : ApiException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors) : base(StatusCodes.Status400BadRequest, "Validation failed")
        {
            Errors = errors;
        }
    }
}
