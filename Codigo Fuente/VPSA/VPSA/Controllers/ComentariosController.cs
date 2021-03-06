﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VPSA.Data;
using VPSA.Models;

namespace VPSA.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly VPSAContext _context;

        public ComentariosController(VPSAContext context)
        {
            _context = context;
        }

        // GET: Comentarios
        public async Task<IActionResult> Index()
        {
            var vPSAContext = _context.Comentarios.Include(c => c.Denuncia).Include(c => c.Empleado);
            return View(await vPSAContext.ToListAsync());
        }

        // GET: Comentarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Denuncia)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // GET: Comentarios/Create
        public async Task<IActionResult> Create(int Id)
        {
            var denuncia = await _context.Denuncias.Where(x=>x.Id == Id).Include(d => d.TipoDenuncia).FirstOrDefaultAsync();
            ViewData["Denuncia"] = denuncia;
            ViewData["EstadoId"] = new SelectList(_context.Set<EstadoDenuncia>(), "Id", "Nombre");
            ViewData["EmpleadoId"] = new SelectList(_context.Set<Empleado>(), "Id", "NombreCompleto");
            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Descripcion,DenunciaId,EmpleadoId,EstadoDenunciaId")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                var denuncia = await _context.Denuncias.FindAsync(comentario.DenunciaId);
                denuncia.EstadoDenunciaId = comentario.EstadoDenunciaId;
                comentario.FechaCreacion = DateTime.Now;
                _context.Add(comentario);
                _context.Update(denuncia);
                await _context.SaveChangesAsync();



                return RedirectToAction("Index", "Denuncias");
            }
            ViewData["DenunciaId"] = new SelectList(_context.Denuncias, "Id", "Id", comentario.DenunciaId);
            ViewData["EmpleadoId"] = new SelectList(_context.Set<Empleado>(), "Id", "Id", comentario.EmpleadoId);
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            ViewData["DenunciaId"] = new SelectList(_context.Denuncias, "Id", "Id", comentario.DenunciaId);
            ViewData["EmpleadoId"] = new SelectList(_context.Set<Empleado>(), "Id", "Id", comentario.EmpleadoId);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,DenunciaId,EmpleadoId,Estadoid")] Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comentario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DenunciaId"] = new SelectList(_context.Denuncias, "Id", "Id", comentario.DenunciaId);
            ViewData["EmpleadoId"] = new SelectList(_context.Set<Empleado>(), "Id", "Id", comentario.EmpleadoId);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Denuncia)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
