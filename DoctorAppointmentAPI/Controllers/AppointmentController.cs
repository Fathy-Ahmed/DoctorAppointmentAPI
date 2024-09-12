using Entities.DTO;
using Entities.Models;
using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc;
using Utilities;

namespace DoctorAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AppointmentDTO appointmentDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var day = appointmentDTO.Date.DayOfWeek.ToString();

            Doctor doctor = (await _unitOfWork.Doctor.GetFirstOrDefault(e=>e.Id==appointmentDTO.DocotrId));

            var startDay = doctor.FromDay;

            var endDay = doctor.ToDay;

            Dictionary<string, int> DaysValue = new Dictionary<string, int>();

            DaysValue[SD.Saturday] = (int)Days.Saturday;  //  1
            DaysValue[SD.Sunday] = (int)Days.Sunday;     //   2
            DaysValue[SD.Monday] = (int)Days.Monday;      //  3
            DaysValue[SD.Tuesday] = (int)Days.Tuesday;   //   4
            DaysValue[SD.Wednesday] = (int)Days.Wednesday; // 5
            DaysValue[SD.Thursday] = (int)Days.Thursday;   // 6
            DaysValue[SD.Friday] = (int)Days.Friday;      //  7

            if (!(DaysValue[day] >= DaysValue[startDay]) || !(DaysValue[day] <= DaysValue[endDay]))
            {
                ModelState.TryAddModelError("Date", $"Doctor not avilable at this day docot work from {startDay} to {endDay}");
                return BadRequest(ModelState);
            }
            //------------------------------ Check the Time -------------------------------------------------------------
            var startTime = doctor.StartTime;
            var endTime = doctor.EndTime;
            var time = new TimeSpan(appointmentDTO.Time.Hour, appointmentDTO.Time.Minute, appointmentDTO.Time.Second);
            if (TimeSpan.Compare(time, startTime) == -1 || TimeSpan.Compare(time, endTime) == 1)
            {
                ModelState.TryAddModelError("Time", $"Doctor not avilable at this time docot work from {startTime} to {endTime}");
                return BadRequest(appointmentDTO);
            }

            //----------------------------------------------------------------------------------------------------
            Appointment appointment = new()
            {
                Message = appointmentDTO.Message,
                PatientName = appointmentDTO.PatientName,
                DoctorId = appointmentDTO.DocotrId,
                PatientId = appointmentDTO.PatientId,
                ForWho = appointmentDTO.ForWho,
                Reason = appointmentDTO.Reason,
                Status = appointmentDTO.Status,
                Time = appointmentDTO.Time,
                Date = appointmentDTO.Date,
            };
            await _unitOfWork.Appointment.AddAsync(appointment);
            await _unitOfWork.Save();

            return Ok("Appointment is successfuly created");
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPost("Cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _unitOfWork.Appointment.GetFirstOrDefault(e=>e.Id==id);
            if (appointment == null)
                return BadRequest("Appointment not found");

            if (appointment.Status != SD.AppointmentIsCanceled)
            {
                appointment.Status = SD.AppointmentIsCanceled;
                await _unitOfWork.Save();
            }

            return Ok("Appointment has been canceled");
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPost("Verify/{id:int}")]
        public async Task<IActionResult> Verify(int id)
        {
            var appointment = await _unitOfWork.Appointment.GetFirstOrDefault(e => e.Id == id);
            if (appointment == null)
                return BadRequest("Appointment not found");

            if (appointment.Status != SD.AppointmentIsVerified)
            {
                appointment.Status = SD.AppointmentIsVerified;
                await _unitOfWork.Save();
            }

            return Ok("Appointment has been Verified");
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPatch("Reschedule")]
        public async Task<IActionResult> Reschedule(AppointmentRescheduleDTO appointmentRescheduleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //------------------------------ Check the Date ----------------------------------------------------

            var day = appointmentRescheduleDTO.Date.DayOfWeek.ToString();

            Doctor doctor = (await _unitOfWork.Doctor.GetFirstOrDefault(e => e.Id == appointmentRescheduleDTO.DocotrId));

            var startDay = doctor.FromDay;

            var endDay = doctor.ToDay;
            Dictionary<string, int> DaysValue = new Dictionary<string, int>();

            DaysValue[SD.Saturday] = (int)Days.Saturday;  //  1
            DaysValue[SD.Sunday] = (int)Days.Sunday;     //   2
            DaysValue[SD.Monday] = (int)Days.Monday;      //  3
            DaysValue[SD.Tuesday] = (int)Days.Tuesday;   //   4
            DaysValue[SD.Wednesday] = (int)Days.Wednesday; // 5
            DaysValue[SD.Thursday] = (int)Days.Thursday;   // 6
            DaysValue[SD.Friday] = (int)Days.Friday;      //  7

            if (!(DaysValue[day] >= DaysValue[startDay]) || !(DaysValue[day] <= DaysValue[endDay]))
            {
                ModelState.TryAddModelError("Date", $"Doctor not avilable at this day docot work from {startDay} to {endDay}");
                return BadRequest(ModelState);
            }
            //------------------------------ Check the Time -------------------------------------------------------------
            var startTime = doctor.StartTime;
            var endTime = doctor.EndTime;
            var time = new TimeSpan(appointmentRescheduleDTO.Time.Hour, appointmentRescheduleDTO.Time.Minute, appointmentRescheduleDTO.Time.Second);
            if (TimeSpan.Compare(time, startTime) == -1 || TimeSpan.Compare(time, endTime) == 1)
            {
                ModelState.TryAddModelError("Time", $"Doctor not avilable at this time docot work from {startTime} to {endTime}");
                return BadRequest(ModelState);
            }

            //----------------------------------------------------------------------------------------------------

            Appointment appointment = await _unitOfWork.Appointment.GetFirstOrDefault(e=>e.Id==appointmentRescheduleDTO.AppointmentId);

            appointment.Date = appointmentRescheduleDTO.Date;
            appointment.Time = appointmentRescheduleDTO.Time;
            await _unitOfWork.Save();

            return Ok("Appointment has successfuly rescheduled");
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    }
}
