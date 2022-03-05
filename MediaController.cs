using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Dapper;

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
            if (msm.IsRuning(mediaStream.StreamId) || msm.Start(mediaStream.StreamId))
                return new JsonResult(new { 
                    file = $"{Global.M3u8FileDir}/{mediaStream.StreamId}/index.m3u8" });

            return new JsonResult(new { file = "" });
        }

        [HttpPost("DeleteMediaStream")]
        public JsonResult DeleteMediaStream(MediaStream mediaStream)
        {
            //stop before delete
            MediaStreamManager msm = new MediaStreamManager(_env);
            msm.Stop(mediaStream.StreamId);

            int result = 0;
            using (var connection = new SqliteConnection($"Data Source={Global.DbFileName}"))
                result = connection.Execute($"DELETE FROM MediaStream WHERE StreamId = '{mediaStream.StreamId}';");

            if (result > 0)
            {
                return new JsonResult(new { result = "true" });
            }

            return new JsonResult(new { result = "false" }); 
        }
    }
}
