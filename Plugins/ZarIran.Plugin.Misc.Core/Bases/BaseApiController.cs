using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Models.Api;

namespace ZarIran.Plugin.Misc.Core.Bases
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected virtual IActionResult SucceedApiResult()
        {
            return Ok(new ApiResult(true));
        }
    }
}