using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRACT4.Data;
using PRACT4.Models;

namespace PRACT4.Controllers
{
    public class EventosController : Controller
    {
        private readonly AgendaDbContext _context;
        public EventosController(AgendaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var listaEventos = _context.Eventos.Include(e => e.Estado).ToList();
            return View(listaEventos);
        }
        public IActionResult Create()
        {
            var estados = _context.Estados.ToList();
            ViewData["EstadoId"] = new SelectList(estados, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Evento evento)
        {
            if (ModelState.IsValid)
            {
                evento.CreatedAt = DateTime.Now;
                _context.Eventos.Add(evento);
                _context.SaveChanges();


                TempData["Message"] = "El evento se ha creado correctamente";
                TempData["MessageType"] = "success";

                return RedirectToAction("Index");
            }


            TempData["Message"] = "Error al crear el evento. Verifique los datos";
            TempData["MessageType"] = "error";

            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Name", evento.EstadoId);
            return View(evento);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = _context.Eventos.Include(e => e.Estado).FirstOrDefault(e => e.Id == id); //SELECT * FROM Eventos WHERE Id = id
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = _context.Eventos.Find(id); //SELECT * FROM Eventos WHERE Id = id
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Name", evento.EstadoId);
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            var current = _context.Eventos.Find(id); //SELECT * FROM Eventos WHERE Id = id
            if (current == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                current.Title = evento.Title;
                current.Descripcion = evento.Descripcion;
                current.StartDate = evento.StartDate;
                current.EndDate = evento.EndDate;
                current.EstadoId = evento.EstadoId;
                current.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["Message"] = "El evento se ha actualizado correctamente";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }

            //Mensaje de error
            TempData["Message"] = "Error al actualizar el evento. Verifica los datos";
            TempData["MessageType"] = "error";
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Name", evento.EstadoId);
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var evento = _context.Eventos.Find(id);
            if (evento == null)
            {
                return NotFound();
            }

            _context.Eventos.Remove(evento);
            _context.SaveChanges();

            //Mensaje de éxito
            TempData["Message"] = "El evento se ha eliminado correctamente";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetEventos()
        {
            var eventos = await _context.Eventos.Include(e => e.Estado).Select(e => new
            {
                id = e.Id,
                title = e.Title,
                start = e.StartDate,
                end = e.EndDate,
                description = e.Descripcion,
                color = e.Estado.Color
            }).ToListAsync();
            return Json(eventos);
        }
    }
}
