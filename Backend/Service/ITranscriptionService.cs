namespace Backend.Service
{
    public interface ITranscriptionService
    {
        Task<object> GetOrGenerateTranscription(int userId, string fileName, string language);
    }
}
