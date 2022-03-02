using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Dapper;

namespace MediaServer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }
        public string M3u8FileDir { get { return Global.M3u8FileDir; } }
        public List<Tuple<string,string>> VideoStreamList { get; private set; }
        public void OnGet()
        {
            List<dynamic> videoStreamList;
            using (var connection = new SqliteConnection($"Data Source={Global.DbFileName}"))
            {
                videoStreamList = connection.Query<dynamic>($"SELECT StreamId,Title FROM MediaStream WHERE Stop=0;").ToList();
            }
            VideoStreamList = new List<Tuple<string, string>>();
            foreach (var videoStream in videoStreamList)
            {
                MediaStreamManager mediaStreamManager = new MediaStreamManager(_env);
                if (mediaStreamManager.IsRuning(videoStream.StreamId) || mediaStreamManager.Start(videoStream.StreamId) == true)
                {
                    VideoStreamList.Add(new Tuple<string, string>(videoStream.StreamId, videoStream.Title));
                }
            }
            //StreamId = "12d2b055";
        }
    }
}