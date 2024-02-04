using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore;
using PriceTrackerService.DBImport;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceTrackerService.Services
{
    public class PriceSubscriptionService
    {
        private readonly PriceTrackerDbContext _context;

        public PriceSubscriptionService(PriceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSubscribedAsync(string advLink, string email)
        {
            if (await _context.PriceSubscriptions.AnyAsync(x => x.Email.Equals(email)) && await _context.PriceSubscriptions.AnyAsync(x => x.AdvLink.Equals(advLink)))
                throw new Exception("Пользователь уже подписан");
            else
            {
                var subscription = new PriceSubscription
                {
                    AdvLink = advLink,
                    Email = email
                };

                _context.PriceSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<int>> GetActualPricesAsync(string email)
        {
            try
            {
                var subscriptions = await _context.PriceSubscriptions
                            .Where(s => s.Email == email)
                            .ToListAsync();
                var prices = new List<int>();

                foreach (var subscription in subscriptions)
                {
                    var html = await GetHtmlFromAdvLink(subscription.AdvLink);
                    var price = ParsePriceFromHtml(html);

                    if (price != -1)
                        prices.Add(price);
                }

                return prices;
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        private async Task<string> GetHtmlFromAdvLink(string advLink)
        {
            var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(advLink);
        }

        private int ParsePriceFromHtml(string html)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var priceElement = document.QuerySelector(".flat-prices__block-current");

            if (priceElement != null)
            {
                var priceText = priceElement.TextContent;

                var cleanPriceText = priceText.Replace(" ", "").Replace("₽", "");

                if (int.TryParse(cleanPriceText, out var priceValue))
                    return priceValue;
            }

            return -1;
        }
    }
}
