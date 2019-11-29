using System;
using System.Linq;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private ApplicationContext dbContext { get; set; }
        public AccountController(ApplicationContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        [HttpPost("login")]
        public AuthResponse AuthOrRegister([FromBody] AuthRequest request)
        {
            var login = request.Login;
            if (String.IsNullOrEmpty(login))
                return new AuthResponse()
                {
                    Message = "пустой логин",
                    Status = false
                };


            var user = dbContext.Users.FirstOrDefault(x => x.Login == login);

            if(user == null)
            {

            }
            else
            {

            }


            return new AuthResponse();
        }

        [HttpGet]
        public BaseResponse Get()
        {
            return new BaseResponse();
        }
    }
}
