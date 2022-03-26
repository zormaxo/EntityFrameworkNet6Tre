﻿using EntityFrameworkNet6Tre.Domain.Common;

namespace EntityFrameworkNet6Tre.Domain
{
    public class Team : BaseDomainObject
    {
        public string Name { get; set; }
        public int LeagueId { get; set; }
        public virtual League League { get; set; }

        public virtual List<Match> HomeMatches { get; set; }
        public virtual List<Match> AwayMatches { get; set; }
    }
}
