using System.Collections.Concurrent;
using System.Text;

namespace TDAmeritradeSharpUI
{
    /// <summary>
    /// Log messages at the bottom of a richTextGBox
    /// </summary>
    public partial class LogControl : UserControl
    {
        /// <summary>
        /// These are the messages not yet added to the richTextBox
        /// They are added on a timer so we don't overwhelm the form as individual messages arrive
        /// </summary>
        private readonly ConcurrentQueue<string> _messageQueue;

        public LogControl()
        {
            InitializeComponent();
            _messageQueue = new ConcurrentQueue<string>();
            MaximumLogLengthChars = 1024 * 1024 * 100;
        }

        /// <summary>
        ///     The title of this control
        /// </summary>
        public string Title {
            get => groupBoxLog.Text;
            set => groupBoxLog.Text = value;
        }

        public int MaximumLogLengthChars { get; set; }
        public bool HideTimestamps { get; set; }

        /// <summary>
        ///     Put a message at the TOP of the panel, along with a timestamp
        /// </summary>
        /// <param name="message">the message to display</param>
        public void LogMessage(string message)
        {
            PushMessageOntoQueue(message);
        }

        public void LogMessage(string format, params object[] args)
        {
            var line = string.Format(format, args);
            LogMessage(line);
        }

        /// <summary>
        ///     Put newLines at the TOP of the panel, along with a timestamp, last lines to the top
        /// </summary>
        /// <param name="newLines">the lines to display</param>
        public void LogMessages(IEnumerable<string> newLines)
        {
            foreach (var message in newLines)
            {
                PushMessageOntoQueue(message);
            }
        }

        /// <summary>
        ///     Erase the log
        /// </summary>
        public void Clear()
        {
            this.SafeInvoke(() => rtbMessages.Clear());
        }

        /// <summary>
        ///     Clear the old contents and add newLines
        /// </summary>
        /// <param name="newLines"></param>
        public void Reset(IEnumerable<string> newLines)
        {
            Clear();
            foreach (var message in newLines)
            {
                PushMessageOntoQueue(message);
            }
        }

        private void PushMessageOntoQueue(string message)
        {
            var msg = HideTimestamps ? $"{DateTime.Now:h:mm:ss.fff} {message}" : message;
            _messageQueue.Enqueue(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_messageQueue.IsEmpty)
            {
                return;
            }
            AddQueuedMessages();
        }

        /// <summary>
        ///     Put messages on the stack at the TOP of the panel, along with a timestamp
        /// </summary>
        private void AddQueuedMessages()
        {
            this.SafeInvoke(() => {
                var sb = new StringBuilder();
                while (!_messageQueue.IsEmpty)
                {
                    if (_messageQueue.TryDequeue(out var msg))
                    {
                        sb.AppendLine(msg);
                    }
                }
                sb.AppendLine(rtbMessages.Text);
                if (sb.Length > MaximumLogLengthChars)
                {
                    var oldLength = sb.Length;
                    var saveStr = sb.ToString().Substring(1, MaximumLogLengthChars / 2);
                    sb = new StringBuilder();
                    sb.Append("Truncated the log from ")
                        .AppendFormat("{0:N0}", oldLength).Append(" to ")
                        .AppendFormat("{0:N0}", saveStr.Length)
                        .AppendLine(" characters");
                    sb.AppendLine(saveStr);
                }
                rtbMessages.Text = sb.ToString();
            });
        }

        private void LogControl_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
    }
}