using System;
using System.Windows.Forms;
using Biovation.CommonClasses.Models;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات کاربر
    /// </summary>
    public class SupremaUserModel : User
    {
        ///// <summary>
        ///// شماره کاربر در دیتابیس
        ///// </summary>
        ///// <value>شماره کاربر در دیتابیس</value>
        //public int Id { get; set; }

        ///// <summary>
        ///// نام کاربری
        ///// </summary>
        ///// <value>نام کاربری</value>
        //public string UserName { get; set; }

        ///// <summary>
        ///// کد بخش
        ///// </summary>
        ///// <value>کد بخش</value>
        //public int DepartmentId { get; set; }

        ///// <summary>
        ///// شماره تماس
        ///// </summary>
        ///// <value>شماره تماس</value>
        //public string TelNumber { get; set; }

        ///// <summary>
        ///// آدرس پست الکترونیک
        ///// </summary>
        ///// <value>آدرس پست الکترونیک</value>
        //public string Email { get; set; }

        /// <summary>
        /// کد کاربر - شماره پرسنلی
        /// </summary>
        /// <value>کد کاربر - شماره پرسنلی</value>
        public string SUserId { get; set; }

        //public string FirstName { get; set; }

        //public string SurName { get; set; }

        ///// <summary>
        ///// رمز عبور کاربر
        ///// </summary>
        ///// <value>رمز عبور</value>
        //public string Password { get; set; }

        ///// <summary>
        ///// رمز عبور به صورت باینری
        ///// </summary>
        ///// <value>رمز عبور به صورت باینری</value>
        //public byte[] Password2 { get; set; }

        /// <summary>
        /// تاریخ آغاز به کار، به فرمت ticks
        /// این داده به صورت تعداد ثانیه های سپری شده از 1/1/1970 تا کنون محاسبه می شود.
        /// </summary>
        /// <value>تاریخ آغاز به کار</value>
        //public int StartDate {
            //get { return _startDate; }
            //set
            //{
                //_startDate = value;

                //var givenDate = new DateTime(1970, 1, 1).AddTicks((long)value * 10000000);
                //IsActive = givenDate >= DateTime.Now;
            //}
        //}

        //private int _startDate;

        ///// <summary>
        ///// تاریخ پایان قرار داد، به فرمت ticks
        ///// این داده به صورت تعداد ثانیه های سپری شده از 1/1/1970 تا کنون محاسبه می شود.
        ///// </summary>
        ///// <value>تاریخ پایان قرار داد</value>
        //public int EndDate
        //{
            //get { return _endDate; }
            //set
            //{
                //_endDate = value;

                //var givenDate = new DateTime(1970, 1, 1).AddTicks((long)value * 10000000);
                //if (givenDate > DateTime.Now)
                //{
                    //IsActive = false;
                //}
            //}
        //}

        //private int _endDate;

        ///// <summary>
        ///// سطح دسترسی کاربر، طبق مقادیر ذکر شده در داکیومنت BioStar
        ///// </summary>
        ///// <value>سطح دسترسی کاربر</value>
        //public int AdminLevel { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <value>مقدار پیش فرض 0</value>
        //public int AuthMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>مقدار پیش فرض 0</value>
        public int AuthLimitCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>مقدار پیش فرض 0</value>
        public int TimedApb { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>مقدار پیش فرض 0</value>
        public int Encryption { get; set; }
    }
}
