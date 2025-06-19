using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

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
                OnPropertyChanged();
                RaiseConnectorPositionsChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged();
                RaiseConnectorPositionsChanged();
            }
        }

        public double Width { get; init; }
        public double Height { get; init; }

        public string Name { get; }

        public ObservableCollection<ConnectorViewModel> Inputs { get; } = [];
        public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];

        public NodeViewModel(string name, double x, double y)
        {
            X = x;
            Y = y;
            Name = name;

            Inputs.Add(new ConnectorViewModel(this, 0,12, true, "Вход 1"));
            Inputs.Add(new ConnectorViewModel(this, 1,12, true, "Вход 2"));

            Outputs.Add(new ConnectorViewModel(this, 0, 12, false, "Выход 1"));
            Outputs.Add(new ConnectorViewModel(this, 1, 12, false, "Выход 2"));

            Width = 130;
            Height = CalculateHeight(Inputs.Count, Outputs.Count);

            Inputs.CollectionChanged += (_, _) => RefreshIndices(Inputs);
            Outputs.CollectionChanged += (_, _) => RefreshIndices(Outputs);
        }


        private double CalculateHeight(int inputs, int outputs)
        {
            return inputs >= outputs ? ((inputs * 12) + inputs* 10) : ((outputs * 12) + outputs * 10);
        }

        private void RefreshIndices(ObservableCollection<ConnectorViewModel> collection)
        {
            for (int i = 0; i < collection.Count; i++)
                collection[i].Index = i;

            foreach (var c in collection)
                c.RaiseChanged();

            CommandManager.InvalidateRequerySuggested();
        }

        private void RaiseConnectorPositionsChanged()
        {
            foreach (var input in Inputs)
                input.RaiseChanged();
            foreach (var output in Outputs)
                output.RaiseChanged();
        }
    }
}