using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenSilver.Samples.Showcase
{
    public class TreeLoader
    {
        private int _timersCount;
        private int _fullyLoadedElementsCount;

        public event EventHandler TreeFullyLoaded;

        public void Initialize(FrameworkElement rootElement, TimeSpan loadDelay)
        {
            SubscribeToLayoutUpdated(rootElement, loadDelay);
        }

        private void SubscribeToLayoutUpdated(FrameworkElement element, TimeSpan loadDelay)
        {
            var timer = new DispatcherTimer { Interval = loadDelay };

            timer.Tick += (s, e) =>
            {
                // If no layout updates have occurred within the interval, the element is considered fully loaded
                element.LayoutUpdated -= OnElementLayoutUpdated1;
                timer.Stop();
                _fullyLoadedElementsCount++;

                // Traverse visual children
                int childCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < childCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i);
                    if (child is FrameworkElement frameworkElement)
                    {
                        SubscribeToLayoutUpdated(frameworkElement, loadDelay);
                    }
                }

                if (_fullyLoadedElementsCount == _timersCount)
                {
                    OnTreeFullyLoaded();
                }
            };

            _timersCount++;

            element.LayoutUpdated += OnElementLayoutUpdated1;
            timer.Start();

            void OnElementLayoutUpdated1(object sender, EventArgs e)
            {
                // Reset the timer whenever a layout update occurs
                timer.Stop();
                timer.Start();
            }
        }

        private void OnTreeFullyLoaded()
        {
            TreeFullyLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void TraverseElements(DependencyObject element, Action<DependencyObject> action)
        {
            action(element);

            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                TraverseElements(child, action);
            }
        }
    }
}
