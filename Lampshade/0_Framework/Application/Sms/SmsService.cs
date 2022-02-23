using System;
using System.Collections.Generic;
using System.Linq;
using Kavenegar;
using Kavenegar.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace _0_Framework.Application.Sms
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string number, string message)
        {
            try
            {
                var api = new KavenegarApi(_configuration.GetSection("SmsSecrets")["ApiKey"]);
                var result = api.Send("100047778", number, message);
            }
            catch (ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
            }
            catch (HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
            }
        }
        
    }
}