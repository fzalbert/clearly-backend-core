using System;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;
using SmsSender;

namespace clearlyApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private ApplicationContext dbContext { get; set; }
        private ISmsProvider smsProvider { get; set; }

        public AuthService(ApplicationContext applicationContext, ISmsProvider smsProvider)
        {
            dbContext = applicationContext;
            this.smsProvider = smsProvider;
        }

        public BaseResponse Auth(User user)
        { 
            var loginType = user.LoginType;

            Random rnd = new Random();
            int randomNumber = rnd.Next(100000, 999999);
            try
            {
                switch (loginType)
                {
                    case LoginType.Email:
                        var toEmail = new String[1];
                        toEmail[0] = user.Login;
                        smsProvider.SendByEmail(toEmail, randomNumber.ToString());
                        break;
                    case LoginType.Phone:
                        smsProvider.Send(null, user.Login, randomNumber.ToString());
                        break;
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException is System.IO.IOException)
                {

                }
            }
            ActivationCode code = new ActivationCode
            {
                User = user,
                Created = DateTime.UtcNow,
                Code = randomNumber.ToString()
            };
            dbContext.ActivationCodes.Add(code);
            dbContext.SaveChanges();

            return new BaseResponse();
        }

        public BaseResponse Register(string login, LoginType loginType)
        {

            Random rnd = new Random();
            int randomNumber = rnd.Next(100000, 999999);

            try
            {
                switch (loginType)
                {
                    case LoginType.Email:
                        var toEmail = new String[1];
                        toEmail[0] = login;
                        smsProvider.SendByEmail(toEmail, randomNumber.ToString());
                        break;
                    case LoginType.Phone:
                        smsProvider.Send(null, login, randomNumber.ToString());
                        break;
                }
            }
            catch(System.Exception ex)
            {
                if(ex.InnerException is System.IO.IOException)
                {

                }
            }

            var user = new User()
            {
                LoginType = loginType,
                Login = login,
                Created = DateTime.UtcNow
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var person = new Person()
            {
                UserId = user.Id
            };

            switch (loginType)
            {
                case LoginType.Email:
                    person.Email = login;
                    break;
                case LoginType.Phone:
                    person.Phone = login;
                    break;
            }

            dbContext.Persons.Add(person);
            dbContext.SaveChanges();

            ActivationCode code = new ActivationCode
            {
                User = user,
                Created = DateTime.UtcNow,
                Code = randomNumber.ToString()
            };
            dbContext.ActivationCodes.Add(code);
            dbContext.SaveChanges();

            return new BaseResponse();
        }
    }
}
