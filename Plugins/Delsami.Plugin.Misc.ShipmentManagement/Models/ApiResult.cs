namespace Delsami.Plugin.Misc.ShipmentManagement.Models
{
    public class ApiResult
    {
        public ApiResult(bool isSucceed, string errorMessage)
        {
            IsSucceed = isSucceed;
            ErrorMessage = errorMessage;
        }

        public bool IsSucceed { get; private set; }

        public string ErrorMessage { get; private set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public ApiResult(bool isSucceed, string errorMessage, T result) : base(isSucceed, errorMessage)
        {
            Result = result;
        }

        public T Result { get; private set; }
    }
}
