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
        private int _biovationServerPort = 9038;
        private string _biovationServerAddress = "localhost";
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

            var restRequest = new RestRequest("Device/Devices", Method.GET);
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

        private void StartProcessButton_Click(object sender, EventArgs e)
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
                
                var result = _restClient.Execute<ResultViewModel>(restRequest);

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
