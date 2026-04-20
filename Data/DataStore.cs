using TrainingCenterApi.Models;

namespace TrainingCenterApi.Data
{
    public static class DataStore
    {
        public static List<Room> Rooms { get; set; } = new List<Room>
        {
            new Room { Id = 1, Name = "Lab 101", BuildingCode = "A", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true },
            new Room { Id = 2, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true, IsActive = true },
            new Room { Id = 3, Name = "Meeting 1", BuildingCode = "A", Floor = 1, Capacity = 8, HasProjector = false, IsActive = true },
            new Room { Id = 4, Name = "Old Storage", BuildingCode = "C", Floor = -1, Capacity = 10, HasProjector = false, IsActive = false }
        };

        public static List<Reservation> Reservations { get; set; } = new List<Reservation>
        {
            new Reservation { Id = 1, RoomId = 1, OrganizerName = "John Doe", Topic = "C# Basics", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0), Status = "confirmed" },
            new Reservation { Id = 2, RoomId = 2, OrganizerName = "Anna Kowalska", Topic = "HTTP and REST Workshop", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 30, 0), Status = "confirmed" },
            new Reservation { Id = 3, RoomId = 1, OrganizerName = "Jane Smith", Topic = "Advanced LINQ", Date = new DateTime(2026, 5, 11), StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(15, 0, 0), Status = "planned" },
            new Reservation { Id = 4, RoomId = 3, OrganizerName = "Bob Team", Topic = "Sprint Planning", Date = new DateTime(2026, 5, 12), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Status = "planned" }
        };
    }
}