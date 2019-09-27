using System.Collections.Generic;
using System.IO;
using System;

namespace lab2
{
    public class Catalogue
    {

        Dictionary<int, Artist> artists = new Dictionary<int, Artist>();
        Dictionary<int, Album> albums = new Dictionary<int, Album>();
        Dictionary<int, Track> tracks = new Dictionary<int, Track>();
        Dictionary<int, Genre> genres = new Dictionary<int, Genre>();

        public Catalogue(StreamReader sr)
        {
            try
            {
                string[] separator = { "-" };
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] infoFromFile = line.Split(separator, StringSplitOptions.None);
                    for (var i = 0; i < infoFromFile.Length; i++)
                    {
                        infoFromFile[i] = infoFromFile[i].Trim(' ');
                    }
                    if (infoFromFile.Length < 3 || infoFromFile[0] == "" || infoFromFile[2] == "")
                    {
                        continue;
                    }

                    Genre outGenre = new Genre("none");
                    if (infoFromFile[3] != "" && !this.genres.TryGetValue(infoFromFile[3].GetHashCode(), out outGenre))
                    {
                        
                        this.genres.Add(infoFromFile[3].GetHashCode(), outGenre);
                    }

                    Artist artist = new Artist(infoFromFile[0], outGenre);
                    if (infoFromFile[1] == "")
                    {
                        infoFromFile[1] = infoFromFile[2];
                    }
                    Album album = new Album(infoFromFile[1], ref artist);
                    artist.AddAlbum(ref album);
                    int year;
                    Int32.TryParse(infoFromFile[4], out year);
                    Track track = new Track(infoFromFile[2], artist, album, outGenre,year);
                    AddTrack(track);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void AddTrack(Track track)
        {
            if (track.Album == null || track.Artist == null || track.Genre == null)
            {
                Console.WriteLine("Bad track metainfo \nnot aded");
                return;
            }
            if (!this.tracks.TryGetValue(track.GetHashCode(), out var outTrack))
            {
                AddAlbum(track.Album);
                outTrack = track;
                if (this.albums.TryGetValue(track.Album.GetHashCode(), out var outAlbum))
                {
                    outAlbum.tracks.Add(track);
                }
                this.tracks.Add(track.GetHashCode(), outTrack);

            }
            else
            {
                Console.WriteLine("track already in catalogue");
            }
        }

        private void AddAlbum(Album album)
        {


            AddArtist(album.Artist);
            Album outAlbum;
            if (this.albums.TryGetValue(album.GetHashCode(), out outAlbum))
            {
                return;
            }
            else
            {
                Artist outArtist;
                if (this.artists.TryGetValue(album.Artist.GetHashCode(), out outArtist))
                {

                    album.Artist = outArtist;
                    outArtist.AddAlbum(ref album);
                }
                else
                {
                    System.Console.WriteLine("artist not found");
                }
                this.albums.Add(album.GetHashCode(), album);

            }




        }

        private void AddArtist(Artist artist)
        {
            Artist outArtist;
            if (!this.artists.TryGetValue(artist.GetHashCode(), out outArtist))
            {
                this.artists.Add(artist.GetHashCode(), artist);
            }
        }


        public void Search(SearchOptions options)
        {
            List<CatalogueItem> result = new List<CatalogueItem>();
            Genre genre = options.genre;
            int year = options.year;
            string name = options.name;

            if (name != null)
            {
                if (options.type == CatalogueTypes.artist || options.type == CatalogueTypes.all)
                {
                    if (artists.TryGetValue(name.ToLower().GetHashCode(), out var artistsResultByWord))
                    {
                        System.Console.WriteLine(artistsResultByWord);
                    }
                }

                if (options.type == CatalogueTypes.album || options.type == CatalogueTypes.all)
                {
                    if (albums.TryGetValue(name.ToLower().GetHashCode(), out var albumsResultByWord))
                    {
                        if ((genre == null || albumsResultByWord.Genre == genre) &&
                        (year == -1 || albumsResultByWord.Year == year))
                        {
                            System.Console.WriteLine(albumsResultByWord);
                        }
                    }
                }

                if (options.type == CatalogueTypes.track || options.type == CatalogueTypes.all)
                {
                    if (tracks.TryGetValue(name.ToLower().GetHashCode(), out var tracksResultByWord))
                    {
                        if ((genre == null || tracksResultByWord.Genre.GetType() == genre.GetType()
                        || genre.GetType().IsSubclassOf(tracksResultByWord.Genre.GetType())) &&
                        (year == -1 || tracksResultByWord.Year == year))
                        {
                            System.Console.WriteLine(tracksResultByWord);
                        }

                    }
                }
            }
            else
            {
                if (options.type == CatalogueTypes.artist || options.type == CatalogueTypes.all)
                {
                    foreach (var artist in artists){
                        if ((genre == null || artist.Value.Genre == genre)){
                            Console.WriteLine(artist.Value);
                        }
                    }
                }

                if (options.type == CatalogueTypes.album || options.type == CatalogueTypes.all)
                {
                    foreach (var album in albums){
                        if ((genre == null || album.Value.Genre.GetType() == genre.GetType()
                        || genre.GetType().IsSubclassOf(album.Value.Genre.GetType())) 
                        && (year == -1 || album.Value.Year == year) ){
                            Console.WriteLine(album.Value);
                        }
                }

                if (options.type == CatalogueTypes.track || options.type == CatalogueTypes.all)
                {
                    foreach (var track in tracks){
                        Console.WriteLine(genre.GetType().IsSubclassOf(track.Value.Genre.GetType()));
                        if ((genre == null || track.Value.Genre.GetType() == genre.GetType()
                        || track.Value.Genre.GetType().IsAssignableFrom (genre.GetType())) &&
                        (year == -1 || track.Value.Year == year))
                        {
                            System.Console.WriteLine(track.Value);
                        }
                    }
                    
                }
            }

        }
    }
}
}
