using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EveWP8.Extensions
{
    public static class VisualTreeExtensions
    {

        public static T GetVisualChild<T>(this UIElement parent) where T : UIElement
        {
            T child = null;

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < numVisuals; i++)
            {
                var element = (UIElement)VisualTreeHelper.GetChild(parent, i);

                child = element as T ?? GetVisualChild<T>(element);

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }


        /// <summary>
        /// Retrieves the first logical child of a specified type using a 
        /// breadth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the queue 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <param name="applyTemplates">Specifies whether to apply templates on the traversed framework elements</param>
        /// <returns>The first logical child of the framework element of the specified type.</returns>
        public static T GetFirstLogicalChildByType<T>(this FrameworkElement parent, bool applyTemplates)
            where T : FrameworkElement
        {
            var queue = new Queue<FrameworkElement>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                var elementAsControl = element as Control;

                if (applyTemplates && elementAsControl != null)
                {
                    elementAsControl.ApplyTemplate();
                }

                if (element is T && element != parent)
                {
                    return (T)element;
                }

                foreach (var visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves all the logical children of a specified type using a 
        /// breadth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the queue 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <param name="applyTemplates">Specifies whether to apply templates on the traversed framework elements</param>
        /// <returns>The logical children of the framework element of the specified type.</returns>
        public static IEnumerable<T> GetLogicalChildrenByType<T>(this FrameworkElement parent, bool applyTemplates)
            where T : FrameworkElement
        {
            if (applyTemplates && parent is Control)
            {
                ((Control)parent).ApplyTemplate();
            }

            var queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();

                if (applyTemplates && element is Control)
                {
                    ((Control)element).ApplyTemplate();
                }

                if (element is T)
                {
                    yield return (T)element;
                }

                foreach (var visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }
        }


        /// <summary>
        /// Retrieves all the visual children of a framework element.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The visual children of the framework element.</returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }





        /// <summary>
        /// Retrieves all the logical children of a framework element using a 
        /// breadth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the queue 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        public static IEnumerable<FrameworkElement> GetLogicalChildrenBreadthFirst(this FrameworkElement parent)
        {
            var queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();

                yield return element;

                foreach (var visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }
        }


    }
}
