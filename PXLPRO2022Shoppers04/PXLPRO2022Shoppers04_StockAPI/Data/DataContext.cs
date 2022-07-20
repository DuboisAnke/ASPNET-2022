using Microsoft.EntityFrameworkCore;

namespace PXLPRO2022Shoppers04_StockAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public DbSet<PXLPRO2022Shoppers04_StockAPI.Models.Stock> Stock { get; set; }
        public DbSet<PXLPRO2022Shoppers04_StockAPI.Models.StockLog> StockLog { get; set; }
    }
}
