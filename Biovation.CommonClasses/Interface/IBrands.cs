using Biovation.CommonClasses.Models;

namespace Biovation.CommonClasses.Interface
{
    /// <summary>
    /// اینترفیس کلی تمامی برندهای ساعت ها که کلاس اصلی برند های مختلف از آن پیروی می کند
    /// </summary>
    public interface IBrands
    {
        //void StartService(string ip , int port);

        /// <summary>
        /// <En>Starts each brand's specific server</En>
        /// <Fa>سرور مخصوص به هر برند را استارت می کند</Fa>
        /// </summary>
        void StartService();
        void StopService();

        /// <summary>
        /// <En>Deserialise JSON received data from socket connection ( that has been sent from BiovationServer )  and send request to EventHandler</En>
        /// <Fa>داده ی دریافت شده از اتصال سوکت را که از BiovationServer دریافت کرده ، از فرمت JSON خارج کرده و به EventHandler ارسال میکند</Fa>
        /// </summary>
        /// <param name="receivedData"></param>
        void EventPasser(DataTransferModel receivedData);

        /// <summary>
        /// <En>Deserialise JSON received data from socket connection ( that has been sent from BiovationServer )  and send request to EventHandler</En>
        /// <Fa>داده ی دریافت شده از اتصال سوکت را که از BiovationServer دریافت کرده ، از فرمت JSON خارج کرده و به EventHandler ارسال میکند</Fa>
        /// </summary>
        /// <param name="receivedData"></param>
        /// <param name="clientConnection"></param>
        void EventPasser(DataTransferModel receivedData, object clientConnection);

        /// <summary>
        /// <En>Returns the brand name of the watch</En>
        /// <Fa>نام برند ساعت را بر می گرداند</Fa>
        /// </summary>
        /// <returns></returns>
        string GetBrandName();
        string GetBrandVersion();

        void RegisterArea(object httpConfiguration);


        bool MigrateUp();
    }
}
