using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SurveyApp.Data;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ankiety = _context.Surveys.ToList();
            return View(ankiety);
        }

        [Authorize(Roles = "Ankieter")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Ankieter")]
        public IActionResult Create(IFormCollection form)
        {
            var myUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Survey nowaAnkieta = new Survey();
            nowaAnkieta.Title = form["title"];
            nowaAnkieta.Description = form["description"];
            nowaAnkieta.CreatorId = myUserId ?? "";

            int iloscPytan = int.Parse(form["iloscPytanUkryta"]);

            for (int i = 1; i <= iloscPytan; i++)
            {
                string trescPytania = form["pytanie_" + i];
                
                if (!string.IsNullOrEmpty(trescPytania))
                {
                    Question nowePytanie = new Question();
                    nowePytanie.Text = trescPytania;

                    var opcjeDoPytania = form["opcje_" + i].ToList();
                    
                    foreach (var opcja in opcjeDoPytania)
                    {
                        if (!string.IsNullOrEmpty(opcja))
                        {
                            Option nowaOpcja = new Option();
                            nowaOpcja.Text = opcja;
                            nowePytanie.Options.Add(nowaOpcja);
                        }
                    }
                    nowaAnkieta.Questions.Add(nowePytanie);
                }
            }

            _context.Surveys.Add(nowaAnkieta);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Vote(int id)
        {
            var ankieta = _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault(x => x.Id == id);

            if (ankieta == null) return RedirectToAction("Index");

            string aktualnyUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var czyGlosowal = _context.Votes
                .Any(v => v.Option.Question.SurveyId == id && v.UserId == aktualnyUser);

            if (czyGlosowal)
            {
                return RedirectToAction("Results", new { id = id });
            }

            return View(ankieta);
        }

        [HttpPost]
        public IActionResult SubmitVote(IFormCollection form, int ankietaId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            bool czyGlosowal = _context.Votes.Any(x => x.Option.Question.SurveyId == ankietaId && x.UserId == user);

            if (czyGlosowal == false)
            {
                foreach (var klucz in form.Keys)
                {
                    if (klucz.StartsWith("pytanie_"))
                    {
                        int idWybranejOpcji = int.Parse(form[klucz]);
                        
                        Vote nowyGlos = new Vote();
                        nowyGlos.OptionId = idWybranejOpcji;
                        nowyGlos.UserId = user;
                        _context.Votes.Add(nowyGlos);
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Results", new { id = ankietaId });
        }

        public IActionResult Results(int id)
        {
            var ankietaZWynikami = _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .ThenInclude(o => o.Votes)
                .FirstOrDefault(s => s.Id == id);

            return View(ankietaZWynikami);
        }

        public async Task<IActionResult> DajMiRole([FromServices] Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Content("Musisz byc najpierw zalogowany!");

            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Ankieter");
                return Content("Gratulacje, jestes teraz Ankieterem! Zrestartuj logowanie (wyloguj i zaloguj ponownie).");
            }
            return Content("Nie znaleziono uzytkownika w bazie!");
        }
    }
}