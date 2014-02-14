using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;

	/// <summary>
	/// Description of ImageConverter.
	/// </summary>
public class Imageconverter
{
    private static readonly ImageConverter _imageConverter = new ImageConverter();

    public static Image GetImageFromByteArray(byte[] byteArrayIn)
    {
        using (var ms = new MemoryStream(byteArrayIn))
        {
            return Image.FromStream(ms);
        }
    }

    public static byte[] CopyImageToByteArray(Image theImage)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            theImage.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }
    }
}