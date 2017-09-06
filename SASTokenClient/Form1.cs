using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SASTokenClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static readonly string DownloadPath = Path.GetTempPath();

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "Downloading file..." + Environment.NewLine;
            try
            {

                var apiUrl = $"http://localhost:55662/api/File?jobId={textBox1.Text}";
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.Accept] = "application/json";
                    //sends and http post to the API url
                    string response = client.UploadString(apiUrl, "");
                    var fileResponse = JsonConvert.DeserializeObject<FileResponse>(response);
                    Uri sasUri = new Uri(fileResponse.FileSasUri);
                    var downloadPath = Path.Combine(DownloadPath, fileResponse.FileName);

                    client.DownloadFile(sasUri, downloadPath);
                    textBox2.Text += $"File successfully downloaded to {downloadPath}";

                }
            }
            catch(Exception ex)
            {
                textBox2.Text = ex.ToString();
            }


        }
    }

    public class FileResponse
    {
        public string FileSasUri { get; set; }
        public string FileName { get; set; }
    }
}
