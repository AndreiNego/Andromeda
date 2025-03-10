﻿using Andromeda.Common;
using Andromeda.Components;
using Andromeda.Utilities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Andromeda.GameProject
{
    [DataContract]
    class Scene : ViewModelBase
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Project Project { get; private set; }

        private bool _isActive;

        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }
        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new();
        
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }
        private void AddGameEntity(GameEntity entity, int index = -1)
      
        {
            Debug.Assert(!_gameEntities.Contains(entity));
            entity.IsActive = IsActive;
            if (index == -1)
            {
                _gameEntities.Add(entity);
            }
            else
            {
                _gameEntities.Insert(index, entity);
            }
            GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
        }
        private void RemoveGameEntity(GameEntity entity)
        {
            Debug.Assert(_gameEntities.Contains(entity));
            entity.IsActive = false;
            _gameEntities.Remove(entity);
            GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
        }


    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
        {
          //  if (_gameEntities == null) _gameEntities = new ObservableCollection<GameEntity>();
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }
            foreach(var entity in _gameEntities)
            {
                entity.IsActive = IsActive;
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = _gameEntities.Count - 1;
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => AddGameEntity(x,entityIndex),
                    $"Add {x.Name} to {Name}"
                    ));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = _gameEntities.IndexOf(x);
                RemoveGameEntity(x);
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameEntity(x,entityIndex),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name}"
                    ));
            });

        }
        public Scene(Project project,string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }
    }
}
