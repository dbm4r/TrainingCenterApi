using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        // GET: /api/rooms (Also handles query string filtering)
        [HttpGet]
        public IActionResult GetRooms([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
        {
            var rooms = DataStore.Rooms.AsQueryable();

            if (minCapacity.HasValue)
                rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
            
            if (hasProjector.HasValue)
                rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);

            if (activeOnly.HasValue && activeOnly.Value)
                rooms = rooms.Where(r => r.IsActive);

            return Ok(rooms.ToList());
        }

        // GET: /api/rooms/{id}
        [HttpGet("{id}")]
        public IActionResult GetRoom(int id)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();
            
            return Ok(room);
        }

        // GET: /api/rooms/building/{buildingCode}
        [HttpGet("building/{buildingCode}")]
        public IActionResult GetRoomsByBuilding(string buildingCode)
        {
            var rooms = DataStore.Rooms
                .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            return Ok(rooms);
        }

        // POST: /api/rooms
        [HttpPost]
        public IActionResult CreateRoom([FromBody] Room room)
        {
            // Auto-increment ID
            room.Id = DataStore.Rooms.Max(r => r.Id) + 1;
            DataStore.Rooms.Add(room);

            // Returns 201 Created and adds a Location header pointing to GetRoom
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        // PUT: /api/rooms/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();

            // Full update
            room.Name = updatedRoom.Name;
            room.BuildingCode = updatedRoom.BuildingCode;
            room.Floor = updatedRoom.Floor;
            room.Capacity = updatedRoom.Capacity;
            room.HasProjector = updatedRoom.HasProjector;
            room.IsActive = updatedRoom.IsActive;

            return Ok(room);
        }

        // DELETE: /api/rooms/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();

            // Check if there are any reservations for this room
            bool hasReservations = DataStore.Reservations.Any(res => res.RoomId == id);
            if (hasReservations)
            {
                return Conflict("Cannot delete the room because it has existing reservations.");
            }

            DataStore.Rooms.Remove(room);
            return NoContent(); // 204 No Content
        }
    }
}