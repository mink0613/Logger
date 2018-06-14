using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace LogSystem
{
    /// <summary>
    /// Base class of logger.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LoggerBase
    {
        /// <summary>
        /// Top level folder path.
        /// </summary>
        private readonly string TOPFOLDERPATH = @"c:\Log\";

        /// <summary>
        /// Low level folder path.
        /// </summary>
        private readonly string LOGFOLDERNAME = @"log\";

        /// <summary>
        /// File format of the log file.
        /// </summary>
        private readonly string FILEFORMAT = @".txt";

        /// <summary>
        /// Use for storing log file name as full path.
        /// </summary>
        private string _logFileName;

        /// <summary>
        /// Lock object for thread safe purpose.
        /// </summary>
        private object _lock = new object();

        private ConcurrentQueue<string> _messageList = new ConcurrentQueue<string>();

        private ManualResetEvent _controller = new ManualResetEvent(false);

        private BackgroundWorker _queueThread = null;

        private Object _queueLockObj = new object();

        /// <summary>
        /// Constructor of LoggerBase.
        /// </summary>
        protected LoggerBase()
        {

        }

        /// <summary>
        /// Initialize base information.
        /// 1. Check the log folder and create if folder does not exist.
        /// 2. Check the log file and create if file does not exist.
        /// 3. File name should contain date of today, as yyyyMMdd, and file format is txt.
        /// </summary>
        protected void Initialize()
        {
            // Check the directory TOPFOLDERPATH.
            // If the directory does not exist, then create new folder.
            if (Directory.Exists(TOPFOLDERPATH) == false)
            {
                Directory.CreateDirectory(TOPFOLDERPATH);
            }

            // Check the directory TOPFOLDERPATH + LOGFOLDERNAME.
            // If the directory does not exist, then create new folder.
            if (Directory.Exists(TOPFOLDERPATH + LOGFOLDERNAME) == false)
            {
                Directory.CreateDirectory(TOPFOLDERPATH + LOGFOLDERNAME);
            }

            _logFileName = TOPFOLDERPATH + LOGFOLDERNAME + DateTime.Today.ToString("yyyyMMdd") + FILEFORMAT;

            // Check a file _logFileName.
            // If the file does not exist, then create new file.
            if (File.Exists(_logFileName) == false)
            {
                try
                {
                    FileStream fileStream = File.Create(_logFileName);
                    fileStream.Close();
                }
                catch (IOException ioe)
                {
                    Console.WriteLine(ioe.Message.ToString());
                    return;
                }
            }

            _queueThread = new BackgroundWorker();
            _queueThread.DoWork += new DoWorkEventHandler(BackgroundDoWork);
            _queueThread.RunWorkerAsync();
        }

        private void BackgroundDoWork(object sender, DoWorkEventArgs e)
        {
            DequeueMessage();
        }

        /// <summary>
        /// Enqueue message to the message buffer "_messageList".
        /// </summary>
        /// <param name="message"></param>
        private void EnqueueMessage(string message)
        {
            lock(_lock)
            {
                _messageList.Enqueue(message);
            }
            
            _controller.Set();
        }

        /// <summary>
        /// Try to dequeue message from the message buffer "_messageList".
        /// If message buffer has multiple message, then 
        /// dequeque message and write log one by one until buffer is empty.
        /// </summary>
        private void DequeueMessage()
        {
            try
            {
                string message;
                while (true)
                {
                    _controller.WaitOne(Timeout.Infinite);

                    StreamWriter logWriter = File.AppendText(_logFileName);

                    while (_messageList.Count > 0)
                    {
                        _messageList.TryDequeue(out message);
                        logWriter.WriteLine(message);
                    }

                    logWriter.Flush();
                    logWriter.Close();

                    _controller.Reset();
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Write log into file.
        /// Log format is {Current time} {Calling Class}: {Calling Method} --> {Log Content}.
        /// </summary>
        /// <param name="callingClass">Class name that writes log.</param>
        /// <param name="callingMethod">Method name of the class that writes log.</param>
        /// <param name="contents">Contents that wish to write in the file.</param>
        protected void Log(string callingClass, string callingMethod, params string[] contents)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            StringBuilder logTextBuilder = new StringBuilder();

            // Format: {Current Time} {Calling Class}: {Calling Method} -->
            logTextBuilder.Append(currentTime + " " + callingClass + ": " + callingMethod + "() --> ");

            foreach (string content in contents)
            {
                logTextBuilder.Append(content + " ");
            }

            EnqueueMessage(logTextBuilder.ToString());
        }

        /// <summary>
        /// This method just returns log file name.
        /// </summary>
        /// <returns>Log file name as string.</returns>
        protected string GetLogFileName()
        {
            return this._logFileName;
        }
    }
}
