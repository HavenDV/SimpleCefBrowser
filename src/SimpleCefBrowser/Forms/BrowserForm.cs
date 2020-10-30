using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using SimpleCefBrowser.Extensions;

namespace SimpleCefBrowser.Forms
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser _browser;
        private string _title;
        private string _address;

        public BrowserForm(string url)
        {
            InitializeComponent();

            _browser = new ChromiumWebBrowser(string.IsNullOrEmpty(url) ? "about:blank" : url)
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(_browser);
            _browser.TitleChanged += OnBrowserTitleChanged;
            _browser.AddressChanged += OnBrowserAddressChanged;

        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeIfRequired(_ => {
                _title = args.Title;
                RefreshWindowCaption();
            }
            );
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeIfRequired(_ =>
            {
                _address = args.Address;
                RefreshWindowCaption();
            });
        }
        const int WM_USER = 0x0400;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_USER)
            {
                //MessageBox.Show(Marshal.PtrToStringUni(m.LParam));
                if (string.IsNullOrEmpty(Marshal.PtrToStringUni(m.LParam)) == false)
                {
                    _browser.Load(Marshal.PtrToStringUni(m.LParam));
                }
                else
                {
                    // Try to load a URL from the clipboard
                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            _browser.Load(Clipboard.GetText());
                            Clipboard.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        _browser.GetBrowser().MainFrame.LoadHtml(e.ToString());
                    }
                }
            }
        }

        private void RefreshWindowCaption()
        {
            //Text = string.Format("{0} - {1}", _title, _address);
            Text = _address;
        }
    }
}
