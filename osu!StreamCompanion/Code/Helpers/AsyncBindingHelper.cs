using System.ComponentModel;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Helpers
{
    /// <summary>
    /// A helper class for creating a binding on an object that may be changed
    /// asynchronously from the bound UI thread.
    /// </summary>
    public class AsyncBindingHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// Get a binding instance that can invoke a property change
        /// on the UI thread, regardless of the originating thread
        /// </summary>
        /// <param name="bindingControl">The UI control this binding is added to</param>
        /// <param name="propertyName">The property on the UI control to bind to</param>
        /// <param name="bindingSource">The source INotifyPropertyChanged to be
        /// observed for changes</param>
        /// <param name="dataMember">The property on the source to watch</param>
        /// <returns></returns>
        public static Binding GetBinding(Control bindingControl,
                                          string propertyName,
                                          INotifyPropertyChanged bindingSource,
                                          string dataMember)
        {
            AsyncBindingHelper helper
              = new AsyncBindingHelper(bindingControl, bindingSource, dataMember);
            return new Binding(propertyName, helper, "Value");
        }

        Control bindingControl;
        INotifyPropertyChanged bindingSource;
        string dataMember;

        private AsyncBindingHelper(Control bindingControl,
                                    INotifyPropertyChanged bindingSource,
                                    string dataMember)
        {
            this.bindingControl = bindingControl;
            this.bindingSource = bindingSource;
            this.dataMember = dataMember;
            bindingSource.PropertyChanged
              += new PropertyChangedEventHandler(bindingSource_PropertyChanged);
        }

        void bindingSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null && e.PropertyName == dataMember)
            {
                if (bindingControl.InvokeRequired)
                {
                    bindingControl.BeginInvoke(
                      new PropertyChangedEventHandler(bindingSource_PropertyChanged),
                      sender,
                      e);
                    return;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// The current value of the data sources' datamember
        /// </summary>
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public object Value
        {
            get
            {
                return bindingSource.GetType().GetProperty(dataMember)
                  .GetValue(bindingSource, null);
            }
            set
            {
                bindingSource.GetType().GetProperty(dataMember)
                    .SetValue(bindingSource, value, null);
            }
        }
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Event fired when the dataMember property on the data source is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
