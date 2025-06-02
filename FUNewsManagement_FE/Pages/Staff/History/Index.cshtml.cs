using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FUNewsManagement_FE.Pages.Staff.History
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<NewsDto> NewsList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                string url = $"NewsArticles?$filter=AuthorId eq {userId}&$orderby=CreatedDate desc";

                // TODO: Gọi API lấy danh sách bài viết đã tạo bởi người dùng hiện tại
                // var response = await client.GetFromJsonAsync<ODataResponse<NewsDto>>(url);
                // if (response != null) NewsList = response.Value;
            }
        }

        public class NewsDto
        {
            public string Title { get; set; }
            public DateTime CreatedDate { get; set; }
            public int Status { get; set; }
        }
    }
}
