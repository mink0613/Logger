using System.ComponentModel;

namespace LogSystem
{
    /// <summary>
    /// Interface of logger application.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ILogger
    {
        /// <summary>
        /// Calculate call stack and pass calling class name, calling method name, and contents to base method.
        /// </summary>
        /// <param name="contents">Contents that wish to write in the file.</param>
        void Log(params string[] contents);

        /// <summary>
        /// Returns log file name.
        /// </summary>
        /// <returns>Log file name as string.</returns>
        string GetLogFileName();
    }
}
