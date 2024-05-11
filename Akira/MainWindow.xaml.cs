using Akira.Models.Auxiliary;
using Akira.ViewModels;
using SharpGL;
using System.Windows;

namespace Akira
{
    public partial class MainWindow : Window
    {
        private MainWindowVM mainVM;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowVM();
            mainVM = new MainWindowVM();
        }

        
        public void OpenItem_Click(Scene scene, dynamic fileOpenDialog)
        {
            if (fileOpenDialog.ShowDialog(this) == true)
            {
                var filePath = fileOpenDialog.FileName;

                scene.Load(openGlCtrl.OpenGL, filePath);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            filePopup.IsOpen = true;
        }
       
        private void GlDOpenGLControl_OpenGLDrawraw(object sender, RoutedEventArgs args)
        {
            mainVM.Draw();


        }


    }
}
