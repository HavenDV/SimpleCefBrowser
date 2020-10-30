using System;
using System.Windows.Forms;
using CefSharp;
using SimpleCefBrowser.Extensions;

namespace SimpleCefBrowser.Forms
{
    public partial class BrowserForm : Form
    {
        #region Constructors

        public BrowserForm(string url)
        {
            InitializeComponent();

            Browser.Load(url);
        }

        #endregion

        #region Event Handlers

        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.InvokeIfRequired(form => form.Text = Browser.Address);
        }

        #endregion

        #region Methods

        // ReSharper disable once InconsistentNaming
        private const int WM_USER = 0x0400;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg != WM_USER)
            {
                return;
            }

            // Try to load a URL from the clipboard
            try
            {
                if (!Clipboard.ContainsText())
                {
                    return;
                }

                var text = Clipboard.GetText();
                Clipboard.Clear();

                switch ((int)m.LParam)
                {
                    case 0:
                        Browser.Load(text);
                        break;

                    case 1:
                        Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(text);
                        break;
                }
            }
            catch (Exception exception)
            {
                Browser.GetBrowser().MainFrame.LoadHtml(exception.ToString());
            }
        }

        #endregion
    }
}
