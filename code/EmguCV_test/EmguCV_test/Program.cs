using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Drawing;

namespace EmguCVOtsuExample
{
    public class ConsumeTime
    {
        private static Stopwatch m_stopWatch = new Stopwatch();
        private static String m_StrTitle = "";
        private static String m_StrStartFileLine = "";
        private static String m_StrEndFileLine = "";
        public static void Start(String StrInfor)
        {
            StackFrame CallStack = new StackFrame(1, true);
            m_StrStartFileLine = String.Format("File : {0} , Line : {1}", CallStack.GetFileName(), CallStack.GetFileLineNumber());
            m_StrTitle = StrInfor;

            m_stopWatch.Start();
        }
        public static void Stop()
        {
            StackFrame CallStack = new StackFrame(1, true);

            m_stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = m_stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            m_StrEndFileLine = String.Format("File : {0} , Line : {1}", CallStack.GetFileName(), CallStack.GetFileLineNumber());

            Console.WriteLine(m_StrStartFileLine + " ~ " + m_StrEndFileLine + " consume time: " + elapsedTime, m_StrTitle);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            ConsumeTime.Start("start");
            //https://www.emgu.com/wiki/files/4.12.0/document/html/R_Project_Emgu_CV_Documentation.htm
            // 1. 讀取影像
            Mat src = CvInvoke.Imread(@"C:\Users\jashv\OneDrive\桌面\1772587654036.jpg", ImreadModes.AnyColor);

            if (src.IsEmpty)
            {
                Console.WriteLine("找不到影像檔案！");
                return;
            }

            // 2. 轉換為灰階 (Otsu 必須在單通道影像上執行)
            Mat gray = new Mat();
            CvInvoke.CvtColor(src, gray, ColorConversion.Bgr2Gray);

            // 3. 套用 Otsu 二值化
            // 參數說明：
            // gray: 輸入影像
            // dest: 輸出影像
            // 0: 門檻值 (在使用 Otsu 模式下，此值會被忽略並自動計算)
            // 255: 最大值 (超過門檻後要設定的數值)
            // ThresholdType.Binary | ThresholdType.Otsu: 關鍵在於加上 .Otsu 旗標
            Mat dest = new Mat();
            double thresholdValue = CvInvoke.Threshold(gray, dest, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            Console.WriteLine($"Otsu 計算出的最佳門檻值為: {thresholdValue}");
            dest.Save(@"C:\Users\jashv\OneDrive\桌面\otsu_result.jpg"); // 儲存結果影像
            ConsumeTime.Stop();
            // 4. 顯示結果
            CvInvoke.Imshow("Original Rgb", src);
            CvInvoke.Imshow("Original Gray", gray);
            CvInvoke.Imshow("Otsu Result", dest);
            
            // 5. 等待按鍵後關閉視窗
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();
        }
    }
}