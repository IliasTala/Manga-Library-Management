using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.ViewModel
{
    public class CompositeViewModel : BaseViewModel
    {
        public MainViewModel MainViewModel { get; }
        public DetailsViewModel DetailsViewModel { get; }

        public CompositeViewModel(MainViewModel mainViewModel, DetailsViewModel detailsViewModel)
        {
            MainViewModel = mainViewModel;
            DetailsViewModel = detailsViewModel;
        }
    }

}
