﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Utils;
using System.Windows;
using System.Threading.Tasks;

namespace Start9.NodeControl
{
    public sealed class ModuleViewModel : AbstractModelBase
    {
        private Double x = 0;

        private Double y = 0;

        private Int32 zIndex = 0;

        private Size size = Size.Empty;

        private ImpObservableCollection<EntryViewModel> inputConnectors = null;

        private ImpObservableCollection<EntryViewModel> outputConnectors = null;

        private Boolean isSelected = false;

        public static ModuleViewModel Create(Module m)
        {
            var mvm = new ModuleViewModel
            {
                Module = m
            };

            mvm.InputConnectors.AddRange(m.ReceiverContract?.Entries.Select(e => new EntryViewModel(m, e)) ?? Array.Empty<EntryViewModel>());
            mvm.OutputConnectors.AddRange(m.MessageContract?.Entries.Select(e => new EntryViewModel(m, e)) ?? Array.Empty<EntryViewModel>());

            return mvm;
        }

        ModuleViewModel()
        {
            
        }

        public Module Module { get; private set; }
        
        public Double X
        {
            get => x;
            set
            {
                if (x == value)
                {
                    return;
                }

                x = value;

                OnPropertyChanged(nameof(X));
            }
        }

        public Double Y
        {
            get => y;
            set
            {
                if (y == value)
                {
                    return;
                }

                y = value;

                OnPropertyChanged(nameof(Y));
            }
        }

        public Int32 ZIndex
        {
            get => zIndex;
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged(nameof(ZIndex));
            }
        }

        public Size Size
        {
            get => size;
            set
            {
                if (size == value)
                {
                    return;
                }

                size = value;

                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> SizeChanged;
        public ImpObservableCollection<EntryViewModel> InputConnectors
        {
            get
            {
                if (inputConnectors == null)
                {
                    inputConnectors = new ImpObservableCollection<EntryViewModel>();
                    inputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsAdded);
                    inputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsRemoved);
                }

                return inputConnectors;
            }
        }

        public ImpObservableCollection<EntryViewModel> OutputConnectors
        {
            get
            {
                if (outputConnectors == null)
                {
                    outputConnectors = new ImpObservableCollection<EntryViewModel>();
                    outputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsAdded);
                    outputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(OutputConnectors_ItemsRemoved);
                }

                return outputConnectors;
            }
        }
        public ICollection<MessagePathViewModel> AttachedConnections
        {
            get
            {
                var attachedConnections = new List<MessagePathViewModel>();

                foreach (var connector in this.InputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                foreach (var connector in this.OutputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        public Boolean IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;

                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private void inputConnectors_ItemsAdded(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (EntryViewModel connector in e.Items)
            {
                connector.ParentNode = this;
            }
        }

        private void inputConnectors_ItemsRemoved(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (EntryViewModel connector in e.Items)
            {
                connector.ParentNode = null;
            }
        }

        private void outputConnectors_ItemsAdded(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (EntryViewModel connector in e.Items)
            {
                connector.ParentNode = this;
            }
        }

        private void OutputConnectors_ItemsRemoved(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (EntryViewModel connector in e.Items)
            {
                connector.ParentNode = null;
            }
        }

    }
}
