using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Dapper;

namespace MediaServer.Pages
{
    public class MediaStreamEditModel : PageModel
    {
        [BindProperty]
        public MediaStream MediaStream { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;

        public MediaStreamEditModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public void OnGet(string streamId)
        {
            System.Diagnostics.Debug.WriteLine(streamId);
            if (!string.IsNullOrEmpty(streamId))
            {
                using (var connection = new SqliteConnection($"Data Source={Global.DbFileName}"))
                {
                    MediaStream = connection.QueryFirst<MediaStream>($"SELECT * FROM MediaStream WHERE StreamId='{streamId}';");
                }
            }
            else
            {
                MediaStream = new MediaStream();
            }
        }

        public IActionResult OnPost()
        {
            System.Diagnostics.Debug.WriteLine(MediaStream.StreamURL);
            /*
                INSERT INTO MediaStream (StreamId, StreamType, StreamURL, Stop, CreateDateTime, FFmpegArg, Title, ProcessId)
                VALUES ('12d2b060', 'RTPS', 'rtsp://200.200.200.140/test6', 0, 20220302105856,
                        '-i {input} -fflags flush_packets -max_delay 2 -hls_flags delete_segments -hls_time 2 -g 30 {output}', '·¿¼ä7',
                        null);             
             */
            ;
            string sql = string.Empty;
            string currStreamId = MediaStream.StreamId;
            if (string.IsNullOrEmpty(currStreamId))
            {
                currStreamId = Guid.NewGuid().ToString().Split("-").First();
                sql += $"INSERT INTO MediaStream (StreamId, StreamType, StreamURL, Stop, CreateDateTime, FFmpegArg, Title, ProcessId)";
                sql += $"VALUES ('{currStreamId}', 'RTPS', '{MediaStream.StreamURL}', {MediaStream.Stop}, {DateTime.Now.ToString("yyyyMMddHHmmss")},";
                sql += $"'{MediaStream.FFmpegArg}', '{MediaStream.Title}',";
                sql += $"null);";
            }
            else
            {
                sql += $"UPDATE MediaStream \n";
                sql += $"SET";
                sql += $"       StreamURL='{MediaStream.StreamURL}',\n";
                sql += $"       Stop={MediaStream.Stop},\n";
                sql += $"       FFmpegArg='{MediaStream.FFmpegArg}',\n";
                sql += $"       Title='{MediaStream.Title}',\n";
                sql += $"       CreateDateTime={DateTime.Now.ToString("yyyyMMddHHmmss")}\n";
                sql += $"WHERE  StreamId = '{currStreamId}'";
            }
            int result = 0;
            using (var connection = new SqliteConnection($"Data Source={Global.DbFileName}"))
            {
                result = connection.Execute(sql);
            }
            if (result > 0)
            {
                MediaStreamManager mediaStreamManager = new MediaStreamManager(_env);
                if (MediaStream.Stop==0)
                {
                    mediaStreamManager.Start(currStreamId);
                }
                else
                {
                    mediaStreamManager.Stop(currStreamId);
                }
                return Page();
                //return Redirect("Admin");
            }

            return null;
        }
    }
}
