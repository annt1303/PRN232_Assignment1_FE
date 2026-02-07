using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace FUNewsManagement_FE.Pages.Staff.News
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public List<NewsDto> NewsList { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<TagDto> Tags { get; set; } = new();
        [BindProperty]
        public NewsDto NewNews { get; set; } = new();
        [BindProperty]
        public NewsDto EditNews { get; set; } = new();

        public string ErrorMessage { get; set; }

        private async Task FetchDataAsync(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetFromJsonAsync<List<NewsDto>>(url);
                if (response != null) NewsList = response;

                var categoriesResponse = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                if (categoriesResponse != null) Categories = categoriesResponse;

                var tagsResponse = await client.GetFromJsonAsync<List<TagDto>>("api/Tag");
                if (tagsResponse != null) Tags = tagsResponse;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Không thể tải dữ liệu. Vui lòng thử lại.";
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            string url = "api/NewsArticles?$orderby=CreatedDate desc";

            if (!string.IsNullOrWhiteSpace(Search))
            {
                url = $"api/NewsArticles?$filter=contains(NewsTitle,'{Uri.EscapeDataString(Search)}')&$orderby=CreatedDate desc";
            }

            await FetchDataAsync(client, url);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            NewNews.CreatedDate = DateTime.UtcNow;
            NewNews.CreatedById = GetAccountIdFromSession();
            NewNews.CreatedBy = GetAccountNameFromSession() ?? "UnknownUser";
            NewNews.NewsArticleId = Guid.NewGuid().ToString();
            NewNews.UpdatedById = GetAccountIdFromSession();
            NewNews.UpdatedBy = GetAccountNameFromSession() ?? "UnknownUser";
            NewNews.ModifiedDate = DateTime.UtcNow;

            return await ExecuteApiRequest(client, async () =>
            {
                var content = JsonContent.Create(NewNews);
                var response = await client.PostAsync("api/NewsArticles", content);
                return response;
            }, "Không thể tạo bài viết.", "POST Create News");
        }

        public async Task<IActionResult> OnGetEditAsync(string id)
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            try
            {
                var response = await client.GetAsync($"api/NewsArticles/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { Error = $"Không thể lấy dữ liệu bài viết. Mã lỗi: {response.StatusCode}" });
                }

                var news = await response.Content.ReadFromJsonAsync<NewsDto>();
                if (news != null)
                {
                    EditNews = news;
                    var categoriesResponse = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                    Categories = categoriesResponse ?? new List<CategoryDto>();
                    var tagsResponse = await client.GetFromJsonAsync<List<TagDto>>("api/Tag");
                    Tags = tagsResponse ?? new List<TagDto>();
                    return new JsonResult(new { EditNews, Categories, Tags });
                }
                return new JsonResult(new { Error = "Không tìm thấy bài viết." });
            }
            catch (Exception)
            {
                return new JsonResult(new { Error = "Không thể tải dữ liệu bài viết." });
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            EditNews.ModifiedDate = DateTime.UtcNow;
            EditNews.UpdatedById = GetAccountIdFromSession();
            EditNews.UpdatedBy = GetAccountNameFromSession() ?? "UnknownUser";

            if (string.IsNullOrEmpty(EditNews.NewsTitle)) EditNews.NewsTitle = "No Title";
            if (string.IsNullOrEmpty(EditNews.Headline)) EditNews.Headline = "No Headline";
            if (string.IsNullOrEmpty(EditNews.NewsContent)) EditNews.NewsContent = "No Content";
            if (string.IsNullOrEmpty(EditNews.NewsSource)) EditNews.NewsSource = "N/A";
            if (EditNews.CategoryId == 0) EditNews.CategoryId = 1;
            if (EditNews.TagIds == null) EditNews.TagIds = new List<int>();

            return await ExecuteApiRequest(client, async () =>
            {
                var content = JsonContent.Create(EditNews);
                var response = await client.PutAsync("api/NewsArticles", content);
                return response;
            }, "Không thể cập nhật bài viết.", "PUT Edit News");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            return await ExecuteApiRequest(client, async () =>
            {
                var response = await client.DeleteAsync($"api/NewsArticles/{id}");
                return response;
            }, "Không thể xóa bài viết.", "DELETE News");
        }

        private async Task<IActionResult> ExecuteApiRequest(HttpClient client, Func<Task<HttpResponseMessage>> apiCall, string errorMessage, string logPrefix)
        {
            try
            {
                var response = await apiCall();
                if (response.IsSuccessStatusCode)
                {
                    await FetchDataAsync(client, "api/NewsArticles?$orderby=CreatedDate desc");
                    return RedirectToPage();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"{errorMessage} Lỗi: {errorContent}";
                    await FetchDataAsync(client, "api/NewsArticles?$orderby=CreatedDate desc");
                    return Page();
                }
            }
            catch (Exception)
            {
                ErrorMessage = $"Đã xảy ra lỗi khi {logPrefix.ToLower().Replace("news", "bài viết")}. Vui lòng thử lại.";
                await FetchDataAsync(client, "api/NewsArticles?$orderby=CreatedDate desc");
                return Page();
            }
        }

        private int GetAccountIdFromSession()
        {
            return HttpContext.Session.GetInt32("AccountId") ?? 0;
        }

        private string GetAccountNameFromSession()
        {
            return HttpContext.Session.GetString("AccountName") ?? "UnknownUser";
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
            public int CreatedById { get; set; }
            public string CreatedBy { get; set; } = string.Empty;
            public int UpdatedById { get; set; }
            public string UpdatedBy { get; set; } = string.Empty;
            public DateTime? ModifiedDate { get; set; }
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