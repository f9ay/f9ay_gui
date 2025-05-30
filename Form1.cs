using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace f9ay
{
    // �w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w C / C++ DLL ���� �w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w
    
    // �w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w WinForms Form �w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w�w
    public partial class Form1 : Form
    {
        private byte[] _raw;          // ������ơ]BGR �� BGRA�^
        private int _rows, _cols;
        private int _channels;        // 3 �� 4

        public Form1()
        {
            InitializeComponent();

            // ��l�� ComboBox
            cmbFormat.Items.Clear();
            cmbFormat.Items.AddRange(new[] { "BMP", "JPEG", "PNG" });
            cmbFormat.SelectedIndex = 0;

            btnExport.Enabled = false;
        }

        // �w�w�w�w�w�w�w�w�w�w�w�w�w �פJ �w�w�w�w�w�w�w�w�w�w�w�w�w
        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "�Ϥ�|*.bmp;*.jpg;*.jpeg;*.png",
                Title = "��ܨӷ��Ϥ�"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            IntPtr ptr = Native.f9ay_read(ofd.FileName, out _rows, out _cols, out _channels);
            if (ptr == IntPtr.Zero)
            {
                MessageBox.Show("Ū�����ѡI", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        // �w�w�w�w�w�w�w�w�w�w�w�w�w �ץX �w�w�w�w�w�w�w�w�w�w�w�w�w
        private void button2_Click(object sender, EventArgs e)
        {
            if (_raw == null) return;

            string fmt = (cmbFormat.SelectedItem?.ToString() ?? "BMP")
                         .Trim().ToUpperInvariant();

            using var sfd = new SaveFileDialog
            {
                Filter = fmt switch
                {
                    "JPEG" => "JPEG �� (*.jpg)|*.jpg",
                    "PNG" => "PNG �� (*.png)|*.png",
                    _ => "BMP �� (*.bmp)|*.bmp"
                },
                Title = "�x�s�ץX��"
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

            MessageBox.Show(ret == 0 ? "�ץX���\�I" : "�ץX���ѡI",
                            "���G",
                            MessageBoxButtons.OK,
                            ret == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        // �w�w�w�w�w�w�w�w�w�w�w�w�w �u��禡�GByte �}�C �� Bitmap �w�w�w�w�w�w�w�w�w�w�w�w�w
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

        // �w�w�w�w�w�w�w�w�w�w�w�w�w �u��禡�GBGRA �� BGR�]�h�� Alpha�^�w�w�w�w�w�w�w�w�w�w�w�w�w
        private static byte[] EnsureBgr(byte[] src, int ch)
        {
            if (ch == 3) return src;               // �w�g�O BGR
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
