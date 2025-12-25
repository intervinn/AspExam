using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AspExam.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AspExam.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AccountController> logger
    )
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_configuration["Endpoint"]!);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Account()
    {
        try
        {
            var cookie = Request.Cookies["AuthToken"] ?? "";
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {cookie}");
            var response = await _httpClient.GetAsync("/api/users/me");
            _logger.LogInformation($"Recv: {response.Content}");

            var data = JsonSerializer.Deserialize<ApiResponse<PartialUser>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            ViewBag.User = data;
        } catch (Exception e)
        {
            _logger.LogError(e, "Failed to get info");
            return RedirectToAction("Login");
        }

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterBody body)
    {
        if (!ModelState.IsValid)
        {
            return View(body);
        }

        try
        {
            _logger.LogInformation($"DATA {body.Email} {body.FirstName} {body.LastName} {body.Password}");
            var str = JsonSerializer.Serialize(body);
            _logger.LogInformation($"SENDING CONTENT: {str}");
            var content = new StringContent(str, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/register", content);
            _logger.LogInformation($"Recv: {response.Content}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, await response.Content.ReadAsStringAsync());
                return View(body);
            }
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Failed to connect to API");
            ModelState.AddModelError(string.Empty, "Unable to connect to API");
            return View(body);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginBody body)
    {
        if (!ModelState.IsValid)
        {
            return View(body);
        }

        try
        {
            var str = JsonSerializer.Serialize(body);
            _logger.LogInformation($"SENDING CONTENT: {str}");
            var content = new StringContent(str, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/login", content);
            _logger.LogInformation($"Recv: {response.Content}");

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (result?.Success == true && result.Data != null)
                {
                    Response.Cookies.Append("AuthToken", result.Data.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(8)
                    });

                    return RedirectToAction("Index", "Home");
                }    
            }

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