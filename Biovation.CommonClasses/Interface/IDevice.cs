using Biovation.CommonClasses.Models;

namespace Biovation.CommonClasses.Interface
{
    /// <summary>
    /// <Fa>اینترفیس کلی ساعت ها که انواع مختلف ساعت ها از آن پیروی می کنند</Fa>
    /// <En>Parent class for all biometric devices to derive from.</En>
    /// </summary>
    public interface IDevices
    {
        /// <summary>
        /// <En>Transfer user to device.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="user">مشخصات کاربر</param>
        /// <returns></returns>
        bool TransferUser(User user);

        /// <summary>
        /// <En>Read all log data from device, since last disconnect.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        ResultViewModel ReadOfflineLog(object cancelationToken, bool fileSave);

        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        int AddDeviceToDataBase();


        /// <summary>
        /// <En>Delete user from device.</En>
        /// <Fa>کاربر را از دستکاه حذف می کند.</Fa>
        /// </summary>
        /// <param name="sUserId">شماره کاربر</param>
        /// <returns></returns>
        bool DeleteUser(uint sUserId);

    }
}
