using DataBaseContext;
using DataBaseContext.DAO;
using DataBaseContext.Model;
using EmailSender;
using NLog;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OzonPriceChecker
{
    public class OzonPriceChecker
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEmailSender _emailSender;

        private readonly OzonPriceCheckerSecret _ozonPriceCheckerSecret;

        public OzonPriceChecker(OzonPriceCheckerSecret ozonPriceCheckerSecret)
        {
            _ozonPriceCheckerSecret = ozonPriceCheckerSecret;
            _emailSender = new GoogleEmailSender(_ozonPriceCheckerSecret.EmailLogin, _ozonPriceCheckerSecret.EmailPassword);
        }

        public async Task<bool> RunAsync()
        {
            var webPageParser = new WebPageParserByHtmlAgilityPack();

            using (var client = new SqliteContext(_ozonPriceCheckerSecret.SQliteConnectionString))
            {
                client.Database.EnsureCreated();
            }

            var priceHistoryDao = new PriceHistoryDao(_ozonPriceCheckerSecret.SQliteConnectionString);

            var allOzonProducts = await priceHistoryDao.GetAllOzonProductsAsync();
            var historySaveMoment = DateTime.Now;
            var emailBody = new StringBuilder($"Проверка цен OZON: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}<br />");
            foreach (var ozonProduct in allOzonProducts)
            {
                emailBody.Append($"Товар: {ozonProduct.Name} ");
                _logger.Trace($"Load product id = {ozonProduct.Id}, name = {ozonProduct.Name}!");
                var lastPriceEntry = await priceHistoryDao.SelectLastProductPriceAsync(ozonProduct.Id);

                var product = webPageParser.GetOzonProductInfoByLink(ozonProduct.ProductLink);
                if (product == null)
                {
                    emailBody.Append("- получить данные не удалось!");
                    _logger.Error($"Failed load product id = {ozonProduct.Id}, name = {ozonProduct.Name}!");
                    continue;
                }

                var postfix = product.IsAvailable ? string.Empty : "<span style='color:red;'>нет в продаже!</span>";
                var productHistory = MapProductIntoOzonProductHistory(product, historySaveMoment);
                productHistory.Id = await priceHistoryDao.SaveProductHistoryAsync(productHistory);

                ResetConsoleColour();
                Console.ForegroundColor = ConsoleColor.Gray;
                if (lastPriceEntry != null)
                {
                    if (lastPriceEntry.Price > productHistory.Price || lastPriceEntry.PricePremium > productHistory.PricePremium)
                    {
                        ChangeConsoleColourPriceDecrease();
                    }

                    if (lastPriceEntry.Price < productHistory.Price || lastPriceEntry.PricePremium < productHistory.PricePremium)
                    {
                        ChangeConsoleColourPriceIncrease();
                    }
                }
                ResetConsoleColour();

                if (product.PricePremium == 0)
                {
                    emailBody.Append($"цена = <b>{product.Price.ToString("N2")}</b> {postfix}");
                }
                else
                {
                    emailBody.Append($"цена = <b>{product.Price.ToString("N2")}</b>, цена Premium = <b style='color:green;'>{product.PricePremium.ToString("N2")}</b> {postfix}");
                }

                emailBody.Append("<br />");
            }

            var emailMessage = new EmailMessage();
            emailMessage.AddressTo = new MailAddress("kbobovskiy@yandex.ru", "Бобовский Константин");
            emailMessage.Subject = $"Импорт цен Ozon {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";
            emailMessage.Body = emailBody.ToString();
            emailMessage.IsBodyInHtml = true;

            if (!_emailSender.TrySendEmail(emailMessage))
            {
                _logger.Error($"Failed to send email!");
            }

            return true;
        }

        private static OzonProductPriceHistory MapProductIntoOzonProductHistory(Product product, DateTime moment)
        {
            var ozonProductPriceHistory = new OzonProductPriceHistory();
            ozonProductPriceHistory.Id = 0;
            ozonProductPriceHistory.ProductId = product.Id;
            ozonProductPriceHistory.Date = moment;
            ozonProductPriceHistory.OfferName = product.OfferName;
            ozonProductPriceHistory.OfferMessageText = product.OfferMessageText;
            ozonProductPriceHistory.PriceBase = product.PriceBase;
            ozonProductPriceHistory.Price = product.Price;
            ozonProductPriceHistory.Discount = product.Discount;
            ozonProductPriceHistory.PricePremium = product.PricePremium;
            ozonProductPriceHistory.IsAvailable = product.IsAvailable;

            return ozonProductPriceHistory;
        }

        private static void ResetConsoleColour()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void ChangeConsoleColourPriceIncrease()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        private static void ChangeConsoleColourPriceDecrease()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}