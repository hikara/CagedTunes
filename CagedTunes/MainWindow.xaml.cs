﻿using System;
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
                initializeMusicGrid();
            else
                setMusicGridItems(musicLib.GetSongsFromPlaylist(sender.ToString().Substring(37)));
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
    }
}
