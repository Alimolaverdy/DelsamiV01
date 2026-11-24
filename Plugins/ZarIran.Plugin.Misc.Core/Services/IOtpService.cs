using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Domain;

namespace ZarIran.Plugin.Misc.Core.Services
{
    public interface IOtpService
    {
        Task Insert(OTP otp);

        Task Update(OTP otp);

        Task<OTP> GetOtpByUsernameAsync(string username);
    }
}
