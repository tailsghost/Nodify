using Nodify.Helpers;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Nodify.ViewModels
{
    public class NodeViewModel : BaseViewModel
    {
        private double _x, _y;
        public double X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged(nameof(X));
                foreach (var c in Connectors)
                    c.RaiseChanged();
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged(nameof(Y));
                foreach (var c in Connectors)
                    c.RaiseChanged();
            }
        }

        public double Width { get; } = 80;
        public double Height { get; } = 50;

        public string Name { get; }

        public ObservableCollection<ConnectorViewModel> Connectors { get; } = [];

        public ICommand AddConnectorCmd { get; }
        public ICommand RemoveConnectorCmd { get; }

        public NodeViewModel(string name, double x, double y)
        {
            X = x;
            Y = y;
            Name = name;

            Connectors.Add(new ConnectorViewModel(this, 0));
            Connectors.Add(new ConnectorViewModel(this, 1));

            AddConnectorCmd = new RelayCommand(_ => Add(), _ => Connectors.Count < 10);
            RemoveConnectorCmd = new RelayCommand(_ => Remove(), _ => Connectors.Count > 2);

            Connectors.CollectionChanged += (_, _) =>
            {
                for (var i = 0; i < Connectors.Count; i++)
                    Connectors[i].Index = i;

                foreach (var c in Connectors)
                    c.RaiseChanged();

                CommandManager.InvalidateRequerySuggested();
            };
        }

        private void Add() => Connectors.Add(new ConnectorViewModel(this, Connectors.Count));
        private void Remove() => Connectors.RemoveAt(Connectors.Count - 1);
    }
}