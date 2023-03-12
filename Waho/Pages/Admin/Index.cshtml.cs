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
        public List<Int64> Quantities { get; set; } = new List<Int64>();
        private double total(List<BillDetail> list)
        {
            double totalM = 0;
            foreach (var b in list)
            {
                totalM += b.Quantity * b.Product.UnitInStock * (1 - b.Discount);
            }
            return totalM;
        }
        public async void OnGetAsync()
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
            var sqlconnectstring = @"server =ADMIN\\DUC;uid=sa;pwd=12345;database=Waho;Trusted_Connection=True;MultipleActiveResultSets=true;";
            var connection = new SqlConnection(sqlconnectstring);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
            SELECT TOP 10 p.productName, SUM(bd.quantity*p.unitInStock*(1-bd.discount)) as TotalQuantity
                FROM BillDetails as bd 
				join Bill as b on bd.billID = b.billID
				join Products as p on bd.productID = p.productID
				GROUP BY p.productName
				ORDER BY TotalQuantity DESC", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ProductNames.Add(reader.GetString(0));
                    Quantities.Add(reader.GetInt64(1));
                }
            }
        }

    }
}