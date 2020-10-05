using System.Runtime.InteropServices;

namespace Biovation.Brands.Shahab.Model
{
    public struct VideoProcessingOptions
    {
        public short ResizeThresh;     // if width of input image is larger than this, it will be resized
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] NumValidChars;   // Number of valid characters usuallay {8, 0}. if e.g. 5 character plates are also available, use {8, 5}
        //public byte num_valid_chars1;   //if Two types of plates are important
        //public byte num_valid_chars2;   //if Two types of plates are important
        public byte medianKernel;       // (0: no kernel) (3, 5, 7 ... median kernel of this size)
        public byte SavePlateOption;  //save_plate_option: 0 don't save anything, 1: save plate only, 2: save whole car image and plate							
                                      //اگر عدد صفر انتخاب شود، فقط رشته پلاک و مستطیل آن گزارش شده و تصویر بریده شده پلاک ارسال نمی شود
                                      //عدد 2 سبب استفاده بیشتر از حافظه و کاهش حدود 5درصدی سرعت پلاک خوانی می شود
        public short VlcNetCacheTime;
        //Limits of character dimensions
        public byte MinCharW;  //minimum with of characters
        public byte MinCharH;  //minimum height of characters
        public byte MaxCharW;  //maximum with of characters
        public byte MaxCharH;  //maximum height of characters

        public float SkewCoefficient;         //more value means more skew: successive characters are not in the same Y position

        public byte IgnoreInvertedPlates;//may not be used
        public byte DetectMotor; //if 1 motor detection is enabled, if 0 No.

        //ب) پارامترهای ویدیو
        public byte NFrmSkipOnSuccess;  //Number of frames to be skipped after successful plate detection
        public byte DiffThresh;            //Difference threshold between current frame and background to suppose entrance of new car 
        public byte PlateBufSize;         // Buffer length of recent successive plates (max = 50). 
        public byte DetectMultiPlate;
        public byte SkipSamePlateTime;// don't report same plate until "some time" elpased

        public byte PlayAudioFromCamera; //in vlc mode we can play audio (from version 7.45)

        public byte PlateType; //0: Only Iran standard, 1: + Arvand, 2: + Arg 
        public byte Reserved2; //

        //internal byte horizontal_thresh;
        // از این دو پارامتر برای تعیین ناحیه پردازش استفاده میشود
        public byte MinThreshHist;
        public byte MaxThreshHist;

        //تنظیمات پیشرفته که به ندرت نیاز است تغییر داده شود
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] blur_kernel;//{ 13, 13 }; //Size of blur kernel used for binarization. Default is 13x13. To handle shadow, try 13x1 or 13x3	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] ImgBinTh;//[2] = { 0.9f, 0.95f }; //Adaptive Binarization Threshold (between 0.5 and 1) default is [0.95, 0.9]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] PltBinTh;//[4] = { 0.8f, 0.85f, 0.92f, 1.0f }; //Adaptive Binarization Threshold (between 0.5 and 1) default is [0.95, 0.9]

        public static void SetDefaultOptions(byte instance, VideoProcessingOptions options)
        {
            options.MinCharW = 5; //minimum with of characters
            options.MinCharH = 7; //minimum height of characters
            options.MaxCharW = 100; //maximum with of characters
            options.MaxCharH = 100; //maximum height of characters
            options.SkewCoefficient = 1.0f; //more value means more skew: successive characters are not in the same Y position
            options.ResizeThresh = 1100;//if width of input image is larger than this, it will be resized
            options.medianKernel = 0;//Kernel size: 0, 3, 5, 7, etc...
            options.IgnoreInvertedPlates = 0;
            options.DetectMotor = 0; //if 1 motor detection is enabled, if 0 No.
            options.DetectMultiPlate = 1;
            options.NumValidChars = new byte[] { 8, 0 };
            //options.num_valid_chars1 = 8; //5 for free 
            //options.num_valid_chars2 = 0; //5 for free //mhh - changed 5 to 0

            options.SavePlateOption = (byte)ESAVE_PLATE_OPTION.SAVE_PLATE_AND_CAR;
            options.NFrmSkipOnSuccess = 10;
            options.PlateBufSize = 12;
            options.Reserved2 = 0; // car recognition is disabled
            options.VlcNetCacheTime = 1000;
            options.PlateType = 0;
            options.DiffThresh = 10; //difference threshold between current frame and background to suppose entrance of new car 
            options.PlayAudioFromCamera = 0;
            options.MinThreshHist = 60;
            options.MaxThreshHist = 170;

            options.blur_kernel = new byte[] { 13, 13 };
            options.ImgBinTh = new[] { 0.9f, 0.95f };
            options.PltBinTh = new[] { 0.8f, 0.85f, 0.92f, 1.0f };
            ShahabApi.anpr_set_params(instance, ref options);
        }
    }
}
