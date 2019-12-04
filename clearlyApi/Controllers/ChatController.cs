using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;
using clearlyApi.Services.Chat;
using clearlyApi.Services.Chat.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Utils;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {

        private ApplicationContext _dbContext { get; set; }
        private WebSocketHandler _webSocketHandler { get; set; }

        public ChatController(ApplicationContext dbContext, WebSocketHandler webSocketHandler)
        {
            this._dbContext = dbContext;
            _webSocketHandler = webSocketHandler;
        }

        [Authorize]
        [HttpPost("sendPhoto")]
        public async Task<IActionResult> SendPhoto(IFormFile file)
        {
            var user = _dbContext.Users
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

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpPost("adminSendPhoto/{toLogin}")]
        public async Task<IActionResult> AdminSendPhoto(IFormFile file, string toLogin)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null || user.UserType != Enums.UserType.Admin)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var toUser = _dbContext.Users
                .FirstOrDefault(x => x.Login == toLogin);
            if(toUser == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "To User not found"
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
                UserId = toUser.Id,
                AdminId = user.Id,
                IsFromAdmin = true
            };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO<string>(message) { Data = fileName});


            var agePickerMessage = new Message
            {
                Type = Enums.MessageType.AgePicker,
                Created = DateTime.UtcNow,
                UserId = toUser.Id,
                AdminId = user.Id,
                IsFromAdmin = true
            };
            _dbContext.Messages.Add(agePickerMessage);
            _dbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO<string>(agePickerMessage));

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("getMessages")]
        public IActionResult GetMessages(
            [FromQuery(Name = "pageNumber")] int pageNumber,
            [FromQuery(Name = "pageSize")] int pageSize
            )
        {
            var user = _dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var messages = _dbContext.Messages
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
            var user = _dbContext.Users
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
            };
            if (user.UserType == Enums.UserType.Admin)
            {
                var toUser = _dbContext.Users
                .FirstOrDefault(x => x.Login == request.ToUserLogin);

                if(toUser == null)
                    return Json(new BaseResponse
                    {
                        Status = false,
                        Message = "User not found"
                    });

                message.AdminId = user.Id;
                message.UserId = toUser.Id;
            }
            else
                message.UserId = user.Id;

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            SendMessageSocket(user.Login, new MessageDTO<string>(message) { Data = message.Content});

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("setAge")]
        public IActionResult SetAge([FromQuery(Name = "age")] string ageInterval)
        {
            var user = _dbContext.Users
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

        [Authorize]
        [HttpGet("setPayType/{type}")]
        public IActionResult SetPayType(PayType type)
        {
            var user = _dbContext.Users
               .Include(u => u.Person)
               .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var orderExpired = _dbContext.Orders.FirstOrDefault(x => x.UserId == user.Id && x.Status == OrderStatus.Request);

            _dbContext.Orders.Remove(orderExpired);

            var order = new Order()
            {
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Status = type == PayType.Cash ? OrderStatus.Cash : OrderStatus.Request
            };

            _dbContext.Orders.Add(order);

            _dbContext.SaveChanges();

            var lastMessage = _dbContext.Messages
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(m => m.Created)
                .Include(x => x.Admin)
                .FirstOrDefault();

            var packages = _dbContext.Packages.Take(3).ToList();

            var message = new Message
            {
                Type = Enums.MessageType.PackagesPicker,
                Content = "",
                IsFromAdmin = true,
                AdminId = lastMessage.AdminId
            };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var messageDTO = new MessageDTO<PackagesList>(message)
            {
                Data = new PackagesList()
                {
                    OrderId = order.Id,
                    Packages = packages.Select(x => new PackageDTOResponse(x)).ToList()
                }
            };

            SendMessageSocket(lastMessage.Admin.Login, messageDTO);

            return Json(new BaseResponse());
        }

        private async Task SendMessageSocket<T>(string login, MessageDTO<T> message)
        {
            await _webSocketHandler.SendMessageAsync(
                    login,
                    JsonConvert.SerializeObject(message)
                    );
        }
    }
}
