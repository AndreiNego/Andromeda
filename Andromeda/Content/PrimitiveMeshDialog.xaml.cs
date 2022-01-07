using Andromeda.ContentToolsAPIStructs;
using Andromeda.DllWrappers;
using Andromeda.Editors;
using Andromeda.GameProject;
using Andromeda.Utilities.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Andromeda.Content
{
    /// <summary>
    /// Interaction logic for PrimitiveMeshDialog.xaml
    /// </summary>
    public partial class PrimitiveMeshDialog : Window
    {
        private static readonly List<ImageBrush> _textures = new List<ImageBrush>();

        private void OnPrimitiveType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdatePrimitive();

        private void OnSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdatePrimitive();

        private void OnScalarBox_ValueChanged(object sender, RoutedEventArgs e) => UpdatePrimitive();
        private float Value(ScalarBox scalarBox, float min)
        {
            float.TryParse(scalarBox.Value, out var result);
            return Math.Max(result, min); 
        }

        private void UpdatePrimitive()
        {
            if (!IsInitialized) return;

            var primitiveType = (PrimitiveMeshType)primTypeComboBox.SelectedItem;
            var info = new PrimitiveInitInfo() { Type = primitiveType };
            var smoothingAngle = 0;

            switch(primitiveType)
            {
                case PrimitiveMeshType.Plane:
                {
                    info.SegmentX = (int)xSliderPlane.Value;
                    info.SegmentY = (int)zSliderPlane.Value;
                        info.Size.X = Value(WidthScalarBoxPlane, 0.001f);
                        info.Size.Y = Value(LengthScalarBoxPlane, 0.001f);
                        break;
                }
                case PrimitiveMeshType.Cube:
                    break;
                case PrimitiveMeshType.UvSphere:
                    {
                        info.SegmentX = (int)xSliderUVSphere.Value;
                        info.SegmentY = (int)zSliderUVSphere.Value;
                        info.Size.X = Value(xScalarBoxUVSphere, 0.001f);
                        info.Size.Y = Value(yScalarBoxUVSphere, 0.001f);
                        info.Size.Z = Value(zScalarBoxUVSphere, 0.001f);
                        smoothingAngle = (int)angleSliderUVSphere.Value;
                    }
                    break;
                case PrimitiveMeshType.IcoSphere:
                    break;
                case PrimitiveMeshType.Cylinder:
                    break;
                case PrimitiveMeshType.Capsule:
                    break;
                default:
                    break;
            }
            var geometry = new Geometry();
            geometry.ImportSettings.SmoothingAngle = smoothingAngle;
            ContentToolsAPI.CreatePrimitiveMesh(geometry, info);
            (DataContext as GeometryEditor).SetAsset(geometry);
            OnTexture_CheckBox_Click(textureCheckBox, null);
        }

        private static void LoadTextures()
        {
            var uris = new List<Uri>
            {
                new Uri(@"pack://application:,,,/Resources/PrimitiveMeshView/UVCheckerMap.png"),
                new Uri(@"pack://application:,,,/Resources/PrimitiveMeshView/PlaneTexture.png"),
                new Uri(@"pack://application:,,,/Resources/PrimitiveMeshView/UVCheckerMap.png"),
            };
            _textures.Clear();

            foreach(var uri in uris)
            {
                var resource = Application.GetResourceStream(uri);
                using var reader = new BinaryReader(resource.Stream);
                var data = reader.ReadBytes((int)resource.Stream.Length);
                var imageSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(data);
                imageSource.Freeze();
                var brush = new ImageBrush(imageSource);
                brush.Transform = new ScaleTransform(1, -1, 0.5, 0.5);
                brush.ViewportUnits = BrushMappingMode.Absolute;
                brush.Freeze();
                _textures.Add(brush);
            }
        }

        static PrimitiveMeshDialog()
        {
            LoadTextures();
        }
        public PrimitiveMeshDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdatePrimitive();
        }

        private void OnTexture_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = _textures[(int)primTypeComboBox.SelectedItem];
            if ((sender as CheckBox).IsChecked == true)
            {
                brush = _textures[(int)primTypeComboBox.SelectedItem];
            }
            else brush = Brushes.White;
            GeometryEditor vm = DataContext as GeometryEditor;
            foreach (var mesh in vm.MeshRenderer.Meshes)
            {
                mesh.Diffuse = brush;
            }
        }

        private void OnSave_Button_Click(object sender,  RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog()
            {
                InitialDirectory = Project.Current.ContentPath,
                Filter = "Asset file (*asset)|*.asset"
            };

            if (dlg.ShowDialog() == true)
            {
                Debug.Assert(!string.IsNullOrEmpty(dlg.FileName));
                var asset = (DataContext as IAssetEditor).Asset;
                Debug.Assert(asset != null);
                asset.Save(dlg.FileName);
            }
        }
    }
}
