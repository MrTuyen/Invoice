﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocketsCmd;

namespace Plugin.Common
{
    public class FileUtils
    {
        public static byte[] ReadFile(String filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentNullException("File path is null");
                }
                if (!File.Exists(filePath))
                {
                    throw new NullReferenceException("File not found. check file path");
                }
                return File.ReadAllBytes(filePath);
            }
            catch (Exception)
            {

            }
            return null;
        }
        public static Boolean WriteFile(byte[] data, String filePath)
        {
            Boolean result = false;
            try
            {
                if (data == null || data.Length <= 0)
                {
                    throw new ArgumentNullException("data null");
                }
                using (FileStream fsattach = new FileStream(filePath, System.IO.FileMode.Create, 
                    FileAccess.Write))
                {
                    BinaryWriter bw = new BinaryWriter(fsattach);
                    bw.Write(data);
                    bw.Flush();
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static String ChooseFile()
        {
            String result = "";
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                var threads = new List<Thread>();
                // Call the ShowDialog method to show the dialog box.
                var thread = new Thread(new ParameterizedThreadStart(param =>
                {
                    DialogResult userClickedOK = openFileDialog1.ShowDialog(new Form() { TopMost = true, WindowState = FormWindowState.Minimized });
                    if (DialogResult.OK == userClickedOK)
                    {
                        var base64 = Convert.ToBase64String(ReadFile(openFileDialog1.FileName));
                        result = String.Format("\"path\":\"{0}\", \"base64\":\"{1}\"",
                            openFileDialog1.FileName.Replace("\\", "\\\\"),
                            base64);
                    }
                }));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                threads.Add(thread);
                foreach (var t in threads)
                    t.Join();
            }
            catch (Exception e)
            {
                WebSocketLogger.WRITE(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return "{" + result + "}";
        }
    }
}
