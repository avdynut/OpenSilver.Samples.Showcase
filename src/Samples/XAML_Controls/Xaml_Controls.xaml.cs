using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilver.Samples.Showcase
{
    public partial class Xaml_Controls : UserControl
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly TreeLoader _treeLoader;

        public Xaml_Controls()
        {
            this.InitializeComponent();

#if OPENSILVER
            NonModalChildWindow.Visibility = Visibility.Collapsed;
#endif
            ScrollBarDemo.Visibility = Visibility.Collapsed;
            ThumbDemo.Visibility = Visibility.Collapsed;
            FrameDemo.Visibility = Visibility.Collapsed; // The Showcase already uses a Frame to change pages anyway

            PrintLog("Xaml_Controls()");
            Loaded += OnXaml_Controls_Loaded;

            _treeLoader = new TreeLoader();
            _treeLoader.TreeFullyLoaded += TreeLoader_TreeFullyLoaded;
            _treeLoader.Initialize(Application.Current.MainWindow, TimeSpan.FromMilliseconds(10));
        }

        private async void OnXaml_Controls_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(5000);
            ApplyTreeElementsPostProcessing(Application.Current.MainWindow);
        }

        private void ApplyTreeElementsPostProcessing(DependencyObject element)
        {
            var elementsCount = 0;
            TraverseNextElement(element);
            PrintLog($"Elements: {elementsCount}");

            void TraverseNextElement(DependencyObject el)
            {
                elementsCount++;
                //Some additional post processing.
                //For example, we restrict popups to the view port area.
                //if (el is Popup popup)
                //{
                //    PopupManager.RestrictPopup(popup);
                //}

                var count = VisualTreeHelper.GetChildrenCount(el);
                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(el, i);
                    TraverseNextElement(child);
                }
            }
        }

        private void TreeLoader_TreeFullyLoaded(object sender, EventArgs e)
        {
            var count = 0;
            _treeLoader.TraverseElements(Application.Current.MainWindow, element =>
            {
                count++;
            });
            PrintLog($"TreeLoader_TreeFullyLoaded {count}");
        }

        private void PrintLog(string message)
        {
            Console.WriteLine($"{_stopwatch.ElapsedMilliseconds} ms: {message}");
        }
    }
}
