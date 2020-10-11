using System.ComponentModel;

namespace Biovation.Brands.Shahab.Model
{
    public class CameraOptions
    {
        [Category("ا) پارامترهای پرکاربرد"), DisplayName("کاهش عرض تصویر"), DefaultValue((short)1100), Description("عرض تصویر برای پردازش سریعتر به این اندازه کوچک خواهد شد.")]
        public short ResizeThresh { get; set; } = 1980;     // if width of input image is larger than this, it will be resized

        [Category("ا) پارامترهای پرکاربرد"), DisplayName("نرخ فریم بر ثانیه"), DefaultValue((byte)15), Description("تعداد فریم بر ثانیه دریافتی از دوربین. برای کسب نتیجه ایده آل، نرخ فریم خود دوربین را هم روی همین مقدار تنظیم کنید")]
        public byte FrameRate { get; set; } = 30;

        [Category("ا) پارامترهای پرکاربرد"), DisplayName("تعداد نویسه های پلاک"), Description("اگر فقط پلاک استاندارد مد نظر است، 8 و 0 بگذارید. اگر پلاک مناطق آزاد را هم می خواهید، 8 و 5 بگذارید.")]
        public byte[] NumValidChars { get; set; } = { 8, 0 };  // Number of valid characters usuallay {8, 0}. if e.g. 5 character plates are also available, use {8, 5}

        [Category("ا) پارامترهای پرکاربرد"), DisplayName("ذخیره سازی تصاویر"), DefaultValue((byte)2), Description("صفر: فقط رشته پلاک و مستطیل آن گزارش می شود. یک: تصویر بریده پلاک هم گزارش می شود. دو: تصویر خودرو هم گزارش می شود.")]
        public byte SavePlateOption { get; set; } = (byte)ESAVE_PLATE_OPTION.SAVE_PLATE_AND_CAR; //save_plate_option: 0 don't save anything, 1: save plate only, 2: save whole car image and plate							
                                                                                                 //عدد 2 سبب استفاده بیشتر از حافظه و کاهش حدود 5درصدی سرعت پلاک خوانی می شود

        [Category("ا) پارامترهای پرکاربرد"), DisplayName("موتور"), DefaultValue(false), Description("اگر پلاک موتور برایتان مهم است، تیک بزنید")]
        public bool DetectMotor { get; set; } = true;//if 1 motor detection is enabled, if 0 No.

        [Category("ب) پارامترهای ویدیو"), DisplayName("گزارش پلاکهای ناقص"), DefaultValue(false), Description("اگر پلاکی کمتر از تعداد استاندارد، مثلا 8، رقم داشت یا تعداد حروف آن بیش از یکی باشد، ناقص تلقی می شود. با تیک زدن این گزینه، اینها هم گزارش خواهند شد.")]
        public bool ReportNonStandardPlates { get; set; } = false;

        [Category("ب) پارامترهای ویدیو"), DisplayName("VLC Cache Time"), DefaultValue((short)1000), Description("مدت زمان بافر کردن جریان شبکه برحسب میلی ثانیه، توسط کتابخانه وی ال سی. اگر ارتباط با دوربین برقرار نمی شود، این عدد را افزایش دهید")]
        public short VlcNetCacheTime { get; set; } = 1000;

        [Category("ب) پارامترهای ویدیو"), DisplayName("دریافت تک تصویر"), DefaultValue(false), Description("اگر در حالت عادی نمی توانید به دوربین وصل شوید، این گزینه را تیک بزنید و دوباره تلاش کنید")]
        public bool TakeShotsFromCamera { get; set; } = false;

        [Category("ج) پارامترهای عمومی"), DisplayName("حداقل عرض نویسه"), DefaultValue((byte)5), Description("اگر عرض نویسه ای (عدد یا حرف) کمتر از این عدد باشد، نادیده گرفته می شود")]
        public byte MinCharW { get; set; } = 5; //minimum with of characters
        [Category("ج) پارامترهای عمومی"), DisplayName("حداقل ارتفاع نویسه"), DefaultValue((byte)7), Description("اگر ارتفاع نویسه ای (عدد یا حرف) کمتر از این عدد باشد، نادیده گرفته می شود")]
        public byte MinCharH { get; set; } = 7; //minimum height of characters
        [Category("ج) پارامترهای عمومی"), DisplayName("حداکثر عرض نویسه"), DefaultValue((byte)100), Description("اگر عرض نویسه ای (عدد یا حرف) بیشتر از این عدد باشد، نادیده گرفته می شود")]
        public byte MaxCharW { get; set; } = 100; //maximum with of characters
        [Category("ج) پارامترهای عمومی"), DisplayName("حداکثر ارتفاع نویسه"), DefaultValue((byte)100), Description("اگر ارتفاع نویسه ای (عدد یا حرف) بیشتر از این عدد باشد، نادیده گرفته می شود")]
        public byte MaxCharH { get; set; } = 100; //maximum height of characters

        [Category("ج) پارامترهای عمومی"), DisplayName("میزان کجی پلاک"), DefaultValue(1.0f), Description("عددی بین 0 و 2: هر چه زاویه پلاک نسبت به افق بیشتر است، این عدد را بزرگتر بگیرید.")]
        public float SkewCoef { get; set; } = 1.0f; //more value means more skew: successive characters are not in the same Y position

        [Category("ج) پارامترهای عمومی"), DisplayName("صرفنظر از دولتی"), DefaultValue(false), Description("اگر پلاک دولتی و پلیس برایتان مهم نیست، این گزینه را تیک بزنید")]
        public bool IgnoreInvertedPlates { get; set; } = false;//may not be used

        //ب) پارامترهای ویدیو

