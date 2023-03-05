﻿using Waho.DataService;
using Waho.WahoModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace Waho.DataService
{
    public class DataServiceManager 
    {
        private readonly WahoContext _context;
        public DataServiceManager(WahoContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        //public List<Category> GetCategories()
        //{
        //    return _context.Categories.ToList();
        //}

        public Employee GetEmployeeByUserAndPass(string userName, string password)
        {
            return _context.Employees.FirstOrDefault(emp => emp.UserName == userName && emp.Password == password);
        }
        public List<SubCategory> GetSubCategories(int id) {
            return _context.SubCategories
                    .Where(sb => sb.CategoryId == id)
                    .ToList();
        }
        public List<Product> GetProductsByCateID(int id) {
            return _context.Products
                            .Where(p => p.SubCategory.CategoryId == id)
                            .Where(p => p.Active == true)
                            .Include(p => p.SubCategory)
                            .ThenInclude(s => s.Category)
                            .Include(p => p.Supplier)
                            .ToList();
        }
        public InventorySheet getInventorySheetByID(int id)
        {
            return _context.InventorySheets
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
        // paging product
        public List<Product> GetProductsPagingAndFilter(int pageIndex, int pageSize,string textSearch, int subCategoryID,int categoryID) {

            List < Product > products = new List < Product >();
            //default 
            var query = _context.Products.Where(p => p.SubCategory.CategoryId == categoryID)
                                         .Where(p => p.Active == true);
            if (subCategoryID > 0 )
            {
                query = query.Where(p => p.SubCategoryId == subCategoryID);
            }
            if(!string.IsNullOrEmpty(textSearch))
            {
                query = query.Where(p => p.ProductName.Contains(textSearch) 
                                || p.Trademark.Contains(textSearch) 
                                || p.Supplier.Branch.Contains(textSearch) 
                                || p.SubCategory.SubCategoryName.Contains(textSearch));
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
        public List<InventorySheet> getInventoryPagingAndFilter(int pageIndex, int pageSize,string textSearch, string userName)
        {
            List<InventorySheet> inventories= new List<InventorySheet>();
            var query = _context.InventorySheets;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                query.Where(i => i.UserName.Contains(userName));
            }
            if (!string.IsNullOrEmpty(textSearch))
            {
                query.Where(i => i.UserNameNavigation.EmployeeName.Contains(textSearch) || i.Description.Contains(textSearch) || i.UserNameNavigation.EmployeeName.Contains(textSearch));
            }
            inventories = query.Include(i => i.UserNameNavigation)
                         .OrderBy(i => i.InventorySheetId)    
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return inventories;
        }
        //paging inventortSheetDetail
        public List<InventorySheetDetail> getInventorySheetDetailPaging(int pageIndex, int pageSize,int id)
        {
            List<InventorySheetDetail> inventorySheetDetails = new List<InventorySheetDetail>();
            inventorySheetDetails = _context.InventorySheetDetails.Include(i => i.InventorySheet)
                         .Include(i => i.Product)
                         .Include(i=> i.InventorySheet.UserNameNavigation)
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
                 query.Where(s => s.Branch.Contains(textSearch) || s.Address.Contains(textSearch) || s.CompanyName.Contains(textSearch) || s.Phone.Contains(textSearch)
                            || s.City.Contains(textSearch) || s.Region.Contains(textSearch));
            }
            suppliers = query.OrderBy(i => i.SupplierId)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return suppliers;
        }
    }
}
