using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace abobaAPP
{
    /// <summary>
    /// Логика взаимодействия для AddNewProductWindow.xaml
    /// </summary>
    public partial class AddNewProductWindow : Window
    {
        private user25Entities context = new user25Entities();

        public AddNewProductWindow()
        {
            InitializeComponent();
            unitTypeComboBox.ItemsSource = context.UnitType.ToList();
            manufacComboBox.ItemsSource = context.ProductManufacturer.ToList();
            suppComboBox.ItemsSource = context.ProductSupplier.ToList();
            categoryComboBox.ItemsSource = context.ProductCategory.ToList();
            if (SystemContext.isEditing)
            {
                LoadData();
                addNewProductButton.Content = "Изменить";
                deleteProductButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            using (var db = new user25Entities())
            {
                Product product = SystemContext.product;
                articleBox.Text = product.ProductArticleNumber;
                nameBox.Text = product.ProductName;
                unitTypeComboBox.SelectedItem = ((List<UnitType>)unitTypeComboBox.ItemsSource).Find(u => u.UnitTypeID == product.UnitTypeID);
                costBox.Text = product.ProductCost.ToString();
                maxDiscBox.Text = product.ProductMaxDiscountAmount.ToString();
                manufacComboBox.SelectedItem = ((List<ProductManufacturer>)manufacComboBox.ItemsSource).Find(pm => pm.ProductManufacturerID == product.ProductManufacturerID);
                suppComboBox.SelectedItem = ((List<ProductSupplier>)suppComboBox.ItemsSource).Find(ps => ps.ProductSupplierID == product.ProductSupplierID);
                categoryComboBox.SelectedItem = ((List<ProductCategory>)categoryComboBox.ItemsSource).Find(pc => pc.ProductCategoryID == product.ProductCategoryID);
                discAmountBox.Text = product.ProductDiscountAmount.ToString();
                quantityBox.Text = product.ProductQuantityInStock.ToString();
                descripBox.Text = product.ProductDescription;
                photoBox.Text = product.ProductPhoto;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.ShowDialog();
        }

        private void addNewProductButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities())
            {
                if (SystemContext.isEditing)
                {
                    foreach (Product product in db.Product)
                    {
                        if (product.ProductID == SystemContext.product.ProductID)
                        {
                            product.ProductArticleNumber = articleBox.Text;
                            product.ProductName = nameBox.Text;
                            product.UnitTypeID = ((UnitType)unitTypeComboBox.SelectedItem).UnitTypeID;
                            product.ProductCost = Convert.ToDecimal(costBox.Text);
                            product.ProductMaxDiscountAmount = Convert.ToByte(maxDiscBox.Text);
                            product.ProductManufacturerID = ((ProductManufacturer)manufacComboBox.SelectedItem).ProductManufacturerID;
                            product.ProductSupplierID = ((ProductSupplier)suppComboBox.SelectedItem).ProductSupplierID;
                            product.ProductCategoryID = ((ProductCategory)categoryComboBox.SelectedItem).ProductCategoryID;
                            product.ProductDiscountAmount = Convert.ToByte(discAmountBox.Text);
                            product.ProductQuantityInStock = Convert.ToByte(quantityBox.Text);
                            product.ProductDescription = descripBox.Text;
                            product.ProductPhoto = photoBox.Text;
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                       
                }
                else
                {
                    Product product = new Product();
                    product.ProductArticleNumber = articleBox.Text;
                    product.ProductName = nameBox.Text;
                    product.UnitTypeID = ((UnitType)unitTypeComboBox.SelectedItem).UnitTypeID;
                    product.ProductCost = Convert.ToDecimal(costBox.Text);
                    product.ProductMaxDiscountAmount = Convert.ToByte(maxDiscBox.Text);
                    product.ProductManufacturerID = ((ProductManufacturer)manufacComboBox.SelectedItem).ProductManufacturerID;
                    product.ProductSupplierID = ((ProductSupplier)suppComboBox.SelectedItem).ProductSupplierID;
                    product.ProductCategoryID = ((ProductCategory)categoryComboBox.SelectedItem).ProductCategoryID;
                    product.ProductDiscountAmount = Convert.ToByte(discAmountBox.Text);
                    product.ProductQuantityInStock = Convert.ToByte(quantityBox.Text);
                    product.ProductDescription = descripBox.Text;
                    product.ProductPhoto = photoBox.Text;
                    db.Product.Add(product);
                }
                db.SaveChanges();
                AdminWindow adminWindow = new AdminWindow();
                this.Close();
                adminWindow.ShowDialog();
            }
            MessageBox.Show("Изменения в базе данных произошли успешно");
        }

        private void deleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities())
            {
                foreach (Product product in db.Product)
                {
                    if (product.ProductID == SystemContext.product.ProductID)
                    {
                        db.Product.Remove(product);
                        db.SaveChanges();
                    }
                }
            }
            MessageBox.Show("Удаление прошло успешно!");
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.ShowDialog();
        }
    }
}
