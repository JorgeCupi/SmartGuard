using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SmartGuard.Core.ViewModels
{
    public class RateViewModel:MvxViewModel
    {
        public ICommand GoToSurvey
        {
            get { return new MvxCommand(() => ShowViewModel<SurveyViewModel>()); }
        }
    }
}
