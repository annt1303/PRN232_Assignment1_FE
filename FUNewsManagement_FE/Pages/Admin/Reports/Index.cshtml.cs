using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FUNewsManagement_FE.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<NewsArticleDTO> Stats { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (StartDate != null && EndDate != null)
            {
                var client = _httpClientFactory.CreateClient("ODataAPI");
                string url = $"api/NewsArticles/show?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<NewsArticleDTO>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result != null)
                        Stats = result.OrderByDescending(x => x.CreatedDate).ToList();
                }
            }
        }

        public class NewsArticleDTO
        {
            public string? NewsArticleId { get; set; }
            public string? NewsTitle { get; set; }
            public string? Headline { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string? NewsContent { get; set; }
            public string? NewsSource { get; set; }
        }
    }
}
