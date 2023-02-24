using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DataServiceManager _dataService; 
        
        public IndexModel(ILogger<IndexModel> logger, DataServiceManager dataService) 
        {
            _logger = logger;
            _dataService = dataService;
        }
        [BindProperty]
        public List<Category> Categories { get; set; }

        public void OnGet()
        {
            //Categories = _dataService.GetCategories();
        }
    }
}