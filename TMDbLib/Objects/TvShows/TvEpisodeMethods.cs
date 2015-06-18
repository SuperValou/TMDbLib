﻿using System;
using System.ComponentModel;

namespace TMDbLib.Objects.TvShows
{
    [Flags]
    public enum TvEpisodeMethods
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("credits")]
        Credits = 1,
        [Description("images")]
        Images = 2,
        [Description("external_ids")]
        ExternalIds = 4,
        [Description("videos")]
        Videos = 8,
        //[Description("rating")]
        //Rating = 16,
        //[Description("account_states")]
        //AccountStates = 32,
    }
}
