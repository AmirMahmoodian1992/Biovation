using Biovation.Brands.Shahab.Model;
using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Shahab
{
    enum ESAVE_PLATE_OPTION { SAVE_NOTHING, SAVE_PLATE_ONLY, SAVE_PLATE_AND_CAR };


    public class ShahabApi
    {
        public delegate void ANPR_EVENT_CALLBACK(int event_type, byte stream, byte plt_idx);

        public const string DLL_NAME = "ANPR.dll";
        public const int WM_USER = 0x0400;
        public const int WM_NEW_FRAME = WM_USER + 100;
        public const int WM_SCENE_CHANGED = WM_USER + 101;
        public const int WM_PLATE_DETECTED = WM_USER + 102;
        public const int WM_PLATE_NOT_DETECTED = WM_USER + 103; //when a car is in the field of camera but its plate is not recognized
        public const int WM_END_OF_VIDEO = WM_USER + 104; //when video file finished or camera closed
        public const int WM_CONNECTED = WM_USER + 105; //Connected to camera (or video file) from Ver 8.43

        //هنگامی که اولین پلاک در صحنه دیده می شود، برای ترسیم مستطیل اطراف آن
        //رویداد تشخیص قطعی پلاک شماره 102 است
        public const int WM_INITIAL_PLATE = WM_USER + 108;
        public const int WM_CAM_NOT_FOUND = WM_USER + 109;

        //1
        //تابع زیر به ازای هر نسخه کتابخانه (مثلا به ازای هر دوربین) حتما باید یکبار فراخوانی شود. 
        //این تابع شبکه های عصبی مورد استفاده را بارگذاری می کند
        [DllImport(DLL_NAME)]
        public static extern short anpr_create(byte instance, [MarshalAs(UnmanagedType.LPWStr)] string security_code, byte log_level = 1, [MarshalAs(UnmanagedType.LPWStr)] string cfg_file = null);

        //1-1
        //این تابع مدیریت تمام رویدادهای کتابخانه را بر عهده دارد
        [DllImport(DLL_NAME)]
        public static extern short anpr_set_event_callback(ANPR_EVENT_CALLBACK callback_fcn);

        //2
        //این تابع مسیر فایل تصویری را دریافت کرده و نتیجه را بر می گرداند: 
        //رشته، میزان اطمینان به رشته حاصله و مستطیل پلاک
        [DllImport(DLL_NAME)]
        public static extern short anpr_recognize(byte instance, [MarshalAs(UnmanagedType.LPWStr)] string fn,
            [MarshalAs(UnmanagedType.LPWStr)] string result, ref float cnf, ref RectangleCoordinates prc);

        //3
        //این تابع مانند تابع بالایی است با این تفاوت که اندیس مستطیل مورد علاقه را هم می گیرد.
        [DllImport(DLL_NAME)]
        public static extern short anpr_recognizeROI(byte instance, byte roi_idx, [MarshalAs(UnmanagedType.LPWStr)] string fn,
            [MarshalAs(UnmanagedType.LPWStr)] string result, ref float cnf, ref RectangleCoordinates prc);


        //4
        //تابع زیر برای بافری است که از دوربین یا فایل گرفته اید و نوعا یک جریان فشرده مثل جی پگ است.
        [DllImport(DLL_NAME)]
        public static extern short anpr_recognize_stream(byte instance, IntPtr compressed_stream, int size, [MarshalAs(UnmanagedType.LPWStr)] string result, ref float cnf, ref RectangleCoordinates prc);

        //5
        //تابع زیر برای زمانی است که بایتهای تصویر به صورت فشرده نشده در آرایه ای قرار دارند
        //مثلا اشاره گر ابتدای یک بیت مپ
        //مثال آن در همین برنامه دیده می شود
        [DllImport(DLL_NAME)]
        public static extern short anpr_recognize_buffer(byte instance, IntPtr bytes, int W, int H, int step, [MarshalAs(UnmanagedType.LPWStr)] string result, ref float cnf, ref RectangleCoordinates prc);

        [DllImport(DLL_NAME)]
        public static extern short anpr_recognize_bufferROI(byte instance, byte roi_idx, IntPtr bytes, int W, int H, int step, [MarshalAs(UnmanagedType.LPWStr)] string result, ref float cnf, ref RectangleCoordinates prc);

        //6
        //خروجی تابع 2 یک رشته فارسی یونیکد است، اگر خروجی انگلیسی «اسکی» را لازم دارید از این تابع استفاده کنید 
        [DllImport(DLL_NAME)]
        public static extern void anpr_get_ascii_result([MarshalAs(UnmanagedType.LPWStr)] string result_fa, [MarshalAs(UnmanagedType.LPStr)] string result_en);//Get ascii results in English

        //7
        //خروجی تابع 2 یک رشته فارسی یونیکد است، اگر خروجی انگلیسی «یونیکد» را لازم دارید از این تابع استفاده کنید 
        [DllImport(DLL_NAME)]
        public static extern void anpr_get_en_result([MarshalAs(UnmanagedType.LPWStr)] string result_fa, [MarshalAs(UnmanagedType.LPWStr)] string result_en);//Get unicode results in English

        //8
        //یافتن نویسه ها از بافر حافظه ای که تنها شامل تصویر پلاک است
        //به عبارتی محل پلاک باید قبلا یافت شده باشد
        [DllImport(DLL_NAME)]
        public static extern void anpr_find_chars(byte instance, IntPtr bytes, int W, int H, int step, RectangleCoordinates roi, [MarshalAs(UnmanagedType.LPWStr)] string result, ref float pcnf);

        //9
        //این تابع برای تنظیم پارامترهای کتابخانه است.
        [DllImport(DLL_NAME)]
        public static extern void anpr_set_params(byte instance, ref VideoProcessingOptions slpr_params);

        //این تابع برای تست برخی پارامترهای کتابخانه است و حتی الامکان نباید استفاده شود.
        [DllImport(DLL_NAME)]
        public static extern void anpr_set_debug_mode(byte instance, byte debug_level);

        [DllImport(DLL_NAME)]
        public static extern void anpr_add_ROI(byte instance, RectangleCoordinates roi);

        [DllImport(DLL_NAME)]
        public static extern void anpr_clear_ROIs(byte instance);

        [DllImport(DLL_NAME)]
        public static extern short anpr_get_plate(byte instance, byte plate_idx, ref PlateResult result);

        [DllImport(DLL_NAME)]
        public static extern void anpr_about();

        [DllImport(DLL_NAME)]
        public static extern short vlpr_start_grabbing(byte instance, [MarshalAs(UnmanagedType.LPStr)] string URL, byte interval_ms, IntPtr hwndDraw, byte take_shots, byte draw_method);
        [DllImport(DLL_NAME)]
        public static extern short vlpr_stop_grabbing(byte instance);

        [DllImport(DLL_NAME)]
        public static extern short vlpr_start_grabbingVLC(byte instance, [MarshalAs(UnmanagedType.LPStr)] string URL, byte interval_ms, IntPtr hwndDraw, byte take_shots, byte draw_method);

        [DllImport(DLL_NAME)]
        public static extern short vlpr_stop_grabbingVLC(byte instance);


        [DllImport(DLL_NAME)]
        public static extern short vlpr_pause_or_resume(byte instance, byte pause);

        [DllImport(DLL_NAME)]
        public static extern short vlpr_get_frame_info(byte instance, ref int W, ref int H, ref int channels, ref int step);

        [DllImport(DLL_NAME)]
        public static extern IntPtr vlpr_get_frame(byte instance);

        //Start Processing of Camera Frames
        [DllImport(DLL_NAME)]
        public static extern short vlpr_start_process(byte instance);

        //Stop Processing of Camera Frames
        [DllImport(DLL_NAME)]
        public static extern short vlpr_stop_process(byte instance);

        //str must be allocated before
        //output is buffer of plate image
        //پلاک پس از عبور خودرو گزارش می شود. لذا به منظور ثبت تصویر پلاک
        //بافر آن نگهداری می شود
        [Obsolete("vlpr_get_last_resultsW is deprecated, please use anpr_get_plate instead.")]
        [DllImport(DLL_NAME)]
        public static extern IntPtr vlpr_get_last_resultsW(byte stream, [MarshalAs(UnmanagedType.LPWStr)] string str, ref RectangleCoordinates pr, ref float cnf, ref IntPtr img_car_buffer, ref byte direction);

        [Obsolete("vlpr_get_last_results is deprecated, please use anpr_get_plate instead.")]
        [DllImport(DLL_NAME)]
        public static extern IntPtr vlpr_get_last_results(byte stream, byte[] str, ref RectangleCoordinates pr, ref float cnf, ref IntPtr img_car_buffer, ref byte direction);

        //Recognize Last Frame Grabbed from camera or video file
        [DllImport(DLL_NAME)]
        public static extern short vlpr_recognize_cur_frame(byte instance, [MarshalAs(UnmanagedType.LPWStr)] string str, ref RectangleCoordinates pr, ref float cnf);
    }
}
