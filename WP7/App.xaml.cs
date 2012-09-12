using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using MonkeySpace.Core;

// Understanding the Windows Phone Application Execution Model
// http://windowsteamblog.com/windows_phone/b/wpdev/archive/2010/07/16/understanding-the-windows-phone-application-execution-model-tombstoning-launcher-and-choosers-and-few-more-things-that-are-on-the-way-part-2.aspx

namespace MonkeySpace
{
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;
            
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // Copy files into iso-storage if required
            if (true)//(!IsolatedStorageSettings.ApplicationSettings.Contains("LastConferenceCode"))
            {   // THIS SHOULD ONLY EVER RUN ONCE !!!
                Debug.WriteLine("Creating files that don't exist in iso-storage...");
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists("monkeyspace12"))
                    {
                        store.CreateDirectory("monkeyspace12");
                        MoveResourceToFile(store, @"Data\sessions.json", @"monkeyspace12\sessions.json");
                    }
                    

                    var ci = new ConferenceInfo {
                        Code = "monkeyspace12",
                        DisplayLocation = "Boston",
                        DisplayName = "MonkeySpace"
                                                    ,
                        StartDate = new DateTime(2012, 10, 17),
                        EndDate = new DateTime(2012, 10, 19)
                    };
                    App.ViewModel.ConfItem = ci;


                    var json = LoadText(@"monkeyspace12\sessions.json");
                    MonkeySpace.Core.ConferenceManager.LoadFromString(json);
                }
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("LastConferenceCode"))
                    IsolatedStorageSettings.ApplicationSettings.Add("LastConferenceCode", "monkeyspace12");

            }
            else
            {
                Debug.WriteLine("Files already exist in iso-storage...");
            }
            
            //
            //HACK: This is just for debugging  ###############
            IsoStorageTest.IsoViewer.GetIsolatedStorageView("*", IsolatedStorageFile.GetUserStoreForApplication() );
            
        }
      
        void MoveResourceToFile (IsolatedStorageFile store, string from, string to)
        {
            Uri uri = new Uri(from, UriKind.Relative); 
            StreamResourceInfo sri = App.GetResourceStream(uri);
            StreamReader sr = new StreamReader(sri.Stream);
            var txt= sr.ReadToEnd();
            sr.Close();
            using (var stream = new IsolatedStorageFileStream(to, FileMode.Create, FileAccess.Write, store))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

     
        private byte[] _imageBytes = null;
        private void SaveToLocalStorage(IsolatedStorageFile store, string from, string to)
        {

            Uri uri = new Uri(from, UriKind.Relative);
            BitmapImage bitmap = new BitmapImage(uri); 
            
            
            if (_imageBytes == null)
            {
                return;
            }

            var isoFile = store; // IsolatedStorageFile.GetUserStoreForApplication();
            
            string filePath = to;
            using (var stream = isoFile.CreateFile(filePath))
            {
                stream.Write(_imageBytes, 0, _imageBytes.Length);
            }
        }


        public static void CreateDirectory(string directoryName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())           
            {
                store.CreateDirectory(directoryName);
            }
        }
        public static string LoadText(string fileName) 
        {
            var text = "";
            using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication()) {
                if (storageFile.FileExists(fileName)) {
                    using (var stream
                        = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, storageFile)) {
                        using (var reader = new StreamReader(stream)) {
                            text = reader.ReadToEnd();
                        }
                    }
                } 
            }
            return text;
        }
        public static void SaveText(string fileName, string text)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, store))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        public static void Save<T>(string fileName, T objectToSave)
		{
			using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
			//using (var stream
			//	= new IsolatedStorageFileStream(fileName, FileMode.Create, storageFile))
            {
                // http://social.msdn.microsoft.com/Forums/en-US/windowsphone7series/thread/3080021c-3960-49a0-ac5b-ebf2680592e1
                MemoryStream ms = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(ms, objectToSave);
                ms.Seek(0, SeekOrigin.Begin);
                using (ms)
                {
                    IsolatedStorageFileStream file_stream = storageFile.CreateFile(fileName);
                    if (file_stream == null) throw new Exception();
                    int READ_CHUNK = 1024 * 1024;
                    int WRITE_CHUNK = 1024 * 1024;
                    byte[] buffer = new byte[READ_CHUNK];
                    while (true)
                    {
                        int read = ms.Read(buffer, 0, READ_CHUNK);
                        if (read <= 0) break;
                        int to_write = read;
                        while (to_write > 0)
                        {
                            file_stream.Write(buffer, 0, Math.Min(to_write, WRITE_CHUNK));
                            to_write -= Math.Min(to_write, WRITE_CHUNK);
                        }
                    }

                    file_stream.Close();
                }
			}
		}
        public static T Load<T>(string fileName) where T : new()

        {
            using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storageFile.FileExists(fileName))
                {
                    using (var stream
                        = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, storageFile))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            return reader.EndOfStream
                                ? new T()
                                : (T)serializer.Deserialize(reader);
                        }
                    }
                }
                else
                {
                    return new T();
                }

            }
        }


        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        public void SaveFavorites()
        {
            var code = (string)IsolatedStorageSettings.ApplicationSettings["LastConferenceCode"];
            App.CreateDirectory(code);
            App.Save<ObservableCollection<MonkeySpace.Core.Session>>(code + "\\Favorites.xml", App.ViewModel.FavoriteSessions);
        }
        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
            SaveFavorites();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SaveFavorites();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            // If something breaks, it's probably due to bad data, so fallback to the 'build in' conference
            IsolatedStorageSettings.ApplicationSettings["LastConferenceCode"] = "monkeyspace12";
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}