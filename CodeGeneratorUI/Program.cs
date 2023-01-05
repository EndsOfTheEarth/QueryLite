using System;
using System.Windows.Forms;

namespace QueryLite.CodeGeneratorUI {

    static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            Application.ThreadException += Application_ThreadException;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}