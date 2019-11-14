using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchMakingController : ControllerBase
    {
       
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetMatch([FromForm] IEnumerable<Guid> userIds, [FromForm] string deviation, [FromServices] MatchCreator creator)
        {
            var dev = double.Parse(deviation, CultureInfo.InvariantCulture);
            return new OkObjectResult(await creator.MakeMatchAsync(userIds, 2, dev));
        }
    }
}