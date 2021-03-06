using EntityFrameworkNet6Tre.Domain.Common;

namespace EntityFrameworkNet6Tre.Domain
{
    public class League : BaseDomainObject
    {
        public string Name { get; set; }

        public List<Team> Teams { get; set; }
    }
}