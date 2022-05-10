namespace example
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;

    public partial class MainForm : Form
    {
        private AppDomain = AppDomain.CreateDomain("asmDomain");

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads a Byte array as raw assmebly then loads and creates defined object from 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [LoaderOptimizationAttribute(LoaderOptimization.MultiDomain)]
        private void loadAsmObject(object sender, TileItemEventArgs e)
        {
            Byte[] rawAssembly = getFileAsm(); // Load the bytes however you wish.

            try
            {
                AppDomain.Unload(appDomain);

                appDomain = AppDomain.CreateDomain("asmDomain");

                AppDomainBridge isolationDomainLoadContext = (AppDomainBridge)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof (AppDomainBridge).ToString());

                // Form is MarshalByRefObject type for the current AppDomain
                MyObject obj = isolationDomainLoadContext.ExecuteFromAssembly(rawAssembly, "MyNamespace.MyObject"/*, new object[] { "Arg1", "Arg2" } Optional args*/) as MyObject;

                obj.callMethod();
            }

            catch (Exception Ex)
            {
                MessageBox.Show("Failed to load Object!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Acts as a shared app domain so we can use AppDomain.CurrentDomain.Load without errors.
        /// </summary>
        private class AppDomainBridge : MarshalByRefObject
        {
            public Object ExecuteFromAssembly(Byte[] rawAsm, string typeName, params object[] args)
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(rawAssembly: rawAsm);

                return Activator.CreateInstance(assembly.GetType(typeName), args);
            }
        }
    }
}
