using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

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
        public List<CategoryDto> Categories { get; set; } = new();
        public List<TagDto> Tags { get; set; } = new();
        public string ErrorMessage { get; set; }

        private int GetAccountIdFromSession()
        {
            return HttpContext.Session.GetInt32("AccountId") ?? 0;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var staffId = GetAccountIdFromSession();

            if (staffId == 0)
            {
                ErrorMessage = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                return Page();
            }

            try
            {
                // Gọi API để lấy danh sách bài viết của staff
                var response = await client.GetFromJsonAsync<List<NewsDto>>($"api/NewsArticles/staff/{staffId}");
                if (response != null)
                {
                    NewsList = response;
                }

                // Lấy danh sách danh mục
                var categoriesResponse = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                Categories = categoriesResponse ?? new List<CategoryDto>();

                // Lấy danh sách tag
                var tagsResponse = await client.GetFromJsonAsync<List<TagDto>>("api/Tag");
                Tags = tagsResponse ?? new List<TagDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Không thể tải dữ liệu lịch sử bài viết. Vui lòng thử lại.";
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