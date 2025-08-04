namespace Backend.Service
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
