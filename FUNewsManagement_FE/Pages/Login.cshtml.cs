using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_FE.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public LoginModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // TODO: Gọi API đăng nhập ở đây
            /*
            var client = _clientFactory.CreateClient("ODataAPI");
            var response = await client.PostAsJsonAsync("auth/login", new { Email, Password });
            if (response.IsSuccessStatusCode)
            {
                // Lưu token và redirect theo role
            }
            else
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
            }
            */

            return Page(); // Tạm thời
        }
    }

}
