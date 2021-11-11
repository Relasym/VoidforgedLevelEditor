using System;
using System.IO;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum GameObjectButtons
    {
        Wall,
        Floor,
        Door,
        Enemy1,
        Enemy2,
        Player,
        GameStart,
        GameEnd,
        LevelTransition
    }

    public enum GameObjectTypes
    {
        GameStart,
        GameEnd,
        LevelTransition,
        Wall,
        Floor,
        Door,
        Player,
        Enemy1,
        Enemy2
    }


    public partial class MainWindow : Window
    {
        public Game CurrentGame { get; set; }
        public Level CurrentLevel { get; set; }
        public Button SelectedButton { get; set; }
        public GameObjectTypes SelectedObjectType { get; set; }
        public Dictionary<GameObjectTypes, BitmapImage> Images { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            //create Test game
            CurrentGame = CreateTestGame();
            CurrentLevel = CurrentGame.GetLevels()[0];

            Images = new();
            BitmapImage bitmapImage = new();
            //bitmapImage.UriSource=
            //Images.Add(GameObjectTypes.Wall,new B

            //SelectedButton = null;
            UpdateView();
        }

        private void New_File_Click(object sender, RoutedEventArgs e)
        {
            CurrentGame = new Game();
            UpdateView();
        }

        private void Open_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentGame = Game.FromJson(File.ReadAllText(openFileDialog.FileName));
            }
            UpdateView();
        }

        private void Save_File_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentGame.ToJson());
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //TODO verify file is saved or uploaded
            Close();
        }

        private void Database_Load_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement load from database
        }

        private void Database_Upload_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement upload to database
        }

        private void Verify_Current_Level_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement Level verification
        }

        private void Verify_All_Levels_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement verification
            MessageBox.Show(CurrentGame.Verify().ToString());
        }

        private void Map_View_Click(object sender, RoutedEventArgs e)
        {
            mapViewBtn.FontWeight = FontWeights.Bold;
            levelViewBtn.FontWeight = FontWeights.Normal;
            levelViewSidePanel.Visibility = Visibility.Hidden;
            mapViewSidePanel.Visibility = Visibility.Visible;


            //start serialisation test code
            testTextBlock.TextWrapping = TextWrapping.Wrap;

            testTextBlock.Text = CurrentGame.ToJson();
            //end serialisation test code
        }

        private void Level_View_Click(object sender, RoutedEventArgs e)
        {
            levelViewBtn.FontWeight = FontWeights.Bold;
            mapViewBtn.FontWeight = FontWeights.Normal;
            mapViewSidePanel.Visibility = Visibility.Hidden;
            levelViewSidePanel.Visibility = Visibility.Visible;
        }

        private void UpdateView()
        {
            currentLevelListBox.Items.Clear();
            foreach (Level level in CurrentGame.Levels)
            {
                currentLevelListBox.Items.Add(level.Name);
            }
            gameNameTextBox.Text = CurrentGame.Name;
            gameModificationDateLabel.Content = CurrentGame.LastModification;
        }

        private void New_Level_Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentGame.AddLevel(new Level("Unnamed Level"));
            UpdateView();
        }

        private void GameNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentGame is not null && gameNameTextBox is not null) //otherwise this gets called during initialization
            {
                CurrentGame.Name = gameNameTextBox.Text;
            }

        }

        private void Btn_Select_Object_Click(object sender, RoutedEventArgs e)
        {
            SelectedObjectType = (GameObjectTypes)((Button)sender).Tag;
        }


        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Point position = e.GetPosition(canvas);
            double xPosition = Math.Floor(position.X / 50);
            double yPosition = Math.Floor(position.Y / 50);
            canvas.Children.Add(CreateBlockButton(SelectedObjectType,50,50,xPosition*50,yPosition*50));
        }

        private void GameObjectBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button Clicked");
        }

        private Button CreateBlockButton(GameObjectTypes type, int width, int height, double x, double y)
        {
            Button button = new();
            button.Width = width;
            button.Height = height;
            button.SetValue(Canvas.LeftProperty, x);
            button.SetValue(Canvas.TopProperty, y);
            //TODO set image according to type here
            button.Click += new RoutedEventHandler(GameObjectBtn_Click);
            Image image = new();
            image.Width = width;
            image.Height = height;
            image.Source = Images[type];
            button.Content = image;
            return button;
        }


        private Game CreateTestGame()
        {
            //TODO remove testfunction
            GameObject gameObject = new(GameObjectTypes.Wall, 20, 100, 50, 50);
            Level level = new("testlevel1");
            level.AddObject(gameObject);
            Game game = new();
            game.Name = "Hello Json";
            game.AddLevel(level);
            gameObject = new GameObject(GameObjectTypes.Door, 0, 0, 0, 0);
            level.AddObject(gameObject);
            level = new("testlevel2");
            game.AddLevel(level);
            return game;
        }
    }
}
