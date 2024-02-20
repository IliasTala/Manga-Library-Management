using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.ViewModel
{
    // Classe de base pour les view models
    public partial class BaseViewModel : ObservableObject
    {
        // Propriété indiquant si l'opération en cours est occupée ou non
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        // Propriété indiquant si l'opération en cours n'est pas occupée
        public bool IsNotBusy => !IsBusy;

        // Constructeur de la classe BaseViewModel
        public BaseViewModel()
        {

        }

        // Méthode générique pour définir une propriété et déclencher les événements de modification
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            // Vérifie si la valeur de la propriété a changé
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            // Met à jour la valeur de la propriété et déclenche l'événement OnPropertyChanged
            backingStore = value;
            OnPropertyChanged(propertyName);

            // Appelle la méthode onChanged si elle est spécifiée
            onChanged?.Invoke();

            return true;
        }
    }
}
