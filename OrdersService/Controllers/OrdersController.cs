using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace OrdersService.Controllers
{

    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private static readonly List<Order> Orders = new List<Order>();
        private readonly HttpClient _httpClient;
        private readonly string _usersServiceUrl;

        public OrdersController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _usersServiceUrl = configuration["UsersService:BaseUrl"]; // Получаем URL из конфигурации

        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // Запрос к сервису пользователей
            var response = await _httpClient.GetAsync($"{_usersServiceUrl}/{order.UserId}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "User service is unavailable.");
            }

            var use =  response.Content.ReadFromJsonAsync<User>();
            var user = use.Result;
            order.Id = Guid.NewGuid().ToString();
            order.UserName = user.Name;
            Orders.Add(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(string id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
