using Microsoft.AspNetCore.Http;
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
        private readonly Author _author;
        public IndexModel(ILogger<IndexModel> logger, WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
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
        public List<double> QuantitiesDay { get; set; } = new List<double>();
        public List<int> Days { get; set; } = new List<int>();

        private List<KeyValuePair<string, double>> temp = new List<KeyValuePair<string, double>>();
        private List<KeyValuePair<int, double>> tempDay = new List<KeyValuePair<int, double>>();
        [BindProperty(SupportsGet = true)]
        public int selectFilter { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public DateTime dateQuery { get; set; } = DateTime.Now;
        [BindProperty(SupportsGet = true)]
        public DateTime dateQueryDay { get; set; } = DateTime.Now;
        private double total(List<BillDetail> list)
        {
            double totalM = 0;
            foreach (var b in list)
            {
                totalM += b.Quantity * b.Product.UnitInStock * (1 - b.Discount);
            }
            return totalM;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
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

            //dash board danh số sản phẩm theo tháng
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
                            where b.OrderDate.Month == dateQuery.Month && b.OrderDate.Year == dateQuery.Year
                            group new { p.ProductName, bd.Quantity, p.UnitInStock, bd.Discount } by p.ProductName into g
                            orderby g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) descending
                            select new { ProductName = g.Key, TotalQuantity = g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) };
                var resultsOrder = await queryOrder.Take(10).ToListAsync();
                // map to list
                if (results.Count == 0)
                {
                    foreach (var ro in resultsOrder)
                    {
                       temp.Add(new KeyValuePair<string, double>(ro.ProductName, ro.TotalQuantity));
                    }
                }
                foreach(var rs in results)
                {
                    foreach (var ro in resultsOrder)
                    {
                        if(rs.ProductName == ro.ProductName)
                        {
                            temp.Add(new KeyValuePair<string, double>(rs.ProductName, rs.TotalQuantity + ro.TotalQuantity));
                        }
                    }
                    temp.Add(new KeyValuePair<string, double>(rs.ProductName, rs.TotalQuantity));
                }
                //sort
                temp = temp.OrderByDescending(x => x.Value).ToList();
                for (int i = 0; i <temp.Count && i < 10; i++)
                {
                    ProductNames.Add(temp[i].Key);
                    Quantities.Add(temp[i].Value);
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
                // order
                var queryOrder = from bd in _context.OderDetails
                                 join b in _context.Oders on bd.OderId equals b.OderId
                                 join p in _context.Products on bd.ProductId equals p.ProductId
                                 where b.OrderDate.Month == dateQuery.Month && b.OrderDate.Year == dateQuery.Year
                                 group new { p.ProductName, bd.Quantity } by p.ProductName into g
                                 orderby g.Sum(x => x.Quantity ) descending
                                 select new { ProductName = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
                var resultsOrder = await queryOrder.Take(10).ToListAsync();
                // map to list
                if (results.Count == 0)
                {
                    foreach (var ro in resultsOrder)
                    {
                        temp.Add(new KeyValuePair<string, double>(ro.ProductName, ro.TotalQuantity));
                    }
                }
                foreach (var rs in results)
                {
                    foreach (var ro in resultsOrder)
                    {
                        if (rs.ProductName == ro.ProductName)
                        {
                            temp.Add(new KeyValuePair<string, double>(rs.ProductName, rs.TotalQuantity + ro.TotalQuantity));
                        }
                    }
                    temp.Add(new KeyValuePair<string, double>(rs.ProductName, rs.TotalQuantity));
                }
                //sort
                temp = temp.OrderByDescending(x => x.Value).ToList();
                for (int i = 0; i < temp.Count && i < 10; i++)
                {
                    ProductNames.Add(temp[i].Key);
                    Quantities.Add(temp[i].Value);
                }
            }
            // doanh số theo ngày trong tháng trong tháng
            //theo doanh số bill
            var queryDay = from bd in _context.BillDetails
                        join b in _context.Bills on bd.BillId equals b.BillId
                        join p in _context.Products on bd.ProductId equals p.ProductId
                        where b.Date.Month == dateQueryDay.Month && b.Date.Year == dateQueryDay.Year
                        group new { bd.Quantity, p.UnitInStock, bd.Discount } by b.Date.Day into g
                        orderby g.Key ascending
                        select new { Day = g.Key, TotalQuantity = g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) };
            var resultsDay = await queryDay.Take(10).ToListAsync();
            // order
            var queryOrderDay = from bd in _context.OderDetails
                             join b in _context.Oders on bd.OderId equals b.OderId
                             join p in _context.Products on bd.ProductId equals p.ProductId
                             where b.OrderDate.Month == dateQueryDay.Month && b.OrderDate.Year == dateQueryDay.Year
                             group new { bd.Quantity, p.UnitInStock, bd.Discount } by b.OrderDate.Day into g
                             orderby g.Key ascending
                             select new { Day = g.Key, TotalQuantity = g.Sum(x => x.Quantity * x.UnitInStock * (1 - x.Discount)) };
            var resultsOrderDay = await queryOrderDay.Take(10).ToListAsync();
            // map to list
            if (resultsDay.Count == 0)
            {
                foreach (var ro in resultsOrderDay)
                {
                    tempDay.Add(new KeyValuePair<int, double>(ro.Day, ro.TotalQuantity));
                }
            }
            foreach (var rs in resultsDay)
            {
                foreach (var ro in resultsOrderDay)
                {
                    if (rs.Day == ro.Day)
                    {
                        tempDay.Add(new KeyValuePair<int, double>(rs.Day, rs.TotalQuantity + ro.TotalQuantity));
                    }
                }
                tempDay.Add(new KeyValuePair<int, double>(rs.Day, rs.TotalQuantity));
            }
            for (int i = 0; i < tempDay.Count; i++)
            {
                Days.Add(tempDay[i].Key);
                QuantitiesDay.Add(temp[i].Value);
            }

            return Page();
        }

    }
}