using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Dto;
using Microsoft.Extensions.Configuration;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public class ExampleAmazonApiService : AmazonApiServiceBase
    {
        private readonly IConfiguration _configuration;

        public ExampleAmazonApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task<IEnumerable<ProductDto>> GetBestSellersAsync()
        {
            var exampleFilePath = _configuration["Amazon:ExampleFilePath"];
            if (string.IsNullOrWhiteSpace(exampleFilePath))
            {
                throw new InvalidOperationException("Amazon:ExampleFilePath nie został skonfigurowany.");
            }

            var exampleFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", exampleFilePath);
            if (!File.Exists(exampleFileFullPath))
            {
                throw new FileNotFoundException($"Nie znaleziono pliku przykładowego: {exampleFileFullPath}");
            }

            var content = await File.ReadAllTextAsync(exampleFileFullPath);
            return ParseAmazonResponse(content);
        }
    }
}
