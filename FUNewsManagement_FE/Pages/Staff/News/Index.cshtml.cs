using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_FE.Pages.Staff.News
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<NewsDto> NewsList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            string url = "NewsArticles?$orderby=CreatedDate desc";

            if (!string.IsNullOrWhiteSpace(Search))
            {
                url = $"NewsArticles?$filter=contains(Title,'{Search}')&$orderby=CreatedDate desc";
            }

            // TODO: Gọi API để lấy danh sách bài viết của staff
            // var response = await client.GetFromJsonAsync<ODataResponse<NewsDto>>(url);
            // if (response != null) NewsList = response.Value;
        }

        public class NewsDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public DateTime CreatedDate { get; set; }
            public int Status { get; set; }
        }
    }
}
