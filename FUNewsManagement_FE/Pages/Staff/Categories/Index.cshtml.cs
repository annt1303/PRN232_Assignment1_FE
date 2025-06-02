using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_FE.Pages.Staff.Categories
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

        public List<CategoryDto> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            string url = "Categories";
            if (!string.IsNullOrWhiteSpace(Search))
            {
                url += $"?$filter=contains(Name,'{Search}')";
            }

            // TODO: Gọi API để lấy danh sách danh mục
            // var response = await client.GetFromJsonAsync<ODataResponse<CategoryDto>>(url);
            // if (response != null) Categories = response.Value;
        }

        public class CategoryDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Status { get; set; }
        }
    }
}
