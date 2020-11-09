using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Biovation.Tools.UserAdapter
{
    public partial class UserAdaptationForm : Form
    {
        private RestClient _restClient;


        public UserAdaptationForm()
        {
            InitializeComponent();
            _restClient = (RestClient)new RestClient("http://localhost:9038/biovation/api").UseSerializer(() => new RestRequestJsonSerializer());
        }


        private void BiovationConnectionButton_Click(object sender, EventArgs e)
        {
            var biovationServerAddress = BiovationServerAddressTextBox.Text.Trim().ToLower().Replace("http://", "")
                .Split(":").First();
            var parseResult = int.TryParse(BiovationServerPortTextBox.Text.Trim(), NumberStyles.Number,
                CultureInfo.InvariantCulture, out var biovationServerPort);

            if (!parseResult || biovationServerPort > 51212 || biovationServerPort < 100)
            {
                MessageBox.Show(@"Invalid port number", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _restClient = (RestClient)new RestClient($"http://{biovationServerAddress}:{biovationServerPort}/biovation/api").UseSerializer(() => new RestRequestJsonSerializer());

            var restRequest = new RestRequest("Device/Devices", Method.GET);
            var result = _restClient.Execute<List<DeviceBasicInfo>>(restRequest);
            if (result.IsSuccessful && result.StatusCode == HttpStatusCode.OK)
            {
                var bindingSource = new BindingSource { DataSource = result.Data };
                BiovationDeviceListComboBox.DataSource = bindingSource.DataSource;
                BiovationDeviceListComboBox.DisplayMember = "Name";
                BiovationDeviceListComboBox.ValueMember = "DeviceId";
            }
            else
            {
                MessageBox.Show(
                    @"Could not connect to biovation service. Wrong address is provided or the service is not available.",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
