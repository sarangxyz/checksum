using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;

namespace checksum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region BUSY_INDICATOR

        public BusyStatusTracker BusyStatusTracker = new BusyStatusTracker();
        void StartBusyOperation()
        {
            BusyStatusTracker.IsBusy = true;
        }

        void EndBusyOperation()
        {
            BusyStatusTracker.IsBusy = false;
        }

        void UpdateProgress(string message)
        {
            
        }

        #endregion BUSY_INDICATOR

        public MainWindow()
        {
            InitializeComponent();
            Clear();

            _chkCRC32.IsChecked = true;
            _chkMD5.IsChecked = true;
            _chkSHA1.IsChecked = true;

            this.DataContext = BusyStatusTracker;
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
            _txtFile.Text = string.Empty;
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

        private async void _btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                Clear();

                _txtFile.Text = openFileDialog.FileName;

                try
                {
                    StartBusyOperation();
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        Checksum.Algo algo = Checksum.Algo.None;
                        if (ShouldCalculateCRC32())
                            algo |= Checksum.Algo.CRC32;
                        if (ShouldCalculateMD5())
                            algo |= Checksum.Algo.MD5;
                        if (ShouldCalculateSHA1())
                            algo |= Checksum.Algo.SHA1;
                        if (ShouldCalculateSHA256())
                            algo |= Checksum.Algo.SHA256;
                        if (ShouldCalculateSHA384())
                            algo |= Checksum.Algo.SHA384;
                        if (ShouldCalculateSHA512())
                            algo |= Checksum.Algo.SHA512;

                        if (algo != 0)
                        {
                            Checksum cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(algo, fs));
                            _txtCRC32.Text = cksum.CRC32;
                            _txtMD5.Text = cksum.MD5;
                            _txtSHA1.Text = cksum.SHA1;
                            _txtSHA256.Text = cksum.SHA256;
                            _txtSHA384.Text = cksum.SHA384;
                            _txtSHA512.Text = cksum.SHA512;
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
        }
        
        #region UI_INTERACTION

        private void _btnCRC32_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtCRC32.Text))
                Clipboard.SetText(_txtCRC32.Text);
        }

        private void _btnMD5_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtMD5.Text))
                Clipboard.SetText(_txtMD5.Text);
        }

        private void _btnSHA1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtSHA1.Text))
                Clipboard.SetText(_txtSHA1.Text);
        }

        private void _btnSHA256_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtSHA256.Text))
                Clipboard.SetText(_txtSHA256.Text);
        }

        private void _btnSHA384_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtSHA384.Text))
                Clipboard.SetText(_txtSHA384.Text);
        }

        private void _btnSHA512_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtSHA512.Text))
                Clipboard.SetText(_txtSHA512.Text);
        }

        private void _btnCopyAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnPaste_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnVerfiy_Click(object sender, RoutedEventArgs e)
        {
            var pastedHash = _txtHASH.Text.Trim();
            if (string.IsNullOrEmpty(pastedHash) || string.IsNullOrEmpty(_txtFile.Text))
                return;

            if (pastedHash == _txtCRC32.Text)
                MessageBox.Show("Matches CRC32", "CheckSum", MessageBoxButton.OK);
            else if (pastedHash == _txtMD5.Text)
                MessageBox.Show("Matches MD5", "CheckSum", MessageBoxButton.OK);
            else if (pastedHash == _txtSHA1.Text)
                MessageBox.Show("Matches SHA1", "CheckSum", MessageBoxButton.OK);
            else if (pastedHash == _txtSHA256.Text)
                MessageBox.Show("Matches SHA256", "CheckSum", MessageBoxButton.OK);
            else if (pastedHash == _txtSHA384.Text)
                MessageBox.Show("Matches SHA384", "CheckSum", MessageBoxButton.OK);
            else if (pastedHash == _txtSHA512.Text)
                MessageBox.Show("Matches SHA512", "CheckSum", MessageBoxButton.OK);
            else
                MessageBox.Show("Failed to match", "CheckSum", MessageBoxButton.OK);
        }
        

        private async void _chkCRC32_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

           if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateCRC32() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.CRC32, fs));
                        _txtCRC32.Text = cksum.CRC32;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            e.Handled = true;
        }

        private async void _chkMD5_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateMD5() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.MD5, fs));
                        _txtMD5.Text = cksum.MD5;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            
            e.Handled = true;
        }

        private async void _chkSHA1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA1() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.SHA1, fs));
                        _txtSHA1.Text = cksum.SHA1;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            
            e.Handled = true;
        }

        private async void _chkSHA256_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA256() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.SHA256, fs));
                        _txtSHA256.Text = cksum.SHA256;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            
            e.Handled = true;
        }

        private async void _chkSHA384_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA384() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.SHA384, fs));
                        _txtSHA384.Text = cksum.SHA384;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            
            e.Handled = true;
        }

        private async void _chkSHA512_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null)
                return;

            if (!string.IsNullOrEmpty(_txtFile.Text) && ShouldCalculateSHA512() && System.IO.File.Exists(_txtFile.Text))
            {
                StartBusyOperation();
                try
                {
                    using (FileStream fs = File.Open(_txtFile.Text, FileMode.Open))
                    {
                        var cksum = await Task.Run<Checksum>(() => Checksum.Evaluate(Checksum.Algo.SHA512, fs));
                        _txtSHA512.Text = cksum.SHA512;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                }
                finally
                {
                    EndBusyOperation();
                }
            }
            
            e.Handled = true;
        }

        #endregion UI_INTERACTION
    }
}

