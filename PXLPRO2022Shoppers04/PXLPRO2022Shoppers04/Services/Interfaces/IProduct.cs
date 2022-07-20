using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Models.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Services.Interfaces
{
    public interface IProduct
    {
        List<Product> GetAll();
        Task<Product> GetByID(long id);
        Task<Mouse> GetMouseByID(long id);
        Task<Keyboard> GetKeyboardByID(long id);
        Task<MousePad> GetMousePadByID(long id);
        Task Update(Product product);
        Task Delete(long id);
        List<ProductCategory> ProductCategories();
        Task AddMouse(Mouse mouse, int stock);
        Task AddKeyboard(Keyboard keyboard, int stock);
        Task AddMousePad(MousePad mousepad, int stock);
        Task UpdateMouse(Mouse mouse, int stock);
        Task UpdateKeyboard(Keyboard keyboard, int stock);
        Task UpdateMousePad(MousePad mousepad, int stock);
    }
}
