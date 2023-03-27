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
                String messageDetail = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset=""utf-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <link rel=""stylesheet"" href=""~/lib/bootstrap/dist/css/bootstrap.min.css"" />
                    <link rel=""stylesheet"" href=""~/css/site.css"" asp-append-version=""true"" />
                    <link rel=""stylesheet"" href=""~/Waho.styles.css"" asp-append-version=""true"" />
                    <link href=""~/css/styles.css"" rel=""stylesheet"" />
                    <link href=""~/css/modal.css"" rel=""stylesheet"" asp-append-version=""true"" />
                    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.3/font/bootstrap-icons.css"">
                </head>
                <body>
                    <div class='col-md-4 mx-auto form-container login-container'>
                        <div class='pb-4 px-3'>
                            <div class='d-flex mt-4 flex-column'>
                                <h1 class='mx-auto d-flex align-items-center'>
                                    <img src='~/img/wahologo.png' width='50px' height='50px' />Wa<span class='primary-color'>Ho</span>
                                </h1>
                                <h4 class='m-auto'>Xác nhận đổi mật khẩu</h4>
                            </div>
                            <hr />
                            <h5>Để cập nhật mật khẩu mới vui lòng bấm vào đường link: <a class='primary-color' href='https://localhost:7178/ResetPassword'>Tạo mật khẩu mới</a></h5>
                            <div>Người gửi :Waho</div>
                            <div>số điện thoại : 0899999999</div>
                            <div>địa chỉ : Hoa Lac, Ha Noi</div>
                            <h5 class='primary-color'>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h5>
                        </div>
                    </div>
                </body>
                </html>";
                await _emailService.SendEmailAsync(email, "Đổi mật khẩu", messageDetail);
                message = "Vui lòng kiểm tra mail";
                return Page();
            }

            ModelState.AddModelError("", "Sai địa chỉ email");
            return Page();
        }
    }
}
