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
        private const int PageSize = 5; //Tamaño de pagina
        public EventosController(AgendaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search, int page =1)
        {
            var query = _context.Eventos.AsQueryable();

            //FILTRO DE BUSQUEDA
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Title.Contains(search) || e.Descripcion.Contains(search) || e.Estado.Name.Contains(search));
            }

            var listaEventos = query
                .OrderBy(e => e.Id)
                .Include(e => e.Estado)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPaginas;
            ViewBag.Search = search;

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
