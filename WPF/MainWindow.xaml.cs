using WPF.ViewModel;
using WPFMAIN;

namespace WPF
{
    public partial class MainWindow
    {
        public static int Size;
        public MainWindow()
        {
            SizeControl sizeControl = new SizeControl();
            sizeControl.ShowDialog();
            if (sizeControl.SizeMatrix != 0 && sizeControl.SizeMatrix >= 8)
            {
                Size = sizeControl.SizeMatrix;
            }
            else
            {
                Size = 8;
            }
            InitializeComponent();

            
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel(BoardControl);

            ContentRendered += (_, __) => BoardControl.DrawGrid();

            Closing += (_, __) => ((MainWindowViewModel)DataContext).Closing();
        }

        private void BoardControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
