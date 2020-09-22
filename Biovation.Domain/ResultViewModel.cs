using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class ResultViewModel
    {
        public int Validate
        {
            get => vValidate;
            set
            {
                vValidate = value;
                sSuccess = vValidate == 1;
            }
        }
        public string Message { get; set; }

        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public long Code { get; set; }

        public long Id { get; set; }

        public bool Success
        {
            get => sSuccess;
            set
            {
                sSuccess = value;
                if (sSuccess)
                {
                    vValidate = 1;
                }
                else
                {
                    vValidate = 0;
                }
            }
        }

        //public HttpResponse StatusCode { get; set; }
        private int vValidate { get; set; }
        private bool sSuccess { get; set; }
    }

    public class ResultViewModel<T>
    {
        public int Validate
        {
            get => vValidate;
            set
            {
                vValidate = value;
                sSuccess = vValidate == 1;
            }
        }
        public bool Success
        {
            get => sSuccess;
            set
            {
                sSuccess = value;
                if (sSuccess)
                {
                    vValidate = 1;
                }
                else
                {
                    vValidate = 0;
                }
            }
        }
        public string Message { get; set; } 
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public T Data { get; set; }
        public long Id { get; set; }
        public long Code { get; set; }
        //public HttpResponse StatusCode { get; set; }

        private int vValidate { get; set; }
        private bool sSuccess { get; set; }
    }
}
