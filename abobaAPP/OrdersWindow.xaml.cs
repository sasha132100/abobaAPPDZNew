using System;
using System.Collections.Generic;
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

namespace abobaAPP
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        public OrdersWindow()
        {
            InitializeComponent();
            LoadComponents();
            if (!SystemContext.isGuest)
                userNameTextBlock.Text = SystemContext.user.UserLogin;
            else
                userNameTextBlock.Text = "Guest";
        }

        private void initializeProducts()
        {
            productViewer.Children.Clear();
            using (var db = new user25Entities())
            {
                List<OrderProduct> orderProducts;
                try
                {
                    IEnumerable<OrderProduct> orderProductSet = (from p in db.OrderProduct select p);
                    orderProducts = orderProductSet.Where(orderProduct => orderProduct.OrderID.ToString().Contains(SystemContext.Order.OrderID.ToString())).ToList<OrderProduct>();
                    foreach (var product in orderProducts)
                    {
                        SystemContext.bucketList.Add((from p in db.Product where product.ProductID == p.ProductID select p).FirstOrDefault());
                    }
                    foreach (var product in SystemContext.bucketList)
                    {
                        ProductManufacturer productManufacturer = new ProductManufacturer();
                        productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                        LoadProducts(product, productManufacturer.ProductManufacturerName);
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка2");
                }
            }
        }

        private void LoadComponents()
        {
            initializeProducts();
            LoadTotalPrice();
            LoadTotalDiscount();
        }

        private void LoadTotalPrice()
        {
            int tp = 0;
            foreach (var product in SystemContext.bucketList)
            {
                tp += (Int32)product.ProductCost;
            }
            currentPriceTextBlock.Text += tp.ToString();
        }

        private void LoadTotalDiscount()
        {
            int td = 0;
            foreach (var product in SystemContext.bucketList)
            {
                td += (Int32)product.ProductDiscountAmount;
            }
            currentDiscountTextBlock.Text += td.ToString();
        }

        private void LoadProducts(Product product, string productManufacturer)
        {
            Grid mainGrid = new Grid() { Background = (Brush)(new BrushConverter().ConvertFrom("Wheat")), Margin = new Thickness(5, 5, 0, 5) };
            Image productImage = new Image() { };
            StackPanel mainStackPanel = new StackPanel();
            Grid discountGrid = new Grid();

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.8, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.1, GridUnitType.Star) });
            mainGrid.Tag = product.ProductID.ToString();
            mainGrid.MouseLeftButtonUp += DeleteFromBucket_Click;

            Grid.SetColumn(productImage, 0);
            if (product.ProductPhoto == "" || product.ProductPhoto == null)
                productImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/picturePlug.png"));
            else
                productImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/{product.ProductPhoto}"));

            Grid.SetColumn(mainStackPanel, 1);
            TextBlock txtName = new TextBlock() { Text = "Наименование товара: ", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            TextBlock txtDescription = new TextBlock() { Text = "Описание товара: ", Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            TextBlock txtManufacturer = new TextBlock() { Text = "Производитель: ", Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            TextBlock txtCost = new TextBlock() { Text = "Цена: ", Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            txtName.Inlines.Add(new TextBlock() { Text = $" {product.ProductName}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            txtDescription.Inlines.Add(new TextBlock() { Text = $" {product.ProductDescription}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            txtManufacturer.Inlines.Add(new TextBlock() { Text = $" {productManufacturer}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            txtCost.Inlines.Add(new TextBlock() { Text = $" {product.ProductCost}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });

            Grid.SetColumn(discountGrid, 2);
            TextBlock txtDiscount = new TextBlock() { Text = "Размер скидки: ", Margin = new Thickness(0, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            txtDiscount.Inlines.Add(new TextBlock() { Text = $"{product.ProductDiscountAmount}%", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });

            mainStackPanel.Children.Add(txtName);
            mainStackPanel.Children.Add(txtDescription);
            mainStackPanel.Children.Add(txtManufacturer);
            mainStackPanel.Children.Add(txtCost);
            discountGrid.Children.Add(txtDiscount);
            mainGrid.Children.Add(productImage);
            mainGrid.Children.Add(mainStackPanel);
            mainGrid.Children.Add(discountGrid);
            productViewer.Children.Add(mainGrid);
        }

        private void DeleteFromBucket_Click(object sender, MouseButtonEventArgs e)
        {
            using (var db = new user25Entities())
            {
                int id = Convert.ToInt32((sender as Grid).Tag.ToString());
                Product product = (from p in db.Product where p.ProductID == id select p).FirstOrDefault();
                if (MessageBox.Show($"Удалить выбранный вами продукт: '{product.ProductName}' из корзины?", "Удаление из корзины", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Product.Remove(product);
                    db.SaveChanges();
                    SystemContext.bucketList.Remove(SystemContext.bucketList.Single(p => p.ProductID == id));
                    LoadComponents();
                    MessageBox.Show("Удаление прошло успешно!");
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            this.Close();
            clientWindow.ShowDialog();
        }

        private void createNewOrderTalonButton_Click(object sender, RoutedEventArgs e)
        {
            var order = SystemContext.Order;
            var app = new Microsoft.Office.Interop.Excel.Application
            {
                SheetsInNewWorkbook = 1
            };

            var workbook = app.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel.Worksheet worksheet = app.Worksheets.Item[1];
            worksheet.Name = "Card";

            worksheet.Cells[1][1] = "Order number";
            worksheet.Cells[1][2] = "Product list";
            worksheet.Cells[1][2] = "Product list";
            worksheet.Cells[1][3] = "Total cost";

            worksheet.Cells[2][1] = order.OrderID;

            var fullProductList = string.Empty;
            fullProductList = order.OrderProduct.Aggregate(fullProductList,
                (current, product) => current + $"{product.Product.ProductName}\n");
            worksheet.Cells[2][2] = fullProductList;
            worksheet.Cells[2][3] = order.OrderProduct.Sum(p => p.Product.ProductCost);

            worksheet.Columns.AutoFit();

            app.Visible = true;

            app.Application.ActiveWorkbook.SaveAs(@"C:\Users\sasha\Downloads\abobaAPP-master (2)\abobaAPP-master\test.xlsx");

            var excelDocument = app.Workbooks.Open(@"C:\Users\sasha\Downloads\abobaAPP-master (2)\abobaAPP-master\test.xlsx");

            excelDocument.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, @"C:\Users\sasha\Downloads\abobaAPP-master (2)\abobaAPP-master\test.pdf");
            excelDocument.Close(false, "", false);
            app.Quit();
        }
    }
}
