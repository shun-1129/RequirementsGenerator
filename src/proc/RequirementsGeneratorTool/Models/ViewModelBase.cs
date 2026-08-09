using System.ComponentModel;
using System.Windows.Input;

namespace RequirementsGeneratorTool.Models
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// INotifyPropertyChanged.PropertyChanged の実装
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// INotifyPropertyChanged.PropertyChangedイベントを発生させる。
        /// </summary>
        /// <param name="propertyName">プロパティ名称</param>
        protected virtual void RaisePropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }

        private Dictionary<string , string> _errorMessageDict = new Dictionary<string, string> ();

        string IDataErrorInfo.Error => string.Join ( Environment.NewLine , _errorMessageDict.Values );

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if ( _errorMessageDict.ContainsKey ( columnName ) )
                {
                    return _errorMessageDict[columnName];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        protected void ClearError ( string propertyName )
        {
            if ( _errorMessageDict.ContainsKey ( propertyName ) )
            {
                _errorMessageDict.Remove ( propertyName );
            }
        }

        protected class _DelegeteCommand : ICommand
        {
            /// <summary>
            /// コマンド本体
            /// </summary>
            private Action<object?> _command;
            /// <summary>
            /// 実行可否
            /// </summary>
            private Func<object? , bool> _canExecute;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="command">コマンド本体</param>
            /// <param name="canExecute">実行可否</param>
            /// <exception cref="ArgumentNullException"></exception>
            public _DelegeteCommand ( Action<object?> command , Func<object? , bool> canExecute )
            {
                if ( canExecute is null )
                {
                    throw new ArgumentNullException ( nameof ( canExecute ) );
                }

                _command = command;
                _canExecute = canExecute;
            }

            /// <summary>
            /// ICommand.Executeの実装
            /// </summary>
            /// <param name="parameter"></param>
            void ICommand.Execute ( object? parameter )
            {
                _command ( parameter );
            }

            // ICommand.Executeの実装
            bool ICommand.CanExecute ( object? parameter )
            {
                if ( _canExecute != null )
                {
                    return _canExecute ( parameter );
                }
                else
                {
                    return true;
                }
            }

            // ICommand.CanExecuteChanged の実装
            event EventHandler? ICommand.CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }

        protected ICommand CreateCommand ( Action<object?> command , Func<object? , bool> canExecute )
        {
            return new _DelegeteCommand ( command , canExecute );
        }
    }
}
