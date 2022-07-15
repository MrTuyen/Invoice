using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DS.Common.Helpers
{
    /// <summary>
    /// Sử dụng thư viện QRCoder trong nuget
    /// </summary>
    public static class QRCodeHelper
    {
        public static string CreateQRCode(string qrInfo)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrInfo, QRCodeGenerator.ECCLevel.M);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(10);
                System.IO.MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                return Convert.ToBase64String(byteImage);
            }
            catch (Exception ex)
            {               
                ConfigHelper.Instance.WriteLog("Lỗi không tạo được QR code.", ex, null, null);
                return null;
            }
        }
    }
}
