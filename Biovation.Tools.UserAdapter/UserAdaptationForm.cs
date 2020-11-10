using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Biovation.Tools.UserAdapter
{
    public partial class UserAdaptationForm : Form
    {
        private RestClient _restClient;
        private readonly Dictionary<uint, uint> _userCodeMappings = new Dictionary<uint, uint>();

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

        private void PickUserAdaptationFileButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = "BiovationUserAdapterData",
                Filter = @"Excel Spread Sheet (*.xlsx)|*.xlsx",
                Title = @"Open the user mapping file"
            };

            var fileSelectionResult = openFileDialog.ShowDialog();
            if (fileSelectionResult == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog.FileName;

                    var connectionString =
                        $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath}; Extended Properties=Excel 12.0;";

                    var dataSet = new DataSet();
                    var adapter = new OleDbDataAdapter("SELECT * FROM [sheet1$]", connectionString);
                    adapter.Fill(dataSet, "UserCodeMappings");
                    var data = dataSet.Tables["UserCodeMappings"];
                    foreach (DataRow row in data.Rows)
                    {
                        var parseResult = uint.TryParse(row["OldUserCardNumber"].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture,
                            out var oldUserCode);

                        if (!parseResult)
                            continue;

                        parseResult = uint.TryParse(row["NewUserCardNumber"].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture,
                            out var newUserCode);

                        if (!parseResult)
                            continue;

                        _userCodeMappings.Add(oldUserCode, newUserCode);
                    }

                    MessageBox.Show(@"User code mappings loaded successfully.", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, @"Error in reading the table data", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
