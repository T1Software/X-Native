namespace XNative.UI.Mvvm
{
    /// <summary>
    /// Base class of every view model of the application.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        private bool _isBusy;

        /// <summary>
        /// Indicates that the view model is running a long operation.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            protected set => SetProperty(ref _isBusy, value);
        }
    }
}
