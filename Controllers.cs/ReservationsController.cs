using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        // GET: /api/reservations (handles query filtering)
        [HttpGet]
        public IActionResult GetReservations([FromQuery] DateTime? date, [FromQuery] string? status, [FromQuery] int? roomId)
        {
            var reservations = DataStore.Reservations.AsQueryable();

            if (date.HasValue)
                reservations = reservations.Where(r => r.Date.Date == date.Value.Date);
            
            if (!string.IsNullOrEmpty(status))
                reservations = reservations.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            
            if (roomId.HasValue)
                reservations = reservations.Where(r => r.RoomId == roomId.Value);

            return Ok(reservations.ToList());
        }

        // GET: /api/reservations/{id}
        [HttpGet("{id}")]
        public IActionResult GetReservation(int id)
        {
            var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            return Ok(reservation);
        }

        // POST: /api/reservations
        [HttpPost]
        public IActionResult CreateReservation([FromBody] Reservation reservation)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
            
            // Rule: Room must exist
            if (room == null) return BadRequest("The specified room does not exist.");
            
            // Rule: Room must be active
            if (!room.IsActive) return BadRequest("Cannot reserve an inactive room.");

            // Rule: Overlapping reservations
            bool hasOverlap = DataStore.Reservations.Any(r => 
                r.RoomId == reservation.RoomId && 
                r.Date.Date == reservation.Date.Date &&
                r.StartTime < reservation.EndTime && 
                reservation.StartTime < r.EndTime);

            if (hasOverlap) return Conflict("The room is already reserved during the requested time.");

            // Add reservation
            reservation.Id = DataStore.Reservations.Max(r => r.Id) + 1;
            DataStore.Reservations.Add(reservation);

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // PUT: /api/reservations/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
            if (room == null) return BadRequest("The specified room does not exist.");
            if (!room.IsActive) return BadRequest("Cannot reserve an inactive room.");

            // Rule: Overlap (ignoring the current reservation we are updating)
            bool hasOverlap = DataStore.Reservations.Any(r => 
                r.Id != id &&
                r.RoomId == updatedReservation.RoomId && 
                r.Date.Date == updatedReservation.Date.Date &&
                r.StartTime < updatedReservation.EndTime && 
                updatedReservation.StartTime < r.EndTime);

            if (hasOverlap) return Conflict("The updated time overlaps with an existing reservation.");

            // Update
            reservation.RoomId = updatedReservation.RoomId;
            reservation.OrganizerName = updatedReservation.OrganizerName;
            reservation.Topic = updatedReservation.Topic;
            reservation.Date = updatedReservation.Date;
            reservation.StartTime = updatedReservation.StartTime;
            reservation.EndTime = updatedReservation.EndTime;
            reservation.Status = updatedReservation.Status;

            return Ok(reservation);
        }

        // DELETE: /api/reservations/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            DataStore.Reservations.Remove(reservation);
            return NoContent(); // 204 No Content
        }
    }
}