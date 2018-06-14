using System.Diagnostics;

namespace LogSystem
{
    /// <summary>
    /// Main class of logger application.
    /// </summary>
    public class Logger : LoggerBase, ILogger
    {
        /// <summary>
        /// Private static readonly variable of logger.
        /// </summary>
        private static readonly Logger _instance = new Logger();

        /// <summary>
        /// Static constructor of Logger
        /// </summary>
        static Logger()
        {

        }

        /// <summary>
        /// Private constructor of Logger
        /// </summary>
        private Logger() : base()
        {
            base.Initialize();
        }

        /// <summary>
        /// Get property of logger instance.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Calculate call stack and pass calling class name, calling method name, and contents to base method.
        /// </summary>
        /// <param name="contents">Contents that wish to write in the file.</param>
        public void Log(params string[] contents)
        {
            StackFrame stack = new StackFrame(1);
            base.Log(stack.GetMethod().DeclaringType.Name, stack.GetMethod().Name, contents);
        }

        /// <summary>
        /// Returns log file name.
        /// </summary>
        /// <returns>Log file name as string.</returns>
        public new string GetLogFileName()
        {
            return base.GetLogFileName();
        }
    }
}
