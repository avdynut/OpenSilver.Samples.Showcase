using CSHTML5.Internal;
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
            WaitElementForLoaded(rootElement, loadDelay);
        }

        private void WaitElementForLoaded(FrameworkElement element, TimeSpan loadDelay)
        {
            var timer = new DispatcherTimer { Interval = loadDelay };

            timer.Tick += (s, e) =>
            {
                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                {
                    return;
                }

                timer.Stop();
                _fullyLoadedElementsCount++;

                // Traverse visual children
                int childCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < childCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i);
                    if (child is FrameworkElement frameworkElement)
                    {
                        WaitElementForLoaded(frameworkElement, loadDelay);
                    }
                }

                if (_fullyLoadedElementsCount == _timersCount)
                {
                    OnTreeFullyLoaded();
                }
            };

            _timersCount++;

            timer.Start();
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
