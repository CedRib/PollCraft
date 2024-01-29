using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Participation> Participations { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Choice> Choices { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=prbd-2223-a17.db")
            // .LogTo(Console.WriteLine, LogLevel.Information)
            // .EnableSensitiveDataLogging()
            .UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<User>()
            .HasMany(p => p.Participations)
            .WithOne(p => p.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Role);


        modelBuilder.Entity<Participation>()
           .HasKey(p => new { p.PollId, p.UserId });

        modelBuilder.Entity<Participation>()
            .HasOne(p => p.Poll)
            .WithMany(p => p.Participations);

        modelBuilder.Entity<Participation>()
            .HasOne(p => p.User)
            .WithMany(u => u.Participations);


        modelBuilder.Entity<Vote>()
            .HasKey(v => new { v.UserId, v.ChoiceId });

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.User)
            .WithMany();

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Choice)
            .WithMany(c => c.Votes);


        modelBuilder.Entity<Poll>()
            .HasOne(p => p.Creator)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Poll);


        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Poll)
            .WithMany(p => p.Comments);


        modelBuilder.Entity<Choice>()
            .HasKey(c => c.ChoiceId);

        modelBuilder.Entity<Choice>()
            .HasOne(c => c.Poll)
            .WithMany(p => p.Choices)
            .HasForeignKey(c => c.PollId)
            .OnDelete(DeleteBehavior.ClientCascade);



        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()

            .HasData(

                new User {

                    UserId = 1, FullName = "Harry Covère", Email = "harry@test.com", Password = SecretHasher.Hash("harry")

                },

                new User {

                    UserId = 2, FullName = "Mélusine Enfayite", Email = "melusine@test.com",

                    Password = SecretHasher.Hash("melusine")

                },

                new User {

                    UserId = 3, FullName = "John Deuf", Email = "john@test.com", Password = SecretHasher.Hash("john")

                },

                new User {

                    UserId = 4, FullName = "Alain Terrieur", Email = "alain@test.com", Password = SecretHasher.Hash("alain")

                },

                new User {

                    UserId = 5, FullName = "Camille Honnête", Email = "camille@test.com",

                    Password = SecretHasher.Hash("camille")

                },

                new User {

                    UserId = 6, FullName = "Jim Nastik", Email = "jim@test.com", Password = SecretHasher.Hash("jim")

                },

                new User {

                    UserId = 7, FullName = "Mehdi Cament", Email = "mehdi@test.com", Password = SecretHasher.Hash("mehdi")

                },

                new User { UserId = 8, FullName = "Ali Gator", Email = "ali@test.com", Password = SecretHasher.Hash("ali") }

            );


        modelBuilder.Entity<Administrator>()

            .HasData(

                new Administrator() { UserId = 9, FullName = "Admin", Email = "admin@test.com", Password = SecretHasher.Hash("admin") }

            );


        modelBuilder.Entity<Poll>()

            .HasData(

                new Poll { PollId = 1, Title = "Meilleure citation ?", CreatorId = 1 },

                new Poll { PollId = 2, Title = "Meilleur film de série B ?", CreatorId = 3 },

                new Poll { PollId = 3, Title = "Plus belle ville du monde ?", CreatorId = 1, Type = PollType.Single },

                new Poll { PollId = 4, Title = "Meilleur animé japonais ?", CreatorId = 5 },

                new Poll { PollId = 5, Title = "Sport pratiqué", CreatorId = 3, IsClosed = true },

                new Poll { PollId = 6, Title = "Langage informatique préféré", CreatorId = 7 }

            );


        modelBuilder.Entity<Comment>()

            .HasData(

                new Comment {

                    CommentId = 1, UserId = 1, PollId = 1, CommentText = "M'en fout",

                    DateTime = DateTime.Parse("2022-12-10 16:40")

                },

                new Comment {

                    CommentId = 2, UserId = 1, PollId = 2, CommentText = "Bonne question!",

                    DateTime = DateTime.Parse("2022-12-01 12:33")

                },

                new Comment {

                    CommentId = 3, UserId = 2, PollId = 1, CommentText = "Moi aussi",

                    DateTime = DateTime.Parse("2022-12-11 22:07")

                },

                new Comment {

                    CommentId = 4, UserId = 3, PollId = 1, CommentText = "Bla bla bla",

                    DateTime = DateTime.Parse("2022-12-27 08:15")

                },

                new Comment {

                    CommentId = 5, UserId = 1, PollId = 6, CommentText = "I love C#",

                    DateTime = DateTime.Parse("2022-12-31 23:59")

                },

                new Comment {

                    CommentId = 6, UserId = 3, PollId = 6, CommentText = "I hate WPF",

                    DateTime = DateTime.Parse("2023-01-01 00:01")

                },

                new Comment {

                    CommentId = 7, UserId = 2, PollId = 1,

                    CommentText =

                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi pulvinar, dolor non commodo commodo, " +

                        "felis libero sagittis tellus, at tristique orci risus hendrerit lorem. Maecenas varius hendrerit lacinia. " +

                        "Vestibulum dapibus, libero nec accumsan pulvinar, felis velit imperdiet libero, sed venenatis massa risus " +

                        "gravida dolor. In et lobortis massa.",

                    DateTime = DateTime.Parse("2023-01-02 08:45")

                }

            );


        modelBuilder.Entity<Choice>()

            .HasData(

                new Choice {

                    ChoiceId = 1, PollId = 1,

                    ChoiceText =

                        "La science est ce que nous comprenons suffisamment bien pour l'expliquer à un ordinateur. L'art, c'est tout ce que nous faisons d'autre. (Knuth)"

                },

                new Choice {

                    ChoiceId = 2, PollId = 1,

                    ChoiceText =

                        "La question de savoir si les machines peuvent penser... est à peu près aussi pertinente que celle de savoir si les sous-marins peuvent nager. (Dijkstra)"

                },

                new Choice {

                    ChoiceId = 3, PollId = 1,

                    ChoiceText =

                        "Nous ne savons pas où nous allons, mais du moins il nous reste bien des choses à faire. (Turing)"

                },

                new Choice {

                    ChoiceId = 4, PollId = 1, ChoiceText = "La constante d’une personne est la variable d’une autre. (Perlis)"

                },

                new Choice {

                    ChoiceId = 5, PollId = 1,

                    ChoiceText =

                        "There are only two kinds of [programming] languages: the ones people complain about and the ones nobody uses. (Stroustrup)"

                },

                new Choice { ChoiceId = 6, PollId = 2, ChoiceText = "Massacre à la tronçonneuse" },

                new Choice {ChoiceId = 7, PollId = 2, ChoiceText = "Braindead" },

                new Choice {ChoiceId = 8, PollId = 2, ChoiceText = "La Nuit des morts-vivants" },

                new Choice {ChoiceId = 9, PollId = 2, ChoiceText = "Psychose" },

                new Choice {ChoiceId = 10, PollId = 2, ChoiceText = "Evil Dead" },

                new Choice {ChoiceId = 11, PollId = 3, ChoiceText = "Charleroi" },

                new Choice {ChoiceId = 12, PollId = 3, ChoiceText = "Charleville-Mézières" },

                new Choice {ChoiceId = 13, PollId = 3, ChoiceText = "Pyongyang" },

                new Choice {ChoiceId = 14, PollId = 3, ChoiceText = "Détroit" },

                new Choice {ChoiceId = 15, PollId = 4, ChoiceText = "One piece" },

                new Choice {ChoiceId = 16, PollId = 4, ChoiceText = "Hunter x Hunter" },

                new Choice {ChoiceId = 17, PollId = 4, ChoiceText = "Fullmetal Alchemist" },

                new Choice {ChoiceId = 18, PollId = 4, ChoiceText = "Death Note" },

                new Choice {ChoiceId = 19, PollId = 4, ChoiceText = "Naruto Shippûden" },

                new Choice {ChoiceId = 20, PollId = 4, ChoiceText = "Dragon Ball Z" },

                new Choice {ChoiceId = 21, PollId = 5, ChoiceText = "Curling" },

                new Choice {ChoiceId = 22, PollId = 5, ChoiceText = "Swamp Football" },

                new Choice {ChoiceId = 23, PollId = 5, ChoiceText = "Fléchettes" },

                new Choice {ChoiceId = 24, PollId = 5, ChoiceText = "Kin-ball" },

                new Choice {ChoiceId = 25, PollId = 5, ChoiceText = "Pétanque" },

                new Choice {ChoiceId = 26, PollId = 5, ChoiceText = "Lancer de tronc" },

                new Choice {ChoiceId = 27, PollId = 6, ChoiceText = "Brainfuck" },

                new Choice {ChoiceId = 28, PollId = 6, ChoiceText = "Java" },

                new Choice {ChoiceId = 29, PollId = 6, ChoiceText = "C#" },

                new Choice {ChoiceId = 30, PollId = 6, ChoiceText = "PHP" },

                new Choice {ChoiceId = 31, PollId = 6, ChoiceText = "Whitespace" },

                new Choice {ChoiceId = 32, PollId = 6, ChoiceText = "Aargh!" }

            );


        modelBuilder.Entity<Vote>()

            .HasData(

                new Vote { UserId = 1, ChoiceId = 1, Type = VoteType.Yes },

                new Vote { UserId = 1, ChoiceId = 2, Type = VoteType.Maybe },

                new Vote { UserId = 1, ChoiceId = 5, Type = VoteType.No },

                new Vote { UserId = 1, ChoiceId = 11, Type = VoteType.Yes },

                new Vote { UserId = 1, ChoiceId = 16, Type = VoteType.Yes },

                new Vote { UserId = 1, ChoiceId = 17, Type = VoteType.No },

                new Vote { UserId = 1, ChoiceId = 27, Type = VoteType.Yes },

                new Vote { UserId = 2, ChoiceId = 3, Type = VoteType.Yes },

                new Vote { UserId = 2, ChoiceId = 9, Type = VoteType.Maybe },

                new Vote { UserId = 2, ChoiceId = 10, Type = VoteType.Yes },

                new Vote { UserId = 2, ChoiceId = 16, Type = VoteType.Yes },

                new Vote { UserId = 2, ChoiceId = 29, Type = VoteType.Yes },

                new Vote { UserId = 3, ChoiceId = 2, Type = VoteType.Yes },

                new Vote { UserId = 3, ChoiceId = 4, Type = VoteType.Maybe },

                new Vote { UserId = 3, ChoiceId = 16, Type = VoteType.Maybe },

                new Vote { UserId = 3, ChoiceId = 20, Type = VoteType.Yes },

                new Vote { UserId = 3, ChoiceId = 32, Type = VoteType.No },

                new Vote { UserId = 4, ChoiceId = 29, Type = VoteType.Yes },

                new Vote { UserId = 5, ChoiceId = 27, Type = VoteType.Yes },

                new Vote { UserId = 5, ChoiceId = 28, Type = VoteType.No },

                new Vote { UserId = 6, ChoiceId = 27, Type = VoteType.Maybe },

                new Vote { UserId = 6, ChoiceId = 28, Type = VoteType.Yes },

                new Vote { UserId = 6, ChoiceId = 29, Type = VoteType.Maybe },

                new Vote { UserId = 7, ChoiceId = 27, Type = VoteType.Maybe },

                new Vote { UserId = 7, ChoiceId = 29, Type = VoteType.Yes },

                new Vote { UserId = 7, ChoiceId = 30, Type = VoteType.Maybe },

                new Vote { UserId = 8, ChoiceId = 27, Type = VoteType.Maybe },

                new Vote { UserId = 8, ChoiceId = 30, Type = VoteType.Yes },

                new Vote { UserId = 8, ChoiceId = 32, Type = VoteType.No }

            );


        modelBuilder.Entity<Participation>()

            .HasData(

                new Participation { PollId = 1, UserId = 1 },

                new Participation { PollId = 1, UserId = 2 },

                new Participation { PollId = 1, UserId = 3 },

                new Participation { PollId = 2, UserId = 2 },

                new Participation { PollId = 3, UserId = 1 },

                new Participation { PollId = 4, UserId = 1 },

                new Participation { PollId = 4, UserId = 2 },

                new Participation { PollId = 4, UserId = 3 },

                new Participation { PollId = 5, UserId = 1 },

                new Participation { PollId = 5, UserId = 2 },

                new Participation { PollId = 5, UserId = 3 },

                new Participation { PollId = 6, UserId = 1 },

                new Participation { PollId = 6, UserId = 2 },

                new Participation { PollId = 6, UserId = 3 },

                new Participation { PollId = 6, UserId = 4 },

                new Participation { PollId = 6, UserId = 5 },

                new Participation { PollId = 6, UserId = 6 },

                new Participation { PollId = 6, UserId = 7 },

                new Participation { PollId = 6, UserId = 8 }

            );
    }
}
