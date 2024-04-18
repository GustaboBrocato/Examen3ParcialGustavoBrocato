//Creado por Gustavo Brocato #202010030250

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Examen3ParcialGustavoBrocato.ViewModel
{
    public class VerNotasViewModel : INotifyPropertyChanged
    {
        private string? _key;
        public string? Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    OnPropertyChanged(nameof(Key));
                }
            }
        }

        private string? _descripcion;
        public string? Descripcion
        {
            get { return _descripcion; }
            set
            {
                if (_descripcion != value)
                {
                    _descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }

        private DateTime _fecha;
        public DateTime Fecha
        {
            get { return _fecha; }
            set
            {
                if (_fecha != value)
                {
                    _fecha = value;
                    OnPropertyChanged(nameof(Fecha));
                }
            }
        }

        private string? _foto;
        public string? Foto
        {
            get { return _foto; }
            set
            {
                if (_foto != value)
                {
                    _foto = value;
                    OnPropertyChanged(nameof(Foto));
                }
            }
        }

        private string? _audio;
        public string? Audio
        {
            get { return _audio; }
            set
            {
                if (_audio != value)
                {
                    _audio = value;
                    OnPropertyChanged(nameof(Audio));
                }
            }
        }

        private bool _visibilidad;
        public bool Visibilidad
        {
            get { return _visibilidad; }
            set
            {
                if (_visibilidad != value)
                {
                    _visibilidad = value;
                    OnPropertyChanged(nameof(Visibilidad));
                }
            }
        }

        public ICommand? TappedCommand { get; set; }
        public ICommand? AudioCommand { get; set; }
        public ICommand? ImageCommand { get; set; }
        public ICommand? EliminarCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
