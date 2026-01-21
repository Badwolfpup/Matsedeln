using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Utils
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private const string ApiKey = "94kngslij458nlbdnlk589ddgmdrdmnb4589ujdgf";

        public ApiService()
        {
            _http = new HttpClient();
            // Check if we are debugging locally
            #if DEBUG
                _http.BaseAddress = new Uri("http://localhost:5127/"); // Your local dev URL
                _http.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
            #else
                _http.BaseAddress = new Uri("http://your-site.smarterasp.net/");
                _http.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
            #endif
            
        }

        // Generic GET: Works for any model in your Shared Library
        public async Task<List<T>?> GetListAsync<T>(string endpoint)
        {
            return await _http.GetFromJsonAsync<List<T>>(endpoint);
        }

        // Generic POST: Perfect for adding new data
        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
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
            var response = await _http.PutAsJsonAsync(endpoint, data);
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
                var result = await response.Content.ReadFromJsonAsync<ImageUploadResult>();
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
