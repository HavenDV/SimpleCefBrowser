using System;
using CefSharp;
using SimpleCefBrowser.Extensions;
using SimpleCefBrowser.Utilities;

namespace SimpleCefBrowser.Forms
{
    internal partial class BrowserForm : InteractionOverSendMessageForm
    {
        #region Constructors

        public BrowserForm(string url)
        {
            InitializeComponent();

            MessageReceived += BrowserForm_MessageReceived;
            ExceptionOccurred += BrowserForm_ExceptionOccurred;

            Browser.Load(url);
        }

        #endregion

        #region Event Handlers

        private void BrowserForm_MessageReceived(object sender, Tuple<string, int> e)
        {
            var text = e.Item1;
            var type = e.Item2;

            switch (type)
            {
                case 0:
                    Browser.Load(text);
                    break;

                case 1:
                    Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(text);
                    break;
            }
        }

        private void BrowserForm_ExceptionOccurred(object sender, Exception e)
        {
            Browser.GetBrowser().MainFrame.LoadHtml(e.ToString());
        }

        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.InvokeIfRequired(form => form.Text = Browser.Address);
        }

        #endregion
    }
}
