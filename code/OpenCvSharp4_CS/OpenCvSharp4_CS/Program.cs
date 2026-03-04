using OpenCvSharp;
using OpenCvSharp.ML;

class Program
{
    static void OtsuThreshold()
    {
        // 1️ 讀取圖片
        Mat src = Cv2.ImRead(@"C:\Users\jashv\OneDrive\桌面\1772587654036.png");

        if (src.Empty())
        {
            Console.WriteLine("圖片讀取失敗");
            return;
        }

        // 2️ 轉為灰階
        Mat gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        // 3️ Otsu 二值化
        Mat binary = new Mat();

        double otsuThreshold = Cv2.Threshold(
            gray,                // 來源
            binary,              // 輸出
            0,                   // 門檻值 (Otsu會自動計算)
            255,                 // 最大值
            ThresholdTypes.Binary | ThresholdTypes.Otsu
        );

        Console.WriteLine($"Otsu 自動計算門檻值 = {otsuThreshold}");

        // 4️ 顯示結果
        Cv2.ImShow("Original", src);
        Cv2.ImShow("Gray", gray);
        Cv2.ImShow("Otsu Binary", binary);

        // 5️ 儲存結果
        Cv2.ImWrite(@"C:\Users\jashv\OneDrive\桌面\OpenCv_Otsu.jpg", binary);

        Cv2.WaitKey();
        Cv2.DestroyAllWindows();
    }

    static void NormalBayes()
    {
        // =============================
        // 1️ 建立訓練資料
        // =============================

        Mat trainingData = Mat.FromArray(new float[,]
        {
                {1.0f, 2.0f},
                {1.2f, 1.9f},
                {3.0f, 3.5f},
                {3.2f, 4.0f}
        });

        Mat labels = Mat.FromArray(new int[]
        {
                0, 0, 1, 1
        }).Reshape(1, 4);   // 轉成 4x1

        // =============================
        // 2️ 建立 NormalBayesClassifier
        // =============================

        using var bayes = OpenCvSharp.ML.NormalBayesClassifier.Create();

        bayes.Train(
            trainingData,
            SampleTypes.RowSample,
            labels
        );

        Console.WriteLine("Training completed.\n");

        // =============================
        // 3️ 測試資料
        // =============================

        Mat testData = Mat.FromArray(new float[,]
        {
                {1.1f, 2.1f},
                {3.1f, 3.8f}
        });

        // =============================
        // 4️ 預測
        // =============================

        for (int i = 0; i < testData.Rows; i++)
        {
            using var sample = testData.Row(i);

            float predicted = bayes.Predict(sample);

            Console.WriteLine($"Sample {i} predicted class: {predicted}");
        }

        // =============================
        // 5️ 預測 + 機率輸出
        // =============================

        Console.WriteLine("\nWith probability output:");

        for (int i = 0; i < testData.Rows; i++)
        {
            using var sample = testData.Row(i);
            using var probs = new Mat();
            using var results = new Mat();

            bayes.PredictProb(
                                sample,
                                results,
                                probs,
                                0   // flags
                            );

            float predicted = results.At<float>(0);

            Console.WriteLine($"Sample {i}");
            Console.WriteLine($"Predicted class: {predicted}");
            Console.WriteLine($"Probabilities:\n{probs.Dump()}");
            Console.WriteLine();
        }
    }
    static void KNN()
    {
        // 建立訓練資料 (4筆資料, 每筆2個特徵)
        float[,] trainDataArray = new float[,]
        {
            { 10, 20 },
            { 15, 25 },
            { 100, 200 },
            { 110, 210 }
        };

        // 對應分類標籤
        int[] labelsArray = new int[]
        {
            0,
            0,
            1,
            1
        };

        // 轉換為 Mat
        Mat trainData = Mat.FromArray(trainDataArray);//new Mat(4, 2, MatType.CV_32F, trainDataArray);
        Mat labels = Mat.FromArray(labelsArray).Reshape(1, labelsArray.Length);//new Mat(4, 1, MatType.CV_32S, labelsArray);

        // 建立 KNN 物件
        var knn = KNearest.Create();

        // 設定 K 值
        knn.DefaultK = 3;

        // 訓練模型
        knn.Train(trainData, SampleTypes.RowSample, labels);

        // 測試資料
        float[,] testDataArray = new float[,]
        {
            { 12, 22 }
        };

        Mat testData = Mat.FromArray(testDataArray);//new Mat(1, 2, MatType.CV_32F, testDataArray);

        // 儲存預測結果
        Mat results = new Mat();

        // 執行預測
        float response = knn.FindNearest(
            testData,
            k: 3,
            results: results
        );

        Console.WriteLine("預測分類: " + response);
    }
    static void Main()
    {
        //C++ OPENCV IMAGE GITHUB: https://github.com/jash-git/jashliao-implements-FANFUHAN-OPENCV-with-VC
        OtsuThreshold();

        //C++ OPENCV ML GITHUB: https://github.com/jash-git/CB_OpenCV249_ML
        NormalBayes();
        KNN();
    }
}
