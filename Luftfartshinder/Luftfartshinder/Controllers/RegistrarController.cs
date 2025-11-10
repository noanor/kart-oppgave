using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models;

namespace Luftfartshinder.Controllers
{
    public class RegistrarController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly ILogger<RegistrarController> _logger;
        private static readonly Dictionary<int, ReviewStatus> _statuses = new();
        private static readonly Dictionary<int, string> _notes = new();

        public RegistrarController(ApplicationContext db, ILogger<RegistrarController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET /Registrar
        public IActionResult Index(string q = "", string from = "", string to = "")
        {
            var data = _db.Set<ObstacleData>().AsEnumerable();
            var mapped = data.Select((o, i) => MapToRow(o, i)).ToList();

            if (!string.IsNullOrWhiteSpace(q))
                mapped = mapped.Where(r =>
                    (r.Name ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (r.Reporter ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (r.Description ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            if (DateTime.TryParse(from, out var fromDt))
                mapped = mapped.Where(r => r.CreatedAt.HasValue && r.CreatedAt.Value >= fromDt).ToList();

            if (DateTime.TryParse(to, out var toDt))
                mapped = mapped.Where(r => r.CreatedAt.HasValue && r.CreatedAt.Value <= toDt).ToList();

            mapped = mapped
                .OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue)
                .Take(500)
                .ToList();

            return View(mapped);
        }

        // GET /Registrar/Details/5
        public IActionResult Details(int id)
        {
            var set = _db.Set<ObstacleData>().AsEnumerable();
            foreach (var item in set)
            {
                var foundId = GetInt(item, "Id", "ObstacleId", "EntityId", "RowId");
                if (foundId.HasValue && foundId.Value == id)
                {
                    var vm = MapToDetails(item, -1);
                    return View(vm);
                }
            }
            return NotFound();
        }

        // POST /Registrar/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            SetStatus(id, ReviewStatus.Approved);
            TempData["Msg"] = $"Obstacle #{id} approved.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /Registrar/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            SetStatus(id, ReviewStatus.Rejected);
            TempData["Msg"] = $"Obstacle #{id} rejected.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNote(int id, string note)
        {
            if (id >= 0)
            {
                _notes[id] = note ?? "";
                TempData["NoteSaved"] = true; 
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private RegistrarRow MapToRow(object o, int fallbackIndex)
        {
            var id = GetInt(o, "Id", "ObstacleId", "EntityId", "RowId") ?? -1;
            var name = GetString(o, "Name", "ObstacleName", "ViewObstacleName", "Title");

            var lat = GetDouble(o, "Latitude", "Lat", "Y", "Northing") ?? 0;
            var lng = GetDouble(o, "Longitude", "Lng", "Lon", "X", "Easting") ?? 0;

            if (lat == 0 || lng == 0)
            {
                var coords = GetString(o, "Coords", "Coordinates", "ViewObstacleCoords", "Position");
                if (!string.IsNullOrWhiteSpace(coords) && coords.Contains(","))
                {
                    var parts = coords.Split(',', StringSplitOptions.TrimEntries);
                    double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat);
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lng);
                }
            }

            var height = GetNullableDouble(o, "Height", "Altitude", "Amsl", "Høyde");
            var reporter = GetString(o, "Reporter", "ReportedBy", "UserName", "Pilot", "CreatedBy");
            var created = GetDate(o, "CreatedAt", "Created", "ReportedAt", "Timestamp", "Date");
            var description = GetString(o, "Description", "Notes", "ViewObstacleDescription", "Desc");
            var note = (id >= 0 && _notes.TryGetValue(id, out var n)) ? n : null;
            var status = (id >= 0 && _statuses.TryGetValue(id, out var s)) ? s : ReviewStatus.Pending;

            return new RegistrarRow
            {
                Id = id,
                Name = string.IsNullOrWhiteSpace(name) ? "Obstacle" : name,
                Latitude = lat,
                Longitude = lng,
                Height = height,
                Reporter = reporter,
                CreatedAt = created,
                Description = description,
                Status = status,
                Note = note,
            };
        }

        private RegistrarDetails MapToDetails(object o, int fallbackIndex)
        {
            var r = MapToRow(o, fallbackIndex);
            return new RegistrarDetails
            {
                Id = r.Id,
                Name = r.Name,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Height = r.Height,
                Reporter = r.Reporter,
                CreatedAt = r.CreatedAt,
                Description = r.Description,
                Status = r.Status
            };
        }

        private void SetStatus(int id, ReviewStatus status)
        {
            if (id < 0) return; 
            _statuses[id] = status;
        }

      

        private static PropertyInfo? FindProp(object o, params string[] names) =>
            names.Select(n => o.GetType().GetProperty(n, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase))
                 .FirstOrDefault(p => p != null);

        private static string? GetString(object o, params string[] names)
        {
            var p = FindProp(o, names);
            return p == null ? null : p.GetValue(o)?.ToString();
        }

        private static int? GetInt(object o, params string[] names)
        {
            var p = FindProp(o, names);
            if (p == null) return null;
            var v = p.GetValue(o);
            if (v == null) return null;
            if (v is int i) return i;
            if (int.TryParse(v.ToString(), out var parsed)) return parsed;
            return null;
        }

        private static double? GetDouble(object o, params string[] names)
        {
            var p = FindProp(o, names);
            if (p == null) return null;
            var v = p.GetValue(o);
            if (v == null) return null;
            if (v is double d) return d;
            if (double.TryParse(v.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) return parsed;
            return null;
        }

        private static double? GetNullableDouble(object o, params string[] names) => GetDouble(o, names);

        private static DateTime? GetDate(object o, params string[] names)
        {
            var p = FindProp(o, names);
            if (p == null) return null;
            var v = p.GetValue(o);
            if (v == null) return null;
            if (v is DateTime dt) return dt;
            if (DateTime.TryParse(v.ToString(), out var parsed)) return parsed;
            return null;
        }
    }


    public enum ReviewStatus { Pending = 0, Approved = 1, Rejected = 2 }

    public class RegistrarRow
    {
        public int Id { get; set; }               
        public string? Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Height { get; set; }
        public string? Reporter { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
        public string? Note { get; set; }
    }

    public class RegistrarDetails : RegistrarRow { }
}

        