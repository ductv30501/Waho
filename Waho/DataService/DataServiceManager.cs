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
    }
}
