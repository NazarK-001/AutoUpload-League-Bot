
namespace StrapiFunctionality
{
    internal class GetPostPut
    {
        private string apiToken = "42f9ccca30aefe01eaaa5cd0423e136b8c25b05e129d1b7173346e9b4dfc80dd46cf5d18530eea1041b52c685a268e638e17d178f7704a82f9dbac764c2993416e3129c8a123bcd5381ead4337867802595e35f8d567b9fa22180f78b72bcf89c3934857e1d5207dc9319c38c6f240f19398667f05111c831367fd0daf898a84";
        private HttpClient httpClient;

        public GetPostPut()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiToken);
        }

        public string Get(string Endpoint)
        {
            HttpResponseMessage response = httpClient.GetAsync(Endpoint).Result;
            response.EnsureSuccessStatusCode();
            string content = response.Content.ReadAsStringAsync().Result;
            string allContent = content;

            // Get the total number of pages from the "pageCount" field in the "meta" section
            int totalPages = GetTotalPages(content);

            // Loop through the remaining pages, starting from page 2
            for (int page = 2; page <= totalPages; page++)
            {
                response = httpClient.GetAsync(Endpoint + "?pagination[page]=" + page).Result;
                response.EnsureSuccessStatusCode();
                content = response.Content.ReadAsStringAsync().Result;
                allContent += content;
            }
            return allContent;
        }

        private int GetTotalPages(string content)
        {
            int startIndex = content.IndexOf("\"pageCount\":") + "\"pageCount\":".Length;
            int endIndex = content.IndexOf(",", startIndex);
            string pageCountString = content.Substring(startIndex, endIndex - startIndex);
            return int.Parse(pageCountString);
        }

        public string Post(string endpoint, HttpContent content)
        {
            HttpResponseMessage response = httpClient.PostAsync(endpoint, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                // Handle error here (e.g., throw an exception or return an error message)
                return "Error: " + response.StatusCode;
            }
        }

        public async Task<string> Post(string endpoint, Stream stream, string fileName)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(stream), "files", fileName);
                var response = await httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var fileId = responseContent.Substring(responseContent.IndexOf("\"id\":\"") + 8);
                    fileId = fileId.Substring(0, fileId.IndexOf(","));
                    return fileId;
                }
                return null;
            }
        }

        public string Put(string endpoint, HttpContent content)
        {
            HttpResponseMessage response = httpClient.PutAsync(endpoint, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                // Handle error here (e.g., throw an exception or return an error message)
                return "Error: " + response.StatusCode;
            }
        }
    }

    internal class StrapiEndpoints
    {
        public StrapiEndpoints(){}

        public Dictionary<string, string> GetDictionary()
        {
            return EndpointsDictionary;
        }

        public static Dictionary<string, string> EndpointsDictionary = new Dictionary<string, string>()
        {
            {"Players", "https://api.laterredumilieu.fr/api/players"},
            {"Maps", "https://api.laterredumilieu.fr/api/maps"},
            {"Games", "https://api.laterredumilieu.fr/api/games"},
            {"Upload", "https://api.laterredumilieu.fr/api/upload" }
        };
    }
}
