using Microsoft.EntityFrameworkCore;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Models.Categories;
using PXLPRO2022Shoppers04.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Services.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;

        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public async Task<Product> GetByID(long id)
        {
            return _context.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.ProductId == id);
        }

        public async Task<Mouse> GetMouseByID(long id)
        {
            return (Mouse)_context.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.ProductId == id);
        }

        public async Task<Keyboard> GetKeyboardByID(long id)
        {
            return (Keyboard)_context.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.ProductId == id);
        }

        public async Task<MousePad> GetMousePadByID(long id)
        {
            return (MousePad)_context.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.ProductId == id);
        }

        public async Task Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var product = _context.Products.Include(x => x.ProductCategory).FirstOrDefault(x => x.ProductId == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                await APIHelper.DeleteStock(product.SSN);
            }
        }

        public async Task AddMouse(Mouse mouse, int stock)
        {
            mouse.ProductCategory = GetProductCategory(mouse.CategoryId);
            var guid = Guid.NewGuid();
            mouse.SSN = guid;
            _context.Products.Add(mouse);
            await _context.SaveChangesAsync();
            await APIHelper.CreateSSN(guid, stock);
        }
        public async Task AddKeyboard(Keyboard keyboard, int stock)
        {
            keyboard.ProductCategory = GetProductCategory(keyboard.CategoryId);
            var guid = Guid.NewGuid();
            keyboard.SSN = guid;
            _context.Products.Add(keyboard);
            await _context.SaveChangesAsync();
            await APIHelper.CreateSSN(guid, stock);
        }
        public async Task AddMousePad(MousePad mousepad, int stock)
        {
            mousepad.ProductCategory = GetProductCategory(mousepad.CategoryId);
            var guid = Guid.NewGuid();
            mousepad.SSN = guid;
            _context.Products.Add(mousepad);
            await _context.SaveChangesAsync();
            await APIHelper.CreateSSN(guid, stock);
        }

        public async Task UpdateMouse(Mouse mouse, int stock)
        {
            mouse.ProductCategory = GetProductCategory(mouse.CategoryId);
            var updatedMouse = _context.Products.Find(mouse.ProductId);
            _context.Products.Update(updatedMouse);
            await APIHelper.UpdateStock(mouse.SSN, stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKeyboard(Keyboard keyboard, int stock)
        {
            keyboard.ProductCategory = GetProductCategory(keyboard.CategoryId);
            var updatedKeyboard = _context.Products.Find(keyboard.ProductId);
            _context.Products.Update(updatedKeyboard);
            await APIHelper.UpdateStock(keyboard.SSN, stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMousePad(MousePad mousepad, int stock)
        {
            mousepad.ProductCategory = GetProductCategory(mousepad.CategoryId);
            var updatedMousePad = _context.Products.Find(mousepad.ProductId);
            _context.Products.Update(updatedMousePad);
            await APIHelper.UpdateStock(mousepad.SSN, stock);
            await _context.SaveChangesAsync();
        }

        public List<ProductCategory> ProductCategories()
        {
            return _context.ProductCategories.ToList();
        }
        public ProductCategory GetProductCategory(int id)
        {
            return _context.ProductCategories.Find(id);
        }


    }
}
