using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCefBrowser.Utilities
{
    internal static class InteractionOverSendMessage
    {
        // ReSharper disable once InconsistentNaming
        public const int WM_USER = 0x0400;

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public static void Send(IntPtr handle, string text, int type)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Clipboard.SetText(text);

            const int msg = 0x400;
            SendMessage(handle, msg, type, type);
        }
    }

    internal class InteractionOverSendMessageForm : Form
    {
        #region Events

        public event EventHandler<Tuple<string, int>>? MessageReceived;
        public event EventHandler<Exception>? ExceptionOccurred;

        private void OnMessageReceived(Tuple<string, int> value)
        {
            MessageReceived?.Invoke(this, value);
        }

        private void OnExceptionOccurred(Exception value)
        {
            ExceptionOccurred?.Invoke(this, value);
        }

        #endregion

        #region Methods

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg != InteractionOverSendMessage.WM_USER)
            {
                return;
            }

            try
            {
                if (!Clipboard.ContainsText())
                {
                    return;
                }

                var text = Clipboard.GetText();
                Clipboard.Clear();

                OnMessageReceived(new Tuple<string, int>(text, (int)message.LParam));
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        #endregion
    }
}
