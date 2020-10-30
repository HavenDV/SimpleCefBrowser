using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCefBrowser.Forms
{
    public partial class FormTestSendMessage : Form
    {
        #region Properties

        private Form BrowserForm { get; }

        #endregion

        #region Constructors

        public FormTestSendMessage(Form browserForm)
        {
            BrowserForm = browserForm ?? throw new ArgumentNullException(nameof(browserForm));

            InitializeComponent();
        }

        #endregion

        #region Methods

        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private void Send(string text, int type)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Clipboard.SetText(text);

            const int msg = 0x400;
            SendMessage(BrowserForm.Handle, msg, type, type);
        }

        #endregion

        #region Event Handlers

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Send(textBoxUrl.Text, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send(richTextBox1.Text, 1);
        }

        #endregion
    }
}
