using System;
using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    /// <summary>
    /// آبجکت شخص
    /// </summary>
    public class User
    {
        /// <summary>
        /// شماره کاربر در دیتابیس
        /// </summary>
        /// <value>شماره کاربر در دیتابیس</value>
        [Id]
        public long Id { get; set; }

        public long Code { get; set; }

        public long UniqueId { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        /// <value>نام کاربری</value>
        public string UserName { get; set; }
        //[Required]
        //public string AccessGroup { get; set; }
        //[Required]
        public DateTime RegisterDate { get; set; }
        ///// <summary>
        ///// کد بخش
        ///// </summary>
        ///// <value>کد بخش</value>
        //public int DepartmentId { get; set; }
        /// <summary>
        /// نام واحد سازمانی
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        /// <value>شماره تماس</value>
        public string TelNumber { get; set; }

        private byte[] _imageBytes;
        public byte[] ImageBytes
        {
            get => _imageBytes;

            set =>
                // ImageBytes = value ;
                _imageBytes = value;
        }

        public string Image
        {
            get => _imageBytes!=null?Convert.ToBase64String(_imageBytes):null;
            set =>
                //Image = value;
                
                _imageBytes = ImageBytes != null?Convert.FromBase64String(value):null;
        }
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// نام خانوادگی و نام
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// رمز عبور کاربر
        /// </summary>
        /// <value>رمز عبور</value>
        public string Password { get; set; }
        /// <summary>
        /// رمز عبور به صورت باینری
        /// </summary>
        /// <value>رمز عبور به صورت باینری</value>
        public byte[] PasswordBytes { get; set; }
        /// <summary>
        /// تاریخ آغاز به کار، به فرمت ticks
        /// این داده به صورت تعداد ثانیه های سپری شده از 1/1/1970 تا کنون محاسبه می شود.
        /// </summary>
        /// <value>تاریخ آغاز به کار</value>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// تاریخ پایان قرار داد، به فرمت ticks
        /// این داده به صورت تعداد ثانیه های سپری شده از 1/1/1970 تا کنون محاسبه می شود.
        /// </summary>
        /// <value>تاریخ پایان قرار داد</value>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// سطح دسترسی کاربر، طبق مقادیر ذکر شده در داکیومنت BioStar
        /// </summary>
        /// <value>سطح دسترسی کاربر</value>
        public int AdminLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>مقدار پیش فرض 0</value>
        public int AuthMode { get; set; }
        public string Email { get; set; }
        public int Type { get; set; }
        public int EntityId { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// مدیر سیستم هست یا خیر
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// مانده حساب اعتباری - مربوط به تغذیه
        /// </summary>
        public double RemainingCredit { get; set; }
        /// <summary>
        /// تعداد سهمیه مجاز - مربوط به تغذیه
        /// </summary>
        public int AllowedStockCount { get; set; }

        [OneToMany]
        public List<FingerTemplate> FingerTemplates { get; set; }
        [OneToMany]
        public List<FaceTemplate> FaceTemplates { get; set; }
        [OneToOne]
        public IdentityCard IdentityCard { get; set; }

        public int GetStartDateInTicks()
        {
            var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
            return Convert.ToInt32(StartDate.Ticks / 10000000 - refDate);
        }

        public void SetStartDateFromTicks(int startDateTicks)
        {
            StartDate = new DateTime(1970, 1, 1).AddTicks(startDateTicks);
        }

        public int GetEndDateInTicks()
        {
            var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
            return Convert.ToInt32(EndDate.Ticks / 10000000 - refDate);
        }

        public void SetEndDateFromTicks(int endDateTicks)
        {
            EndDate = new DateTime(1970, 1, 1).AddTicks(endDateTicks);
        }
    }
}