        [Category("ب) پارامترهای ویدیو"), DisplayName("وقفه پس از موفقیت"), DefaultValue((byte)0), Description("بعد از ثبت موفق یک پلاک، این تعداد فریم را پردازش نکن")]
        public byte NFrmSkipOnSuccess { get; set; } = 0; //Number of frames to be skipped after successful plate detection

        [Category("ب) پارامترهای ویدیو"), DisplayName("عدم گزارش پلاک تکراری"), DefaultValue((byte)10), Description("برای جلوگیری از گزارش پلاک تکراری مقداری بر حسب ثانیه تنظیم نمایید")]
        public byte SkipSamePlateTime { get; set; } = 15;

        [Category("ب) پارامترهای ویدیو"), DisplayName("ورود خودرو"), DefaultValue((byte)20)]
        [Description("آستانه‏ی تشخیص ورود خودرو\nبرای تصاویر شب، مقدار 7 و برای روز مقدار 20 مناسب است\nمقدار بزرگتر حساسیت کمتری دارد و خودروی کمتری تشخیص می دهد")]
        public byte DiffThresh { get; set; } = 20;         //Difference threshold between current frame and background to suppose entrance of new car 

        [Category("ب) پارامترهای ویدیو"), DisplayName("بافر پلاکها"), DefaultValue((byte)20), Description("برای پیشگیری از گزارش پلاکهای تکراری، این تعداد پلاک مشابه، بافر شده و سپس یکی گزارش می شود.")]
        public byte PlateBufSize { get; set; } = 15;      // Buffer length of recent successive plates (max = 50). 

        [Category("ا) پارامترهای پرکاربرد"), DisplayName("تشخیص چند پلاک"), DefaultValue(true), Description("اگر مجوز چند پلاکه خریده اید، با تیک زدن این پارامتر، می توانید چند پلاک را در یک تصویر بخوانید")]
        public bool DetectMultiPlate { get; set; } = true;

        //[Category("ب) پارامترهای ویدیو")]
        //public byte skip_same_plate_time { get; set; } = 60;// don't report same plate until "some time" elpased

        [Category("ب) پارامترهای ویدیو"), DisplayName("پخش صدا"), DefaultValue(false), Description("در حالت کار با وی ال سی، می توانید صدا را هم داشته باشید. برای برخی دوربینها حتما باید این تیک را بزنید که استریم ویدیو را دریافت کنید")]
        public bool PlayAudioFromCamera { get; set; } = false; //in vlc mode we can play audio (from version 7.45)

        [Category("ج) پارامترهای عمومی"), DisplayName("نوع پلاک"), DefaultValue((byte)0), Description("صفر: فقط پلاک استاندارد، یک: پلاک استاندارد + پلاک اروند، دو: پلاک استاندارد + پلاک ارگ")]
        public byte PlateType { get; set; } = 0;// 
        [Category("ج) پارامترهای عمومی"), DisplayName("رزرو"), DefaultValue((byte)0), Description("برای استفاده های بعدی رزرو شده است")]
        public bool Reserved2 { get; set; } = false;// 

        [Category("ب) پارامترهای ویدیو"), DisplayName("آستانه حداقل هیستوگرام"), DefaultValue((byte)50), Description("عددی بین 0 و 100 به منظور تشخیص محل خودرو و پردازش همان منطقه به جای کل تصویر")]
        public byte MinThreshHist { get; set; } = 50;

        [Category("ب) پارامترهای ویدیو"), DisplayName("آستانه حداکثر هیستوگرام"), DefaultValue((byte)170), Description("عددی بین 100 و 200 به منظور تشخیص محل خودرو و پردازش همان منطقه به جای کل تصویر")]
        public byte MaxThreshHist { get; set; } = 170;

        [Category("ه) پارامترهای عیب یابی"), DisplayName("تکرار ویدیو"), DefaultValue(false), Description("برای اینکه سیستم را با یک فایل ویدیویی زیر بار بگذارید، این تیک را بزنید. با اتمام فایل، از نو شروع می شود.")]
        public bool Repeat { get; set; } = false;

        [Category("ه) پارامترهای عیب یابی"), DisplayName("حالت دیباگ"), DefaultValue((byte)0), Description("مورد استفاده توسعه دهندگان کتابخانه است. تغییر ندهید.")]
        public byte DebugLevel { get; set; } = 0;

        [Category("د) پارامترهای پیشرفته"), DisplayName("کرنل میانه"), DefaultValue((byte)0), Description("فیلتر میانه با این ابعاد. برای عدم اعمال فیلتر، صفر وارد کنید")]
        public byte MedianKernel { get; set; } = 3;      // (0: no kernel) (3, 5, 7 ... median kernel of this size)                                                                                                   //اگر عدد صفر انتخاب شود، فقط رشته پلاک و مستطیل آن گزارش شده و تصویر بریده شده پلاک ارسال نمی شود

        [Category("د) پارامترهای پیشرفته"), DisplayName("فیلتر نرم کننده"), DefaultValue((byte)0), Description("برای باینری کردن وفقی استفاده می شود. اگر تصویرتان سایه دارد 13 و 3 را امتحان کنید.")]
        public byte[] BlurKernel { get; set; } = { 13, 13 };

        [Category("د) پارامترهای پیشرفته"), DisplayName("آستانه ی باینری سراسری"), DefaultValue((byte)0), Description("برای باینری سازی وفقی کل تصویر استفاده می شود")]
        public float[] ImgBinTh { get; set; } = { 0.9f, 0.95f };

        [Category("د) پارامترهای پیشرفته"), DisplayName("آستانه ی باینری محلی"), DefaultValue((byte)0), Description("برای باینری سازی وفقی تصویر پلاک استفاده می شود")]
        public float[] PltBinTh { get; set; } = { 0.8f, 0.85f, 0.92f, 1.0f };
    }
}
