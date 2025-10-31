using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGiver_Foundation.Models
{
    public class IncidentAssignment
    {
        public int IncidentAssignmentId { get; set; }
        public DateTime AssignedDate { get; set; }

        // register. null = not attended, (date) = attended
        public DateTime? AttendedDate { get; set; }

        // Foreign Key to the DisasterReport
        public int DisasterReportId { get; set; }
        public DisasterReport DisasterReport { get; set; }

        // Foreign Key to the Volunteer
        public int VolunteerId { get; set; }
        public Volunteer Volunteer { get; set; }
    }
}