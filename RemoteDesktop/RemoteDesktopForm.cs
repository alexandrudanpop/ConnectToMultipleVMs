using System;
using System.Windows.Forms;
using MSTSCLib;
using AxMSTSCLib;
using System.Collections.Generic;

namespace RemoteDesktop
{
    public partial class RemoteConnectionForm : Form
    {
        // Used to dinamically add tabs
        private DynamicTabGenerator tabGenerator;

        // Used to validate credentials
        private Validation validation;

        //Restore and store credentials that achieved a successful connection
        private List<ConnectionCredentials> credentialsList;

        private StoreManager storeManager;

        // Timer Object User to Invalidate Connection button for 5 seconds
        Timer connectionTimer;

        // Constructor
        public RemoteConnectionForm()
        {
            InitializeComponent();

            tabGenerator = new DynamicTabGenerator();
            validation = new Validation();
            credentialsList = new List<ConnectionCredentials>();
            storeManager = new StoreManager();

            connectionTimer = new Timer();
            connectionTimer.Interval = 10000;
            connectionTimer.Tick += Timer_Tick;

            txtPassword.PasswordChar = '*';
            WindowState = FormWindowState.Maximized;

            LoadExistingCredentials();
        }

        private void LoadExistingCredentials()
        {
            credentialsList = storeManager.LoadCredentials();

            if (credentialsList.Count != 0)
            {
                txtServer.Text = credentialsList[0].ServerIP;
                txtUserName.Text = credentialsList[0].UserName;
                txtPassword.Text = credentialsList[0].UserPassword;
            }
        }

        // Try to connect to specified remote
        private void ConnectRemote()
        {
            connectionTimer.Start();
            btnConnect.Enabled = false;
            try
            {
                var remote = GetTabPageSpecificRemoteControl(null);

                if (remote == null)
                {
                    MessageBox.Show("Form not properly configured.");
                    return;
                }

                remote.Server = txtServer.Text;
                remote.UserName = txtUserName.Text;

                var secured = (IMsTscNonScriptable)remote.GetOcx();
                secured.ClearTextPassword = txtPassword.Text;
                remote.Connect();
            }
            catch (Exception err)
            {
                // Notify for specific errors
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK);
            }
        }

        // Verify if connection is still opened
        private void DisconnectRemote(AxMsTscAxNotSafeForScripting remote)
        {
            try
            {
                if (remote.Connected.ToString() == "1")
                {
                    remote.Disconnect();
                }
            }
            catch { }
        }

        // Validate connection credentials
        private bool ValidateInput()
        {
            if (!validation.Validate(GetCurrentPageCredentials()))
            {
                MessageBox.Show("Server, Username or Password cannot be empty.");
                return false;
            }

            return true;
        }

        // Will create an instance of ConnectionCredentials with values from the form
        private ConnectionCredentials GetCurrentPageCredentials()
        {
            ConnectionCredentials credentials = new ConnectionCredentials();
            credentials.ServerIP = txtServer.Text;
            credentials.UserName = txtUserName.Text;
            credentials.UserPassword = txtPassword.Text;

            return credentials;
        }

        // Extract specific connection control from a specific tab page
        // Page param must be null if you want method to detect user selected tab
        private AxMsTscAxNotSafeForScripting GetTabPageSpecificRemoteControl(TabPage page)
        {
            if (page == null)
            {
                page = tabCtrl.SelectedTab;
            }

            foreach (var ctrl in page.Controls)
            {
                if (ctrl is AxMsTscAxNotSafeForScripting)
                {
                    return (AxMsTscAxNotSafeForScripting)ctrl;
                }
            }

            return null;
        }

        // Connect Button event handler
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                ConnectRemote();
            }
        }

        // Disconnect Button event handler
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectRemote(GetTabPageSpecificRemoteControl(null));
        }

        // The ">" - add tab button that dynamically creates a new tab with remote control inside
        private void btnCreateTab_Click(object sender, EventArgs e)
        {
            tabGenerator.CreateNewTab(tabCtrl);
        }

        // Event Raised when tab is changed
        private void tabCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (credentialsList.Count >= tabCtrl.SelectedIndex + 1 )
            {
                int index = tabCtrl.SelectedIndex;
                txtServer.Text = credentialsList[index].ServerIP;
                txtUserName.Text = credentialsList[index].UserName;
                txtPassword.Text = credentialsList[index].UserPassword;
            }
            else
            {
                txtServer.Text = string.Empty;
                txtUserName.Text = string.Empty;
                txtPassword.Text = string.Empty;
            }

            btnConnect.Enabled = true;
        }

        // Will check if remote control is connected after the 10 sec interval
        // the reason for this is because connection cannot be checked instantly
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!validation.ValidateConnection(GetTabPageSpecificRemoteControl(null)))
            {
                MessageBox.Show("Connection failed. Please check your credentials.");
            }
            else
            {
                var credentials = GetCurrentPageCredentials();

                if (!validation.AreCredentialsDuplicate(credentials, credentialsList))
                {
                    credentialsList.Add(credentials);
                }
            }
            btnConnect.Enabled = true;
            connectionTimer.Stop();
        }

        // Form Closing Event Handler
        // Make sure all connections are closed
        private void RemoteConnectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TabPage tabPage in tabCtrl.TabPages)
            {
                var remote = GetTabPageSpecificRemoteControl(tabPage);
                DisconnectRemote(remote);
            }

            if (credentialsList.Count > 0)
            {
                storeManager.SaveCredentials(credentialsList);
            }
        }
    }
}
