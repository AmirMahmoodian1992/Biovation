using System;
using DataAccessLayer.Attributes;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Models
{
    public class UserGroup
    {
        /// <summary>
        /// شماره کاربر در دیتابیس
        /// </summary>
        /// <value>شماره کاربر در دیتابیس</value>
        [Id]
        public int Id { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        /// <value>نام دسته بندی</value>
        public string Name { get; set; }

        /// <summary>
        /// گروه دسترسی
        /// </summary>
        public string AccessGroup { get; set; }

        /// <summary>
        /// توضیحات گروه دسترسی
        /// </summary>
        public string Description { get; set; }

        [OneToMany]
        public List<UserGroupMember> Users { get; set; }

        //[OneToMany]
        //public List<User> Users { get; set; }
    }
}
