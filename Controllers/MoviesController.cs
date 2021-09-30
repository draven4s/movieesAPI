using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projektas.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Reflection;
using RestSharp;
using Microsoft.AspNetCore.Cors;

namespace projektas
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private DBController DB = new DBController();
        private readonly ILogger<MoviesController> _logger;

        private string baseUrl = "http://www.omdbapi.com/";
        public MoviesController(ILogger<MoviesController> logger)
        {
            _logger = logger;
        }
        [HttpGet("byName")]
        public MoviesSearchData FindMoviesByName(string name)
        {
            var actualResult = DB.GetList<Movies>(x => x.Title.Contains(name)).ToList();
            MoviesSearchData MoviesReturnResult = new MoviesSearchData { };
            //var Test = DB.Update<Movies>(n => n.name);
            if (actualResult.Count < 5)
            {
                var movieList = FetchMovies(name);
                actualResult = DB.GetList<Movies>(x => x.Title.Contains(name)).ToList();
                return movieList;
            }
            else
            {
                MoviesReturnResult.moviesList = actualResult;
                MoviesReturnResult.response = true;
                MoviesReturnResult.message = "OK.";
                return MoviesReturnResult;
            }
        }
        [HttpGet("byImdbid")]
        public MovieByImdb SearchByImdbid(string imdbid)
        {
            var actualResult = DB.Get<Movies>(x => x.imdbID.Contains(imdbid));
            MovieByImdb MoviesReturnResult = new MovieByImdb { };
            //var Test = DB.Update<Movies>(n => n.name);
            if (actualResult == null)
            {
                var movieList = FetchMovieDataImdb(imdbid);
             
                return movieList;
            }
            else
            {
                MoviesReturnResult.movie = actualResult;
                MoviesReturnResult.response = true;
                MoviesReturnResult.message = "OK.";
                return MoviesReturnResult;
            }
        }

        private MoviesSearchData FetchMovies(string name)
        {
            MoviesSearchData MoviesReturnResult = new MoviesSearchData { };
            try
            {
                List<Movies> moviesList = new List<Movies>();

                IRestClient client = new RestClient(baseUrl);
                IRestRequest request = new RestRequest("", Method.GET) {};
                request.AddParameter("apikey", "854f8ef6");
                request.AddParameter("s", name);
                IRestResponse<Movies> response = client.Execute<Movies>(request);

                if (response.IsSuccessful)
                {
                    var parsedData = JObject.Parse(response.Content);
                    if (parsedData["Response"].ToObject<bool>() == true)
                    {
                        MoviesReturnResult.moviesList = FetchMovieData(parsedData["Search"].ToObject<List<Movies>>());
                        MoviesReturnResult.response = true;
                        MoviesReturnResult.message = response.StatusDescription;
                    }
                    else
                    {
                        MoviesReturnResult.response = false;
                        MoviesReturnResult.message = parsedData["Error"].ToString();
                    }
                }
                else
                {
                    MoviesReturnResult.response = false;
                    MoviesReturnResult.message = response.ErrorMessage;
                }
                return MoviesReturnResult;
            }
            catch (Exception exception)
            {
                MoviesReturnResult.response = false;
                MoviesReturnResult.message = exception.Message;
                return MoviesReturnResult;
            }
        }


        private List<Movies> FetchMovieData(List<Movies> moviesList)
        {
            var editedMoviesList = new List<Movies>();
            IRestClient client = new RestClient(baseUrl);
            IRestRequest request = new RestRequest("", Method.GET) { };
            request.AddParameter("apikey", "854f8ef6");

            foreach (var m in moviesList)
            {
                request.AddOrUpdateParameter("i", m.imdbID);
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    
                    var parsedData = JObject.Parse(response.Content);
                    var movieData = parsedData.ToObject<Movies>();
                    movieData.imdbLink = "https://www.imdb.com/title/" + movieData.imdbID;
                    var actualResult = DB.Get<Movies>(x => x.imdbID.Contains(movieData.imdbID));
                    if (actualResult == null)
                    {
                        editedMoviesList.Add(movieData);
                        DB.Save(movieData);
                    }
                    else
                    {
                        editedMoviesList.Add(movieData);
                    }
                }
                else
                {

                }
                
            }
            return editedMoviesList;

        }
        private MovieByImdb FetchMovieDataImdb(string imdbId)
        {
            var movieImdbData = new MovieByImdb();
            IRestClient client = new RestClient(baseUrl);
            IRestRequest request = new RestRequest("", Method.GET) { };
            request.AddParameter("apikey", "854f8ef6");

            request.AddOrUpdateParameter("i", imdbId);
            IRestResponse response = client.Execute(request);
            try
            {
                if (response.IsSuccessful)
                {
                    var parsedData = JObject.Parse(response.Content);
                    var movieData = parsedData.ToObject<Movies>();
                    movieData.imdbLink = "https://www.imdb.com/title/" + movieData.imdbID;
                    movieImdbData.movie = movieData;
                    movieImdbData.response = true;
                    movieImdbData.message = response.StatusDescription;
                    DB.Save(movieData);
                }
                else
                {

                }
            }
            catch(Exception e)
            {
                movieImdbData.response = false;
                movieImdbData.message = e.Message;
                return movieImdbData;
            }
            
            return movieImdbData;

        }
    }

    
}
