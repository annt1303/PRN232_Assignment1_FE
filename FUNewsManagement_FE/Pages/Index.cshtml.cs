using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FUNewsManagement_FE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<NewsDto> NewsList { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<TagDto> Tags { get; set; } = new();
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");

            try
            {
                // G?i API ?? l?y t?t c? tin t?c
                var response = await client.GetFromJsonAsync<List<NewsDto>>("api/NewsArticles");
                if (response != null)
                {
                    NewsList = response;
                }

                // L?y danh sách danh m?c
                var categoriesResponse = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                Categories = categoriesResponse ?? new List<CategoryDto>();

                // L?y danh sách tag
                var tagsResponse = await client.GetFromJsonAsync<List<TagDto>>("api/Tag");
                Tags = tagsResponse ?? new List<TagDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Không th? t?i danh sách tin t?c. Vui lòng th? l?i.";
            }

            return Page();
        }

        public class NewsDto
        {
            public string NewsArticleId { get; set; } = string.Empty;
            public string NewsTitle { get; set; } = string.Empty;
            public string Headline { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string NewsSource { get; set; } = string.Empty;
            public int CategoryId { get; set; }
            public string Category { get; set; } = string.Empty;
            public bool NewsStatus { get; set; }
            public List<int> TagIds { get; set; } = new();
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }

        public class TagDto
        {
            public int TagId { get; set; }
            public string TagName { get; set; } = string.Empty;
        }
    }
}