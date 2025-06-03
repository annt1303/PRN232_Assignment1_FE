using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_FE.Pages.Staff.Home
{
    public class IndexModel : PageModel
    {
     
        private bool IsStaff() => HttpContext.Session.GetInt32("AccountRole") == 2;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsStaff()) return RedirectToPage("/Error");
            return Page();
        }
    }
}
