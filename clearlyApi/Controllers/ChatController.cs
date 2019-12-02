using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {

        private ApplicationContext dbContext { get; set; }

        public ChatController(ApplicationContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize]
        [HttpPost("photo")]
        public async Task<IActionResult> SendPhoto(IFormFile file)
        {
            var user = dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            if (file == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "File empty"
                });

            string fileName = $"{CryptHelper.CreateMD5(DateTime.Now.ToString())}{Path.GetExtension(file.FileName)}";
            string path = $"{System.IO.Directory.GetCurrentDirectory()}\\Files\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var fileStream = new FileStream(path + fileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var message = new Message
            {
                Type = Enums.MessageType.Photo,
                Content = fileName,
                Created = DateTime.UtcNow,
                UserId = user.Id

            };

            dbContext.Messages.Add(message);
            dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("messages")]
        public IActionResult getMessages(
            [FromQuery(Name = "pageNumber")] int pageNumber,
            [FromQuery(Name = "pageSize")] int pageSize
            )
        {
            var user = dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var messages = dbContext.Messages
                .Where(m => m.UserId == user.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new DataResponse<Message>
            {
                Data = messages
            });
        }




        [Authorize]
        [HttpPost("message")]
        public IActionResult SendMessage([FromBody] MessageRequest request)
        {
            var user = dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            if (request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });

            var message = new Message
            {
                Type = Enums.MessageType.Text,
                Content = request.Text,
                UserId = user.Id

            };

            dbContext.Messages.Add(message);
            dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("setAge")]
        public IActionResult SetAge([FromQuery(Name = "age")] string ageInterval)
        {
            var user = dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            if(String.IsNullOrEmpty(ageInterval))
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "возраст не задан"
                });

            user.Person.Age = ageInterval;

            return Json(new BaseResponse());

        }

    }
}
