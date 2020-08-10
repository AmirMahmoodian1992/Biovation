using System;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore.Domain
{
    /// <summary>
    /// All of exeptions which relates to Data Access logics
    /// </summary>
    public class DataAccessException :Exception
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Create DataAccessException by code & title
        /// </summary>
        /// <param name="code"></param>
        /// <param name="title"></param>
        public DataAccessException(int code,string title)
        {
            Code = code;
            Title = title;   
        }

        /// <summary>
        /// Create DataAccessException by code, title & message
        /// </summary>
        /// <param name="code"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public DataAccessException(int code, string title,string message)
        {
            Code = code;
            Title = title;
            Description = message;
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message => "[" + Code + "]: " + Title + "\n" + Description;
    }
}
