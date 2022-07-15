using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WebSocketsCmd;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using System.Security.Cryptography.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Asn1;
using Plugin.Common;
using System.Configuration;
using Org.BouncyCastle.X509.Store;
using System.Collections;
using System.Net;
using System.Net.Security;

namespace Plugin.Signer
{

    public class RSAPKCS1SHA256SignatureDescription : SignatureDescription
    {

        /// <summary>
        /// Registers the http://www.w3.org/2001/04/xmldsig-more#rsa-sha256 algorithm
        /// with the .NET CrytoConfig registry. This needs to be called once per
        /// appdomain before attempting to validate SHA256 signatures.
        /// </summary>
        public static void Register()
        {
            CryptoConfig.AddAlgorithm(
                typeof(RSAPKCS1SHA256SignatureDescription),
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
        }

        /// <summary>
        /// .NET calls this parameterless ctor
        /// </summary>
        public RSAPKCS1SHA256SignatureDescription()
        {
            KeyAlgorithm = "System.Security.Cryptography.RSACryptoServiceProvider";
            DigestAlgorithm = "System.Security.Cryptography.SHA256Managed";
            FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
            DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            var asymmetricSignatureDeformatter =
                (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(DeformatterAlgorithm);
            asymmetricSignatureDeformatter.SetKey(key);
            asymmetricSignatureDeformatter.SetHashAlgorithm("SHA256");
            return asymmetricSignatureDeformatter;
        }

        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            var asymmetricSignatureFormatter =
                (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(FormatterAlgorithm);
            asymmetricSignatureFormatter.SetKey(key);
            asymmetricSignatureFormatter.SetHashAlgorithm("SHA256");
            return asymmetricSignatureFormatter;
        }
    }

    public class PdfSigner : BaseSigner
    {
        const float UNIT_PER_INCH = 72;
        const float UNIT_PER_CM = (float)28.34646;
        const float UNIT_PER_MM = (float)2.834646;
        const float DEFAULT_DPI = 96; //thấp là 72

        //hash sign algorithm
        const string OID_sha1RSA = "1.2.840.113549.1.1.5"; //"sha1RSA"
        const string OID_sha256RSA = "1.2.840.113549.1.1.11"; //sha256RSA
        const string SIGN_METHOD_SHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

        private int SigTextSize = 9;
        private int SoTextSize = 13;
        private const string SigTextFormat = "Signed by: {0}\r\nEmail: {1}\r\nOrganization: {2}\r\nSigned on: {3:MM/dd/yyyy HH:mm:ss}";
        private const string SigTextFormatVN = "Ký bởi: {0}\r\nEmail: {1}\r\nCơ quan: {2}\r\nThời gian ký: {3:MM/dd/yyyy HH:mm:ss}";
        private string fontFileName = "";

        //lưu chứng thư dùng để ký => các lần gọi tiếp theo không cần phải chọn
        private static X509Certificate2 certificate;

        #region Sign

        ////Tính toán vị trí chữ ký: 
        private void SetSigPosition(PdfReader reader, PdfSignatureAppearance sap, int oldSigCount, signatureConfig objConfig, IList<X509Certificate> chain)
        {
            if (objConfig.visibleMode == 2)
                sap.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS; //1/2/3
            else sap.CertificationLevel = PdfSignatureAppearance.NOT_CERTIFIED;// NOT_CERTIFIED=0
            if (chain != null) sap.Certificate = chain[0];
            int _page = reader.NumberOfPages;
            //if (reader.NumberOfPages > objConfig.pageNo) _page = objConfig.pageNo;
            float llX1 = objConfig.llX; float llY1 = objConfig.llY; //góc dưới trái
            float urX1 = objConfig.urX; float urY1 = objConfig.urY; //góc trên phải 
            iTextSharp.text.Image img = null;
            try
            {
                if (objConfig.img == null)
                {
                    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(objConfig.ImageWidth, objConfig.ImageHeight))
                    {
                        using (System.Drawing.Graphics grp = System.Drawing.Graphics.FromImage(bmp))
                        {
                            //Set the Color of the Watermark text.
                            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                            //Set the Font and its size.
                            System.Drawing.Font font = new System.Drawing.Font("Arial", 30, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                            ////I am drawing a oval around my text.
                            grp.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 3), 1, 1, bmp.Width - 5, bmp.Height - 5);
                            System.Drawing.Image bitmap = (System.Drawing.Image)System.Drawing.Bitmap.FromFile("check.png");
                            //grp.DrawImage(bitmap, new System.Drawing.Point(bmp.Width / 2, 5));
                            grp.DrawImage(bitmap, new System.Drawing.Point(300, 60));
                            String signedBy = "";
                            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3")).Count > 0)
                            {
                                signedBy = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3"))[0].ToString();
                            };
                            string[] watermarkTexts = new string[] { "Signature Valid", string.Format("Ký bởi: {0}", signedBy), string.Format("Ký ngày: {0}", DateTime.Now.ToString("dd/MM/yyyy")) };
                            List<string> lines = new List<string>();
                            foreach (string watermarkText in watermarkTexts)
                            {
                                //int lineIndex = 0;
                                double lineHeight = font.Height;
                                string temp = string.Empty;
                                string line = string.Empty;
                                float lineLength = 0;
                                string[] elements = watermarkText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < elements.Length; i++)
                                {
                                    bool flag = false;
                                    temp += elements[i] + " ";
                                    lineLength = grp.MeasureString(temp, font).Width;
                                    if (lineLength < bmp.Width - 5)
                                    {
                                        line = temp;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                    if (i == elements.Length - 1 && temp != string.Empty)
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        lines.Add(line.Trim());
                                        temp = elements[i] + " ";
                                    }
                                }
                            }

                            int y = 0;
                            foreach (string line in lines)
                            {
                                //Determine the size of the Watermark text.
                                System.Drawing.SizeF textSize = grp.MeasureString(line, font);

                                //Position the text and draw it on the image.
                                y += ((int)textSize.Height + 10);
                                System.Drawing.Point position = new System.Drawing.Point(10, y);
                                //Point position = new Point((bmp.Width - ((int)textSize.Width + 10)), (bmp.Height - ((int)textSize.Height + 10)));
                                grp.DrawString(line, font, brush, position);
                            }

                            // Convert the image to byte[]
                            System.IO.MemoryStream stream = new System.IO.MemoryStream();
                            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageBytes = stream.ToArray();
                            objConfig.img = imageBytes;
                        }
                    }
                    img = iTextSharp.text.Image.GetInstance(objConfig.img);
                }
            }
            catch (Exception ex)
            {
                img = null;
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Invalid image: {0}", ex.ToString()));
            };
            iTextSharp.text.Rectangle sigRect = new iTextSharp.text.Rectangle(llX1, llY1, urX1, urY1);
            sap.SetVisibleSignature(sigRect, _page, null);
            sap.SignatureGraphic = img;
            sap.SignatureGraphic.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            sap.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC;
        }

