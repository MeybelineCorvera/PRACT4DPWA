using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRACT4.Data;
using PRACT4.Models;

namespace PRACT4.Controllers
{
    public class EstadosController : Controller
    {
        private readonly AgendaDbContext _context;
        private const int PageSize = 5; //Tamaño de pagina
        public EstadosController(AgendaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchString, int page = 1)
        {
            var query = _context.Estados.AsQueryable();

            //FILTRO DE BUSQUEDA
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.Name.Contains(searchString) || e.Descripcion.Contains(searchString));
            }

            var totalRecords = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            var listaEstados = query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(listaEstados);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Estado estado)
        {
            if (ModelState.IsValid)
            {
                estado.CreatedAt = DateTime.Now;
                _context.Estados.Add(estado);
                _context.SaveChanges();


                TempData["Message"] = "El estado se ha creado correctamente";
                TempData["MessageType"] = "success";

                return RedirectToAction("Index");
            }


            TempData["Message"] = "Error al crear los datos.Verifique los datos";
            TempData["MessageType"] = "error";

            return View(estado);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = _context.Estados.Find(id);

            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = _context.Estados.Find(id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Estado estado)
        {
            if (id != estado.Id)
            {
                return NotFound();
            }

            var current = _context.Estados.Find(id);
            if (current == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                current.Name = estado.Name;
                current.Descripcion = estado.Descripcion;
                current.Color = estado.Color;
                current.UpdatedAt = DateTime.Now;

                _context.Update(current);
                _context.SaveChanges();

                // Mensaje de éxito
                TempData["Message"] = "El estado se ha actualizado correctamente";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }

            // Mensaje de error
            TempData["Message"] = "Error al actualizar el estado. Verifica los datos";
            TempData["MessageType"] = "error";
            return View(estado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var estado = _context.Estados.Find(id);
            if (estado == null)
            {
                return NotFound();
            }

            _context.Estados.Remove(estado);
            _context.SaveChanges();

            // Mensaje de éxito
            TempData["Message"] = "El estado se ha eliminado correctamente";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index");
        }

    }
}
