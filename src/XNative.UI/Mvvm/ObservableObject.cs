using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XNative.UI.Mvvm
{
    /// <summary>
    /// Minimal <see cref="INotifyPropertyChanged"/> implementation shared by
    /// the view models of the application.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Assigns a new value to a backing field and raises
        /// <see cref="PropertyChanged"/> when the value actually changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <returns><c>true</c> when the value changed; otherwise <c>false</c>.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
