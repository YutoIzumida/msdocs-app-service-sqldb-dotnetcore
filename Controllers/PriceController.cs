using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Controllers
{
    public class PriceController : Controller
    {
        private readonly MyDatabaseContext _context;

        public PriceController(MyDatabaseContext context)
        {
            _context = context;
        }

        // 一覧
        public async Task<IActionResult> Index()
        {
            var prices = await _context.Prices.ToListAsync();
            return View(prices);
        }

        // 詳細
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var price = await _context.Prices
                .FirstOrDefaultAsync(m => m.ID == id);
            if (price == null) return NotFound();

            return View(price);
        }

        // 新規作成（GET）
        public IActionResult Create()
        {
            return View();
        }

        // 新規作成（POST）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ItemName,Amount")] Price price)
        {
            if (ModelState.IsValid)
            {
                price.UpdatedDate = DateTime.UtcNow;
                _context.Add(price);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(price);
        }

        // 編集（GET）
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var price = await _context.Prices.FindAsync(id);
            if (price == null) return NotFound();
            return View(price);
        }

        // 編集（POST）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ItemName,Amount")] Price price)
        {
            if (id != price.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    price.UpdatedDate = DateTime.UtcNow;
                    _context.Update(price);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(price.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(price);
        }

        // 削除（GET）
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var price = await _context.Prices
                .FirstOrDefaultAsync(m => m.ID == id);
            if (price == null) return NotFound();

            return View(price);
        }

        // 削除（POST）
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price != null)
            {
                _context.Prices.Remove(price);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ★ Ajaxで価格を増減させるAPI ★
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustPrice(int id, decimal change)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null) return NotFound();

            price.Amount += change;
            price.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, newAmount = price.Amount, updated = price.UpdatedDate.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        private bool PriceExists(int id)
        {
            return _context.Prices.Any(e => e.ID == id);
        }
    }
}
