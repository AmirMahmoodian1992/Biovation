using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Microsoft.Win32;
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
        private int _biovationServerPort = 9038;
        private string _biovationServerAddress = "localhost";
        private string _biovationServerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyQ29kZSI6IjEyMzQ1Njc4OSIsInVuaXF1ZUlkIjoiMTIzNDU2Nzg5IiwianRpIjoiNDQ0NGY2Y2ItNWRlZC00ZWQwLWI3ZWQtODAzMzBjOTdmZjI1IiwiZXhwIjoxNjM3NTcwODMwfQ.bvALo-ieWeYreBRX4TE4aarV8LWPX5WkHnlQ5N9PqSs";
        private readonly Dictionary<uint, uint> _userCodeMappings = new Dictionary<uint, uint>();

        public UserAdaptationForm()
        {
            InitializeComponent();
            _restClient = (RestClient)new RestClient($"http://{_biovationServerAddress}:{_biovationServerPort}/biovation/api").UseSerializer(() => new RestRequestJsonSerializer());
        }


        private void BiovationConnectionButton_Click(object sender, EventArgs e)
        {
            _biovationServerAddress = BiovationServerAddressTextBox.Text.Trim().ToLower().Replace("http://", "")
                .Split(":").First();
            var parseResult = int.TryParse(BiovationServerPortTextBox.Text.Trim(), NumberStyles.Number,
                CultureInfo.InvariantCulture, out _biovationServerPort);

            if (!parseResult || _biovationServerPort > 51212 || _biovationServerPort < 100)
            {
                MessageBox.Show(@"Invalid port number", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _restClient = (RestClient)new RestClient($"http://{_biovationServerAddress}:{_biovationServerPort}/biovation/api").UseSerializer(() => new RestRequestJsonSerializer());

            var restRequest = new RestRequest("v2/Device/OnlineDevices", Method.GET);
            restRequest.AddHeader("Authorization", _biovationServerToken);
            var result = _restClient.Execute<List<DeviceBasicInfo>>(restRequest);
            if (result.IsSuccessful && result.StatusCode == HttpStatusCode.OK)
            {
                var bindingSource = new BindingSource { DataSource = result.Data };
                BiovationDeviceListComboBox.DataSource = bindingSource.DataSource;
                BiovationDeviceListComboBox.DisplayMember = "Name";
                BiovationDeviceListComboBox.ValueMember = "DeviceId";

                BiovationDeviceListComboBox.Enabled = true;
                BiovationDeviceLabel.Enabled = true;
                DownloadSampleButton.Enabled = true;
                PickUserAdaptationFileButton.Enabled = true;
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
            try
            {
                if (BiovationDeviceListComboBox.SelectedItem is null || (int)BiovationDeviceListComboBox.SelectedValue < 1)
                {
                    MessageBox.Show(@"لطفا یک دستگاه را انتخاب کنید و مجدد تلاش کنید", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"لطفا یک دستگاه را انتخاب کنید و مجدد تلاش کنید", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

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

                    var registrySoftwareKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes");
                    var oleDbKeys = registrySoftwareKey?.GetSubKeyNames().Where(subKey => subKey.Contains("Microsoft.ACE.OLEDB"));

                    if (oleDbKeys is null)
                    {
                        MessageBox.Show(
                            @"No OleDb found on device, please install appropriate version of OleDb (12.0, 14.0 or newer)");
                        return;
                    }

                    foreach (var oleDbKey in oleDbKeys)
                    {

                        try
                        {
                            var connectionString =
                                $"Provider={oleDbKey};Data Source='{filePath}'; Extended Properties='Excel 12.0;IMEX=1;';";

                            var dataSet = new DataSet();
                            var adapter = new OleDbDataAdapter("SELECT * FROM [sheet1$]", connectionString);
                            adapter.Fill(dataSet, "UserCodeMappings");
                            var data = dataSet.Tables["UserCodeMappings"];
                            _userCodeMappings.Clear();

                            foreach (DataRow row in data.Rows)
                            {
                                var parseResult = uint.TryParse(row["OldUserCardNumber"].ToString(), NumberStyles.Integer,
                                    CultureInfo.InvariantCulture,
                                    out var oldUserCode);

                                if (!parseResult)
                                    continue;

                                parseResult = uint.TryParse(row["NewUserCardNumber"].ToString(), NumberStyles.Integer,
                                    CultureInfo.InvariantCulture,
                                    out var newUserCode);

                                if (!parseResult)
                                    continue;

                                _userCodeMappings.Add(oldUserCode, newUserCode);
                            }

                            break;
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }

                    StartProcessButton.Enabled = true;
                    MessageBox.Show(@"فایل تغییرات کد کاربر ها با موفقیت بارگیری شد.", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, @"Error in reading the table data", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void UserAdaptationForm_Load(object sender, EventArgs e)
        {

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private async void StartProcessButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = BiovationDeviceListComboBox.SelectedItem is null ? new DeviceBasicInfo() : (DeviceBasicInfo)BiovationDeviceListComboBox.SelectedItem;
                var selectedDeviceId = selectedItem.DeviceId;
                if (selectedDeviceId <= 0)
                {
                    MessageBox.Show(@"دستگاه انتخاب شده صحیح نیست، لطفا مجددا تلاش نمایید.");
                    return;
                }

                var restRequest = new RestRequest("/v2/Device/{id}/UserAdaptation", Method.POST);
                restRequest.AddUrlSegment("id", selectedDeviceId.ToString());
                restRequest.AddJsonBody(_userCodeMappings);
                restRequest.AddHeader("Authorization", _biovationServerToken);

                var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                if (result.IsSuccessful && result.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show(@"عملیات با موفقیت آغاز شد");
                }
                else
                {
                    MessageBox.Show(
                        @"خطا در عملیات", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    @"خطا در عملیات", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
