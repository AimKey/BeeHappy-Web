using Microsoft.Extensions.Configuration;

namespace Services.Implementations;

public class WebMetaService
{
    private readonly IConfiguration _configuration;

    public WebMetaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetWebExtensionDownloadUrl()
    {
        return _configuration["WebMeta:ExtensionDownloadLink"] ?? "https://github.com/AimKey/BeeHappy-Extension";
    }
}