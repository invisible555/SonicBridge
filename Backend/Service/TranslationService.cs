using System.Net.Http;
using System.Threading.Tasks;

namespace Backend.Service
{  
    public class TranslationService : ITranslationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TranslationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            var form = new MultipartFormDataContent
        {
            { new StringContent(text), "text" },
            { new StringContent(sourceLang), "source_lang" },
            { new StringContent(targetLang), "target_lang" }
        };

            var client = _httpClientFactory.CreateClient("WhisperApi");
            var response = await client.PostAsync("/translate", form);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(content);

            return content;
        }
    }

}
