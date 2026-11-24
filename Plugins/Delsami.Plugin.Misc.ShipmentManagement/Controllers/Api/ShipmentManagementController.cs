using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.ShipmentManagement.Models;
using ZarIran.Plugin.Misc.ShipmentManagement.Models.Apis;
using ZarIran.Plugin.Misc.ShipmentManagement.Services;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payment.Core.Events;
using Nop.Core.Events;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Models.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Controllers.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/shipment")]
    [ApiVersion("1")]

    public class ShipmentManagementController : ControllerBase
    {
        #region Fields

        private readonly IShipmentDeliveryService _shipmentDeliveryService;
        private readonly IHttpContextAccessor _httpContextAcceossor;
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public ShipmentManagementController(IShipmentDeliveryService shipmentDeliveryService,
                                            IHttpContextAccessor httpContextAcceossor,
                                            ILogger logger,
                                            IEventPublisher eventPublisher)
        {
            _shipmentDeliveryService = shipmentDeliveryService;
            _httpContextAcceossor = httpContextAcceossor;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        [HttpGet("ValidateDeliveryCode")]
        public async Task<IActionResult> ValidateDeliveryCode(string shipmentCode, int deliveryCode)
        {
            var result = await _shipmentDeliveryService.ValidateDeliveryCodeAsync(shipmentCode, deliveryCode);

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpGet("CheckDeliveryCode")]
        public async Task<IActionResult> CheckDeliveryCode(string shipmentCode, int deliveryCode)
        {
            var result = await _shipmentDeliveryService.CheckDeliveryCodeAsync(shipmentCode, deliveryCode);

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpGet("CheckDeliveryCode")]
        [ApiVersion("2")]
        public async Task<IActionResult> CheckDeliveryCodeV2(string shipmentCode, int deliveryCode)
        {
            var result = await _shipmentDeliveryService.CheckDeliveryCodeAsync(shipmentCode, deliveryCode);

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpPut("Shipped")]
        public async Task<IActionResult> Shipped([FromBody] ShippedRequestModel request)
        {
            var result = await _shipmentDeliveryService.ShippedAsync(request.ShipmentCode);

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpPost("Payment")]
        public async Task<IActionResult> Payment([FromBody] InsertPaymentRequestModel request)
        {
            var result = await _shipmentDeliveryService.InsertPaymentAsync(request);

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpGet("OrderDetails/{shipmentCode}")]
        public async Task<IActionResult> OrderDetails(string shipmentCode)
        {
            if (!UserIsAutneticated())
                return Unauthorized();

            var (result, errorMessage) = await _shipmentDeliveryService.GetOrderDetailsByShipmentIdAsync(shipmentCode);

            var isSucceed = result != null;
            return Ok(new ApiResult<ZarIranOrderDetailModel>(isSucceed, errorMessage, result));
        }

        [HttpGet("OrderDetails/{shipmentCode}")]
        [ApiVersion("2")]
        public async Task<IActionResult> OrderDetailsV2(string shipmentCode)
        {
            if (!UserIsAutneticated())
                return Unauthorized();

            var (result, errorMessage) = await _shipmentDeliveryService.GetOrderDetailsByShipmentIdAsync(shipmentCode);

            var isSucceed = result != null;
            return Ok(new ApiResult<ZarIranOrderDetailModel>(isSucceed, errorMessage, result));
        }

        [HttpPost("CODPayment")]
        public async Task<IActionResult> CheckOrMoneyPayment([FromBody] InsertCODPaymentRequestModel request)
        {
            await _logger.InformationAsync(Newtonsoft.Json.JsonConvert.SerializeObject(request));

            try
            {
                Request.EnableBuffering();

                // Read the request body
                using var reader = new StreamReader(
                    Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();

                // Reset the position so the next middleware can read it
                Request.Body.Position = 0;

                // Log the request body
                await _logger.InformationAsync($"Request Body: {body}");

            }
            catch
            {
            }

            if (!UserIsAutneticated())
                return Unauthorized();

            var validationResult = await _shipmentDeliveryService.ValidateCODDeliveryCodeAsync(request.HasDeliveryCode, request.DevliveryCode, request.ShipmentCode);

            if (!validationResult.Result)
            {
                return BadRequest(new ApiResult(false, validationResult.ErrorMessage));
            }
            var result = await _shipmentDeliveryService.InsertCODPaymentAsync(request);

            if (result.Payment is not null)
            {
                List<Payment.Core.Domain.Payment> payments = new() { result.Payment };

                await _eventPublisher.PublishAsync(new CashOnDeliveryPaymentsReceivedEvent(payments));
            }

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        [HttpPost("CODPayment")]
        [ApiVersion("2")]
        public async Task<IActionResult> CheckOrMoneyPaymentV2([FromBody] InsertCODPaymentsRequestModel request)
        {
            await _logger.InformationAsync(Newtonsoft.Json.JsonConvert.SerializeObject(request));

            try
            {
                Request.EnableBuffering();

                // Read the request body
                using var reader = new StreamReader(
                    Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();

                // Reset the position so the next middleware can read it
                Request.Body.Position = 0;

                // Log the request body
                await _logger.InformationAsync($"Request Body: {body}");

            }
            catch (Exception ex)
            {
            }

            if (!UserIsAutneticated())
                return Unauthorized();

            var validationResult = await _shipmentDeliveryService.ValidateCODDeliveryCodeAsync(request.HasDeliveryCode, request.DevliveryCode, request.ShipmentCode);

            if (!validationResult.Result)
            {
                return BadRequest(new ApiResult(false, validationResult.ErrorMessage));
            }

            var result = await _shipmentDeliveryService.InsertCODPaymentsAsync(request);

            if (result.Payments.Any())
                await _eventPublisher.PublishAsync(new CashOnDeliveryPaymentsReceivedEvent(result.Payments));

            return Ok(new ApiResult(result.Result, result.ErrorMessage));
        }

        private bool UserIsAutneticated()
        {
            var bearerToken = "4499d81d-5d3c-4c85-be0a-e3ed407d17a9";

            try
            {
                var authHeader = _httpContextAcceossor.HttpContext.Request.Headers["Authorization"].ToString();

                return bearerToken.Equals(authHeader.Substring("Bearer ".Length));
            }
            catch
            {
            }
            return false;
        }

        #endregion
    }
}
