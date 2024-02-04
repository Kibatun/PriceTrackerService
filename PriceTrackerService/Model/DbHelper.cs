using PriceTrackerService.DBImport;

namespace PriceTrackerService.Model
{
    public class DbHelper
    {
        private PriceTrackerDbContext _context/*EF_DataContext _context*/;

        public DbHelper(PriceTrackerDbContext context)/*(EF_DataContext context)*/
        {
            _context = context;
        }
    }
}
