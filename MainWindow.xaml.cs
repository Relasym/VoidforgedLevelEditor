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
using Google.Cloud.Firestore;
using System.Text.Json;

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
        public Dictionary<Button, GameObject> ObjectMap { get; set; }
        private FirestoreDb db;

        int maxXPosition = 11;
        int maxYPosition = 8;
        public MainWindow()
        {
            InitializeComponent();

            //create Test game
            //CurrentGame = CreateTestGame();
            //CurrentLevel = CurrentGame.GetLevels()[0];
            CurrentGame = new Game();
            ObjectMap = new();

            dbConnect();
            DrawCanvasBorder();
            UpdateView();

        }

        private void dbConnect()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"voidforged.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                db = FirestoreDb.Create("voidforged-460d4");
            }
            catch (Exception)
            {
                Showmessage("Could not connect to Database");
            }
            
        }

        private void New_File_Click(object sender, RoutedEventArgs e)
        {
            CurrentGame = new Game();
            mainCanvas.Children.Clear();
            DrawCanvasBorder();
            UpdateView();
        }

        private void Open_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentGame = Game.FromJson(File.ReadAllText(openFileDialog.FileName));
                if(CurrentGame.Levels.Count>0)
                {
                LoadLevel(CurrentGame.Levels[0]);
                }
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
        private void LoadLevel(Level level)
        {
            CurrentLevel = level;
            mainCanvas.Children.Clear();
            DrawCanvasBorder();
            ObjectMap.Clear();
            foreach(GameObject currentObject in level.Objects)
            {
                if (currentObject != null)
                {
                    //create button and add to map
                    Button button = CreateBlockButton(currentObject.Type, 64, 64, currentObject.XPosition * 64, currentObject.YPosition * 64);
                    mainCanvas.Children.Add(button);
                    ObjectMap.Add(button, currentObject);
                }
            }
        }

        private void DrawCanvasBorder()
        {
            DrawingGroup group = new DrawingGroup();

            for (int i = 0; i <= maxXPosition; i++)
            {
                for (int j = 0; j <= maxYPosition; j++)
                {
                    if (i == 0 || j == 0 || i == maxXPosition || j == maxYPosition)
                    {
                        ImageDrawing image = new ImageDrawing();
                        image.Rect = new Rect(i * 64, j * 64, 64, 64);
                        image.ImageSource = (ImageSource)this.Resources[GameObjectTypes.Wall];
                        group.Children.Add(image);
                    }
                }
            }

            DrawingImage drawingImage = new DrawingImage(group);
            Image fullImage = new Image();
            fullImage.Source = drawingImage;

            mainCanvas.Children.Add(fullImage);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //TODO verify file is saved or uploaded
            Close();
        }

        private async void Database_Load_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement load from database
            try
            {
                Google.Cloud.Firestore.DocumentReference documentReference = db.Collection("games").Document("current");
                DocumentSnapshot snap = await documentReference.GetSnapshotAsync();
                if (snap.Exists)
                {
                    Dictionary<string, object> dict = snap.ToDictionary();
                    string json = dict["current"].ToString();
                    Game game = Game.FromJson(json);
                    CurrentGame = game;
                    LoadLevel(game.Levels[0]);
                    Showmessage("Load successful");
                }
            }
            catch (Exception ex)
            {
                Showmessage("Load Failed: " + ex.Message);
            }
            UpdateView();
            
        }

        private async void Database_Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Google.Cloud.Firestore.DocumentReference documentReference = db.Collection("games").Document("current");
                //Google.Cloud.Firestore.CollectionReference collectionReference = firestoreDb.Collection("games");
                //Google.Cloud.Firestore.DocumentReference docRef = new();
                string json = CurrentGame.ToJson();
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("current", json);
                //await collectionReference.AddAsync(dict);
                await documentReference.SetAsync(dict);

                //CollectionReference collectionReference = firestoreDb.Collection("games");
                //await collectionReference.AddAsync(CurrentGame);
                Showmessage("Upload successful");
            }
            catch (Exception ex)
            {
                Showmessage("Upload Failed" + ex.Message);
            }
            

        }

        private bool LevelName_is_Available(string name)
        {
            foreach(Level level in CurrentGame.Levels)
            {
                if(level.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        private string GetLevelName()
        {
            string baseString = "level ";
            int counter = 0;
            string levelName=baseString+counter.ToString();
            while (!LevelName_is_Available(levelName))
            {
                counter++;
                levelName = baseString + counter.ToString();
            }
            return levelName;
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
            //mapViewBtn.FontWeight = FontWeights.Bold;
            //levelViewBtn.FontWeight = FontWeights.Normal;
            //levelViewSidePanel.Visibility = Visibility.Hidden;
            //mapViewSidePanel.Visibility = Visibility.Visible;
            Showmessage("Map View not implement yet");


            //start serialisation test code
            //testTextBlock.TextWrapping = TextWrapping.Wrap;

            //testTextBlock.Text = CurrentGame.ToJson();
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
            CurrentGame.AddLevel(new Level(GetLevelName()));
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
            testTextBlock.Text = SelectedObjectType.ToString();
            double xPosition = Math.Floor(position.X / 64);
            double yPosition = Math.Floor(position.Y / 64);
            if (xPosition > 0 && yPosition > 0 && xPosition < maxXPosition && yPosition < maxYPosition)
            {
                Button button = CreateBlockButton(SelectedObjectType, 64, 64, xPosition * 64, yPosition * 64);
                GameObject gameObject = new(SelectedObjectType, xPosition, yPosition, 1, 1);
                if (CurrentLevel == null)
                {
                    Level level = new Level(GetLevelName());
                    CurrentGame.AddLevel(level);
                    CurrentLevel = level;
                    UpdateView();
                }
                CurrentLevel.AddObject(gameObject);
                //add gameobject to level here
                canvas.Children.Add(button);
                ObjectMap.Add(button, gameObject);
            }

        }

        private void GameObjectBtn_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = (Button)sender;
            CurrentLevel.RemoveObject(ObjectMap[button]);
            ObjectMap.Remove(button);
            Canvas canvas = (Canvas)button.Parent;
            canvas.Children.Remove(button);
        }


        private Button CreateBlockButton(GameObjectTypes type, int width, int height, double x, double y)
        {
            Button button = new();
            button.Width = width;
            button.Height = height;
            button.SetValue(Canvas.LeftProperty, x);
            button.SetValue(Canvas.TopProperty, y);
            button.Background = Brushes.DarkRed;
            button.Click += new RoutedEventHandler(GameObjectBtn_Click);
            Image image = new();
            image.Width = width;
            image.Height = height;
            image.Source = (ImageSource)this.Resources[type];
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

        private Level FindLevel(string item)
        {
            foreach(Level level in CurrentGame.Levels)
            {
                if(level.Name == item)
                {
                    return level;
                }
            }
            return null;
        }

        private void Showmessage(string message)
        {
            MessageBox.Show(message);
        }

        private void currentLevelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox)sender;
            var item = box.SelectedItem;
            if (item != null)
            {
                Level level = FindLevel(item.ToString());
                if(level != null)
                {
                    LoadLevel(level);
                }
                
            }
        }
    }
}
