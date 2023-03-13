using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        public IndexModel(ILogger<IndexModel> logger, WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
        }
        public int numberBill { get; set; } = 0;
        public double totalMoney { get; set; } = 0;
        public int numberReturn { get; set; } = 0;
        public double totalMoneyReturn { get; set; } = 0;
        private DateTime now = DateTime.Today;
        private List<BillDetail> billDetailInday { get; set; }
        private List<ReturnOrder> returnOrders { get; set; }
        private List<BillDetail> billDetailYesterday { get; set; }
        private List<BillDetail> billDetailMonth { get; set; }
        public double percentYes { get; set; }
        private double totalMoneyYes { get; set; }
        public double percentMonthYes { get; set; }
        private double totalMoneyMonthYes { get; set; }
        public List<string> ProductNames { get; set; } = new List<string>();
        public List<double> Quantities { get; set; } = new List<double>();
        [BindProperty(SupportsGet = true)]
        public int selectFilter { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public DateTime dateQuery { get; set; } = DateTime.Now;
        private double total(List<BillDetail> list)
        {
            double totalM = 0;
            foreach (var b in list)
            {
                totalM += b.Quantity * b.Product.UnitInStock * (1 - b.Discount);
            }
            return totalM;
        }
        public async Task OnGetAsync()
        {
            //hóa đơn trong ngày
            numberBill = _context.Bills.Where(b => b.Date == now).Count();
            //get billdetail in day
            billDetailInday = _dataService.GetBillDetails(now);
            totalMoney = total(billDetailInday);
            //hoàn đơn trong ngày
            numberReturn = _context.ReturnOrders.Where(b => b.Date == now).Count();
            //hoàn đơn
            returnOrders = _dataService.GetReturOrderByDay(now);
            foreach (var b in returnOrders)
            {
                totalMoneyReturn += b.PaidCustomer;
            }
            //get bill detail yesterday
            billDetailYesterday = _dataService.GetBillDetails(now.AddDays(-1));
                totalMoneyYes = total(billDetailYesterday);
            if (totalMoneyYes != 0)
            {
                percentYes = ((totalMoney- totalMoneyYes) / totalMoneyYes) * 100;
            }
            else
            {
                percentYes = numberBill* 100;
            }
            billDetailMonth = _dataService.GetBillDetails(now.AddMonths(-1));
            
            totalMoneyMonthYes = total(billDetailMonth);
            if (totalMoneyMonthYes != 0)
            {
                percentMonthYes = ((totalMoney - totalMoneyMonthYes) / totalMoneyMonthYes) * 100;
            }
            else
            {
                percentMonthYes = numberBill * 100;
            }

            //dash board 
            if (selectFilter == 1)
            {
                //theo doanh số bill
                var query = from bd in _context.BillDetails
                            join b in _context.Bills on bd.BillId equals b.BillId
                            join p in _context.Products on bd.ProductId equals p.ProductId
                            where b.Date.Month == dateQuery.Month && b.Date.Year == dateQuery.Year
                            group new { p.ProductName, bd.Quantity, p.UnitInStock, bd.Discount } by p.ProductName into g
                            orderby g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) descending
                            select new { ProductName = g.Key, TotalQuantity = g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) };
                var results = await query.Take(10).ToListAsync();
                // order
                var queryOrder = from bd in _context.OderDetails
                            join b in _context.Oders on bd.OderId equals b.OderId
                            join p in _context.Products on bd.ProductId equals p.ProductId
                            //where b. .Month == dateQuery.Month && b.Date.Year == dateQuery.Year
                            group new { p.ProductName, bd.Quantity, p.UnitInStock, bd.Discount } by p.ProductName into g
                            orderby g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) descending
                            select new { ProductName = g.Key, TotalQuantity = g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) };
                var resultsOrder = await queryOrder.Take(10).ToListAsync();
                foreach (var result in results)
                {
                    ProductNames.Add(result.ProductName);
                    Quantities.Add(result.TotalQuantity);
                }
            }
            else
            {
                //theo số lượng
                var query = from bd in _context.BillDetails
                            join b in _context.Bills on bd.BillId equals b.BillId
                            join p in _context.Products on bd.ProductId equals p.ProductId
                            where b.Date.Month == dateQuery.Month && b.Date.Year == dateQuery.Year
                            group new { p.ProductName, bd.Quantity} by p.ProductName into g
                            orderby g.Sum(x => x.Quantity) descending
                            select new { ProductName = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
                var results = await query.Take(10).ToListAsync();
                foreach (var result in results)
                {
                    ProductNames.Add(result.ProductName);
                    Quantities.Add(result.TotalQuantity);
                }
            }
            
            
        }

    }
}