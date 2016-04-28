﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Model.Logic
{
    public class Movie
    {
        public static List<Tools.Scaffold.Movie> All()
        {
            return new List<Tools.Scaffold.Movie>();
        }

        public static void Save(Ado.Video video)
        {
            try
            {
                var name = video.Name.Trim();

                using (var db = new Ado.PlayOnEntities())
                {
                    var adoMovie = db.Movies.FirstOrDefault(q => q.Name == name);

                    if (adoMovie == null) adoMovie = new Ado.Movie();

                    if (adoMovie.Id == 0)
                    {
                        adoMovie.Name = name;

                        db.Movies.Add(adoMovie);
                    }
                    else
                    {
                        db.Entry(adoMovie).State = EntityState.Modified;
                    }

                    adoMovie.Videos.Add(video);

                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}
