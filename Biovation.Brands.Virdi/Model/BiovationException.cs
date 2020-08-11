using System;

namespace Biovation.Brands.Virdi.Model {

    /// <summary>
    /// Exception class specificly for Biovation. this kind of exception carry Code,Title,Message to handle clarity in debuging
    /// </summary>
    public class BiovationException : Exception {

        public int Code;
        public string Title;
        public readonly string message;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Exception Code</param>
        /// <param name="title">Title of Exception</param>
        /// <param name="message">Message of Exception</param>
        public BiovationException(int code, string title, string message) {
            Code = code;
            Title = title;
            this.message = message;
        }
        /// <summary>
        /// Parsing Exception to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Exception: (" + Code + ") " + Title + "\n" + message;
        }
    }

}
