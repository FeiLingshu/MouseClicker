using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Screen = System.Windows.Forms.Screen;
using WinFormRect = System.Drawing.Rectangle;

namespace MouseClicker
{
    public static class Settings
    {
        private static FileStream stream = null;

        public static void CheckFile(MainWindow UI)
        {
            string path = string.Empty;
            using (Process process = Process.GetCurrentProcess())
            {
                string mypath = process.MainModule.FileName;
                path = $"{Path.GetDirectoryName(mypath)}\\{Path.GetFileNameWithoutExtension(mypath)}.bin";
            }
            void WriteFile()
            {
                byte[] bytes = new byte[29];
                bytes[0] = (byte)0;
                bytes[1] = (byte)((0 >> 24) & 0xFF);
                bytes[2] = (byte)((0 >> 16) & 0xFF);
                bytes[3] = (byte)((0 >> 8) & 0xFF);
                bytes[4] = (byte)((0) & 0xFF);
                bytes[5] = (byte)((0 >> 24) & 0xFF);
                bytes[6] = (byte)((0 >> 16) & 0xFF);
                bytes[7] = (byte)((0 >> 8) & 0xFF);
                bytes[8] = (byte)((0) & 0xFF);
                bytes[9] = (byte)0;
                bytes[10] = (byte)((0 >> 24) & 0xFF);
                bytes[11] = (byte)((0 >> 16) & 0xFF);
                bytes[12] = (byte)((0 >> 8) & 0xFF);
                bytes[13] = (byte)((0) & 0xFF);
                bytes[14] = (byte)((1 >> 24) & 0xFF);
                bytes[15] = (byte)((1 >> 16) & 0xFF);
                bytes[16] = (byte)((1 >> 8) & 0xFF);
                bytes[17] = (byte)((1) & 0xFF);
                bytes[18] = (byte)0;
                bytes[19] = (byte)0;
                bytes[20] = (byte)((0 >> 24) & 0xFF);
                bytes[21] = (byte)((0 >> 16) & 0xFF);
                bytes[22] = (byte)((0 >> 8) & 0xFF);
                bytes[23] = (byte)((0) & 0xFF);
                bytes[24] = (byte)((0 >> 24) & 0xFF);
                bytes[25] = (byte)((0 >> 16) & 0xFF);
                bytes[26] = (byte)((0 >> 8) & 0xFF);
                bytes[27] = (byte)((0) & 0xFF);
                bytes[28] = (byte)0;
                File.WriteAllBytes(path, bytes);
            }
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length == 29)
                {
                    if (bytes[0] == (byte)1)
                    {
                        UI.SettingIcon_1.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                        UI.Setting_1.IsChecked = true;
                        int _1 = bytes[1] << 24 | bytes[2] << 16 | bytes[3] << 8 | bytes[4];
                        int _2 = bytes[5] << 24 | bytes[6] << 16 | bytes[7] << 8 | bytes[8];
                        bool flag = false;
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (screen.WorkingArea.Contains(new WinFormRect(_1, _2, (int)UI.Width, (int)UI.Height)))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            UI.WindowStartupLocation = WindowStartupLocation.Manual;
                            UI.Left = _1;
                            UI.Top = _2;
                        }
                    }
                    if (bytes[9] == (byte)1)
                    {
                        UI.SettingIcon_2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                        UI.Setting_2.IsChecked = true;
                        int _3 = bytes[10] << 24 | bytes[11] << 16 | bytes[12] << 8 | bytes[13];
                        switch (_3)
                        {
                            case 0:
                                UI.Type1.IsChecked = true;
                                break;
                            case 1:
                                UI.Type2.IsChecked = true;
                                break;
                            case 2:
                                UI.Type3.IsChecked = true;
                                break;
                            default:
                                goto case 0;
                        }
                        int _4 = bytes[14] << 24 | bytes[15] << 16 | bytes[16] << 8 | bytes[17];
                        if (_4 < 1) _4 = 1;
                        UI.Time.Text = _4.ToString();
                        if (bytes[18] == (byte)1)
                        {
                            UI.Delay.IsChecked = true;
                        }
                        if (bytes[19] == (byte)1)
                        {
                            UI.Move.IsChecked = true;
                        }
                        int _5 = bytes[20] << 24 | bytes[21] << 16 | bytes[22] << 8 | bytes[23];
                        int _6 = bytes[24] << 24 | bytes[25] << 16 | bytes[26] << 8 | bytes[27];
                        bool flag = false;
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (screen.Bounds.Contains(_5, _6))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            UI.MousePoint = new Point(_5, _6);
                            UI.PointSTD.Text = $"{_5},{_6}";
                        }
                        if (bytes[28] == (byte)1 && bytes[19] == (byte)1)
                        {
                            UI.MovePlus.IsChecked = true;
                        }
                    }
                }
                else
                {
                    WriteFile();
                }
            }
            else
            {
                WriteFile();
            }
            stream = File.OpenWrite(path);
        }

        public enum Dataindex : long
        {
            WindowFlag = 0L,
            WindowData = 1L,
            SettingFlag = 9L,
            TypeData = 10L,
            TimeData = 14L,
            DelayData = 18L,
            MoveData = 19L,
            PointData = 20L,
            MovePlusData = 28L,
            AllData = TypeData
        }

        public static void SaveData(Dataindex type, byte[] data)
        {
            if (stream != null && data != null)
            {
                stream.Seek((long)type, SeekOrigin.Begin);
                stream.Write(data, 0, data.Length);
            }
        }

        public static byte[] GetBytes(Type type, object data)
        {
            if (type == typeof(bool))
            {
                byte[] bytes = new byte[1] { (bool)data ? (byte)1 : (byte)0 };
                return bytes;
            }
            if (type == typeof(int))
            {
                int intdata = (int)data;
                byte[] bytes = new byte[4]
                {
                    (byte)((intdata >> 24) & 0xFF),
                    (byte)((intdata >> 16) & 0xFF),
                    (byte)((intdata >> 8) & 0xFF),
                    (byte)((intdata) & 0xFF)
                };
                return bytes;
            }
            if (type == typeof(Point))
            {
                Point pointdata = (Point)data;
                byte[] bytes = new byte[8]
                {
                    (byte)(((int)pointdata.X >> 24) & 0xFF),
                    (byte)(((int)pointdata.X >> 16) & 0xFF),
                    (byte)(((int)pointdata.X >> 8) & 0xFF),
                    (byte)(((int)pointdata.X) & 0xFF),
                    (byte)(((int)pointdata.Y >> 24) & 0xFF),
                    (byte)(((int)pointdata.Y >> 16) & 0xFF),
                    (byte)(((int)pointdata.Y >> 8) & 0xFF),
                    (byte)(((int)pointdata.Y) & 0xFF)
                };
                return bytes;
            }
            return null;
        }

        public static void Dispose()
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }
    }
}
