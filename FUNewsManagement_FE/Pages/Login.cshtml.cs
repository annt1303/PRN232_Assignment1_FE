using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("ODataAPI");
            var loginData = new { Email, Password };

            try
            {
                var response = await client.PostAsJsonAsync("api/SystemAccount/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Account != null)
                    {
                        HttpContext.Session.SetInt32("AccountId", result.Account.AccountId);
                        HttpContext.Session.SetString("AccountName", result.Account.AccountName);
                        HttpContext.Session.SetString("AccountEmail", result.Account.AccountEmail);
                        HttpContext.Session.SetInt32("AccountRole", result.Account.AccountRole);

                        return result.Account.AccountRole == 1
                            ? RedirectToPage("/Admin/Home/Index")
                            : RedirectToPage("/Staff/Home/Index");
                    }

                    ModelState.AddModelError("", "Invalid account data.");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email or password.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();
            return RedirectToPage("Login");
        }
        public class LoginResponse
        {
            public string Token { get; set; }
            public AccountDetails Account { get; set; }
        }

        public class AccountDetails
        {
            public int AccountId { get; set; }
            public string AccountName { get; set; }
            public string AccountEmail { get; set; }
            public int AccountRole { get; set; }
            public string AccountPassword { get; set; }
        }
    }
}