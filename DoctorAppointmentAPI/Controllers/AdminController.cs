using Entities.DTO;
using Entities.Models;
using Entities.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace DoctorAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles =SD.AdminRole)]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            AdminDashboardDTO adminDashboardDTO = new AdminDashboardDTO()
            {
                UsersNum = await _unitOfWork.Patient.Count(),
                AppointmentNum = await _unitOfWork.Appointment.Count(),
                AcceptedDoctorsNum = await _unitOfWork.Doctor.CountAcceptedDoctoes(),
                RequestedDoctorsNum = await _unitOfWork.Doctor.CountRequestedDoctoes(),
            };

            return Ok(adminDashboardDTO);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<Patient> patients = (await _unitOfWork.Patient.GetAll());
            List<AdminUsersDTO> usersDTO = new ();

            AdminUsersDTO userDTO;

            foreach (var patient in patients)
            {
                userDTO = new AdminUsersDTO();
                var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == patient.ApplicationUserId);
                if (user != null)
                {
                    userDTO.Name = patient.Name;
                    userDTO.Age = patient.Age;
                    userDTO.Gender = patient.Gender;
                    userDTO.Email = user.Email;
                    userDTO.PhoneNumber = user.PhoneNumber;
                    userDTO.UserName = user.UserName;

                    usersDTO.Add(userDTO);
                }

            }

            return Ok(usersDTO);

        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetDoctors")]
        public async Task<IActionResult> GetDoctors()
        {
            List<AdminDoctorsDTO> adminDoctorsDTO = new();
            IEnumerable<Doctor> doctors = await _unitOfWork.Doctor.GetAll();
            AdminDoctorsDTO DoctorDTO;
            foreach (var doctor in doctors)
            {
                DoctorDTO = new();
                var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == doctor.ApplicationUserId);
                if (user != null)
                {
                    DoctorDTO.Name = doctor.FirstName + " " + doctor.LastName;
                    DoctorDTO.Gender = doctor.Gender;
                    DoctorDTO.Specialties = doctor.Specialties;
                    DoctorDTO.Status = doctor.Status;

                    DoctorDTO.UserName = user.UserName;
                    DoctorDTO.Email = user.Email;
                    DoctorDTO.PhoneNumber = user.PhoneNumber;


                    adminDoctorsDTO.Add(DoctorDTO);
                }

            }

            return Ok(adminDoctorsDTO);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetRequestsDoctors")]
        public async Task<IActionResult> GetRequestsDoctors()
        {
            List<AdminRequestsDoctorDTO> adminRequestsDoctorDTO = new();
            List<Doctor> doctor = (await _unitOfWork.Doctor.GetAll()).Where(e => e.Status == SD.DoctorIsPending).ToList();
            AdminRequestsDoctorDTO doctorDTO;
            foreach (var item in doctor)
            {
                doctorDTO = new()
                {
                    Id = item.Id,
                    Name = item.FirstName + " " + item.LastName,
                    ExperienceYears = item.ExperienceYears,
                    Img = item.Img,
                    Qualification = item.Qualification,
                    Specialties = item.Specialties
                };
                adminRequestsDoctorDTO.Add(doctorDTO);
            }

            return Ok(adminRequestsDoctorDTO);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            Doctor doctor = await _unitOfWork.Doctor.GetFirstOrDefault(e=>e.Id==id);

            if (doctor == null)
                return BadRequest("Doctor not found");

            doctor.Status = SD.DoctorIsAprovied;
            await _unitOfWork.Save();

            return Ok("RequestsDoctors");

        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    }
}
