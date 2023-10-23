﻿using API_university_labor_exchange.Entities;
using API_university_labor_exchange.Models;
using API_university_labor_exchange.Models.JobPositionDTOs;
using API_university_labor_exchange.Services.Implementations;
using API_university_labor_exchange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_university_labor_exchange.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class JobPositionController :ControllerBase
    {
        private IJobPositionService _jobPositionService;
        private IStudentService _studentService;

        public JobPositionController(IJobPositionService jobPositionService, IStudentService studentService)
        {
            _jobPositionService = jobPositionService;
            _studentService = studentService;   
        }

        [HttpGet("GetAllInterships")]
        public ActionResult<ICollection<ReadJobPositionDto>> GetAllInterships()
        {
            ICollection<ReadJobPositionDto> jobPosition = _jobPositionService.GetAllJobPosition();

            return Ok(_jobPositionService.GetAllInterships(jobPosition));
        }

        [HttpGet("GetAllJobs")]
        public ActionResult<ICollection<ReadJobPositionDto>> GetAllJobs()
        {
            ICollection<ReadJobPositionDto> jobPosition = _jobPositionService.GetAllJobPosition();

            return Ok(_jobPositionService.GetAllJobs(jobPosition));
        }

        [HttpGet("GetJobPositions")]
        public ActionResult<ICollection<ReadJobPositionCompanyDTO>> GetJobPositions()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var student = _studentService.GetStudent(Int32.Parse(userIdClaim));

            var jobPosition = _jobPositionService.GetJobPosition(student.Legajo);

            if (jobPosition != null)
                return Ok(jobPosition);
            return NotFound();

        }

        [HttpPut("SetJobPositionState")]

        public ActionResult SetJobPositionState(SetJobPositionStateDTO jobPosition)
        {
            _jobPositionService.SetJobPositionState(jobPosition);
            return Ok();
        }

        [HttpPost("AddStudentJobPosition")]
        public ActionResult AddStudentJobPosition([FromBody] int idJobPosition) 
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int studentId))
                return Unauthorized();

            var student = _studentService.GetStudent(studentId);

            if (student == null)
                return NotFound("No se encontro al estudiante");
            var legajo = student.Legajo;

            _jobPositionService.AddStudentJobPosition(legajo, idJobPosition);

            return Ok("Postulación agregada con exito");
        }
    }
}
