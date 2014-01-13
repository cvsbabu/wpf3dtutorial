/*
 * Use this code at your own risk.  This code has no warranty
 * and the author is not responsible for any damage, harm,
 * or dissatisfaction from using or running the code.  Make 
 * sure that you understand the code before running it.
 * 
 * You may re-use this code in whole or in part, but please
 * give the author credit in your code comments.
 * 
 * Mike Hodnick
 * www.kindohm.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using _3DTools;

namespace WPF3DTutorial
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        private void simpleButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();
            SetCamera();

            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Point3D point0 = new Point3D(0, 0, 0);
            Point3D point1 = new Point3D(5, 0, 0);
            Point3D point2 = new Point3D(0, 0, 5);
            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);
            triangleMesh.TriangleIndices.Add(0);
            triangleMesh.TriangleIndices.Add(2);
            triangleMesh.TriangleIndices.Add(1);
            Vector3D normal = new Vector3D(0, 1, 0);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(
                new SolidColorBrush(Colors.DarkKhaki));
            GeometryModel3D triangleModel = new GeometryModel3D(
                triangleMesh, material);
            ModelVisual3D model = new ModelVisual3D();
            model.Content = triangleModel;
            this.mainViewport.Children.Add(model);
        }

        private void cubeButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();
            SetCamera();

            Model3DGroup cube = new Model3DGroup();
            Point3D p0 = new Point3D(0, 0, 0);
            Point3D p1 = new Point3D(5, 0, 0);
            Point3D p2 = new Point3D(5, 0, 5);
            Point3D p3 = new Point3D(0, 0, 5);
            Point3D p4 = new Point3D(0, 5, 0);
            Point3D p5 = new Point3D(5, 5, 0);
            Point3D p6 = new Point3D(5, 5, 5);
            Point3D p7 = new Point3D(0, 5, 5);

            //front side triangles
            cube.Children.Add(CreateTriangleModel(p3, p2, p6));
            cube.Children.Add(CreateTriangleModel(p3, p6, p7));
            //right side triangles
            cube.Children.Add(CreateTriangleModel(p2, p1, p5));
            cube.Children.Add(CreateTriangleModel(p2, p5, p6));
            //back side triangles
            cube.Children.Add(CreateTriangleModel(p1, p0, p4));
            cube.Children.Add(CreateTriangleModel(p1, p4, p5));
            //left side triangles
            cube.Children.Add(CreateTriangleModel(p0, p3, p7));
            cube.Children.Add(CreateTriangleModel(p0, p7, p4));
            //top side triangles
            cube.Children.Add(CreateTriangleModel(p7, p6, p5));
            cube.Children.Add(CreateTriangleModel(p7, p5, p4));
            //bottom side triangles
            cube.Children.Add(CreateTriangleModel(p2, p3, p0));
            cube.Children.Add(CreateTriangleModel(p2, p0, p1));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = cube;
            this.mainViewport.Children.Add(model);
        }

        private Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(
                new SolidColorBrush(Colors.DarkKhaki));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);

            if (normalsCheckBox.IsChecked == true)
                group.Children.Add(BuildNormals(p0, p1, p2, normal));

            if (wireframeCheckBox.IsChecked == true)
            {
                ScreenSpaceLines3D wireframe = new ScreenSpaceLines3D();
                wireframe.Points.Add(p0);
                wireframe.Points.Add(p1);
                wireframe.Points.Add(p2);
                wireframe.Points.Add(p0);
                wireframe.Color = Colors.LightBlue;
                wireframe.Thickness = 3;

                this.mainViewport.Children.Add(wireframe);
            }

            return group;
        }

        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void ClearViewport()
        {
            ModelVisual3D m;
            for (int i = mainViewport.Children.Count - 1; i >= 0; i--)
            {
                m = (ModelVisual3D)mainViewport.Children[i];
                if (m.Content is DirectionalLight == false)
                    mainViewport.Children.Remove(m);
            }
        }

        private void SetCamera()
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            Point3D position = new Point3D(
                Convert.ToDouble(cameraPositionXTextBox.Text),
                Convert.ToDouble(cameraPositionYTextBox.Text),
                Convert.ToDouble(cameraPositionZTextBox.Text)
            );
            Vector3D lookDirection = new Vector3D(
                Convert.ToDouble(lookAtXTextBox.Text),
                Convert.ToDouble(lookAtYTextBox.Text),
                Convert.ToDouble(lookAtZTextBox.Text)
            );
            camera.Position = position;
            camera.LookDirection = lookDirection;
        }

        private Model3DGroup BuildNormals(
    Point3D p0,
    Point3D p1,
    Point3D p2,
    Vector3D normal)
        {
            Model3DGroup normalGroup = new Model3DGroup();
            Point3D p;
            ScreenSpaceLines3D normal0Wire = new ScreenSpaceLines3D();
            ScreenSpaceLines3D normal1Wire = new ScreenSpaceLines3D();
            ScreenSpaceLines3D normal2Wire = new ScreenSpaceLines3D();
            Color c = Colors.Blue;
            int width = 1;
            normal0Wire.Thickness = width;
            normal0Wire.Color = c;
            normal1Wire.Thickness = width;
            normal1Wire.Color = c;
            normal2Wire.Thickness = width;
            normal2Wire.Color = c;
            double num = 1;
            double mult = .01;
            double denom = mult * Convert.ToDouble(normalSizeTextBox.Text);
            double factor = num / denom;
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p0);
            normal0Wire.Points.Add(p0);
            normal0Wire.Points.Add(p);
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p1);
            normal1Wire.Points.Add(p1);
            normal1Wire.Points.Add(p);
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p2);
            normal2Wire.Points.Add(p2);
            normal2Wire.Points.Add(p);

            //Normal wires are not models, so we can't
            //add them to the normal group.  Just add them
            //to the viewport for now...
            this.mainViewport.Children.Add(normal0Wire);
            this.mainViewport.Children.Add(normal1Wire);
            this.mainViewport.Children.Add(normal2Wire);

            return normalGroup;
        }

        private Point3D[] GetRandomTopographyPoints()
        {
            //create a 10x10 topography.
            Point3D[] points = new Point3D[100];
            Random r = new Random();
            double y;
            double denom = 1000;
            int count = 0;
            for (int z = 0; z < 10; z++)
            {
                for (int x = 0; x < 10; x++)
                {
                    System.Threading.Thread.Sleep(1);
                    y = Convert.ToDouble(r.Next(1, 999)) / denom;
                    points[count] = new Point3D(x, y, z);
                    count += 1;
                }
            }
            return points;
        }

        private void topographyButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();
            SetCamera();
            Model3DGroup topography = new Model3DGroup();
            Point3D[] points = GetRandomTopographyPoints();
            for (int z = 0; z <= 80; z = z + 10)
            {
                for (int x = 0; x < 9; x++)
                {
                    topography.Children.Add(
                        CreateTriangleModel(
                                points[x + z],
                                points[x + z + 10],
                                points[x + z + 1])
                    );
                    topography.Children.Add(
                        CreateTriangleModel(
                                points[x + z + 1],
                                points[x + z + 10],
                                points[x + z + 11])
                    );
                }
            }
            ModelVisual3D model = new ModelVisual3D();
            model.Content = topography;
            this.mainViewport.Children.Add(model);
        }



    }
}