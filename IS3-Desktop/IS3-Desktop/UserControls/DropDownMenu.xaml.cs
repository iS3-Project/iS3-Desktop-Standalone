using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace DigitalGeotec.UserControls
{
    /// <summary>
    /// Resizable and draggable custom window control
    /// </summary>
    public class DropDownMenu : ContentControl
    {
        public DropDownMenu()
        {
            DefaultStyleKey = typeof(DropDownMenu);
            this.MouseEnter += DropDownMenu_MouseEnter;
            this.MouseLeave += DropDownMenu_MouseLeave;
        }

        private void DropDownMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            GoToState(true, "Hidden");
        }

        private void DropDownMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            GoToState(true, "Visible");
        }


        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (oldContent != null && oldContent is UIElement)
            {
                (oldContent as UIElement).MouseEnter -= DropDownMenu_MouseEnter;
                (oldContent as UIElement).MouseLeave -= DropDownMenu_MouseLeave;
            }
            if (newContent != null && newContent is UIElement)
            {
                (newContent as UIElement).MouseEnter += DropDownMenu_MouseEnter;
                (newContent as UIElement).MouseLeave += DropDownMenu_MouseLeave;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            bool isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            GoToState(false, isDesignMode ? "Visible" : "Hidden"); //Show submenu when in designmode
        }

        /// <summary>
        /// Identifies the <see cref="MenuContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuHeaderProperty =
                        DependencyProperty.Register("MenuHeader", typeof(object), typeof(DropDownMenu), null);

        /// <summary>
        /// Gets or sets MenuContent.
        /// </summary>
        public object MenuHeader
        {
            get { return (object)GetValue(MenuHeaderProperty); }
            set { SetValue(MenuHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template that is used to display the content of the
        /// control's header.
        /// </summary>
        /// <value>
        /// The template that is used to display the content of the control's
        /// header. The default is null.
        /// </value>
        public DataTemplate MenuHeaderTemplate
        {
            get { return (DataTemplate)GetValue(MenuHeaderTemplateProperty); }
            set { SetValue(MenuHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuHeaderTemplateProperty =
                DependencyProperty.Register(
                        "MenuHeaderTemplate",
                        typeof(DataTemplate),
                        typeof(DropDownMenu), null);

    }
}