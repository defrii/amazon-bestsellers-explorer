namespace AmazonBestSellersExplorer.WebAPI.Services.Core
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private init; }
        public T? Value { get; private init; }
        public string? ErrorMessage { get; private init; }

        public static ServiceResult<T> Success(T value)
            => new() { IsSuccess = true, Value = value };

        public static ServiceResult<T> Failure(string errorMessage)
            => new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
