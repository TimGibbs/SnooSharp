namespace SnooSharp;

public interface ISnooClient
{
    Task<DataResponse> GetData(DateTime date);
    Task<IEnumerable<DataResponse>> GetDataResponses(DateTime start, DateTime end);
}