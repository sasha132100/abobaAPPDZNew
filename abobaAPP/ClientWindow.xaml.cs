using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
            searchingBox.Text = "";
            LoadComboBox();
            discountComboBox.SelectedItem = "Показать все";
            initializeProducts("No");
            CheckOutCountInOrder();
            if (!SystemContext.isGuest)
                userNameTextBlock.Text = SystemContext.user.UserLogin;
            else
                userNameTextBlock.Text = "Guest";
        }

        private void LoadComboBox()
        {
            discountComboBox.Items.Add("Скидка 0-9.99%");
            discountComboBox.Items.Add("Скидка 10-14.99%");
            discountComboBox.Items.Add("Скидка 15 и выше");
            discountComboBox.Items.Add("Показать все");
        }

        private void CheckOutCountInOrder()
        {
            using (var db = new user25Entities())
            {
                if (SystemContext.isGuest)
                {
                    if ((from o in db.Order where o.UserID == null select o).FirstOrDefault() != null)
                    {
                        SystemContext.Order = (from o in db.Order where o.UserID == null select o).FirstOrDefault();
                        if ((from op in db.OrderProduct where SystemContext.Order.OrderID == op.OrderID select op).FirstOrDefault() != null)
                        {
                            myOrdersButton.Visibility = Visibility.Visible;
                        }
                        else
                            myOrdersButton.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        myOrdersButton.Visibility = Visibility.Hidden;
                        Order order = new Order();
                        order.OrderStatusID = 1;
                        order.PickupPointID = 18;
                        order.OrderCreateDate = DateTime.Now;
                        order.OrderDeliveryDate = DateTime.Now;
                        order.UserID = null;
                        order.OrderGetCode = (from o in db.Order select o).LastOrDefault().OrderGetCode + 1;
                        db.Order.Add(order);
                        db.SaveChanges();
                        SystemContext.Order = (from o in db.Order where o.OrderGetCode == order.OrderGetCode select o).FirstOrDefault();
                    }    
                }
                else
                {
                    if ((from o in db.Order where SystemContext.user.UserID == o.UserID select o).FirstOrDefault() != null)
                    {
                        SystemContext.Order = (from o in db.Order where SystemContext.user.UserID == o.UserID select o).FirstOrDefault();
                        if ((from op in db.OrderProduct where SystemContext.Order.OrderID == op.OrderID select op).FirstOrDefault() != null)
                        {
                            myOrdersButton.Visibility = Visibility.Visible;
                        }
                        else
                            myOrdersButton.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        myOrdersButton.Visibility = Visibility.Hidden;
                        Order order = new Order();
                        order.OrderStatusID = 1;
                        order.PickupPointID = 18;
                        order.OrderCreateDate = DateTime.Now;
                        order.OrderDeliveryDate = DateTime.Now;
                        order.UserID = SystemContext.user.UserID;
                        order.OrderGetCode = db.Order.OrderByDescending(o => o.OrderID).FirstOrDefault().OrderGetCode + 1;
                        db.Order.Add(order);
                        db.SaveChanges();
                        SystemContext.Order = (from o in db.Order where o.OrderGetCode == order.OrderGetCode select o).FirstOrDefault();
                    }
                }
            }
        }

        private void initializeProducts(string isChanged)
        {
            productViewer.Children.Clear();
            using (var db = new user25Entities())
            {
                List<Product> products;
                try
                {
                    if (searchingBox.Text == "")
                        products = (from p in db.Product select p).ToList<Product>();
                    else
                    {
                        IEnumerable<Product> productSet = (from p in db.Product select p);
                        products = productSet.Where(product => product.ProductName.Contains($"{searchingBox.Text}")).ToList<Product>();
                    }
                    foreach (var product in products)
                    {
                        if (isChanged == "No")
                        {
                            ProductManufacturer productManufacturer = new ProductManufacturer();
                            productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                            LoadComponent(product, productManufacturer.ProductManufacturerName);
                        }
                        else
                        {
                            if (discountComboBox.SelectedItem.ToString() == "Скидка 0-9.99%" && product.ProductDiscountAmount < 10 && product.ProductDiscountAmount >= 0)
                            {
                                ProductManufacturer productManufacturer = new ProductManufacturer();
                                productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                                LoadComponent(product, productManufacturer.ProductManufacturerName);
                            }
                            else if (discountComboBox.SelectedItem.ToString() == "Скидка 10-14.99%" && product.ProductDiscountAmount < 15 && product.ProductDiscountAmount >= 10)
                            {
                                ProductManufacturer productManufacturer = new ProductManufacturer();
                                productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                                LoadComponent(product, productManufacturer.ProductManufacturerName);
                            }
                            else if (discountComboBox.SelectedItem.ToString() == "Скидка 15 и выше" && product.ProductDiscountAmount >= 15)
                            {
                                ProductManufacturer productManufacturer = new ProductManufacturer();
                                productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                                LoadComponent(product, productManufacturer.ProductManufacturerName);
                            }
                            else if (discountComboBox.SelectedItem.ToString() == "Показать все")
                            {
                                ProductManufacturer productManufacturer = new ProductManufacturer();
                                productManufacturer = (from pm in db.ProductManufacturer where product.ProductManufacturerID == pm.ProductManufacturerID select pm).FirstOrDefault();
                                LoadComponent(product, productManufacturer.ProductManufacturerName);
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка2");
                }
                
            }
        }

        private void LoadComponent(Product product, string productManufacturer)
        {
            Grid mainGrid = new Grid() { Background = (Brush)(new BrushConverter().ConvertFrom("Wheat")), Margin = new Thickness(5, 5, 0, 5) };
            Image productImage = new Image() { };
            StackPanel mainStackPanel = new StackPanel();
            Grid discountGrid = new Grid();

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.8, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.1, GridUnitType.Star) });
            mainGrid.Tag = product.ProductID.ToString();
            mainGrid.MouseLeftButtonUp += AddInBucket_Click;

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

        private void AddInBucket_Click(object sender, MouseButtonEventArgs e)
        {
            using (var db = new user25Entities())
            {
                int id = Convert.ToInt32((sender as Grid).Tag.ToString());
                Product product = (from p in db.Product where p.ProductID == id select p).FirstOrDefault();
                if (MessageBox.Show($"Добавить выбранный вами продукт: '{product.ProductName}' в корзину?", "Добавление в корзину", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    OrderProduct orderProduct = new OrderProduct();
                    orderProduct.ProductID = product.ProductID;
                    orderProduct.OrderID = SystemContext.Order.OrderID;
                    orderProduct.Count = 1;
                    db.OrderProduct.Add(orderProduct);
                    db.SaveChanges();
                }
                if ((from op in db.OrderProduct where SystemContext.Order.OrderID == op.OrderID select op).FirstOrDefault() != null)
                    myOrdersButton.Visibility = Visibility.Visible;
            }
        }
            

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.ShowDialog();
        }

        private void myOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow();
            this.Close();
            ordersWindow.ShowDialog();
        }

        private void discount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            initializeProducts("Yes");
        }

        private void searchingBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            initializeProducts("No");
        }
    }
}
