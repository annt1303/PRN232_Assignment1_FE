using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

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
        public List<CategoryDto> ParentCategories { get; set; } = new();
        [BindProperty]
        public CategoryDto EditCategory { get; set; } = new();
        [BindProperty]
        public CategoryDto NewCategory { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            string url = "api/Category";

            if (!string.IsNullOrWhiteSpace(Search))
            {
                url += $"?$filter=contains(categoryName,'{Uri.EscapeDataString(Search)}')";
            }

            try
            {
                var response = await client.GetFromJsonAsync<List<CategoryDto>>(url);
                if (response != null)
                {
                    Categories = response;
                }
                var allCategories = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                if (allCategories != null)
                {
                    ParentCategories = allCategories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return Page();
            }
            return Page();
        }

        // Phương thức này sẽ được gọi khi bấm "Sửa"
        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            try
            {
                // Lấy thông tin danh mục theo id
                var response = await client.GetFromJsonAsync<CategoryDto>($"api/Category/{id}");
                if (response != null)
                {
                    EditCategory = response;
                }
                else
                {
                    return NotFound();
                }

                // Lấy danh sách danh mục cha để điền vào dropdown
                var allCategories = await client.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                if (allCategories != null)
                {
                    ParentCategories = allCategories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category: {ex.Message}");
                return Page();
            }

            // Trả về dữ liệu dạng JSON để JavaScript xử lý
            return new JsonResult(new { EditCategory, ParentCategories });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var content = JsonContent.Create(EditCategory);
            var response = await client.PutAsync($"api/Category", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var content = JsonContent.Create(NewCategory);
            var response = await client.PostAsync("api/Category", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ODataAPI");
            var response = await client.DeleteAsync($"api/Category/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            return Page();
        }

        public class CategoryDto
        {
            public int categoryId { get; set; }
            public string categoryName { get; set; }
            public string categoryDesciption { get; set; }
            public int parentCategoryId { get; set; }
            public string parentCategoryName { get; set; }
            public int Status { get; set; } = 1;
        }
    }
}