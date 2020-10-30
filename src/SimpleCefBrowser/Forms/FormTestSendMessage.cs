using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCefBrowser.Forms
{
    public partial class FormTestSendMessage : Form
    {
        private Form _theBrowserForm;

        public FormTestSendMessage(Form theBrowserForm)
        {
            InitializeComponent();
            _theBrowserForm = theBrowserForm;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string url = textBoxUrl.Text;
            GCHandle GCH = GCHandle.Alloc(url, GCHandleType.Pinned);
            IntPtr pUrl = GCH.AddrOfPinnedObject();
            Clipboard.SetText(url);
            int msg = 0x400;
            SendMessage(_theBrowserForm.Handle, msg, IntPtr.Zero, IntPtr.Zero);// pUrl);
            GCH.Free();
        }

        [System.Runtime.InteropServices.DllImport("user32",
                                              EntryPoint = "SendMessage",
                                              ExactSpelling = false,
                                              CharSet = CharSet.Auto,
                                              SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd,
                                         int m,
                                         IntPtr wParam,
                                         IntPtr lParam);
    }
}
