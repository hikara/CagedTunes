using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private AboutForm aboutForm;
        private RenamePlaylist renamePlaylist;
        private String currentlyPlaying = null;

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
            else if (sender.ToString().Substring(37).Length <= 7 || sender.ToString().Substring(37,7) != ".Count:")
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
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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
            this.IsEnabled = true;
            initializeMusicGrid();
            musicGrid.SelectedIndex = musicGrid.Items.Count - 1;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            musicLib.Save();
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (musicGrid.SelectedItem != null)
            {
                Song s = musicLib.GetSong(((Song)musicGrid.SelectedItem).Id);
                if (s.Filename != null)
                {
                    e.CanExecute = s.Filename != currentlyPlaying;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Song s = musicLib.GetSong(((Song)musicGrid.SelectedItem).Id);
            mediaPlayer.Open(new Uri(s.Filename));
            mediaPlayer.Play();
            currentlyPlaying = s.Filename;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Open(new Uri(currentlyPlaying));
            mediaPlayer.Stop();
            currentlyPlaying = null;
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = currentlyPlaying != null;
        }

        private void DeleteSongFromAllMusicBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (playlistBox.SelectedItem.ToString() == "All Music")
            {
                
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this song?", "Delete A Song", buttons);
               
                if (result == MessageBoxResult.Yes)
                {
                    musicLib.DeleteSong(((Song)musicGrid.SelectedItem).Id);
                    if (musicGrid.Items.Count > 0)
                    {
                        musicGrid.SelectedItem = musicGrid.Items[0];
                    }
                }
                initializeMusicGrid();
            }
            else
            {
                musicLib.RemoveSongFromPlaylist(((Song)musicGrid.SelectedItem).Position, ((Song)musicGrid.SelectedItem).Id, playlistBox.SelectedItem.ToString());
                setMusicGridItems(musicLib.GetSongsFromPlaylist(playlistBox.SelectedItem.ToString()));

            }
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Play = new RoutedUICommand(
            "Play", "Play", typeof(CustomCommands),
            new InputGestureCollection()
            {
            new KeyGesture(Key.P, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand DeleteSongFromAllMusic = new RoutedUICommand(
            "DeleteSongFromAllMusic", "DeleteSongFromAllMusic", typeof(CustomCommands),
            new InputGestureCollection()
            {
            new KeyGesture(Key.Delete)
            });
    }
}
