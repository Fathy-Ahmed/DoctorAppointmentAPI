using Entities.DTO;
using Entities.Models;
using Entities.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utilities;

namespace DoctorAppointmentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        this._unitOfWork = unitOfWork;
        this._userManager = userManager;
    }
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    [HttpPost]
    [Authorize(Roles =SD.UserRole)]
    // Not tested Yet
    public async Task<IActionResult> Register(PatientLoginDTO patientDTO)
    {

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Patient patient = new()
        {
            Age = patientDTO.Age,
            Gender = patientDTO.Gender,
            MedicalHistory = patientDTO.MedicalHistory,
            Name = patientDTO.Name,
            ApplicationUserId = User.Claims.FirstOrDefault(e => e.Type == "uId").Value
        };

        await _unitOfWork.Patient.AddAsync(patient);
        await _unitOfWork.Save();

        return Ok("Patient is Created");
    }

    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    [HttpGet("Profile")]
    [Authorize(Roles = SD.UserRole)]
    // Not tested Yet
    public async Task<IActionResult> Profile()
    {
        var UserId = User.Claims.FirstOrDefault(e => e.Type == "uId").Value;

        ApplicationUser user=await _userManager.FindByIdAsync(UserId);

        Patient patient = await _unitOfWork.Patient.GetByUserId(UserId);

        PatientDTO patientDTO = new()
        {
            ApplicationUserId = UserId,
            Age = patient.Age,
            Gender = patient.Gender,
            MedicalHistory = patient.MedicalHistory,
            Name = patient.Name,
            Email=user.Email,
            UserName=user.UserName,
            PhoneNumber=user.PhoneNumber,
        };

        return Ok(patientDTO);
    }
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    [HttpPut("EditeProfile")]
    [Authorize(Roles = SD.UserRole)]
    // Not tested Yet
    public async Task<IActionResult> EditeProfile(PatientDTO patientDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ApplicationUser user = await _userManager.FindByIdAsync(patientDTO.ApplicationUserId);

        if (user.PhoneNumber != patientDTO.PhoneNumber)
        {
            IdentityResult result = await _userManager.SetPhoneNumberAsync(user, patientDTO.PhoneNumber);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("PhoneNumber", error.Description);
                }
                return BadRequest(ModelState);
            }
        }
        if (user.UserName != patientDTO.UserName)
        {
            IdentityResult result = await _userManager.SetUserNameAsync(user, patientDTO.UserName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("UserName", error.Description);
                }
                return BadRequest(ModelState);
            }
        }

        Patient patient = await _unitOfWork.Patient.GetByUserId(patientDTO.ApplicationUserId);
        patient.Name = patientDTO.Name;
        patient.Age = patientDTO.Age;
        patient.Gender = patientDTO.Gender;
        patient.MedicalHistory = patientDTO.MedicalHistory;

        await _unitOfWork.Save();

        return Ok("Updated");

    }
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    [HttpGet("GetPatientAppointments")]
    [Authorize(Roles = SD.UserRole)]
    // Not tested Yet
    public async Task<IActionResult> GetPatientAppointments()
    {
        var UserId = User.Claims.FirstOrDefault(e => e.Type == "uId").Value;
        Doctor doctor;
        Patient patient = await _unitOfWork.Patient.GetByUserId(UserId);
        IEnumerable<Appointment> appointments = _unitOfWork.Appointment.GetAppointmentsForPatient(patient.Id).Where(e => e.Status != SD.AppointmentIsCanceled).OrderByDescending(e => e.Date).ThenByDescending(e => e.Time);
        PatientAppointmentsDTO appointmentsDTO;
        List<PatientAppointmentsDTO> appointmentList = new ();
        foreach (var appointment in appointments)
        {
            appointmentsDTO = new PatientAppointmentsDTO();

            doctor = await _unitOfWork.Doctor.GetFirstOrDefault(e=>e.Id== appointment.DoctorId);

            var UserDoctor = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == doctor.ApplicationUserId);

            appointmentsDTO.AppointmentId = appointment.Id;
            appointmentsDTO.Time = appointment.Time;
            appointmentsDTO.Date = appointment.Date;
            appointmentsDTO.Status = appointment.Status;
            appointmentsDTO.Reason = appointment.Reason;
            //-----------------------------------------
            appointmentsDTO.DoctorPhoneNumber = UserDoctor.PhoneNumber;
            appointmentsDTO.ClinicLocation = doctor.ClinicLocation;
            appointmentsDTO.DoctorClinicName = "I Will Add it";
            appointmentsDTO.DoctorImg = doctor.Img;
            appointmentsDTO.DoctorName = doctor.FirstName + " " + doctor.LastName;
            appointmentsDTO.DoctorId = doctor.Id;
            appointmentList.Add(appointmentsDTO);
        }

        return Ok(appointmentList);
    }
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\



}
