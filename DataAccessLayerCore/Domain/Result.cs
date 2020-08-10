using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore.Domain {
    /// <summary>
    /// A Generic wrapper for all object type which report transaction running status. 
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    public class Result<T>
    {
        public bool Success { get; set; } = true;
        public string Code { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int SeverityLevel { get; set; }
        public T Data { get; set; }

        /// <summary>
        /// Use this method when you neet to convert Result<List<T>> to Result<T>
        /// It means that if you need a single instance of object but your database(and your repository consequently) return a List of that object, this method will be usefull for you.
        /// </summary>
        /// <param name="list"></param>
        public void FetchFromResultList(Result<List<T>> list)
        {
            Success = list.Success;
            Code = list.Code;
            Title = list.Title;
            Message = list.Message;
            SeverityLevel = list.SeverityLevel;
            Data = list.Data.FirstOrDefault();
        }
        

    }
}
