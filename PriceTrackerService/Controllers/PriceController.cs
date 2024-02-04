using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerService.DBImport;
using PriceTrackerService.Model;
using PriceTrackerService.Services;

namespace PriceTrackerService.Controllers
{
    public class PriceSubscriptionRequest
    {
        public string AdvLink { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Контроллер для отслеживания изменений цен на квартиры.
    /// </summary>
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly PriceSubscriptionService _priceSubscriptionService;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="priceSubscriptionService">Сервис подписок на изменения цен.</param>
        public PriceController( PriceSubscriptionService priceSubscriptionService)
        {
            _priceSubscriptionService = priceSubscriptionService;
        }

        /// <summary>
        /// HTTP POST метод для подписки на изменение цены квартиры.
        /// </summary>
        /// <param name="request">Запрос на подписку.</param>
        /// <returns>Результат операции.</returns>
        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] string adv, string email)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                if (await _priceSubscriptionService.IsSubscribedAsync(adv, email))
                    return Ok(ResponseHandler.GetAppResponse(type, "Вы успешно подписаны на изменения цены."));
                else
                    return BadRequest("Не удалось добавить подписку.");
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        /// <summary>
        /// HTTP GET метод для получения актуальных цен на квартиры для подписанного пользователя.
        /// </summary>
        /// <param name="email">Email пользователя.</param>
        /// <returns>Результат операции.</returns>
        [HttpGet("actual-prices")]
        public async Task<IActionResult> GetActualPrices([FromBody] string email)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                List<int> prices = await _priceSubscriptionService.GetActualPricesAsync(email);
                return Ok(ResponseHandler.GetAppResponse(type, prices));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }
    }
}