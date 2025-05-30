using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace f9ay
{
    // ────────────────────────────── C / C++ DLL 介面 ──────────────────────────────
    
    // ────────────────────────────── WinForms Form ────────────────────────────────
    public partial class Form1 : Form
    {
        private byte[] _raw;          // 像素資料（BGR 或 BGRA）
        private int _rows, _cols;
        private int _channels;        // 3 或 4

        public Form1()
        {
            InitializeComponent();

            // 初始化 ComboBox
            cmbFormat.Items.Clear();
            cmbFormat.Items.AddRange(new[] { "BMP", "JPEG", "PNG" });
            cmbFormat.SelectedIndex = 0;

            btnExport.Enabled = false;
        }

        // ───────────── 匯入 ─────────────
        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "圖片|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "選擇來源圖片"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            IntPtr ptr = Native.f9ay_read(ofd.FileName, out _rows, out _cols, out _channels);
            if (ptr == IntPtr.Zero)
            {
                MessageBox.Show("讀取失敗！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int bytes = _rows * _cols * _channels;
                _raw = new byte[bytes];
                Marshal.Copy(ptr, _raw, 0, bytes);

                picturePreview.Image?.Dispose();
                picturePreview.Image = BytesToBitmap(_raw, _cols, _rows, _channels);

                btnExport.Enabled = true;
            }
            finally
            {
                Native.f9ay_free(ptr);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // ───────────── 匯出 ─────────────
        private void button2_Click(object sender, EventArgs e)
        {
            if (_raw == null) return;

            string fmt = (cmbFormat.SelectedItem?.ToString() ?? "BMP")
                         .Trim().ToUpperInvariant();

            using var sfd = new SaveFileDialog
            {
                Filter = fmt switch
                {
                    "JPEG" => "JPEG 檔 (*.jpg)|*.jpg",
                    "PNG" => "PNG 檔 (*.png)|*.png",
                    _ => "BMP 檔 (*.bmp)|*.bmp"
                },
                Title = "儲存匯出檔"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            int ret = fmt switch
            {
                "JPEG" => Native.f9ay_jpeg_export(
                              sfd.FileName,
                              EnsureBgr(_raw, _channels), _rows, _cols, 3),
                "PNG" => Native.f9ay_png_export(
                              sfd.FileName,
                              _raw, _rows, _cols, _channels),
                _ => Native.f9ay_bmp_export(
                              sfd.FileName,
                              EnsureBgr(_raw, _channels), _rows, _cols, 3)
            };

            MessageBox.Show(ret == 0 ? "匯出成功！" : "匯出失敗！",
                            "結果",
                            MessageBoxButtons.OK,
                            ret == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        // ───────────── 工具函式：Byte 陣列 → Bitmap ─────────────
        private static Bitmap BytesToBitmap(byte[] data, int w, int h, int ch)
        {
            var fmt = ch == 4 ? PixelFormat.Format32bppArgb
                              : PixelFormat.Format24bppRgb;
            var bmp = new Bitmap(w, h, fmt);
            var rect = new Rectangle(0, 0, w, h);
            var bd = bmp.LockBits(rect, ImageLockMode.WriteOnly, fmt);
            Marshal.Copy(data, 0, bd.Scan0, data.Length);
            bmp.UnlockBits(bd);
            return bmp;
        }

        // ───────────── 工具函式：BGRA → BGR（去掉 Alpha）─────────────
        private static byte[] EnsureBgr(byte[] src, int ch)
        {
            if (ch == 3) return src;               // 已經是 BGR
            int pixels = src.Length / 4;
            byte[] dst = new byte[pixels * 3];
            for (int i = 0, j = 0; i < src.Length; i += 4, j += 3)
            {
                dst[j] = src[i];     // B
                dst[j + 1] = src[i + 1];   // G
                dst[j + 2] = src[i + 2];   // R
            }
            return dst;
        }
    }
    internal static class Native
    {
        private const string DllName = "f9ay_lib.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr f9ay_read(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            out int rows, out int cols, out int channels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int f9ay_bmp_export(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            byte[] data, int rows, int cols, int channels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int f9ay_jpeg_export(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            byte[] data, int rows, int cols, int channels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int f9ay_png_export(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            byte[] data, int rows, int cols, int channels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void f9ay_free(IntPtr ptr);
    }

}
