using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using Compositional.Composer;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public partial class LoginForm : Form
    {
        [ComponentPlug]
        public static IComposer Composer { get; set; }

        public static string OathToken;
//        public static readonly string BaseAddress = "http://localhost:60515";
        public static readonly string BaseAddress = "https://www.haftdong.com/";
        public static readonly string LoginUrlPrefix = "/token";
        public static readonly string UrlPrefix = "/api/web";

        public LoginForm()
        {
            if(!string.IsNullOrEmpty(Properties.Settings.Default.Token))
                return;

            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent("grant_type=password&username=" + txtUserName.Text + "&password=" + txtPassWord.Text, Encoding.UTF8, "application/json");
            var response = client.PostAsync(LoginUrlPrefix, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                JsonObject jsonBody = JsonObject.Parse(body);
                OathToken = jsonBody["access_token"];

                if (chbRememberPassword.Checked)
                {
                    Properties.Settings.Default.UserName = txtUserName.Text;
                    Properties.Settings.Default.Token = OathToken;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.Reset();
                }

                Hide();
                var mainForm = new MainForm();
                Composer.InitializePlugs(mainForm);
                mainForm.ShowDialog();
            }

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
