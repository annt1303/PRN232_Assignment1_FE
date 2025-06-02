using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FUNewsManagement_FE.Pages.Staff.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ProfileDto Profile { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // TODO: Gọi API để lấy thông tin user hiện tại
                // var response = await client.GetFromJsonAsync<ProfileDto>($"Accounts({userId})");
                // if (response != null) Profile = response;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // TODO: Gọi API PUT /odata/Accounts(userId) để cập nhật
                // await client.PutAsJsonAsync($"Accounts({userId})", Profile);
            }

            return RedirectToPage();
        }

        public class ProfileDto
        {
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }
    }
}
