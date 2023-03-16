using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;
using Waho.MailManager;
using Waho.WahoModels;

namespace Waho.Pages
{
    public class FogotModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        private readonly EmailService _emailService;


        public FogotModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, EmailService emailService)
        {
            _context = context;
            _dataService = dataService;
            _emailService = emailService;
        }
        public string message { get; set; }
        [BindProperty]
        public string email { get; set; }
        public async Task OnGetAsync()
        {
            //employee = await
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Employee _employee = _dataService.GetEmployeeByEmail(email);
            if (_employee == null)
            {
                ModelState.AddModelError("", "Sai địa chỉ email");
            }
            else
            {
                String messageDetail = "<!DOCTYPE html>\n"
                        + "<html lang=\"en\">\n"
                        + "\n"
                        + "<head>\n"
                        + "</head>\n"
                        + "\n"
                        + "<body>\n"
                        + "    <h3 style=\"color: blue;\">Để cập nhật mật khẩu mới vui lòng bấm vào đường link dưới đây</h3>\n"
                        + "    <a  href=\"https://localhost:7178/ResetPassword" + "\">tạo mật khẩu mới</a>\n"
                        + "    <div>Người gửi :Waho</div>\n"
                        + "    <div>số điện thoại : 0899999999</div>\n"
                        + "    <div>địa chỉ : Hoa Lac, Ha Noi</div>\n"
                        + "    <h3 style=\"color: blue;\">Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h3>\n"
                        + "\n"
                        + "</body>\n"
                        + "\n"
                        + "</html>";
                await _emailService.SendEmailAsync(email, "Đổi mật khẩu", messageDetail);
                message = "Vui lòng kiểm tra mail";
                return Page();
            }

            ModelState.AddModelError("", "Sai địa chỉ email");
            return Page();
        }
    }
}
