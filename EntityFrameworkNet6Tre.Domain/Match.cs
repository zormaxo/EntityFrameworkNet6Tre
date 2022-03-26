using EntityFrameworkNet6Tre.Domain.Common;

namespace EntityFrameworkNet6Tre.Domain
{
    public class Match : BaseDomainObject
    {
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; }

        public DateTime Date { get; set; }
    }
}
