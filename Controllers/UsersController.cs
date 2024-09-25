using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsersService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> Users = new List<User>();

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            // Проверка уникальности email
            if (Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("A user with this email already exists.");
            }

            // Создание пользователя с сохранением пароля
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password // Сохранение пароля без хеширования
            };

            Users.Add(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { user.Id, user.Name, user.Email });
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Возвращаем данные пользователя без пароля для безопасности
            return Ok(new { user.Id, user.Name, user.Email });
        }
    }
}
