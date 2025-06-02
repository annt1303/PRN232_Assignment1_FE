using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_FE.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<AccountDto> Accounts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            string url = "Accounts";
            if (!string.IsNullOrWhiteSpace(Search))
            {
                url += $"?$filter=contains(Email,'{Search}')";
            }

            // TODO: Gọi API lấy danh sách tài khoản
            // var response = await client.GetFromJsonAsync<ODataResponse<AccountDto>>(url);
            // if (response != null) Accounts = response.Value;
        }

        public class AccountDto
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public int Role { get; set; }
        }

        // public class ODataResponse<T>
        // {
        //     public List<T> Value { get; set; }
        // }
    }
}
