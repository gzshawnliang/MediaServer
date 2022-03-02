using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Dapper;

namespace MediaServer.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }
        public List<MediaStream> VideoStreamList { get; private set; }
        public void OnGet()
        {
            using (var connection = new SqliteConnection($"Data Source={Global.DbFileName}"))
            {
                VideoStreamList = connection.Query<MediaStream>($"SELECT * FROM MediaStream;").ToList();
            }
        }
    }
}