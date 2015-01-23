using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Sedentary.Framework.Behavior
{
    public class ShowWindowNearTrayBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // AssociatedObject.Loaded += AssociatedObject_Loaded;

            AssociatedObject.ContentRendered += OnAssociatedObjectOnContentRendered;

            SetInitialPos();
        }

        private void OnAssociatedObjectOnContentRendered(object s, EventArgs e)
        {
            SetPosition();
        }

        private void SetInitialPos()
        {
            var window = AssociatedObject;
            var screen = Screen.PrimaryScreen.WorkingArea;

            window.Top = screen.Height;
            window.Left = screen.Width;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ContentRendered -= OnAssociatedObjectOnContentRendered;
        }

        private void SetPosition()
        {
            var window = AssociatedObject;
            var screen = Screen.PrimaryScreen.WorkingArea;

            window.Top = screen.Height - window.ActualHeight;

            var animation = new DoubleAnimation(
                screen.Width,
                screen.Width - window.ActualWidth,
                new Duration(TimeSpan.FromSeconds(0.8d)));

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Left"));

            storyboard.Begin(window);
        }
    }
}