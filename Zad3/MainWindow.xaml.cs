using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zad3
{
    // Model reprezentujący element listy filmów/albumów muzycznych
    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        private string title;
        private string director;
        private string publisher;
        private MediaType mediaType;
        private TimeSpan duration;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public string Director
        {
            get { return director; }
            set
            {
                director = value;
                NotifyPropertyChanged();
            }
        }

        public string Publisher
        {
            get { return publisher; }
            set
            {
                publisher = value;
                NotifyPropertyChanged();
            }
        }

        public MediaType MediaType
        {
            get { return mediaType; }
            set
            {
                mediaType = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
 
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Typ wyliczeniowy reprezentujący typ nośnika
    public enum MediaType
    {
        VHS,
        DVD,
        BlueRay,
        Cassette,
        CD,
        Pendrive
    }

    // Klasa przechowująca listę filmów/albumów muzycznych
    [Serializable]
    public class ItemList : ObservableCollection<Item>
    {
        public void Export(string fileName)
        {
            using (FileStream stream = File.Create(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static ItemList Import(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (ItemList)formatter.Deserialize(stream);
            }
        }
    }

    // Główne okno aplikacji
    public partial class MainWindow : Window
    {
        private ItemList itemList;

        public MainWindow()
        {
            InitializeComponent();
            itemList = new ItemList();
            DataContext = itemList;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item();
            EditWindow editWindow = new EditWindow(item);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                itemList.Add(item);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (Item)listBox.SelectedItem;

            if (selectedItem != null)
            {
                EditWindow editWindow = new EditWindow(selectedItem);
                editWindow.Owner = this;
                editWindow.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (Item)listBox.SelectedItem;
            if (selectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć ten element?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    itemList.Remove(selectedItem);
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Plik listy (*.dat)|*.dat";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ItemList importedList = ItemList.Import(openFileDialog.FileName);
                    itemList.Clear();

                    foreach (Item item in importedList)
                    {
                        itemList.Add(item);
                    }

                    MessageBox.Show("Importowanie zakończone pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas importowania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Plik listy (*.dat)|*.dat";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    itemList.Export(saveFileDialog.FileName);
                    MessageBox.Show("Eksportowanie zakończone pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas eksportowania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public partial class EditWindow : Window, INotifyPropertyChanged
    {
        private Item item;

        public EditWindow(Item item)
        {
            InitializeComponent();
            this.item = item;
            DataContext = this.item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void InitializeComponent()
        {
            this.Title = "Edit Item";
            this.Width = 400;
            this.Height = 300;

            Grid grid = new Grid();
            this.Content = grid;

            // Tworzenie kolumn w siatce
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);

            // Tworzenie wierszy w siatce
            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = new GridLength(30);
            RowDefinition rowDef2 = new RowDefinition();
            rowDef2.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDef1);
            grid.RowDefinitions.Add(rowDef2);
            RowDefinition rowDef3 = new RowDefinition();
            rowDef3.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDef3);
            RowDefinition rowDef4 = new RowDefinition();
            rowDef4.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDef4);
            RowDefinition rowDef5 = new RowDefinition();
            rowDef5.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDef5);
            RowDefinition rowDef6 = new RowDefinition();
            rowDef6.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDef6);

            // Tworzenie etykiet
            Label titleLabel = new Label();
            titleLabel.Content = "Title:";
            Grid.SetRow(titleLabel, 0);
            Grid.SetColumn(titleLabel, 0);
            grid.Children.Add(titleLabel);

            Label directorLabel = new Label();
            directorLabel.Content = "Director:";
            Grid.SetRow(directorLabel, 1);
            Grid.SetColumn(directorLabel, 0);
            grid.Children.Add(directorLabel);

            Label publisherLabel = new Label();
            publisherLabel.Content = "Publisher:";
            Grid.SetRow(publisherLabel, 2);
            Grid.SetColumn(publisherLabel, 0);
            grid.Children.Add(publisherLabel);

            Label mediaTypeLabel = new Label();
            mediaTypeLabel.Content = "Media Type:";
            Grid.SetRow(mediaTypeLabel, 3);
            Grid.SetColumn(mediaTypeLabel, 0);
            grid.Children.Add(mediaTypeLabel);

            Label durationLabel = new Label();
            durationLabel.Content = "Duration:";
            Grid.SetRow(durationLabel, 4);
            Grid.SetColumn(durationLabel, 0);
            grid.Children.Add(durationLabel);

            // Tworzenie TextBox i ComboBox
            TextBox titleTextBox = new TextBox();
            titleTextBox.SetBinding(TextBox.TextProperty, new Binding("Title") { Mode = BindingMode.TwoWay });
            Grid.SetRow(titleTextBox, 0);
            Grid.SetColumn(titleTextBox, 1);
            grid.Children.Add(titleTextBox);

            TextBox directorTextBox = new TextBox();
            directorTextBox.SetBinding(TextBox.TextProperty, new Binding("Director") { Mode = BindingMode.TwoWay });
            Grid.SetRow(directorTextBox, 1);
            Grid.SetColumn(directorTextBox, 1);
            grid.Children.Add(directorTextBox);

            TextBox publisherTextBox = new TextBox();
            publisherTextBox.SetBinding(TextBox.TextProperty, new Binding("Publisher") { Mode = BindingMode.TwoWay });
            Grid.SetRow(publisherTextBox, 2);
            Grid.SetColumn(publisherTextBox, 1);
            grid.Children.Add(publisherTextBox);

            ComboBox mediaTypeComboBox = new ComboBox();
            mediaTypeComboBox.ItemsSource = Enum.GetValues(typeof(MediaType));
            mediaTypeComboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding("MediaType") { Mode = BindingMode.TwoWay });
            Grid.SetRow(mediaTypeComboBox, 3);
            Grid.SetColumn(mediaTypeComboBox, 1);
            grid.Children.Add(mediaTypeComboBox);

            TextBox durationTextBox = new TextBox();
            durationTextBox.SetBinding(TextBox.TextProperty, new Binding("Duration") { Mode = BindingMode.TwoWay });
            Grid.SetRow(durationTextBox, 4);
            Grid.SetColumn(durationTextBox, 1);
            grid.Children.Add(durationTextBox);

            // Tworzenie przycisków
            Button saveButton = new Button();
            saveButton.Content = "Save";
            saveButton.Click += SaveButton_Click;
            Grid.SetRow(saveButton, 5);
            Grid.SetColumn(saveButton, 0);
            grid.Children.Add(saveButton);

            Button cancelButton = new Button();
            cancelButton.Content = "Cancel";
            cancelButton.Click += CancelButton_Click;
            Grid.SetRow(cancelButton, 5);
            Grid.SetColumn(cancelButton, 1);
            grid.Children.Add(cancelButton);
        }
    }
}