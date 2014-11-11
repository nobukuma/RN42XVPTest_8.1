using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

using StrawhatNet.Study.RN42XVPTest.ViewModel;

namespace StrawhatNet.Study.RN42XVPTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        private StreamSocket socket;

        // コンストラクター
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarMenuItem_About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            BluetoothControlPanelUtil.ShowBluetoothControlPanel();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DevConnectionPage.xaml", UriKind.Relative));
        }

        private bool GetBoolValue(bool? value)
        {
            return value.HasValue && value.Value;
        }

        private async void ToggleSwitch_Clicked(object sender, RoutedEventArgs e)
        {
            bool[] ledStatus = new bool[] {
                GetBoolValue(led1ToggoleSwitch.IsChecked),
                GetBoolValue(led2ToggoleSwitch.IsChecked),
                GetBoolValue(led3ToggoleSwitch.IsChecked),
                GetBoolValue(led4ToggoleSwitch.IsChecked),
            };
            await CommunicateViaRFCOMM(ledStatus);
        }

        private async void ButtonAllOn_Click(object sender, RoutedEventArgs e)
        {
            await SetAllSwitches(true);
        }

        private async void ButtonAllOff_Click(object sender, RoutedEventArgs e)
        {
            await SetAllSwitches(false);
        }

        private async Task SetAllSwitches(bool value)
        {
            ToggleSwitch[] switches = { led1ToggoleSwitch, led2ToggoleSwitch, led3ToggoleSwitch, led4ToggoleSwitch };
            foreach (var sw in switches)
            {
                sw.IsChecked = value;
            }

            bool[] ledStatus = new bool[] { value, value, value, value };
            await CommunicateViaRFCOMM(ledStatus);
        }

        private async Task CommunicateViaRFCOMM(bool[] ledStatus)
        {
            if (this.socket == null)
            {
                return;
            }

            byte value = (byte)(
                (ledStatus[0] ? 0x08 : 0)
                | (ledStatus[1] ? 0x04 : 0)
                | (ledStatus[2] ? 0x02 : 0)
                | (ledStatus[3] ? 0x01 : 0));

            try
            {
                DataWriter dw = new DataWriter(socket.OutputStream);
                dw.WriteBytes(new byte[] { value });
                var writeBytesSize = await dw.StoreAsync();
                await dw.FlushAsync();
            }
            catch (Exception)
            {
                Disconnect();
            }
            return;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();

            var dataContext = DataContext as MainPageViewModel;
            dataContext.BluetoothDeviceInfo = new BluetoothDeviceInfo();

            return;
        }

        private void Disconnect()
        {
            this.socket.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (PhoneApplicationService.Current.State.ContainsKey(DevConnectionPage.OUTARG_BLUETOOTHDEVINFO))
                {
                    var state = PhoneApplicationService.Current.State;

                    this.socket = state[DevConnectionPage.OUTARG_SOCKET] as StreamSocket;

                    var dataContext = DataContext as MainPageViewModel;
                    dataContext.BluetoothDeviceInfo = state[DevConnectionPage.OUTARG_BLUETOOTHDEVINFO] as BluetoothDeviceInfo;
                }
            }
            else {
                this.socket = null;

                var dataContext = DataContext as MainPageViewModel;
                dataContext.BluetoothDeviceInfo = new BluetoothDeviceInfo();
            }
            return;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            return;
        }

    }
}