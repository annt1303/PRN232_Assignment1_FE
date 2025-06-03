using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
            string url = "api/SystemAccount";

            if (!string.IsNullOrWhiteSpace(Search))
            {
                url += $"?search={Search}";
            }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<AccountDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null)
                    Accounts = result;
            }
        }


        public class AccountDto
        {
            public short AccountId { get; set; }
            public string? AccountEmail { get; set; }
            public string? AccountName { get; set; }
            public int? AccountRole { get; set; }
            public string? AccountPassword { get; set; }
        }

    }
}
