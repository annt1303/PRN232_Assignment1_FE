using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public List<NewsReportDto> Stats { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (StartDate != null && EndDate != null)
            {
                var client = _httpClientFactory.CreateClient("ODataAPI");
                string url = $"Reports?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";

                // TODO: Gọi API lấy dữ liệu báo cáo
                // var response = await client.GetFromJsonAsync<ODataResponse<NewsReportDto>>(url);
                // if (response != null) Stats = response.Value.OrderByDescending(x => x.CreatedDate).ToList();
            }
        }

        public class NewsReportDto
        {
            public string Title { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