        /// <summary>
        /// tính toán xâu hiển thị cùng chữ ký
        /// </summary>
        /// <param name="sap"></param>
        /// <param name="objConfig"></param>
        /// <param name="chain"></param>
        private void SetSigText(PdfSignatureAppearance sap, signatureConfig objConfig, IList<X509Certificate> chain)
        {
            //các thông số text hiển thị và ảnh (nếu có), nếu mode là page header thì theo chuẩn
            sap.SignDate = DateTime.Now;

            try
            {
                SigTextSize = int.Parse(ConfigurationManager.AppSettings["SigTextSize"]);
                SoTextSize = int.Parse(ConfigurationManager.AppSettings["SoTextSize"]);
            }
            catch (Exception ex)
            {
                SigTextSize = 9;
                SoTextSize = 13;
            };


            //0=invisible, 1=đóng dấu, 2=ký lãnh đạo, 3=pageheader, 4=ký nháy, 5=manual
            //get CN, O-org, C=country from cert {1.2.840.113549.1.9.1}
            //https://github.com/bcgit/bc-csharp/blob/master/crypto/src/asn1/x509/X509Name.cs
            String signedBy = ""; String org = ""; String email = ""; String country = "";
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3")).Count > 0)
            {
                signedBy = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3"))[0].ToString();
            };
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10")).Count > 0)
            {
                org = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10"))[0].ToString();
            };
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1")).Count > 0)
            {
                email = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1"))[0].ToString();
            };
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.6")).Count > 0)
            {
                country = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.6"))[0].ToString();
            };
            float txtSize = SigTextSize;
            sap.Layer2Text = "Ký bởi " + signedBy + "\nThời gian: " + sap.SignDate.ToString("dd/MM/yyyy"); //theo thông tư 01: cơ quan, tổ chức ký + thời gian
            sap.Acro6Layers = true;

            //thiết lập font để hiển thị unicode
            var resource_data = Properties.Resources.times;//Properties.Resources.times; //resource có sẵn font arial và time, thử dùng arial
            var fontFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".ttf";
            Common.FileUtils.WriteFile(resource_data, fontFileName);

            //for green tick and question marks
            //sap.Acro6Layers = false;
            //sap.Layer4Text = PdfSignatureAppearance.questionMark;

            //set font
            BaseFont bf = null;
            if (File.Exists(fontFileName))
            {
                bf = BaseFont.CreateFont(fontFileName, BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font NormalFont = new iTextSharp.text.Font(bf, 11, Font.BOLD /*iTextSharp.text.Font.NORMAL*/, BaseColor.RED);
                sap.Layer2Font = NormalFont;
            };
        }

        /// <summary>
        /// Ký trên xâu hash, dùng chứng thư X509 từ windows certstore
        /// </summary>
        /// <param name="sigAppearance"></param>
        /// <param name="objConfig"></param>
        /// <param name="card"></param>
        /// <param name="chain"></param>
        private void SetSigCryptoFromX509(PdfSignatureAppearance sigAppearance, signatureConfig objConfig, X509Certificate2 card, X509Certificate[] chain)
        {
            try
            {
                //Add SHA256 info
                //CryptoConfig.AddAlgorithm(typeof(RsaPkCs1Sha256SignatureDescription), @"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                //"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), SIGN_METHOD_SHA256);

                String signedBy = ""; String org = ""; String email = "";
                if (objConfig.visibleMode == 3) //pageheader ký số hóa
                {//ký bởi=CN, cơ quan=O, location=
                    if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3")).Count > 0)
                    {
                        signedBy = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3"))[0].ToString();
                    };
                    if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10")).Count > 0)
                    {
                        org = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10"))[0].ToString();
                    };
                    if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1")).Count > 0)
                    {
                        email = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1"))[0].ToString();
                    };
                };


                sigAppearance.Certificate = chain[0];
                int estimatedSize = 8192; //dự toán mem cho dictionary
                PdfSignature dic = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
                dic.Reason = sigAppearance.Reason;
                dic.Location = sigAppearance.Location;
                sigAppearance.SignatureCreator = signedBy;// lấy thông tin từ chứng thư - tên người được cấp
                dic.SignatureCreator = sigAppearance.SignatureCreator;
                dic.Name = signedBy;
                dic.Contact = sigAppearance.Contact;
                dic.Date = new PdfDate(sigAppearance.SignDate); // time-stamp will over-rule this
                sigAppearance.CryptoDictionary = dic;

                Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
                exc[PdfName.CONTENTS] = estimatedSize * 2 + 2;
                sigAppearance.PreClose(exc); //cần preclose trước khi sap.GetRangeStream();

                Stream pdfData = sigAppearance.GetRangeStream();
                byte[] hash;
                bool isSHA2 = false;

                string signhashAlg = card.SignatureAlgorithm.Value; //OID, cert.SignatureAlgorithm.FriendlyName - "sha256RSA";
                if (signhashAlg == OID_sha256RSA && objConfig.hashAlg == "SHA2") //cho token của BCY => nếu tính hash với SHA256 thì lỗi PDF
                {
                    isSHA2 = true;
                    hash = iTextSharp.text.pdf.security.DigestAlgorithms.Digest(pdfData, "SHA-256"); //SHA-256; trong đó có định nghĩa cả SHA-384, SHA-512

                    //cho token của BCY => nếu tính hash với SHA256 thì lỗi PDF
                    //isSHA2 = false;
                    //hash = iTextSharp.text.pdf.security.DigestAlgorithms.Digest(pdfData, "SHA1"); //hay SHA-1
                }
                else
                    hash = iTextSharp.text.pdf.security.DigestAlgorithms.Digest(pdfData, "SHA1"); //hay SHA-1

                //debug: nếu SHA1: độ dài 20 byte, nếu SHA2: độ dài 32 byte
                byte[] ocsp = null; //không thiết lập thông tin CRL, OCSP

                iTextSharp.text.pdf.security.PdfPKCS7 sgn = null;
                if (isSHA2 == true)
                    sgn = new iTextSharp.text.pdf.security.PdfPKCS7(null, chain, "SHA-256", false); //signer
                else
                    sgn = new iTextSharp.text.pdf.security.PdfPKCS7(null, chain, "SHA1", false); // hay SHA-1
                //second hash sh: tính hash tiếp cho các attribute
                byte[] sh = sgn.getAuthenticatedAttributeBytes(hash, ocsp, null, iTextSharp.text.pdf.security.CryptoStandard.CMS);
                //CryptoStandard.CADES; cái nào hỗ trợ SHA256 ? sh: 65 byte

                //TimeStamp: nếu đặt thông tin HTTP tới timestamp server thì truyền vào dể Itextsharp gọi
                //time server
                ITSAClient tsc = null;
                if (objConfig.withTSA)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(objConfig.tsaUrl))
                        {
                            if (!String.IsNullOrEmpty(objConfig.tsaLogin)) tsc = new TSAClientBouncyCastle(objConfig.tsaUrl, objConfig.tsaLogin, objConfig.tsaPass);
                            else tsc = new TSAClientBouncyCastle(objConfig.tsaUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        //lỗi thì kh dùng TSA nữa
                        WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name + " could not connect to TSA server", ex.Message);
                        tsc = null;
                    }
                };


                //================================================
                //3. Ký trên sh second hash. Hàm này ký RSA+SHA256
                //================================================
                System.Security.Cryptography.RSACryptoServiceProvider rsa;
                if (isSHA2 == true)
                {
                    var privKey = (RSACryptoServiceProvider)card.PrivateKey;
                    var enhCsp = new RSACryptoServiceProvider().CspKeyContainerInfo;
                    var cspparams = new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, privKey.CspKeyContainerInfo.KeyContainerName);
                    rsa = new RSACryptoServiceProvider(cspparams);
                }
                else
                {
                    rsa = card.PrivateKey as RSACryptoServiceProvider;
                }

