namespace Backend.Service
{
    public interface ITranscriptionService
    {
        Task<string> GetOrGenerateTranscription(int userId, string fileName, string language);
    }
}
