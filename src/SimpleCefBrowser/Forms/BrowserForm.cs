using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using SimpleCefBrowser.Extensions;

namespace SimpleCefBrowser.Forms
{
    public partial class BrowserForm : Form
    {
        #region Properties

        private ChromiumWebBrowser Browser { get; }
        private string Address { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public BrowserForm(string url)
        {
            InitializeComponent();

            Browser = new ChromiumWebBrowser(string.IsNullOrEmpty(url) ? "about:blank" : url)
            {
                Dock = DockStyle.Fill,
            };
            Browser.TitleChanged += OnBrowserTitleChanged;
            Browser.AddressChanged += OnBrowserAddressChanged;

            Controls.Add(Browser);
        }

        #endregion

        #region Event Handlers

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeIfRequired(_ => RefreshWindowCaption());
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeIfRequired(_ =>
            {
                Address = args.Address;
                RefreshWindowCaption();
            });
        }

        #endregion

        #region Methods

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
            catch (Exception e)
            {
                Browser.GetBrowser().MainFrame.LoadHtml(e.ToString());
            }
        }

        private void RefreshWindowCaption()
        {
            Text = Address;
        }

        #endregion
    }
}
