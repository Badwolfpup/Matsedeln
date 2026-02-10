using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matsedeln.Utils
{
    public class ApiService
    {
        private static readonly HttpClient _http;
        private static readonly JsonSerializerOptions _jsonOptions;
        private const string ApiKey = "94kngslij458nlbdnlk589ddgmdrdmnb4589ujdgf";

        public static ApiService Instance { get; } = new ApiService();

        static ApiService()
        {
            _http = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true
            };
#if DEBUG
            _http.BaseAddress = new Uri("http://localhost:5127/");
            _http.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
#else
                _http.BaseAddress = new Uri("http://badwolfpup-001-site2.mtempurl.com");
                _http.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
#endif
        }

        private ApiService() { }

        // Generic GET: Works for any model in your Shared Library
        public async Task<List<T>?> GetListAsync<T>(string endpoint)
        {
            return await _http.GetFromJsonAsync<List<T>>(endpoint, _jsonOptions);
        }

        // Generic POST: Perfect for adding new data
        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, data, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"===== POST {endpoint} failed: {response.StatusCode} =====");
                    System.Diagnostics.Debug.WriteLine(errorContent);
                    System.Diagnostics.Debug.WriteLine("===== END ERROR =====");
#if DEBUG
                    System.Windows.MessageBox.Show(errorContent, $"POST Failed: {response.StatusCode}");
#endif
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POST {endpoint} exception: {ex.Message}");
                throw;
            }
        }

        // Generic DELETE: Works for any model in your Shared Library
        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _http.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        // Generic PUT: Perfect for patching data
        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            var response = await _http.PutAsJsonAsync(endpoint, data, _jsonOptions);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UploadImageAsync(string localFilePath)
        {
            if (!System.IO.File.Exists(localFilePath)) return null;

            using var content = new MultipartFormDataContent();

            // Read the file from disk
            var fileStream = System.IO.File.OpenRead(localFilePath);
            var fileContent = new StreamContent(fileStream);

            // Add the file to the request (the name "file" must match the Controller parameter)
            content.Add(fileContent, "file", System.IO.Path.GetFileName(localFilePath));

            var response = await _http.PostAsync("api/upload", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ImageUploadResult>(_jsonOptions);
                // The server returns the RELATIVE path (e.g., /images/guid.png)
                return result?.path;
            }

            return null;
        }

    }

    public class ImageUploadResult
    {
        public string path { get; set; }
    }
}
