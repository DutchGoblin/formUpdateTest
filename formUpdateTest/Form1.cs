using Newtonsoft.Json;
using System.Diagnostics;

namespace formUpdateTest
{
    public partial class Form1 : Form
    {

        public static string CurrentVersion = "1.0.1";
        private string response;
        private List<GitHubRelease> releases;
        private GitHubRelease latestRelease;
        private string latestVersion;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Version: {CurrentVersion}";

        }



        private async Task CheckForUpdates()
        {
            try
            {
                string repositoryOwner = "DutchGoblin";
                string repositoryName = "formUpdateTest";
                string apiUrl = $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/releases";
                latestVersion = "";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    response = await client.GetStringAsync(apiUrl);
                    releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response);
                    latestRelease = releases.FirstOrDefault();

                    if (latestVersion != null)

                    {
                        latestVersion = latestRelease.TagName;

                        if (new Version(latestVersion) > new Version(CurrentVersion))
                        {
                            timer1.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No releases found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                MessageBox.Show($"HTTP error: {httpRequestException.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private async Task DownloadAndInstallUpdate(GitHubRelease release)
        {
            var asset = release.Assets.FirstOrDefault(a => a.DownloadUrl.EndsWith(".exe"));
            if
                (asset == null)
            {
                MessageBox.Show("No executable found in the release.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string tempFilePath = Path.Combine(Path.GetTempPath(), "formUpdateTestNew.exe");
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(asset.DownloadUrl);
                using (var fs = new FileStream(tempFilePath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            MessageBox.Show("Download complete. The application will now update and restart.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Replace the current executable with the new one (may need to use a separate update process)

            string updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "updater.exe");
            Process.Start(updaterPath, $"{tempFilePath} \"{Application.ExecutablePath}\"");
            Application.Exit();
        }

        private class GitHubRelease
        {
            [JsonProperty("tag_name")]
            public string TagName { get; set; }
            [JsonProperty("assets")]
            public List<GitHubAsset> Assets { get; set; }

        }
        private class GitHubAsset
        {
            [JsonProperty("browser_download_url")]
            public string DownloadUrl { get; set; }
        }

        private async void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            if (latestVersion == null) { return; }

            if (new Version(latestVersion) > new Version(CurrentVersion))
            {
                var result = MessageBox.Show("A new update is available. Do you want to download and install it now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    await DownloadAndInstallUpdate(latestRelease);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (toolStripStatusLabel1.ForeColor == Color.Black)
            { 
            toolStripStatusLabel1.ForeColor = Color.Red;
            }
            if (toolStripStatusLabel1.ForeColor == Color.Red)
            { 
            toolStripStatusLabel1.ForeColor= Color.Black;
            }
        }
    }
}
