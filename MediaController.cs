using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaServer
{
    [Route("[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public MediaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("Run")]
        public JsonResult Run(MediaStream mediaStream)
        {
            MediaStreamManager msm=new MediaStreamManager(_env);
            //if(string.IsNullOrEmpty(mediaStream.StreamId))
            //    mediaStream.StreamId = Guid.NewGuid().ToString().Split("-").First();
            if (msm.Start(mediaStream.StreamId))
                return new JsonResult(new { 
                    file = $"{Global.M3u8FileDir}/{mediaStream.StreamId}/index.m3u8" });

            return new JsonResult(new { file = "" });
        }

        //todo:
        [HttpPost("StopTest")]
        public JsonResult StopTest(MediaStream mediaStream)
        {
            return new JsonResult(new { file = "" });
        }

        [HttpGet]
        public IEnumerable<dynamic> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new 
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Random.Shared.Next(-20, 55)
            })
            .ToArray();
        }
    }
}
