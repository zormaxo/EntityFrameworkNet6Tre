// See https://aka.ms/new-console-template for more information
using EntityFrameworkNet6Tre.Data;
using EntityFrameworkNet6Tre.Domain;

FootballLeageDbContext context = new FootballLeageDbContext();

/* Simple Insert Operation Methods */
await AddNewLeague();
//await AddNewTeamsWithLeague();

async Task AddNewLeague()
{
    //// Adding a new League Object
    var league = new League { Name = "Seria A" };
    await context.Leagues.AddAsync(league);
    //await context.SaveChangesAsync();

    //// Function To add new teams related to the new league object.
    await AddTeamsWithLeague(league);
    await context.SaveChangesAsync();
}

async Task AddTeamsWithLeague(League league)
{
    var teams = new List<Team>
    {
        new Team
        {
             Name = "Juventus",
             League = league
        },
        new Team
        {
            Name = "AC Milan",
            League = league
        },
        new Team
        {
            Name = "AS Roma",
            League = league
        }
    };

    //// Operation to add multiple objects to database in one call.
    await context.AddRangeAsync(teams);
}

async Task AddNewTeamsWithLeague()
{
    var league = new League { Name = "Bundesliga" };
    var team = new Team { Name = "Bayern Munich", League = league };
    await context.AddAsync(team);
    await context.SaveChangesAsync();
}