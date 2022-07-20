using Newtonsoft.Json;
using PXLPRO2022Shoppers04_StockAPI.Models;
using PXLPRO2022Shoppers04_StockAPI.ViewModel;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Helpers
{
    public class APIHelper
    {
        public static async Task<Stock> GetStockOfProduct(Guid guid)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:44300/api/stock/{guid}");
            string res = response.Content.ReadAsStringAsync().Result;
            var stock = JsonConvert.DeserializeObject<Stock>(res);
            return stock;
        }
        public static async Task<Guid> CreateSSN(Guid guid, int amount)
        {
            //Api call to create
            using var client = new HttpClient();
            var stock = new Stock();
            stock.SSN = guid;
            stock.Amount = amount;

            var json = JsonConvert.SerializeObject(stock);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:44300/api/stock", data);
            string res = response.Content.ReadAsStringAsync().Result;

            //Error handling
            var errorMsgViewModel = JsonConvert.DeserializeObject<ErrorMessageViewModel>(res);
            if (errorMsgViewModel.ErrorCode != 0) await CreateSSN(Guid.NewGuid(), amount);
            return guid;
        }

        public static async Task<Stock> UpdateStock(Guid guid, int amount)
        {
            using var client = new HttpClient();
            Stock stock = new Stock
            {
                SSN = guid,
                Amount = amount
            };
            var json = JsonConvert.SerializeObject(stock);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:44300/api/stock", data);
            string res = response.Content.ReadAsStringAsync().Result;
            return stock;
        }

        public static async Task<bool> DecreaseStock(Guid guid, int amount)
        {
            bool enoughStock = false;
            var stock = await GetStockOfProduct(guid);
            using var client = new HttpClient();
            stock.SSN = guid;
            stock.Amount -= amount;

            if (stock.Amount < 0)
            {
                //Don't allow user to buy this item.
                enoughStock = false;
            }
            else if (stock.Amount == 0 || stock.Amount >= amount)
            {
                enoughStock = true;
                var json2 = JsonConvert.SerializeObject(stock);
                var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                var response3 = await client.PutAsync($"https://localhost:44300/api/stock", data2);
                string res3 = response3.Content.ReadAsStringAsync().Result;
            }

            return enoughStock;
        }

        public static async Task<Guid> IncreaseStock(Guid guid, int amount)
        {
            var stock = await GetStockOfProduct(guid);
            using var client = new HttpClient();
            stock.SSN = guid;
            stock.Amount += amount;

            var json2 = JsonConvert.SerializeObject(stock);
            var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
            var response3 = await client.PutAsync($"https://localhost:44300/api/stock", data2);
            string res3 = response3.Content.ReadAsStringAsync().Result;
            return guid;
        }

        public static async Task<Guid> DeleteStock(Guid guid)
        {
            using var client = new HttpClient();
            var stock = new Stock();
            stock.SSN = guid;
            var response2 = await client.DeleteAsync($"https://localhost:44300/api/stock/{stock.SSN}");
            string res2 = response2.Content.ReadAsStringAsync().Result;
            return guid;
        }
    }
}
