using Entities.DTO;
using Entities.Models;
using Entities.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace DoctorAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DoctorController
            (IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPost]
        [Authorize(Roles =SD.DoctorRole)]
        public async Task<IActionResult> Register([FromForm]DoctorRegisterDTO doctorDTO)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            //Add Photot
            string RootPath = webHostEnvironment.WebRootPath;
            if (doctorDTO.file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var Upload = Path.Combine(RootPath, @"images\Doctors");
                var extension = Path.GetExtension(doctorDTO.file.FileName);

                using (var fileStream = new FileStream(Path.Combine(Upload, fileName + extension), FileMode.Create))
                {
                    doctorDTO.file.CopyTo(fileStream);
                }

                doctorDTO.Img = @"images\Doctors\" + fileName + extension;
            }
            //--------------------------------------------------------------------
            Doctor doctor = new()
            {
                
                ClinicLocation = doctorDTO.ClinicLocation,
                FirstName = doctorDTO.FirstName,
                LastName = doctorDTO.LastName,
                FromDay = doctorDTO.FromDay,
                ToDay = doctorDTO.ToDay,
                Gender = doctorDTO.Gender,
                EndTime = doctorDTO.EndTime,
                Img = doctorDTO.Img,
                StartTime = doctorDTO.StartTime,
                Qualification = doctorDTO.Qualification,
                Specialties = doctorDTO.Specialties,
                NumberOfPatientInDay = doctorDTO.NumberOfPatientInDay,
                ExperienceYears = doctorDTO.ExperienceYears,
            };
            //doctor.ApplicationUserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
            doctor.ApplicationUserId=User.Claims.FirstOrDefault(e => e.Type == "uId").Value;
            await _unitOfWork.Doctor.AddAsync(doctor);
            await _unitOfWork.Save();

            return Ok("Created but not approced yet");
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _unitOfWork.Doctor.GetAll(e => e.Status == SD.DoctorIsAprovied));
             
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _unitOfWork.Doctor.GetFirstOrDefault(e=>e.Id==id);
            if(doctor is null)
                return BadRequest("User Not Found");
            doctor.ApplicationUser = await _userManager.FindByIdAsync(doctor.ApplicationUserId);
            return Ok(doctor);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("Profile")]
        [Authorize(Roles = SD.DoctorRole)]
        // Not tested Yet
        public async Task<IActionResult> Profile()
        {
            var UserId = User.Claims.FirstOrDefault(e => e.Type == "uId").Value;

            ApplicationUser user = await _userManager.FindByIdAsync(UserId);

            Doctor doctor = await _unitOfWork.Doctor.GetByUserId(UserId);

            DoctorDTO doctorDTO = new()
            {
                FirstName =doctor.FirstName,
                LastName =doctor.LastName,
                ClinicLocation=doctor.ClinicLocation,
                EndTime=doctor.EndTime,
                ExperienceYears=doctor.ExperienceYears,
                FromDay=doctor.FromDay,
                ToDay=doctor.ToDay,
                Gender=doctor.Gender,
                Img=doctor.Img,
                NumberOfPatientInDay=doctor.NumberOfPatientInDay,
                Qualification=doctor.Qualification,
                Specialties=doctor.Specialties,
                StartTime=doctor.StartTime,


                ApplicationUserId = UserId,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };

            return Ok(doctorDTO);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpPut("EditeProfile")]
        [Authorize(Roles =SD.DoctorRole)]
        // Not tested Yet
        public async Task<IActionResult> EditeProfile(DoctorDTO doctorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Photo File
            if (doctorDTO.file != null && doctorDTO.file.Length > 0)
            {
                string RootPath = webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString();
                var Upload = Path.Combine(RootPath, @"images\Doctors");
                var extension = Path.GetExtension(doctorDTO.file.FileName);
                if (doctorDTO.Img != null)
                {
                    var OldImg = Path.Combine(RootPath, doctorDTO.Img.TrimStart('\\'));
                    if (System.IO.File.Exists(OldImg))
                    {
                        System.IO.File.Delete(OldImg);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(Upload, filename + extension), FileMode.Create))
                {
                    await doctorDTO.file.CopyToAsync(fileStream);
                }
                doctorDTO.Img = @"images\Doctors\" + filename + extension;
            }

            ApplicationUser user=await _userManager.FindByIdAsync(doctorDTO.ApplicationUserId);
           
            if (user.PhoneNumber != doctorDTO.PhoneNumber)
            {
                IdentityResult result = await _userManager.SetPhoneNumberAsync(user, doctorDTO.PhoneNumber);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("PhoneNumber", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            if (user.UserName != doctorDTO.UserName)
            {
                IdentityResult result = await _userManager.SetUserNameAsync(user, doctorDTO.UserName);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("UserName", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }

            Doctor doctor = await _unitOfWork.Doctor.GetByUserId(doctorDTO.ApplicationUserId);
            doctor.FirstName = doctorDTO.FirstName;
            doctor.LastName = doctorDTO.LastName;
            doctor.Qualification = doctorDTO.Qualification;
            doctor.ClinicLocation = doctorDTO.ClinicLocation;
            doctor.StartTime = doctorDTO.StartTime;
            doctor.EndTime = doctorDTO.EndTime;
            doctor.ToDay = doctorDTO.ToDay;
            doctor.FromDay = doctorDTO.FromDay;
            doctor.ExperienceYears = doctorDTO.ExperienceYears;
            doctor.Gender = doctorDTO.Gender;
            doctor.Specialties = doctorDTO.Specialties;
            doctor.Img = doctorDTO.Img;
            doctor.NumberOfPatientInDay = doctorDTO.NumberOfPatientInDay;
            await _unitOfWork.Save();

            return Ok("Updated");

        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("GetDoctorAppointments")]
        [Authorize(Roles =SD.DoctorRole)]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            string UserId = User.Claims.FirstOrDefault(e => e.Type == "uId").Value;
            Doctor doctor = await _unitOfWork.Doctor.GetByUserId(UserId);
            List<Appointment> appointment = _unitOfWork.Appointment.GetAppointmentsForDoctor(doctor.Id);
            List<DoctorAppointmentsDTO> appointmentsDTO = new List<DoctorAppointmentsDTO>();
            DoctorAppointmentsDTO DTO;
            Patient patient;
            ApplicationUser PatientUser;

            foreach (var item in appointment)
            {
                DTO = new DoctorAppointmentsDTO();
                patient = new Patient();
                PatientUser = new ApplicationUser();
                patient = (await _unitOfWork.Patient.GetFirstOrDefault(e=>e.Id== item.PatientId));
                PatientUser = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == patient.ApplicationUserId);

                DTO.AppointmentId = item.Id;
                DTO.Date = item.Date;
                DTO.Time = item.Time;
                DTO.Status = item.Status;
                DTO.Message = item.Message;
                DTO.Reason = item.Reason;

                DTO.PatientName = patient.Name;

                DTO.PatientEmail = PatientUser.Email;
                DTO.PatientPhoneNumber = PatientUser.PhoneNumber;

                appointmentsDTO.Add(DTO);
            }

            return Ok(appointmentsDTO);
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\




    }
}
