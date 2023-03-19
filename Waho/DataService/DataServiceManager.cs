using Waho.DataService;
using Waho.WahoModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Waho.DataService
{
    public class DataServiceManager
    {
        private readonly WahoContext _context;
        public DataServiceManager(WahoContext context)
        {
            _context = context;
        }

        public Employee GetEmployeeByUserAndPass(string userName, string password)
        {
            return _context.Employees.FirstOrDefault(emp => emp.UserName == userName && emp.Password == password);
        }
        //get employee by email
        public Employee GetEmployeeByEmail(string email)
        {
            return _context.Employees.FirstOrDefault(emp => emp.Email == email);
        }
        public Employee GetEmployeeByUserName(string userName)
        {
            return _context.Employees.FirstOrDefault(emp => emp.UserName == userName);
        }
        public List<SubCategory> GetSubCategories(int id)
        {
            return _context.SubCategories
                    .Where(sb => sb.CategoryId == id)
                    .ToList();
        }
        public List<Product> GetProductsByCateID(int id)
        {
            return _context.Products
                            .Where(p => p.SubCategory.CategoryId == id)
                            .Where(p => p.Active == true)
                            .Include(p => p.SubCategory)
                            .ThenInclude(s => s.Category)
                            .Include(p => p.Supplier)
                            .ToList();
        }

        // paging bill
        public List<Bill> GetBillsPagingAndFilter(int pageIndex, int pageSize, string textSearch, string status, string raw_dateFrom, DateTime dateFrom, DateTime dateTo)
        {

            List<Bill> bills = new List<Bill>();
            //default 
            var query = from b in _context.Bills select b;

            if (!string.IsNullOrEmpty(textSearch))
            {
                query = query.Include(b => b.Customer).Where(b => b.BillId.ToString().Contains(textSearch)
                                || b.Customer.CustomerName.Contains(textSearch));
            }

            if (status != "all")
            {
                query = query.Where(b => (b.BillStatus.Contains(status)));
            }

            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                query = query.Where(b => (b.Date >= dateFrom && b.Date <= dateTo));
            }

            bills = query.Where(b => b.Active == true)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            return bills;
        }


        public InventorySheet getInventorySheetByID(int id)
        {
            return _context.InventorySheets.Where(i=> i.Active == true)
                                .Include(p => p.UserNameNavigation)
                                .Where(i => i.InventorySheetId == id).FirstOrDefault();
        }
        public List<InventorySheetDetail> GetInventorySheetDetails(int inventorySheetID)
        {
            List<InventorySheetDetail> inventorySheetDetails= new List<InventorySheetDetail>();
            inventorySheetDetails = _context.InventorySheetDetails
                                            .Include(p => p.Product)
                                            .Include(p => p.InventorySheet)
                                            .Where(i => i.InventorySheetId == inventorySheetID)
                                            .ToList() ;
            return inventorySheetDetails;
        }
        public List<Supplier> getSupplierList()
        {
            return _context.Suppliers.Where(s => s.Active == true).ToList();
        }
        public List<ReturnOrderProduct> GetReturnOrderDetails(int returnOrderID)
        {
            return _context.ReturnOrderProducts
                                        .Include(r => r.Product)
                                        .Include(r => r.ReturnOrder)
                                        .Where(r => r.ReturnOrderId == returnOrderID)
                                        .ToList();
        }
        public ReturnOrder getReturnOrderByID(int id)
        {
            return _context.ReturnOrders.Where(i => i.Active == true)
                                .Include(p => p.UserNameNavigation)
                                .Include(r => r.Customer)
                                .Where(i => i.ReturnOrderId == id).FirstOrDefault();
        }
        
        // paging product
        public List<Product> GetProductsPagingAndFilter(int pageIndex, int pageSize, string textSearch, int subCategoryID, int categoryID)
        {

            List<Product> products = new List<Product>();
            //default 
            var query = _context.Products.Where(p => p.SubCategory.CategoryId == categoryID)
                                         .Where(p => p.Active == true);
            if (subCategoryID > 0)
            {
                query = query.Where(p => p.SubCategoryId == subCategoryID);
            }
            if (!string.IsNullOrEmpty(textSearch))
            {
                query = query.Where(p => p.ProductName.ToLower().Contains(textSearch.ToLower()) 
                                || p.Trademark.ToLower().Contains(textSearch.ToLower()) 
                                || p.Supplier.Branch.ToLower().Contains(textSearch.ToLower()) 
                                || p.SubCategory.SubCategoryName.ToLower().Contains(textSearch.ToLower()));
            }

            products = query.Where(p => p.SubCategory.CategoryId == categoryID)
                    .Include(p => p.SubCategory)
                    .ThenInclude(s => s.Category)
                    .Include(p => p.Supplier)
                    .OrderBy(p => p.ProductName)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            return products;
        }
        // paging inventory sheet
        public List<InventorySheet> getInventoryPagingAndFilter(int pageIndex, int pageSize, string textSearch, string userName, string raw_dateFrom, string raw_dateTo)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            DateTime defaultDate = DateTime.Parse("0001-01-01");
            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                dateFrom = DateTime.Parse(raw_dateFrom);
            }
            else
            {
                raw_dateFrom = "";
            }
            if (!string.IsNullOrEmpty(raw_dateTo))
            {
                dateTo = DateTime.Parse(raw_dateTo);
            }
            else
            {
                raw_dateTo = "";
            }
            List<InventorySheet> inventories= new List<InventorySheet>();
            var query = _context.InventorySheets.Include(i => i.UserNameNavigation)
                                                .Where(i => i.Active == true)
                                                .Where(i => i.UserNameNavigation.EmployeeName.ToLower().Contains(textSearch.ToLower())
                                                            || i.Description.ToLower().Contains(textSearch.ToLower()));
            if (!string.IsNullOrEmpty(raw_dateFrom) && !string.IsNullOrEmpty(raw_dateTo) && (dateFrom.CompareTo(defaultDate) != 0 || dateTo.CompareTo(defaultDate) != 0))
            {
                query = query.Where(i => i.Date >= dateFrom && i.Date <= dateTo);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(i => i.UserName.Contains(userName));
            }
            inventories = query
                         .OrderBy(i => i.InventorySheetId)    
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return inventories;
        }
        //paging inventortSheetDetail
        public List<InventorySheetDetail> getInventorySheetDetailPaging(int pageIndex, int pageSize, int id)
        {
            List<InventorySheetDetail> inventorySheetDetails = new List<InventorySheetDetail>();
            inventorySheetDetails = _context.InventorySheetDetails.Include(i => i.InventorySheet)
                         .Include(i => i.Product)
                         .Include(i => i.InventorySheet.UserNameNavigation)
                         .Where(i => i.InventorySheetId == id)
                         .OrderBy(i => i.InventorySheetId)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return inventorySheetDetails;
        }
        //paging suppliers
        public List<Supplier> GetSupplierPagingAndFilter(int pageIndex, int pageSize, string textSearch)
        {
            List<Supplier> suppliers= new List<Supplier>();
            var query = _context.Suppliers.Where(s => s.Active == true);
            if (!string.IsNullOrEmpty(textSearch))
            {
                query = query.Where(s => s.Branch.ToLower().Contains(textSearch.ToLower()) || s.Address.ToLower().Contains(textSearch.ToLower()) || s.CompanyName.ToLower().Contains(textSearch.ToLower()) || s.Phone.ToLower().Contains(textSearch.ToLower())
                            || s.City.ToLower().Contains(textSearch.ToLower()) || s.Region.ToLower().Contains(textSearch.ToLower()) || s.TaxCode.ToLower().Contains(textSearch.ToLower()));
            }
            suppliers = query.OrderBy(s => s.SupplierId)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return suppliers;
        }

        // paging return orders
        public List<ReturnOrder> getreturnOrderPagingAndFilter(int pageIndex, int pageSize, string textSearch, string userName,string status, string raw_dateFrom, string raw_dateTo)
        {
            // filter by status and date
            Boolean _status = status == "true" ? true : false;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                dateFrom = DateTime.Parse(raw_dateFrom);
            }
            else
            {
                raw_dateFrom = "";
            }
            if (!string.IsNullOrEmpty(raw_dateTo))
            {
                dateTo = DateTime.Parse(raw_dateTo);
            }
            else
            {
                raw_dateTo = "";
            }
            
            List<ReturnOrder> returnOrders = new List<ReturnOrder>();
            var query = _context.ReturnOrders.Include(i => i.UserNameNavigation)
                                                .Include(i => i.Customer)
                                                .Where(i => i.Active == true)
                                                .Where(i => i.UserName.Contains(userName))
                                                .Where(i => i.UserNameNavigation.EmployeeName.ToLower().Contains(textSearch.ToLower())
                                                            || i.Description.ToLower().Contains(textSearch.ToLower())
                                                            || i.Customer.CustomerName.ToLower().Contains(textSearch.ToLower()));
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.State == _status);
            }
            DateTime defaultDate = DateTime.Parse("0001-01-01");
            if (!string.IsNullOrEmpty(raw_dateFrom) && !string.IsNullOrEmpty(raw_dateTo) && (dateFrom.CompareTo(defaultDate) != 0 || dateTo.CompareTo(defaultDate) != 0))
            {
                query = query.Where(i => i.Date >= dateFrom && i.Date <= dateTo);
            }
            returnOrders = query
                         .OrderBy(i => i.ReturnOrderId)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return returnOrders;
        }

        //paging return order
        public List<ReturnOrderProduct> getReturnOrderProductPaging(int pageIndex, int pageSize, int id)
        {
            List<ReturnOrderProduct> returnOrders = new List<ReturnOrderProduct>();
            returnOrders = _context.ReturnOrderProducts
                          .Include(r => r.ReturnOrder)
                          .Include(i => i.Product)
                          .Where(r => r.ReturnOrderId == id)
                          .OrderBy(i => i.ReturnOrderId)
                          .Skip((pageIndex - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();
            return returnOrders;
        }

        //get billDetails by day
        public List<BillDetail> GetBillDetails(DateTime date)
        {
            return _context.BillDetails
                                .Include(b => b.Bill)
                                .Include(b => b.Product)
                                .Where(b => b.Bill.Date == date).ToList();
        }
        //get GetReturOrder by day
        public List<ReturnOrder> GetReturOrderByDay(DateTime date)
        {
            return _context.ReturnOrders
                                .Where(b => b.Date == date).ToList();
        }
    }
}
