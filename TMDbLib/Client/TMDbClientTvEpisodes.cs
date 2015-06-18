﻿using System;
using System.Globalization;
using System.Linq;
using RestSharp;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;
using TMDbLib.Utilities;

namespace TMDbLib.Client
{
    public partial class TMDbClient
    {
        /// <summary>
        /// Retrieve a specific episode using TMDb id of the associated tv show.
        /// </summary>
        /// <param name="tvShowId">TMDb id of the tv show the desired episode belongs to.</param>
        /// <param name="seasonNumber">The season number of the season the episode belongs to. Note use 0 for specials.</param>
        /// <param name="episodeNumber">The episode number of the episode you want to retrieve.</param>
        /// <param name="extraMethods">Enum flags indicating any additional data that should be fetched in the same request.</param>
        /// <param name="language">If specified the api will attempt to return a localized result. ex: en,it,es </param>
        public TvEpisode GetTvEpisode(int tvShowId, int seasonNumber, int episodeNumber, TvEpisodeMethods extraMethods = TvEpisodeMethods.Undefined, string language = null)
        {
            RestRequest req = new RestRequest("tv/{id}/season/{season_number}/episode/{episode_number}");
            req.AddUrlSegment("id", tvShowId.ToString(CultureInfo.InvariantCulture));
            req.AddUrlSegment("season_number", seasonNumber.ToString(CultureInfo.InvariantCulture));
            req.AddUrlSegment("episode_number", episodeNumber.ToString(CultureInfo.InvariantCulture));

            language = language ?? DefaultLanguage;
            if (!String.IsNullOrWhiteSpace(language))
                req.AddParameter("language", language);

            string appends = string.Join(",",
                                         Enum.GetValues(typeof(TvEpisodeMethods))
                                             .OfType<TvEpisodeMethods>()
                                             .Except(new[] { TvEpisodeMethods.Undefined })
                                             .Where(s => extraMethods.HasFlag(s))
                                             .Select(s => s.GetDescription()));

            if (appends != string.Empty)
                req.AddParameter("append_to_response", appends);

            IRestResponse<TvEpisode> response = _client.Get<TvEpisode>(req);

            // No data to patch up so return
            if (response.Data == null) 
                return null;

            // Patch up data, so that the end user won't notice that we share objects between request-types.
            if (response.Data.Videos != null)
                response.Data.Videos.Id = response.Data.Id ?? 0;

            if (response.Data.Credits != null)
                response.Data.Credits.Id = response.Data.Id ?? 0;

            if (response.Data.Images != null)
                response.Data.Images.Id = response.Data.Id ?? 0;

            if (response.Data.ExternalIds != null)
                response.Data.ExternalIds.Id = response.Data.Id ?? 0;

            return response.Data;
        }

        /// <summary>
        /// Returns a credits object for the specified episode.
        /// </summary>
        /// <param name="tvShowId">The TMDb id of the target tv show.</param>
        /// <param name="seasonNumber">The season number of the season the episode belongs to. Note use 0 for specials.</param>
        /// <param name="episodeNumber">The episode number of the episode you want to retrieve information for.</param>
        /// <param name="language">If specified the api will attempt to return a localized result. ex: en,it,es </param>
        public Credits GetTvEpisodeCredits(int tvShowId, int seasonNumber, int episodeNumber, string language = null)
        {
            return GetTvEpisodeMethod<Credits>(tvShowId, seasonNumber, episodeNumber, TvEpisodeMethods.Credits, dateFormat: "yyyy-MM-dd", language: language);
        }

        /// <summary>
        /// Retrieves all images all related to the season of specified episode.
        /// </summary>
        /// <param name="tvShowId">The TMDb id of the target tv show.</param>
        /// <param name="seasonNumber">The season number of the season the episode belongs to. Note use 0 for specials.</param>
        /// <param name="episodeNumber">The episode number of the episode you want to retrieve information for.</param>
        /// <param name="language">
        /// If specified the api will attempt to return a localized result. ex: en,it,es.
        /// For images this means that the image might contain language specifc text
        /// </param>
        public StillImages GetTvEpisodeImages(int tvShowId, int seasonNumber, int episodeNumber, string language = null)
        {
            return GetTvEpisodeMethod<StillImages>(tvShowId, seasonNumber, episodeNumber, TvEpisodeMethods.Images, language: language);
        }

        /// <summary>
        /// Returns an object that contains all known exteral id's for the specified episode.
        /// </summary>
        /// <param name="tvShowId">The TMDb id of the target tv show.</param>
        /// <param name="seasonNumber">The season number of the season the episode belongs to. Note use 0 for specials.</param>
        /// <param name="episodeNumber">The episode number of the episode you want to retrieve information for.</param>
        public ExternalIds GetTvEpisodeExternalIds(int tvShowId, int seasonNumber, int episodeNumber)
        {
            return GetTvEpisodeMethod<ExternalIds>(tvShowId, seasonNumber, episodeNumber, TvEpisodeMethods.ExternalIds);
        }

        public ResultContainer<Video> GetTvEpisodeVideos(int tvShowId, int seasonNumber, int episodeNumber)
        {
            return GetTvEpisodeMethod<ResultContainer<Video>>(tvShowId, seasonNumber, episodeNumber, TvEpisodeMethods.Videos);
        }

        private T GetTvEpisodeMethod<T>(int tvShowId, int seasonNumber, int episodeNumber, TvEpisodeMethods tvShowMethod, string dateFormat = null, string language = null) where T : new()
        {
            RestRequest req = new RestRequest("tv/{id}/season/{season_number}/episode/{episode_number}/{method}");
            req.AddUrlSegment("id", tvShowId.ToString(CultureInfo.InvariantCulture));
            req.AddUrlSegment("season_number", seasonNumber.ToString(CultureInfo.InvariantCulture));
            req.AddUrlSegment("episode_number", episodeNumber.ToString(CultureInfo.InvariantCulture));

            req.AddUrlSegment("method", tvShowMethod.GetDescription());

            if (dateFormat != null)
                req.DateFormat = dateFormat;

            language = language ?? DefaultLanguage;
            if (!String.IsNullOrWhiteSpace(language))
                req.AddParameter("language", language);

            IRestResponse<T> resp = _client.Get<T>(req);

            return resp.Data;
        }
    }
}
