using Exam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.MVVM.ModelClass
{
    class MainViewModelClass : ObservableObject
    {
        public RelayCommand NewTestCommand { get; set; }

        public RelayCommand PassTestCommand { get; set; }

        private object _currentView;
        public NewTestModelClass NewTestVM { get; set; }

        public PassTestViewModelClass PassTestVM { get; set; }
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModelClass()
        {
            NewTestVM = new NewTestModelClass();
            PassTestVM = new PassTestViewModelClass();

            CurrentView = PassTestVM;

            NewTestCommand = new RelayCommand (o => { CurrentView = NewTestVM; });

            PassTestCommand = new RelayCommand(o => { CurrentView = PassTestVM; });
        }
    }
}
