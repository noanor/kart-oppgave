<<<<<<< HEAD
﻿using System.Text.Json;                 
using IOFile = System.IO.File;            
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
=======
﻿using System.Globalization;
>>>>>>> b8762d7 (Docker Compose og Refaktorisering av registrar sider)
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Luftfartshinder.Models.ViewModel;

namespace Luftfartshinder.Controllers
{
    public class RegistrarController : Controller
    {
        
        
        private static readonly Dictionary<int, ReviewStatus> _statuses = new();
        private static readonly Dictionary<int, string> _notes = new();
        private readonly IReportRepository reportRepository;

        public RegistrarController(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        // GET /Registrar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await reportRepository.GetAllAsync();
            return View("Index", reports);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var report = reportRepository.GetByIdAsync(id).Result;

            if (report != null)
            {
                var editReport = new EditReportRequest
                {
                    Id = report.Id,
                    Author = report.Author,
                    AuthorId = report.AuthorId,
                    Obstacles = report.Obstacles,
                    RegistrarNote = report.RegistrarNote,
                    ReportDate = report.ReportDate
                };

                return View(editReport);

            }

            return null;

        }

    }


    public enum ReviewStatus { Pending = 0, Approved = 1, Rejected = 2 }

    
}

        