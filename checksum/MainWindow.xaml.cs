using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.IO;

namespace checksum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _fileName = null;

        public MainWindow()
        {
            InitializeComponent();
            Clear();

            _chkCRC32.IsChecked = true;
            _chkMD5.IsChecked = true;
            _chkSHA1.IsChecked = true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //  credits:
            //  http://stackoverflow.com/questions/21881124/how-do-you-get-navigateuri-to-work-in-a-wpf-window
            //
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        
        private void Clear()
        {
            _txtCRC32.Text = string.Empty;
            _txtMD5.Text = string.Empty;
            _txtSHA1.Text = string.Empty;
            _txtSHA256.Text = string.Empty;
            _txtSHA384.Text = string.Empty;
            _txtSHA512.Text = string.Empty;
        }


        private bool ShouldCalculateCRC32()
        {
            return _chkCRC32.IsChecked.HasValue && _chkCRC32.IsChecked.Value == true;
        }

        private bool ShouldCalculateMD5()
        {
            return _chkMD5.IsChecked.HasValue && _chkMD5.IsChecked.Value == true;
        }

        private bool ShouldCalculateSHA1()
        {
            return _chkSHA1.IsChecked.HasValue && _chkSHA1.IsChecked.Value == true;
        }

        private bool ShouldCalculateSHA256()
        {
            return _chkSHA256.IsChecked.HasValue && _chkSHA256.IsChecked.Value == true;
        }
        
        private bool ShouldCalculateSHA384()
        {
            return _chkSHA384.IsChecked.HasValue && _chkSHA384.IsChecked.Value == true;
        }

        private bool ShouldCalculateSHA512()
        {
            return _chkSHA512.IsChecked.HasValue && _chkSHA512.IsChecked.Value == true;
        }

        private void _btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                Clear();

                _fileName = openFileDialog.FileName;
                _txtFile.Text = _fileName;

                //  note: you cannot reopen the stream once and be done
                //  CRC32 will go to the end of the stream and further checksum
                //  will just return same value for any file- cause we don't have
                //  anything to compute checksum from
                //  
                using (FileStream fs = File.Open(_fileName, FileMode.Open))
                {
                    UpdateCRC32(fs);

                    fs.Seek(0, SeekOrigin.Begin);
                    UpdateMD5(fs);

                    fs.Seek(0, SeekOrigin.Begin);
                    UpdateSHA1(fs);

                    fs.Seek(0, SeekOrigin.Begin);
                    UpdateSHA256(fs);

                    fs.Seek(0, SeekOrigin.Begin);
                    UpdateSHA384(fs);

                    fs.Seek(0, SeekOrigin.Begin);
                    UpdateSHA512(fs);        
                }
            }
        }

        #region CHECKSUM_CALCULATION

        private string CalculateCheckSum(FileStream fs, System.Security.Cryptography.HashAlgorithm algo)
        {
            string hashStr = string.Empty;
            byte[] hash = algo.ComputeHash(fs);
            hashStr = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            return hashStr;
        }
        
        private string CalculateCRC32CheckSum(FileStream fs)
        {
            string crc32Hash = string.Empty;
            using (DamienG.Security.Cryptography.Crc32 crc32 = new DamienG.Security.Cryptography.Crc32())
                crc32Hash = CalculateCheckSum(fs, crc32);

            return crc32Hash;
        }

        private void UpdateCRC32(FileStream fs)
        {
            if (ShouldCalculateCRC32())
                _txtCRC32.Text = CalculateCRC32CheckSum(fs);
            
        }

        private string CalculateMD5CheckSum(FileStream fs)
        {
            string md5Hash = string.Empty;
            using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                md5Hash = CalculateCheckSum(fs, md5);

            return md5Hash;
        }

        private void UpdateMD5(FileStream fs)
        {
            if(ShouldCalculateMD5())
                _txtMD5.Text = CalculateMD5CheckSum(fs);
        }

        private string CalculateSHA1CheckSum(FileStream fs)
        {
            string sha1Hash = string.Empty;
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
                sha1Hash = CalculateCheckSum(fs, sha1);
            return sha1Hash;
        }

        private void UpdateSHA1(FileStream fs)
        {
            if (ShouldCalculateSHA1())
                _txtSHA1.Text = CalculateSHA1CheckSum(fs);
        }

        private string CalculateSHA256CheckSum(FileStream fs)
        {
            string sha256Hash = string.Empty;
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                sha256Hash = CalculateCheckSum(fs, sha256);
            return sha256Hash;
        }

        private void UpdateSHA256(FileStream fs)
        {
            if (ShouldCalculateSHA256())
                _txtSHA256.Text = CalculateSHA256CheckSum(fs);
        }

        private string CalculateSHA384CheckSum(FileStream fs)
        {
            string sha384Hash = string.Empty;
            using (System.Security.Cryptography.SHA384 sha384 = System.Security.Cryptography.SHA384.Create())
                sha384Hash = CalculateCheckSum(fs, sha384);
            return sha384Hash;
        }

        private void UpdateSHA384(FileStream fs)
        {
            if (ShouldCalculateSHA384())
                _txtSHA384.Text = CalculateSHA384CheckSum(fs);
        }

        private string CalculateSHA512CheckSum(FileStream fs)
        {
            string sha512Hash = string.Empty;
            using (System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create())
                sha512Hash = CalculateCheckSum(fs, sha512);
            return sha512Hash;
        }

        private void UpdateSHA512(FileStream fs)
        {
            if (ShouldCalculateSHA512())
                _txtSHA512.Text = CalculateSHA512CheckSum(fs);
        }

        #endregion CHECKSUM_CALCULATION

        #region UI_INTERACTION

        private void _btnCRC32_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnMD5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnSHA1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnSHA256_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnSHA384_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnSHA512_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnCopyAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnPaste_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnVerfiy_Click(object sender, RoutedEventArgs e)
        {

        }

        

        private void _chkCRC32_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if(!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateCRC32() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateCRC32(fs);
            }            

            e.Handled = true;
        }

        private void _chkMD5_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateMD5() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateMD5(fs);
            }

            e.Handled = true;
        }

        private void _chkSHA1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA1() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateSHA1(fs);
            }

            e.Handled = true;
        }

        private void _chkSHA256_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA256() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateSHA256(fs);
            }

            e.Handled = true;

        }

        private void _chkSHA384_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA384() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateSHA384(fs);
            }

            e.Handled = true;
        }

        private void _chkSHA512_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA512() && System.IO.File.Exists(_txtFile.Text))
            {
                using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    UpdateSHA512(fs);
            }

            e.Handled = true;
        }

        #endregion UI_INTERACTION
    }
}

