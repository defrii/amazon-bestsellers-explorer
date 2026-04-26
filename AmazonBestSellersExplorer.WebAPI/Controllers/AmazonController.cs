using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Services.API;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AmazonController : ControllerBase
	{
		private readonly IAmazonApiService _amazonApiService;

		public AmazonController(IAmazonApiService amazonApiService)
		{
			_amazonApiService = amazonApiService;
		}

		// GET: api/amazon/best-sellers
		[HttpGet("best-sellers")]
		public async Task<IActionResult> GetBestSellers()
		{
			try
			{
				var result = await _amazonApiService.GetBestSellersAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return Problem(detail: ex.Message, statusCode: 500);
			}
		}
	}
}