                byte[] extSignature;
                if (isSHA2 == true)
                    extSignature = rsa.SignData(sh, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"));
                else
                    extSignature = rsa.SignData(sh, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA1"));

                sgn.SetExternalDigest(extSignature, null, "RSA"); // externalSignature.GetEncryptionAlgorithm() = "RSA"
                byte[] encodedSig = sgn.GetEncodedPKCS7(hash, tsc, null, null, iTextSharp.text.pdf.security.CryptoStandard.CMS);

                //=======================================================
                //4. chèn xâu byte chữ ký vào file đích
                //=======================================================
                byte[] paddedSig = new byte[estimatedSize];
                System.Array.Copy(encodedSig, 0, paddedSig, 0, encodedSig.Length);

                PdfDictionary dic2 = new PdfDictionary();
                dic2.Put(PdfName.CONTENTS, new PdfString(paddedSig).SetHexWriting(true));
                sigAppearance.Close(dic2);

            }
            catch (Exception ex)
            {
                //WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
        }

        public int Sign(byte[] input, signatureConfig objConfig, out byte[] output, out string subject)
        {
            output = null;
            subject = string.Empty;
            if (input == null || input.Length == 0)
            {
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, "bad input!");
                return (int)SIGNING_RESULT.BadInput;
            }
            MemoryStream inputStream;
            CertUtils certHandle = new CertUtils();
            if (certificate == null)
                certificate = certHandle.GetCertificate(string.Empty);

            if (certificate == null)
            {
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, "Not found certificate");
                return (int)SIGNING_RESULT.BadKey;
            }
            //một số máy, bị lỗi CryptographicException: Key not valid for use in specified state
            try
            {
                if (certificate.PrivateKey == null) //if (certificate.PrivateKey == null) // certificate.HasPrivateKey == false
                {
                    WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, "Not found private key");
                    return (int)SIGNING_RESULT.NotFoundPrivateKey;
                }
            }
            catch (Exception ex)
            {
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, "Error on check certificate.PrivateKey - invalid state");
                return (int)SIGNING_RESULT.NotFoundPrivateKey;
            }
            // get subject
            subject = certificate.SubjectName.Name;
            PdfReader reader = null;
            try
            {
                inputStream = new MemoryStream(input);
                //reader from input file stream
                reader = new PdfReader(inputStream);

                using (MemoryStream outputStream = new MemoryStream())
                using (var stamper = PdfStamper.CreateSignature(reader, outputStream, '\0', null, true)) //append=true => multi signature OK
                {
                    var cp = new Org.BouncyCastle.X509.X509CertificateParser();
                    var chain = new[] { cp.ReadCertificate(certificate.RawData) };
                    var sap1 = stamper.SignatureAppearance;

                    //1. SetSigPosition(sig, reader.AcroFields.GetSignatureNames().Count, chain);
                    SetSigPosition(reader, sap1, reader.AcroFields.GetSignatureNames().Count, objConfig, chain);
                    //2. SetSigText(sig, chain);
                    //SetSigText(sap1, objConfig, chain);
                    //3. SetSigCryptoFromX509(sig, certificate, chain);
                    SetSigCryptoFromX509(sap1, objConfig, certificate, chain);
                    //sap1.GetAppearance().ShowTextAligned(PdfContentByte.ALIGN_CENTER, "This text is centered", 250, 700, 0);
                    output = outputStream.ToArray();

                    string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plugin Log");
                    using (FileStream file = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plugin Log") + @"\plugin.pdf", FileMode.Create, System.IO.FileAccess.Write))
                    {
                        file.Write(output, 0, output.Length);
                    }
                };
                return (int)SIGNING_RESULT.Success;

            }
            catch (Exception e)
            {
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return (int)SIGNING_RESULT.SigningFailed;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (File.Exists(fontFileName))
                {
                    File.Delete(fontFileName);
                }
                certificate = null; //đặt lại để mỗi lần gọi cần chọn lại cert (trong signPDF3 thì không - để dùng cùng 1 cert cho cả số, ngày tháng, dấu)
            }
        }

        private void SetSigPosition(PdfSignatureAppearance sigAppearance, int oldSigCount, IList<X509Certificate> chain)
        {
            String signedBy = ""; String org = ""; String email = "";
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3")).Count > 0)
            {
                signedBy = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3"))[0].ToString();
            };
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10")).Count > 0)
            {
                org = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10"))[0].ToString();
            };
            if (chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1")).Count > 0)
            {
                email = chain[0].SubjectDN.GetValues(new DerObjectIdentifier("1.2.840.113549.1.9.1"))[0].ToString();
            };

            var signedOn = sigAppearance.SignDate.ToString("dd/MM/yyyy HH:mm:ss K");
            //SigTextFormatVN = "Ký bởi: {0}\r\nEmail: {1}\r\nCơ quan: {2}\r\nThời gian ký: {3:MM/dd/yyyy HH:mm:ss}";
            var signatureText = String.Format(SigTextFormat, signedBy, email, org, signedOn);

            var signedByLenght = signatureText.Length;
            float llx = (100 + 20) * (oldSigCount % 5),
                    lly = (25 + 20) * (oldSigCount / 5),
                    urx = signedByLenght * SigTextSize * (3f / 10f),
                    ury = 60;
            sigAppearance.SetVisibleSignature(new Rectangle(llx, lly, urx, ury), 1, null);
        }
        #endregion
        #region Verify
        public override bool Verify(byte[] signedData)
        {
            if (signedData == null)
            {
                throw new ArgumentException("Parameter is null.", "signedData");
            }
            bool result = false;
            PdfReader reader = new PdfReader(signedData);
            AcroFields af = reader.AcroFields;
            var names = af.GetSignatureNames();
            for (int k = 0; k < names.Count; ++k)
            {
                String name = (String)names[k];
                PdfPKCS7 pk = af.VerifySignature(name);
                result = pk.Verify();
            }
            return result;
        }

        #endregion

    }

    public class signatureConfig
    {
        //loại hình chữ ký: 
        // 0 = invisible => (0,0,0,0)
        // 1 = normal, ký phát hành của tổ chức - con dấu => đúng chuẩn thì ./.
        // 2 = leader, ký cá nhân của thủ trưởng - chữ ký cá nhân => đúng chuẩn thì ./.
        // 3 = pageheader, ký của tổ chức khi scan văn bản giấy, ảnh và text theo quy định
        // 4 = ký nháy, ký nháy của người đề xuất, duyệt review,.. => cần tính vị trí
        // 5 = manual: theo tọa độ bên ngoài đưa vào llX, llY,.. ở dưới
        // 6 = chèn số ký hiệu / ngày / tháng ???? đặc thù văn bản
        public int visibleMode { get; set; }
        public int rendermode { get; set; }//chế độ, mapping: 0=DESCRIPTION, 1=NAME_AND_DESCRIPTION, 2=GRAPHIC_AND_DESCRIPTION, 3=GRAPHIC. 
        //với plugin: các thông số người ký,.. => lấy từ cert
        public string sigLocation { get; set; }
        public string sigReason { get; set; }
        public string sigContact { get; set; }
        public string layer2text { get; set; }  //nếu đặt thì sẽ kh hiển thị location/reason/contact

        public int pageNo { get; set; }
        public byte[] img { get; set; } //byte array chứa ảnh chữ ký/con dấu
        //tọa độ vị trí của vùng chữ ký (chữ nhật). cách tính: bên ngoài tính
        public float llX { get; set; }  //vị trí low left X --nếu chế độ  manual
        public float llY { get; set; } //vị trí low left Y
        public float urX { get; set; }  //vị trí upper right X
        public float urY { get; set; } //vị trí upper right Y

        public string sigFieldName { get; set; }//tên trường, nếu có => sau này có thể tìm theo field name,  ví dụ ký nháy
        //============== một số lựa chọn ký + hash: SHA1RSA hay SHA256RSA ??
        public string hashAlg { get; set; }// hash algorithm : SHA1, SHA256 ; ký kết hợp SHA1RSA hoặc SHA25RSA
        //=============== thông tin time server
        public bool withTSA { get; set; } //có dùng TimeStamp không
        public String tsaUrl { get; set; } //nếu có thì đặt thông tin kết nối
        public String tsaLogin { get; set; }
        public String tsaPass { get; set; }

        public int CertificationLevel { get; set; }  // PdfSignatureAppearance.NOT_CERTIFIED = 0, CERTIFIED_NO_CHANGES_ALLOWED=1, CERTIFIED_FORM_FILLING=2, CERTIFIED_FORM_FILLING_AND_ANNOTATIONS=3
        //ký lãnh đạo => 3, các chữ ký tiếp theo: 0

        public String searchTerm { get; set; }  //xâu tìm kiếm
        //phần thông tin số ký hiệu, ngày, tháng, năm (dùng trong chế độ phát hành văn bản-đóng dấu cơ quan)
        public String docNo { get; set; }  //số của văn bản
        public String docDay { get; set; } //ngày văn bản
        public String docMonth { get; set; } //tháng của văn bản

        //Phần thông tin ký di động, dùng SIMPKI
        public String userPhone { get; set; }  //số điện thoại của người dùng SIMPKI
        public String userNotification { get; set; }//thông báo hiển thị cho người dùng trên di động
        public int ImageWidth { get; set; }  //chiều rộng ảnh
        public int ImageHeight { get; set; } //chiều cao ảnh


        public signatureConfig ShallowCopy()
        {
            return (signatureConfig)this.MemberwiseClone();
        }
    }



}

