using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CagedTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;
        private MusicLib musicLib;
        private AddPlaylist newPlaylist;
        private Microsoft.Win32.OpenFileDialog openFileDialog;
        private AboutForm aboutForm;
        private RenamePlaylist renamePlaylist;

        public MainWindow()
        {
            InitializeComponent();

            mediaPlayer = new MediaPlayer();

            initializeMusicLib();
            initializeMusicGrid();
            initializePlaylistBox();
        }


        private void playlistBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.ToString().Substring(37) == "All Music")
            {
                musicGrid.IsReadOnly = false;
                initializeMusicGrid();
            }    
            else if (sender.ToString().Substring(37,7) != ".Count:")
            {
                musicGrid.IsReadOnly = true;
                setMusicGridItems(musicLib.GetSongsFromPlaylist(sender.ToString().Substring(37)));
            }
        }

        private void setMusicGridItems(ObservableCollection<Song> songItems)
        {
            musicGrid.ItemsSource = songItems;
            if (musicGrid.Items.Count > 0)
            {
                musicGrid.SelectedItem = musicGrid.Items[0];
            }
        }

        private void initializeMusicLib()
        {
            try
            {
                musicLib = new MusicLib();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file: " + e.Message);
            }
        }

        private void initializeMusicGrid()
        {
            List<string> items = new List<string>(musicLib.SongIds);
            ObservableCollection<Song> songItems = new ObservableCollection<Song>();
            for (int count = 0; count < items.Count(); count++)
            {
                Song s = musicLib.GetSong(Convert.ToInt32(items[count]));
                songItems.Add(s);
            }
            setMusicGridItems(songItems);
        }

        private void initializePlaylistBox()
        {
            playlistBox.Items.Add("All Music");
            playlistBox.SelectedItem = playlistBox.Items[0];

            foreach (string playlist in musicLib.Playlists)
            {
                playlistBox.Items.Add(playlist);
            }
        }

        private void addNewSong_Click(object sender, RoutedEventArgs e)
        {
           this.IsEnabled = false;
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = "*.wma;*.wav;*mp3";
            openFileDialog.Filter = "Media files|*.mp3;*.m4a;*.wma;*.wav|MP3 (*.mp3)|*.mp3|M4A (*.m4a)|*.m4a|Windows Media Audio (*.wma)|*.wma|Wave files (*.wav)|*.wav|All files|*.*";

            // Show open file dialog box
            bool? result = openFileDialog.ShowDialog();

            // Load the selected song
            if (result == true)
            {
                Song s = null;
                try
                {
                    s = new Song();
                    s = musicLib.GetSongDetails(openFileDialog.FileName);
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    DisplayError("You did not select a valid song file.");
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
                if (s != null)
                {
                    musicLib.AddSong(s);
                }
            }
        }

        private void addNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            newPlaylist = new AddPlaylist();
            newPlaylist.currentMusicLib = musicLib;
            newPlaylist.ShowDialog();
            musicLib = newPlaylist.currentMusicLib;
            playlistBox.Items.Clear();
            initializePlaylistBox();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (playlistBox.SelectedItem.ToString() == "All Music" || (playlistBox.SelectedItem.ToString().Length >= 7 && playlistBox.SelectedItem.ToString().Substring(0, 7) == ".Count:"))
            {
                MessageBox.Show("Sorry, what you have selected is unable to be deleted.", "Error");
            }
            else
            {
                musicLib.DeletePlaylist(playlistBox.SelectedItem.ToString());
                playlistBox.Items.Clear();
                initializePlaylistBox();
            }
        }

        private void RenameCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (playlistBox.SelectedItem.ToString() == "All Music" || (playlistBox.SelectedItem.ToString().Length >= 7 && playlistBox.SelectedItem.ToString().Substring(0, 7) == ".Count:"))
            {
                MessageBox.Show("Sorry, what you have selected is unable to be renamed.", "Error");
            }
            else
            {
                renamePlaylist = new RenamePlaylist(playlistBox.SelectedItem.ToString());
                renamePlaylist.currentMusicLib = musicLib;
                renamePlaylist.ShowDialog();
                playlistBox.Items.Clear();
                musicLib = renamePlaylist.currentMusicLib;
                initializePlaylistBox();
            }
        }
        private void DisplayError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "MiniPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
