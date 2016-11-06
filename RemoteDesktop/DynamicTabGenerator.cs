using AxMSTSCLib;
using System.ComponentModel;
using System.Windows.Forms;

namespace RemoteDesktop
{
    public class DynamicTabGenerator
    {
        private AxMsTscAxNotSafeForScripting remoteConControl;

        // Dinamically create Tab Page Control and remote connection control inside it
        public void CreateNewTab(TabControl tabControl)
        {
            
            TabPage page = new TabPage("VM" 
                + (tabControl.TabPages.Count + 1).ToString());

            tabControl.TabPages.Add(page);

            remoteConControl = CreateRemoteControl();

            page.Controls.Add(remoteConControl);
        }

        // Create the remote connection object / control
        private AxMsTscAxNotSafeForScripting CreateRemoteControl()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(RemoteConnectionForm));
            remoteConControl = new AxMsTscAxNotSafeForScripting();

            //Code above is related to UI stuff - same as designer file code for the controls on tab1/2 
            ((ISupportInitialize)(remoteConControl)).BeginInit();
            remoteConControl.Anchor = ((AnchorStyles)((((AnchorStyles.Top 
                                                       | AnchorStyles.Bottom)
                                                       | AnchorStyles.Left)
                                                       | AnchorStyles.Right)));
            remoteConControl.Enabled = true;
            remoteConControl.Location = new System.Drawing.Point(0, 0);
            remoteConControl.Name = "remoteConControl";
            remoteConControl.OcxState = ((AxHost.State)(resources.GetObject("remoteConControl.OcxState")));
            remoteConControl.Size = new System.Drawing.Size(1001, 670);
            remoteConControl.TabIndex = 0;
            remoteConControl.Dock = DockStyle.Fill;

            return remoteConControl;
        }
    }
}
