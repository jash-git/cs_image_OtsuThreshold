using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;

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
    static void Pause()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }
    static void Main()
    {
        ConsumeTime.Start("start");
        
        string inputPath = @"C:\Users\jashv\OneDrive\桌面\1772587654036.jpg";
        string outputPath = @"C:\Users\jashv\OneDrive\桌面\output.jpg";

        using var image = Image.Load<Rgba32>(inputPath);

        int width = image.Width;
        int height = image.Height;

        int[] histogram = new int[256];
        byte[,] grayData = new byte[width, height];

        // 讀取像素
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                var row = accessor.GetRowSpan(y);

                for (int x = 0; x < width; x++)
                {
                    var pixel = row[x];

                    byte gray = (byte)(0.299 * pixel.R +
                                       0.587 * pixel.G +
                                       0.114 * pixel.B);

                    grayData[x, y] = gray;
                    histogram[gray]++;
                }
            }
        });

        // === Otsu 計算 ===

        int totalPixels = width * height;
        float sum = 0;

        for (int i = 0; i < 256; i++)
            sum += i * histogram[i];

        float sumB = 0;
        int wB = 0;
        int threshold = 0;
        float maxVariance = 0;

        for (int t = 0; t < 256; t++)
        {
            wB += histogram[t];
            if (wB == 0) continue;

            int wF = totalPixels - wB;
            if (wF == 0) break;

            sumB += t * histogram[t];

            float mB = sumB / wB;
            float mF = (sum - sumB) / wF;

            float between = wB * wF * (mB - mF) * (mB - mF);

            if (between > maxVariance)
            {
                maxVariance = between;
                threshold = t;
            }
        }

        Console.WriteLine($"Otsu Threshold = {threshold}");

        // 建立輸出圖
        using var output = new Image<L8>(width, height);

        output.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                var row = accessor.GetRowSpan(y);

                for (int x = 0; x < width; x++)
                {
                    row[x] = grayData[x, y] > threshold
                        ? new L8(255)
                        : new L8(0);
                }
            }
        });

        output.Save(outputPath);
        ConsumeTime.Stop();
        Console.WriteLine("Done");

        Pause();
    }
}