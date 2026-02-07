using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

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
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        private int GetAccountIdFromSession()
        {
            return HttpContext.Session.GetInt32("AccountId") ?? 0;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var accountId = GetAccountIdFromSession();

            if (accountId == 0)
            {
                ErrorMessage = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                return Page();
            }

            try
            {
                var response = await client.GetFromJsonAsync<ProfileDto>($"api/SystemAccount/{accountId}");
                if (response != null)
                {
                    Profile = response;
                }
                else
                {
                    ErrorMessage = "Không tìm thấy thông tin tài khoản.";
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Lỗi khi kết nối đến máy chủ: {ex.Message}";
            }
            catch (Exception)
            {
                ErrorMessage = "Đã xảy ra lỗi khi tải thông tin tài khoản.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var accountId = GetAccountIdFromSession();

            if (accountId == 0)
            {
                ErrorMessage = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                return Page();
            }

            if (string.IsNullOrEmpty(Profile.accountPassword))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu mới.";
                return Page();
            }

            try
            {
                var request = new
                {
                    accountId = Profile.accountId,
                    accountName = Profile.accountName,
                    accountEmail = Profile.accountEmail,
                    accountRole = Profile.accountRole,
                    accountPassword = Profile.accountPassword
                };

                var content = JsonContent.Create(request);
                var response = await client.PutAsync("api/SystemAccount", content);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Cập nhật mật khẩu thành công.";
                    Profile.accountPassword = null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Không thể cập nhật mật khẩu. Lỗi: {errorContent}";
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Lỗi khi kết nối đến máy chủ: {ex.Message}";
            }
            catch (Exception)
            {
                ErrorMessage = "Đã xảy ra lỗi khi cập nhật mật khẩu.";
            }

            return Page();
        }

        public class ProfileDto
        {
            public int accountId { get; set; }
            public string accountName { get; set; } = string.Empty;
            public string accountEmail { get; set; } = string.Empty;
            public int accountRole { get; set; }
            public string accountPassword { get; set; } = string.Empty;
        }
    }
}