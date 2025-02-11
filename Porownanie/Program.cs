using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using OpenCL.Net;

string inputPath = @"C:\Users\PC\Downloads\obraz\test.jpg";
string outputPath1 = @"C:\Users\PC\Downloads\obraz\przy1.jpg";
string outputPath2 = @"C:\Users\PC\Downloads\obraz\przy2.jpg";
string outputPath3 = @"C:\Users\PC\Downloads\obraz\przy3.jpg";


Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(inputPath);
Process currentProcess = Process.GetCurrentProcess();

// Test 1: GetPixel/SetPixel
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
long memoryBefore1 = currentProcess.PrivateMemorySize64;
Stopwatch sw1 = new Stopwatch();
sw1.Start();
Bitmap result1 = ProcessUsingGetPixel(bitmap);
sw1.Stop();
currentProcess.Refresh();
long memoryAfter1 = currentProcess.PrivateMemorySize64;
result1.Save(outputPath1, System.Drawing.Imaging.ImageFormat.Jpeg);
Console.WriteLine($"Czas GetPixel/SetPixel: {sw1.Elapsed}");
Console.WriteLine($"Zużycie RAM GetPixel/SetPixel: {memoryAfter1 - memoryBefore1} bajtów");
result1.Dispose();

// Wyczyść pamięć przed drugim testem
bitmap.Dispose();
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
bitmap = (Bitmap)System.Drawing.Image.FromFile(inputPath);

// Test 2: Tablica bajtów
long memoryBefore2 = currentProcess.PrivateMemorySize64;
Stopwatch sw2 = new Stopwatch();
sw2.Start();
Bitmap result2 = ProcessUsingByteArray(bitmap);
sw2.Stop();
currentProcess.Refresh();
long memoryAfter2 = currentProcess.PrivateMemorySize64;
result2.Save(outputPath2, System.Drawing.Imaging.ImageFormat.Jpeg);
Console.WriteLine($"Czas operacji na tablicy bajtów: {sw2.Elapsed}");
Console.WriteLine($"Zużycie RAM operacji na tablicy bajtów: {memoryAfter2 - memoryBefore2} bajtów");
result2.Dispose();

// Test 3: Przetwarzanie na GPU
Stopwatch sw3 = new Stopwatch();
sw3.Start();
Bitmap result3 = ProcessUsingGPU(bitmap);
sw3.Stop();
currentProcess.Refresh();
result3.Save(outputPath3, System.Drawing.Imaging.ImageFormat.Jpeg);
Console.WriteLine($"Czas operacji na GPU: {sw3.Elapsed}");
result3.Dispose();

bitmap.Dispose();
Console.ReadLine();

static Bitmap ProcessUsingGetPixel(Bitmap bitmap)
{
    Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
    for (int y = 0; y < bitmap.Height; y++)
    {
        for (int x = 0; x < bitmap.Width; x++)
        {
            Color oldColor = bitmap.GetPixel(x, y);
            int r = (int)(0.75 * oldColor.R);
            int g = (int)(0.75 * oldColor.G);
            int b = (int)(0.75 * oldColor.B);
            result.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
    }
    return result;
}

static Bitmap ProcessUsingByteArray(Bitmap bitmap)
{
    byte[] byteArray = BitmapToByteArray(bitmap);
    for (int i = 0; i < byteArray.Length; i++)
    {
        byteArray[i] = (byte)(0.75 * byteArray[i]);
    }
    return ByteArrayToBitmap(byteArray, bitmap.Width, bitmap.Height);
}

static Bitmap ProcessUsingGPU(Bitmap bitmap)
{
    // Implementacja OpenCL dla przetwarzania obrazu na GPU
    byte[] byteArray = BitmapToByteArray(bitmap);
    int length = byteArray.Length;

    Platform[] platforms = Cl.GetPlatformIDs(out ErrorCode error);
    Device[] devices = Cl.GetDeviceIDs(platforms[0], DeviceType.Gpu, out error);
    Context context = Cl.CreateContext(null, 1, devices, null, IntPtr.Zero, out error);
    CommandQueue queue = Cl.CreateCommandQueue(context, devices[0], CommandQueueProperties.None, out error);

    string kernelSource = "__kernel void darken(__global uchar* imageData) { int i = get_global_id(0); imageData[i] = (uchar)(imageData[i] * 0.75f); }";
    OpenCL.Net.Program program = Cl.CreateProgramWithSource(context, 1, new[] { kernelSource }, null, out error);
    Cl.BuildProgram(program, 1, devices, string.Empty, null, IntPtr.Zero);
    Kernel kernel = Cl.CreateKernel(program, "darken", out error);

    IMem buffer = Cl.CreateBuffer(context, MemFlags.ReadWrite | MemFlags.CopyHostPtr, (IntPtr)(length * sizeof(byte)), byteArray, out error);
    Cl.SetKernelArg(kernel, 0, buffer);
    Cl.EnqueueNDRangeKernel(queue, kernel, 1, null, new IntPtr[] { (IntPtr)length }, null, 0, null, out _);
    Cl.EnqueueReadBuffer(queue, buffer, Bool.True, IntPtr.Zero, (IntPtr)(length * sizeof(byte)), byteArray, 0, null, out _);

    Cl.ReleaseKernel(kernel);
    Cl.ReleaseProgram(program);
    Cl.ReleaseMemObject(buffer);
    Cl.ReleaseCommandQueue(queue);
    Cl.ReleaseContext(context);

    return ByteArrayToBitmap(byteArray, bitmap.Width, bitmap.Height);
}

static byte[] BitmapToByteArray(Bitmap bitmap)
{
    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

    int byteCount = bmpData.Stride * bitmap.Height;
    byte[] byteArray = new byte[byteCount];
    Marshal.Copy(bmpData.Scan0, byteArray, 0, byteCount);
    bitmap.UnlockBits(bmpData);

    return byteArray;
}

static Bitmap ByteArrayToBitmap(byte[] byteArray, int width, int height)
{
    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
    Rectangle rect = new Rectangle(0, 0, width, height);
    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

    Marshal.Copy(byteArray, 0, bmpData.Scan0, byteArray.Length);
    bitmap.UnlockBits(bmpData);

    return bitmap;
}
    

