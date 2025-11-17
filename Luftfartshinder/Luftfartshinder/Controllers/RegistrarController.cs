using System.Text.Json;                 
using IOFile = System.IO.File;            
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Controllers
{
    public class RegistrarController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly ILogger<RegistrarController> _logger;
        private static readonly Dictionary<int, ReviewStatus> _statuses = new();
        private static readonly Dictionary<int, string> _notes = new();
        private static readonly object _notesLock = new();
        private static bool _notesLoaded = false;
        private static readonly string _notesFilePath =
            IOPath.Combine(AppContext.BaseDirectory, "App_Data", "registrar_notes.json");
        private static readonly object _statusesLock = new();
        private static readonly string _statusesFilePath =
            Path.Combine(AppContext.BaseDirectory, "App_Data", "registrar_statuses.json");
        private static bool _statusesLoaded = false;
        public RegistrarController(ApplicationContext db, ILogger<RegistrarController> logger)
        {
            _db = db;
            _logger = logger;
            EnsureNotesLoaded();
        }

        private void EnsureStatusesLoaded()
        {
            lock (_statusesLock)
            {
                if (_statusesLoaded) return;

                try
                {
                    var dir = Path.GetDirectoryName(_statusesFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir!);

                    if (System.IO.File.Exists(_statusesFilePath))
                    {
                        var json = System.IO.File.ReadAllText(_statusesFilePath);
                        var data = System.Text.Json.JsonSerializer
                            .Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();

                        _statuses.Clear();
                        foreach (var kv in data)
                        {
                           
                            if (Enum.IsDefined(typeof(ReviewStatus), kv.Value))
                                _statuses[kv.Key] = (ReviewStatus)kv.Value;
                        }
                    }
                }
                catch
                {
                  
                }

                _statusesLoaded = true;
            }
        }

        private void PersistStatuses()
        {
            lock (_statusesLock)
            {
                try
                {
                    var dir = Path.GetDirectoryName(_statusesFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir!);

                    var data = _statuses.ToDictionary(k => k.Key, v => (int)v.Value);
                    var json = System.Text.Json.JsonSerializer.Serialize(
                        data,
                        new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                    System.IO.File.WriteAllText(_statusesFilePath, json);
                }
                catch
                {

                }
            }
        }

        private static void EnsureNotesLoaded()
        {
            lock (_notesLock)
            {
                if (_notesLoaded) return;

                try
                {
                    var dir = IOPath.GetDirectoryName(_notesFilePath);
                    if (!string.IsNullOrEmpty(dir) && !IODirectory.Exists(dir))
                        IODirectory.CreateDirectory(dir);

                    if (IOFile.Exists(_notesFilePath))
                    {
                        var json = IOFile.ReadAllText(_notesFilePath);
                        var data = JsonSerializer.Deserialize<Dictionary<int, string>>(json)
                                   ?? new Dictionary<int, string>();
                        _notes.Clear();
                        foreach (var kv in data)
                            _notes[kv.Key] = kv.Value ?? "";
                    }
                }
                catch { 
                
                }

                _notesLoaded = true;
            }
        }

        private static void PersistNotes()
        {
            lock (_notesLock)
            {
                try
                {
                    var dir = IOPath.GetDirectoryName(_notesFilePath);
                    if (!string.IsNullOrEmpty(dir) && !IODirectory.Exists(dir))
                        IODirectory.CreateDirectory(dir);

                    var json = JsonSerializer.Serialize(_notes, new JsonSerializerOptions { WriteIndented = true });
                    IOFile.WriteAllText(_notesFilePath, json);
                }
                catch 
                { 
                
                }
            }
        }

        // GET /Registrar
        public IActionResult Index(string q = "", string from = "", string to = "")
        {
            EnsureStatusesLoaded();
            var data = _db.Set<Obstacle>().AsEnumerable();   
            var mapped = data.Select((o, i) => MapToRow(o, i)).ToList();
            return View(mapped);
        }



        // GET /Registrar/Details/
        public IActionResult Details(int id)
        {
            EnsureStatusesLoaded();
            var set = _db.Set<Obstacle>().AsEnumerable();
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
            var entity = _db.Obstacles.FirstOrDefault(o => o.Id == id);

            if (entity != null)
            {
           
                SetStatus(id, ReviewStatus.Approved);
                PersistStatuses();

                var obstacleName = string.IsNullOrWhiteSpace(entity.Name)
                    ? $"Obstacle {id}"
                    : entity.Name;
            }

           
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /Registrar/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var entity = _db.Obstacles.FirstOrDefault(o => o.Id == id);

            if (entity != null)
            {
                SetStatus(id, ReviewStatus.Rejected);
                PersistStatuses();

                var obstacleName = string.IsNullOrWhiteSpace(entity.Name)
                    ? $"Obstacle {id}"
                    : entity.Name;
            }

           
            return RedirectToAction(nameof(Details), new { id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNote(int id, string note)
        {
            if (id >= 0)
            {
                _notes[id] = note ?? "";
                PersistNotes();
                TempData["NoteSaved"] = true; 
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var entity = _db.Obstacles.FirstOrDefault(o => o.Id == id);

            if (entity != null)
            {
                string obstacleName = entity.Name;   

                _db.Obstacles.Remove(entity);
                _db.SaveChanges();

                PersistStatuses();

                TempData["Msg"] = $"{obstacleName} has been deleted.";
            }

            return RedirectToAction(nameof(Index));
        }


        private RegistrarRow MapToRow(object o, int fallbackIndex)
        {
            var id = GetInt(o, "Id", "ObstacleId", "EntityId", "RowId") ?? -1;
            var type = GetString(o, "Type");
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
                Type = type,
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
                Type = r.Type,
                Name = r.Name,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Height = r.Height,
                Reporter = r.Reporter,
                CreatedAt = r.CreatedAt,
                Description = r.Description,
                Status = r.Status,
                Note = r.Note,
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
        public string? Type { get; set; }
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

        