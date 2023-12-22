using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace TimerTool
{
    public sealed class InvokeCommandAction : TriggerAction<DependencyObject>
    {
        private string commandName;

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), null);

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), null);

        public static readonly DependencyProperty EventArgsConverterProperty = DependencyProperty.Register("EventArgsConverter", typeof(IValueConverter), typeof(InvokeCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty EventArgsConverterParameterProperty = DependencyProperty.Register("EventArgsConverterParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty EventArgsParameterPathProperty = DependencyProperty.Register("EventArgsParameterPath", typeof(string), typeof(InvokeCommandAction), new PropertyMetadata(null));

        //
        // 摘要:
        //     Gets or sets the name of the command this action should invoke.
        //
        // 值:
        //     The name of the command this action should invoke.
        //
        // 言论：
        //     This property will be superseded by the Command property if both are set.
        public string CommandName
        {
            get
            {
                ReadPreamble();
                return commandName;
            }
            set
            {
                if (CommandName != value)
                {
                    WritePreamble();
                    commandName = value;
                    WritePostscript();
                }
            }
        }

        //
        // 摘要:
        //     Gets or sets the command this action should invoke. This is a dependency property.
        //
        // 值:
        //     The command to execute.
        //
        // 言论：
        //     This property will take precedence over the CommandName property if both are
        //     set.
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        //
        // 摘要:
        //     Gets or sets the command parameter. This is a dependency property.
        //
        // 值:
        //     The command parameter.
        //
        // 言论：
        //     This is the value passed to ICommand.CanExecute and ICommand.Execute.
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        //
        // 摘要:
        //     Gets or sets the IValueConverter that is used to convert the EventArgs passed
        //     to the Command as a parameter.
        //
        // 言论：
        //     If the Microsoft.Xaml.Behaviors.InvokeCommandAction.Command or Microsoft.Xaml.Behaviors.InvokeCommandAction.EventArgsParameterPath
        //     properties are set, this property is ignored.
        public IValueConverter EventArgsConverter
        {
            get
            {
                return (IValueConverter)GetValue(EventArgsConverterProperty);
            }
            set
            {
                SetValue(EventArgsConverterProperty, value);
            }
        }

        //
        // 摘要:
        //     Gets or sets the parameter that is passed to the EventArgsConverter.
        public object EventArgsConverterParameter
        {
            get
            {
                return GetValue(EventArgsConverterParameterProperty);
            }
            set
            {
                SetValue(EventArgsConverterParameterProperty, value);
            }
        }

        //
        // 摘要:
        //     Gets or sets the parameter path used to extract a value from an System.EventArgs
        //     property to pass to the Command as a parameter.
        //
        // 言论：
        //     If the Microsoft.Xaml.Behaviors.InvokeCommandAction.Command propert is set, this
        //     property is ignored.
        public string EventArgsParameterPath
        {
            get
            {
                return (string)GetValue(EventArgsParameterPathProperty);
            }
            set
            {
                SetValue(EventArgsParameterPathProperty, value);
            }
        }

        //
        // 摘要:
        //     Specifies whether the EventArgs of the event that triggered this action should
        //     be passed to the Command as a parameter.
        //
        // 言论：
        //     If the Microsoft.Xaml.Behaviors.InvokeCommandAction.Command, Microsoft.Xaml.Behaviors.InvokeCommandAction.EventArgsParameterPath,
        //     or Microsoft.Xaml.Behaviors.InvokeCommandAction.EventArgsConverter properties
        //     are set, this property is ignored.
        public bool PassEventArgsToCommand { get; set; }

        //
        // 摘要:
        //     Invokes the action.
        //
        // 参数:
        //   parameter:
        //     The parameter to the action. If the action does not require a parameter, the
        //     parameter may be set to a null reference.
        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject == null)
            {
                return;
            }

            ICommand command = ResolveCommand();
            if (command != null)
            {
                object obj = CommandParameter;
                if (obj == null && !string.IsNullOrWhiteSpace(EventArgsParameterPath))
                {
                    obj = GetEventArgsPropertyPathValue(parameter);
                }

                if (obj == null && EventArgsConverter != null)
                {
                    obj = EventArgsConverter.Convert(parameter, typeof(object), EventArgsConverterParameter, CultureInfo.CurrentCulture);
                }

                if (obj == null && PassEventArgsToCommand)
                {
                    obj = parameter;
                }

                if (command.CanExecute(obj))
                {
                    command.Execute(obj);
                }
            }
        }

        private object GetEventArgsPropertyPathValue(object parameter)
        {
            object obj = parameter;
            string[] array = EventArgsParameterPath.Split('.');
            foreach (string name in array)
            {
                obj = obj.GetType().GetProperty(name)!.GetValue(obj, null);
            }

            return obj;
        }

        private ICommand ResolveCommand()
        {
            ICommand result = null;
            if (Command != null)
            {
                result = Command;
            }
            else if (base.AssociatedObject != null)
            {
                PropertyInfo[] properties = base.AssociatedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, CommandName, StringComparison.Ordinal))
                    {
                        result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
                    }
                }
            }

            return result;
        }
    }
}
