﻿using Andromeda.Components;
using Andromeda.GameProject;
using Andromeda.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Andromeda.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction;
        private bool _propertyChanged;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, 
            Action<(Transform transform, Vector3)> forEachAction)
        {
           
            if (DataContext is not MSTransform vm)
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }
            var selection = vm.SelectedComponents.Select(x=>selector(x)).ToList();
            return new Action(() =>
            {
                selection.ForEach(x=>forEachAction(x));
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }
        #region UndoRedo

        private Action GetPositionAction() => GetAction(x=> (x, x.Position), (x) => x.transform.Position = x.transform.Position);
        private Action GetRotationAction() => GetAction(x => (x, x.Rotation), (x) => x.transform.Rotation = x.transform.Rotation);
        private Action GetScaleAction() => GetAction(x => (x, x.Scale), (x) => x.transform.Scale = x.transform.Scale);

        private void RecordActions(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }
        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }
        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }
        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseEventArgs e)
        {
            RecordActions(GetPositionAction(), "Position Changed");
        }
        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseEventArgs e)
        {
            RecordActions(GetRotationAction(), "Rotation Changed");
        }
        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseEventArgs e)
        {
            RecordActions(GetRotationAction(), "Scale Changed");
        }
        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyChanged && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
        #endregion
    }
}
