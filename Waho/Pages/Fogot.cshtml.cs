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
        public string Errmessage { get; set; }
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
                String messageDetail = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset=""utf-8"" />
                    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha2/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-aFq/bzH65dt+w6FI2ooMVUpc+21e0SRygnTpmBvdBgSdnuTN7QbdgL+OapgHtvPp"" crossorigin=""anonymous"">
                </head>
                <body>
                    <div class='col-md-4 mx-auto form-container login-container'>
                        <div class='pb-4 px-3'>
                            <div class='d-flex mt-4 flex-column'>
                                <h1 class='mx-auto d-flex align-items-center'>
                                    Wa<span style=""color:#ff6533"">Ho</span>
                                </h1>
                                <h4 class='m-auto'>Xác nhận đổi mật khẩu</h4>
                            </div>
                            <hr />
                            <h5>Để cập nhật mật khẩu mới vui lòng bấm vào đường link: <a style=""color:#ff6533"" href='https://localhost:7178/ResetPassword'>Tạo mật khẩu mới</a></h5>
                            <div>Người gửi :Waho</div>
                            <div>số điện thoại : 0899999999</div>
                            <div>địa chỉ : Hoa Lac, Ha Noi</div>
                            <h5 style=""color:#ff6533"">Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h5>
                        </div>
                    </div>
                </body>
                </html>";
                await _emailService.SendEmailAsync(email, "Đổi mật khẩu", messageDetail);
                message = "Vui lòng kiểm tra email của bạn";
                TempData["successMessage"] = message;
                return Page();
            }

            ModelState.AddModelError("", "Sai địa chỉ email");
            Errmessage = "Sai địa chỉ email";
            return Page();
        }
    }
}
