using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspExam.Models;
using AspExam.Data.DTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace AspExam.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<HomeController> logger
    )
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_configuration["Endpoint"]!);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Link()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Link(CreateLinkBody body)
    {
        if (!ModelState.IsValid)
        {
            return View(body);
        }

        try
        {
            string auth = Request.Cookies["AuthToken"] ?? "";
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

            var str = JsonSerializer.Serialize(body);
            _logger.LogInformation($"SENDING CONTENT: {str}");
            var content = new StringContent(str, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/link/create", content);
            _logger.LogInformation($"Recv: {response.Content}");

            ModelState.AddModelError(string.Empty, await response.Content.ReadAsStringAsync());
            return View(body);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Failed to connect to API");
            ModelState.AddModelError(string.Empty, "Unable to connect to API");
            return View(body);
        }
    }
}
