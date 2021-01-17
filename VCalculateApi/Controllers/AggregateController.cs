using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VCalculateApi.Models;
using System.ComponentModel.DataAnnotations;
using VCalculateApi.Interfaces;
using System.Threading.Tasks;

namespace VCalculateApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggregateController: ControllerBase
    {
        private readonly ILogger<AggregateController> _logger;
        private readonly IStorageClient _storageClient;

        public AggregateController(ILogger<AggregateController> logger, IStorageClient storageClient)
        {
            _logger = logger;
            _storageClient = storageClient;
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult<Aggregate>> Get([FromRoute, Required] string name, [FromQuery] int? since, [FromQuery] int? to)
        {
            var (sum, avg) = await _storageClient.RetrieveData(name, since, to);
            return Ok(new Aggregate {sum = sum, avg = avg});
        }
    }
}