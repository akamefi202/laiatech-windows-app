using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace laiatech_wpf
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Selection : UserControl
    {
        public String Props
        {
            get => (String)GetValue(PropsProperty);
            set => SetValue(PropsProperty, value);
        }

        public static readonly DependencyProperty PropsProperty
            = DependencyProperty.Register(

                  nameof(Props),
                  typeof(String),
                  typeof(Selection),
                  new PropertyMetadata("")
              );

        public int Value = -1;
        public String[] propList = null;
        public Button[] propButtons = null;

        public EventHandler ValueChanged;
        
        public Selection()
        {
            InitializeComponent();

            IsEnabled = false;
        }

        public void init()
        {
            propList = ((String)this.GetValue(PropsProperty)).Split('*');
            propButtons = new Button[propList.Length];

            for (int i = 0; i < propList.Length; i++)
            {
                String propTitle = propList[i];

                propButtons[i] = new Button();
                propButtons[i].Content = propTitle;

                if (i == Value)
                {
                    propButtons[i].Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                    propButtons[i].Foreground = Brushes.White;
                }
                else
                {
                    propButtons[i].Background = Brushes.White;
                    propButtons[i].Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                }

                propButtons[i].Width = 100;
                propButtons[i].Margin = new Thickness(10, 0, 10, 0);
                propButtons[i].BorderThickness = new Thickness(0);

                Style borderStyle = new Style(typeof(Border));
                borderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(10)));
                propButtons[i].Resources.Add(typeof(Border), borderStyle);

                propButtons[i].Click += new RoutedEventHandler(onPropButton_Click);

                panel.Children.Add(propButtons[i]);
            }
        }

        private void onPropButton_Click(object sender, RoutedEventArgs e)
        {
            Button propButton = sender as Button;

            for (int i = 0; i < propList.Length; i ++)
            {
                if (propList[i] == propButton.Content as String)
                {
                    this.Value = i;
                    propButtons[i].Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                    propButtons[i].Foreground = Brushes.White;
                }
                else
                {
                    propButtons[i].Background = Brushes.White;
                    propButtons[i].Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                }
            }

            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }

        public void setValue(int value)
        {
            this.Value = value;

            for (int i = 0; i < propList.Length; i++)
            {
                if (i == value)
                {
                    propButtons[i].Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                    propButtons[i].Foreground = Brushes.White;
                }
                else
                {
                    propButtons[i].Background = Brushes.White;
                    propButtons[i].Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                }
            }
        }
    }
}
