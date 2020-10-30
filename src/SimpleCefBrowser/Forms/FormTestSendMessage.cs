using System;
using System.Windows.Forms;
using SimpleCefBrowser.Utilities;

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

        #region Event Handlers

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            InteractionOverSendMessage.Send(BrowserForm.Handle, textBoxUrl.Text, 0);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            InteractionOverSendMessage.Send(BrowserForm.Handle, richTextBox1.Text, 1);
        }

        #endregion
    }
}
