﻿using System;
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

namespace CagedTunes
{
    /// <summary>
    /// Interaction logic for RenamePlaylist.xaml
    /// </summary>
    public partial class RenamePlaylist : Window
    {
        public MusicLib currentMusicLib { get; set; }
        private string oldName;
        public RenamePlaylist(string old)
        {
            InitializeComponent();
            oldName = old;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string potentialPlaylistName = playlistSubmissionBox.Text.ToString().Trim();
            if (potentialPlaylistName == "" || potentialPlaylistName == "All Music" || currentMusicLib.PlaylistExists(potentialPlaylistName)
                || (potentialPlaylistName.Length >= 7 && potentialPlaylistName.Substring(0, 7) == ".Count:"))
            {
                MessageBox.Show("Sorry, that is an invalid name for a playlist, please select another.", "Error");
            }
            else
            {
                currentMusicLib.RenamePlaylist(oldName, potentialPlaylistName);
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
